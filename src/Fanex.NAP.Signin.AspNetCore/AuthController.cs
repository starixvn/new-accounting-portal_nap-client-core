using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Fanex.Data;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Models;
using Fanex.NAP.Client.Models.ParameterObject;
using Fanex.NAP.Client.Services;
using Fanex.NAP.Signin.AspNetCore.Models;
using Fanex.NAP.Signin.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ONEbook.UM.Services;

namespace Fanex.NAP.Signin.AspNetCore
{
    public class AuthController : Controller
    {
        #region members

        private const string FormTokenName = "__SecureFormToken";
        private const string OtpSite = "NAP";

        private readonly INapSession napSession;
        private readonly ISecurityService securityService;
        private readonly IGroupService groupService;
        private readonly IUserService userService;
        private readonly IMaintenanceService maintenanceService;
        private readonly INapSettings napSettings;
        private readonly IConfiguration configuration;

        #endregion members

        #region ctors

        public AuthController(
            INapSession napSession,
            ISecurityService securityService,
            IGroupService groupService,
            IUserService userService,
            IMaintenanceService maintenanceService,
            INapSettings napSettings,
            IConfiguration configuration)
        {
            this.napSession = napSession;
            this.securityService = securityService;
            this.groupService = groupService;
            this.userService = userService;
            this.maintenanceService = maintenanceService;
            this.napSettings = napSettings;
            this.configuration = configuration;
        }

        #endregion ctors

        #region sign in

        private string FormToken
        {
            get
            {
                var token = HttpContext.Session.GetString(FormTokenName);

                return string.IsNullOrEmpty(token) ? string.Empty : token;
            }

            set
            {
                HttpContext.Session.SetString(FormTokenName, value);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {

            var indexViewModel = new IndexViewModel();

            if (!napSettings.EnableDirectLogin)
            {
                return Redirect(napSettings.HubUrl);
            }

            GenerateNewToken();
            return View("Index", indexViewModel);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(string username, string password, string loginToken = "")
        {
            var indexViewModel = new IndexViewModel();

            if (IsValidUserPassword(username, password, loginToken))
            {
                var user = userService.Get(username.Trim());

                if (IsAuthorizedUser(user))
                {
                    if (IsActivatedUser(user))
                    {
                        return await LoginFlow(user);
                    }
                    else
                    {
                        indexViewModel.Message = "Your account was suspended";
                    }
                }
                else
                {
                    indexViewModel.Message = "You have no authorization"; ;
                }
            }
            else
            {
                indexViewModel.Message = "Invalid Username or Password";
            }

            GenerateNewToken();
            indexViewModel.FormToken = FormToken;

            return View("Index", indexViewModel);
        }

        private bool IsValidUserPassword(string username, string password, string token)
        {
            if (!IsValidToken(token)
                || string.IsNullOrEmpty(username)
                || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var user = userService.Get(username.Trim());

            if (user == null || user.Id == 0)
            {
                return false;
            }

            string hashPassword = SHA256(user.Password + token);
            if (password != hashPassword)
            {
                return false;
            }

            return true;
        }


        private bool IsAuthorizedUser(User user)
        {
            var group = user.GroupId == 0 ? null : groupService.Get(user.GroupId);

            var siteLevelList = userService.GetSiteLevel(user.Id, napSettings.Environment);

            return (group == null || group.IsActive) &&
                 siteLevelList.Any(x => x.IsActive);
        }

        private bool IsActivatedUser(User user)
        {
            var siteLevelList = userService.GetSiteLevel(user.Id, napSettings.Environment);

            bool userIsActiveOnThisSite = false;

            foreach (var siteLevel in siteLevelList)
            {
                if (siteLevel.SiteId == napSettings.SiteId && siteLevel.IsActive)
                {
                    userIsActiveOnThisSite = true;
                }
            }

            return userIsActiveOnThisSite;
        }

        #endregion sign in

        #region sign out

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> LogOut()
        {
            await DoSignOut();
            return RedirectToAction("Index");
        }

        #endregion sign out

        #region re check in

        [AllowAnonymous]
        public ActionResult ReCheckIn()
        {
            if (!napSettings.EnableReCheckIn)
            {
                return Json(new
                {
                    IsKickedOut = 0
                });
            }

            var isKickedOut = napSession.OnUserId <= 0 || securityService.ReCheckIn(napSession.OnUserId, napSettings.SiteId) > 0 ? 1 : 0;

            return Json(new
            {
                napSession.OnUserId,
                IsKickedOut = isKickedOut
            });
        }

        #endregion re check in

        #region helpers

        private async Task DoSignOut()
        {
            if (napSession.OnUserId != 0)
            {
                await securityService.LogoutAsync(napSession.OnUserId);
            }

            napSession.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            napSession.ClearAll();
        }

        private void CreateUserMenu(User user)
        {
            napSession.MenuItemList = userService.GetMenuItem(
                user.Id,
                napSettings.SiteId,
                napSettings.Environment,
                true)
                .ToList();
        }

        private async Task CreateAuthInfo(User user)
        {
            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new[] { identity }));
        }

        private LoginParameter SetUserCheckInCriteria(int userId)
        {
            return new LoginParameter
            {
                UserId = userId,
                SiteId = napSettings.SiteId,
                LoginDate = DateTime.Now,
                NetSessionId = HttpContext?.Session.Id,
                Ip = GetClientIP(),
                BrowserName = Request?.Headers["User-Agent"].ToString(),
                CountryCode = ResolveCountry(HttpContext.Request)?.GeoId.ToString(CultureInfo.CurrentCulture),
                Domain = HttpContext.Request.Host.ToString(),
                Platform = Environment.OSVersion.Platform.ToString()
            };
        }

        private string GetClientIP()
        {
            var clientIP = HttpContext.Request.Headers["x-forwarded-for"].ToString();
            if (string.IsNullOrWhiteSpace(clientIP))
            {
                clientIP = HttpContext?.Connection.RemoteIpAddress?.ToString();
            }

            return clientIP;
        }

        private static RegionInfo ResolveCountry(HttpRequest request)
        {
            var culture = ResolveCulture(request);

            if (culture != null)
            {
                return new RegionInfo(culture.LCID);
            }

            return null;
        }

        private static CultureInfo ResolveCulture(HttpRequest request)
        {
            var userLangs = request.Headers["Accept-Language"].ToString();
            var firstLang = userLangs.Split(',').FirstOrDefault();
            var defaultLang = string.IsNullOrEmpty(firstLang) ? "en" : firstLang;
            return CultureInfo.CreateSpecificCulture(defaultLang);
        }

        private async Task SaveUserSession(User user, string userName, int onUserId)
        {
            napSession.Level = userService.GetUserLevel(user.Id, napSettings.SiteId);
            napSession.UserId = user.Id;
            napSession.UserName = userName;
            napSession.OnUserId = onUserId;

            CreateUserMenu(user);
            await CreateAuthInfo(user);
        }


        private string SHA256(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Hey guy! I dont want to hash an empty or white space string");
            }

            var hashAlgorithm = new SHA256Managed();
            var hashData = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            string encrypted = BitConverter.ToString(hashData).Replace("-", string.Empty).ToLower();

            return encrypted;
        }
        #endregion helpers

        #region Token

        private void AddTokenToCookie(string loginToken)
        {
            HttpContext.Response.Cookies.Append(
                FormTokenName,
                loginToken,
                new CookieOptions { Path = "/" }
            );
        }

        private bool IsValidToken(string loginToken)
        {
            return !string.IsNullOrWhiteSpace(loginToken)
                && FormToken == loginToken
                && Request.Cookies[FormTokenName] != null
                && loginToken == Request.Cookies[FormTokenName];
        }

        private void GenerateNewToken()
        {
            string loginToken = Guid.NewGuid().ToString("N");
            FormToken = loginToken;
            AddTokenToCookie(loginToken);
        }



        #endregion

        #region common

        [AllowAnonymous]
        public ActionResult Offline()
        {
            var isDownTime = maintenanceService.IsUnderMaintenance();
            if (!isDownTime)
            {
                return Redirect(Url.Content("~/"));
            }

            var downTime = maintenanceService.GetUnderMaintenanceTime();

            var viewModel = new MaintenanceViewModel { UmtFrom = downTime.From, UmtTo = downTime.To };

            return View("Offline", viewModel);
        }

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View("Error");
        }

        [AllowAnonymous]
        public ActionResult Static(string url)
        {
            var contentType = "application/octet-stream";
            if (url.EndsWith("css"))
            {
                contentType = "text/css";
            }
            else if (url.EndsWith("js"))
            {
                contentType = "text/javascript";
            }

            var resourceName = $"Fanex.NAP.Signin.AspNetCore.Resources.{url}";
            var text = ReadResourceContent(resourceName);
            return Content(text, contentType);
        }

        [AllowAnonymous]
        public ActionResult RecheckinScript()
        {
            if (!napSettings.EnableReCheckIn)
            {
                return Content(string.Empty, "text/javascript");
            }

            var resourceName = "Fanex.NAP.Signin.AspNetCore.Resources.recheckin.js";
            var text = ReadResourceContent(resourceName);
            var rootUrl = Url.Content("~/");

            return Content($"window.rootUrl='{rootUrl}';\r\n{text}", "text/javascript");
        }


        private string ReadResourceContent(string resourceName)
        {
            var assembly = typeof(AuthController).Assembly;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        #endregion

        #region Password

        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View("ChangePassword", new UpdatePasswordModel());
        }

        [HttpPost]
        [Authorize]
        public JsonResult ChangePassword(UpdatePasswordModel criteria)
        {
            var user = userService.Get(napSession.UserId);
            var oldPwd = criteria.OldUserPwd;

            if (oldPwd.Equals(user.Password))
            {
                var password = criteria.UserPwd;
                securityService.UpdatePassword(napSettings.SiteId, user.Id, password);
            }
            else
            {
                return Json(new { ErrorMsg = "Wrong password" });
            }

            return Json(new { ErrorMsg = "" });
        }


        #endregion

        #region SSO
        private const int TimeExipration = 60;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SSO(string userId, string onuserId, string time, string redirectUrl = "")
        {
            if (await IsAuthenticated(userId, onuserId, time))
            {
                redirectUrl = string.IsNullOrEmpty(redirectUrl) ? Url.Content("~") : redirectUrl;
                return Redirect(redirectUrl);
            }

            return Redirect(Url.Content("~/Auth/Index"));
        }


        public async Task<bool> IsAuthenticated(string userId, string onuserId, string time)
        {
            var currentTime = await securityService.GetTimeAsync();
            var userIdDecrypt = Cryptography.DecryptPassword(userId.Replace(" ", "+"));
            var onuserIdDecrypt = Cryptography.DecryptPassword(onuserId.Replace(" ", "+"));
            var timeDecrypt = Cryptography.DecryptPassword(time.Replace(" ", "+"));
            var expiration = Convert.ToDateTime(currentTime) - Convert.ToDateTime(timeDecrypt);

            if (expiration.TotalSeconds <= TimeExipration)
            {
                var criteria = SetSsoLoginParameter(userIdDecrypt, onuserIdDecrypt);

                var onUserId = await securityService.SSOLoginAsync(criteria);
                if (onUserId != 0)
                {
                    var user = await userService.GetAsync(criteria.UserId);
                    napSession.UserId = user.Id;
                    napSession.UserName = user.Username;
                    napSession.OnUserId = onUserId;
                    napSession.Level = await userService.GetUserLevelAsync(user.Id, napSettings.SiteId);

                    if (napSettings.SiteId != 0)
                    {
                        napSession.MenuItemList = await userService.GetMenuItemAsync(user.Id,
                                                                                napSettings.SiteId,
                                                                                napSettings.Environment,
                                                                                true) as IList<MenuItem>;
                    }

                    await CreateAuthInfo(user);

                    return true;
                }
            }

            return false;
        }

        private SSOLoginParameter SetSsoLoginParameter(string userId, string onuserId)
        {
            return new SSOLoginParameter
            {
                UserId = Convert.ToInt32(userId),
                SiteId = napSettings.SiteId,
                CheckOnUserId = Convert.ToInt32(onuserId),
                OnUserId = 0,
                LoginDate = DateTime.Now,
                NetSessionId = HttpContext?.Session.Id,
                IP = GetClientIP(),
                BrowserName = Request?.Headers["User-Agent"].ToString(),
                CountryCode = ResolveCountry(HttpContext.Request)?.GeoId.ToString(CultureInfo.CurrentCulture),
                Domain = HttpContext.Request.Host.ToString(),
                Platform = Environment.OSVersion.Platform.ToString()
            };
        }

        #endregion

        #region LoginFlow

        private async Task<ActionResult> LoginFlow(User user)
        {
            var ignoredOtp = !napSettings.EnableOtp
                || (await userService.IgnoredOtpAsync(user.Id, OtpSite));

            if (ignoredOtp)
            {
                return await DefaultLoginFlow(user);
            }
            else
            {
                HttpContext.Session.SetInt32("OtpUserId", user.Id);
                return RedirectToAction("OtpLoginFlow");
            }
        }

        private async Task<ActionResult> DefaultLoginFlow(User user)
        {
            var criteria = SetUserCheckInCriteria(user.Id);
            var onUserId = await securityService.LoginAsync(criteria);

            await SaveUserSession(user, user.Username, onUserId);
            return Redirect("~/");
        }

        [HttpGet]
        public async Task<ActionResult> OtpLoginFlow()
        {
            var otpUserId = HttpContext.Session.GetInt32("OtpUserId");
            var userId = otpUserId.HasValue ? otpUserId.Value : 0;
            if (userId == 0)
            {
                return Redirect(configuration.GetSection("AuthSettings:LoginPath").Value);
            }

            var ignoredOtp = !napSettings.EnableOtp
                || (await userService.IgnoredOtpAsync(userId, OtpSite));

            if (ignoredOtp)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> OtpLoginFlow(string otp)
        {
            ViewBag.Message = string.Empty;
            var otpUserId = HttpContext.Session.GetInt32("OtpUserId");
            var userId = otpUserId.HasValue ? otpUserId.Value : 0;
            var ignoredOtp = !napSettings.EnableOtp
                || (await userService.IgnoredOtpAsync(userId, OtpSite));
            if (userId == 0 || ignoredOtp)
            {
                HttpContext.Session.SetInt32("OtpUserId", 0);
                return RedirectToAction("Index");
            }

            var validOtp = !string.IsNullOrEmpty(otp)
                && (await userService.VerifyOtpAsync(userId, OtpSite, otp));

            if (validOtp)
            {
                HttpContext.Session.SetInt32("OtpUserId", 0);
                var user = userService.Get(userId);
                return await DefaultLoginFlow(user);
            }

            ViewBag.Message = "Invalid OTP. Please try again";

            return View();
        }

        #endregion

        [AllowAnonymous]
        public ActionResult WhoAmI()
        {
            return Content($"Environment:{Environment.GetEnvironmentVariable("ASPNETCORE_APPL_PATH")}");
        }
    }
}

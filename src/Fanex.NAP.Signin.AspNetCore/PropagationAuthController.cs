using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Models;
using Fanex.NAP.Client.Models.ParameterObject;
using Fanex.NAP.Client.Models.ValueObject;
using Fanex.NAP.Client.Resources;
using Fanex.NAP.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.NAP.Signin.AspNetCore
{
    public class PropagationAuthController : Controller
    {
        #region members

        private readonly INapSession napSession;
        private readonly ISecurityService securityService;
        private readonly IUserService userService;
        private readonly INapSettings napSettings;

        #endregion members

        #region ctors

        public PropagationAuthController(INapSession napSession, ISecurityService securityService, IUserService userService, INapSettings napSettings)
        {
            this.napSession = napSession;
            this.securityService = securityService;
            this.userService = userService;
            this.napSettings = napSettings;
        }

        #endregion ctors

        #region sign in

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(string username, string password, string redirectUrl, string loginToken = "")
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Content("Invalid input!");
            }

            var user = userService.Get(username.Trim());
            if (user == null)
            {
                return Content(ViewResources.Invalid_Username_or_Password);
            }

            string hashPassword = SHA256(user.Password + loginToken);
            if (password != hashPassword)
            {
                return Content(ViewResources.Invalid_Username_or_Password);
            }

            var criteria = SetUserCheckInCriteria(user.Id);
            var onUserId = securityService.Login(criteria);

            await SaveUserSession(user, username, onUserId);

            redirectUrl = WebUtility.UrlDecode(redirectUrl);
            return Redirect(redirectUrl);
        }

        #endregion sign in

        #region sign out

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> LogOut(string redirectUrl)
        {
            await DoSignOut();

            redirectUrl = WebUtility.UrlDecode(redirectUrl);
            return Redirect(redirectUrl);
        }

        #endregion sign out

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
            napSession.MenuItemList = (IList<MenuItem>)userService.GetMenuItem(
                user.Id,
                napSettings.SiteId,
                napSettings.Environment,
                true);
        }

        private async Task CreateAuthInfo(User user)
        {
            var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
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
                Domain = HttpContext.Request?.Host.ToString(),
                Platform = Environment.OSVersion.Platform.ToString()
            };
        }

        private string GetClientIP()
        {
            var clientIP = HttpContext.Request?.Headers["x-forwarded-for"].ToString();
            if (string.IsNullOrWhiteSpace(clientIP))
            {
                clientIP = HttpContext?.Connection?.RemoteIpAddress?.ToString();
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
            napSession.UserId = user.Id;
            napSession.UserName = userName;
            napSession.OnUserId = onUserId;

            CreateUserMenu(user);
            await CreateAuthInfo(user);
        }

        private bool UserIsActiveOnThisSite(IEnumerable<SiteLevel> siteLevelList)
        {
            bool userIsActiveOnThisSite = false;

            foreach (var siteLevel in siteLevelList)
            {
                if (siteLevel.Site.Id == napSettings.SiteId && siteLevel.IsActive)
                {
                    userIsActiveOnThisSite = true;
                }
            }

            return userIsActiveOnThisSite;
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

        #region SSO
        private const int TimeExipration = 30;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SSO(string userId, string onuserId, string time, string redirectUrl)
        {
            if (await IsAuthenticated(userId, onuserId, time))
            {
                redirectUrl = WebUtility.UrlDecode(redirectUrl);

                return Redirect(redirectUrl);
            }

            return Content("Invalid parameter");
        }


        public async Task<bool> IsAuthenticated(string userId, string onuserId, string time)
        {
            var currentTime = securityService.GetTime();
            var userIdDecrypt = Fanex.Data.Cryptography.DecryptPassword(userId.Replace(" ", "+"));
            var onuserIdDecrypt = Fanex.Data.Cryptography.DecryptPassword(onuserId.Replace(" ", "+"));
            var timeDecrypt = Fanex.Data.Cryptography.DecryptPassword(time.Replace(" ", "+"));
            var expiration = Convert.ToDateTime(currentTime) - Convert.ToDateTime(timeDecrypt);

            if (expiration.TotalSeconds <= TimeExipration)
            {
                var criteria = SetSsoLoginParameter(userIdDecrypt, onuserIdDecrypt);

                var onUserId = securityService.SSOLogin(criteria);
                if (onUserId != 0)
                {
                    var user = userService.Get(criteria.UserId);
                    napSession.UserId = user.Id;
                    napSession.UserName = user.Username;
                    napSession.OnUserId = onUserId;

                    if (napSettings.SiteId != 0)
                    {
                        napSession.MenuItemList = userService.GetMenuItem(user.Id, napSettings.SiteId, napSettings.Environment, true).ToList();
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
                Domain = HttpContext.Request?.Host.ToString(),
                Platform = Environment.OSVersion.Platform.ToString()
            };
        }

        #endregion

    }
}

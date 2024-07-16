namespace Fanex.NAP.SignIn;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Models;
using Fanex.NAP.Client.Models.ParameterObject;
using Fanex.NAP.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Models;
using ONEbook.UM.AspNetCore.Filters;
using ONEbook.UM.Services;
using ViewModel;

[AllowAnonymous]
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

        set => HttpContext.Session.SetString(FormTokenName, value);
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (!napSettings.EnableDirectLogin)
        {
            return Redirect(napSettings.HubUrl);
        }

        HttpContext.Session.SetInt32("OtpUserId", 0);
        GenerateNewToken();
        var indexViewModel = new IndexViewModel { FormToken = FormToken };
        return View("Index", indexViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Index(string username, string password, string loginToken = "")
    {
        var indexViewModel = new IndexViewModel();

        if (IsValidUserPassword(username, password, loginToken))
        {
            var user = await userService.GetAsync(username.Trim());

            if (IsAuthorizedUser(user))
            {
                if (IsActivatedUser(user))
                {
                    return await LoginFlow(user);
                }

                indexViewModel.Message = "Your account was suspended";
            }
            else
            {
                indexViewModel.Message = "You have no authorization";
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

        var hashPassword = ToSHA256(user.Password + token);
        return password == hashPassword;
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

        var userIsActiveOnThisSite = false;

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

    [HttpGet]
    public async Task<ActionResult> LogOut(string redirectUrl = "")
    {
        await DoSignOut();

        redirectUrl = string.IsNullOrEmpty(redirectUrl) ? $"{SignInHelper.PathBase}Auth" : redirectUrl;

        return Redirect(redirectUrl);
    }

    #endregion sign out

    #region re check in

    public ActionResult ReCheckIn()
    {
        if (!napSettings.EnableReCheckIn || maintenanceService.IsUnderMaintenance())
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
        var isMaintenance = await maintenanceService.IsUnderMaintenanceAsync();
        if (napSession.OnUserId != 0 && !isMaintenance)
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
                napSettings.Environment)
            .ToList();
    }

    private async Task CreateAuthInfo(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
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
            NetSessionId = HttpContext.Session.Id,
            Ip = GetClientIP(),
            BrowserName = Request.Headers["User-Agent"].ToString(),
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
            clientIP = HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        return clientIP;
    }

    private static RegionInfo ResolveCountry(HttpRequest request)
    {
        var culture = ResolveCulture(request);

        if (culture == null)
        {
            return null;
        }

        try
        {
            return new RegionInfo(culture.LCID);
        }
        catch (CultureNotFoundException)
        {
            return null;
        }
    }

    private static CultureInfo ResolveCulture(HttpRequest request)
    {
        var userLanguages = request.Headers["Accept-Language"].ToString();
        var firstLang = userLanguages.Split(',').FirstOrDefault();
        var defaultLang = string.IsNullOrEmpty(firstLang) ? "en" : firstLang;
        return CultureInfo.CreateSpecificCulture(defaultLang);
    }

    private async Task SaveUserSession(User user, string userName, int onUserId)
    {
        napSession.Level = await userService.GetUserLevelAsync(user.Id, napSettings.SiteId);
        napSession.UserId = user.Id;
        napSession.UserName = userName;
        napSession.OnUserId = onUserId;

        CreateUserMenu(user);
        await CreateAuthInfo(user);
    }


    private static string ToSHA256(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Hey guy! I do not want to hash an empty or white space string");
        }

        using var hashAlgorithm = SHA256.Create();
        var hashData = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        var encrypted = BitConverter.ToString(hashData).Replace("-", string.Empty).ToLower();

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
        var loginToken = Guid.NewGuid().ToString("N");
        FormToken = loginToken;
        AddTokenToCookie(loginToken);
    }

    #endregion

    #region common

    [SkipUMFilter]

    public ActionResult Offline()
    {
        var isDownTime = maintenanceService.IsUnderMaintenance();
        if (!isDownTime)
        {
            return Redirect(SignInHelper.PathBase);
        }

        var downTime = maintenanceService.GetUnderMaintenanceTime();

        var viewModel = new MaintenanceViewModel { UmtFrom = downTime.From, UmtTo = downTime.To };

        return View("Offline", viewModel);
    }

    public ActionResult AccessDenied()
    {
        return View("AccessDenied");
    }

    public ActionResult Error()
    {
        return View("Error");
    }

    public ActionResult RecheckinScript()
    {
        if (!napSettings.EnableReCheckIn)
        {
            return Content(string.Empty, "text/javascript");
        }

        const string text = SignInHelper.ReCheckInScript;

        return Content($"window.rootUrl='{SignInHelper.PathBase}';\r\n{text}", "text/javascript");
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
    public JsonResult ChangePassword([FromBody] UpdatePasswordModel criteria)
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
    private const int TimeExpirationInSecond = 60;

    [HttpGet]
    public async Task<IActionResult> SSO(string userId, string onuserId, string time, string redirectUrl = "")
    {
        var logged = await IsSSOAuthenticated(time);
        if (!logged)
        {
            return Redirect($"{SignInHelper.PathBase}Auth/Index");
        }

        var userIdDecrypt = Cryptography.DecryptPassword(userId.Replace(" ", "+"));
        var onuserIdDecrypt = Cryptography.DecryptPassword(onuserId.Replace(" ", "+"));

        var criteria = CreateSsoLoginParameter(userIdDecrypt, onuserIdDecrypt);
        var ssoLoginOnUserId = await maintenanceService.IsUnderMaintenanceAsync()
        ? criteria.UserId
        : await securityService.SSOLoginAsync(criteria);

        await CreateSSOAuth(userId, ssoLoginOnUserId);

        var nextUrl = string.IsNullOrEmpty(redirectUrl) ? SignInHelper.PathBase : redirectUrl;
        return Redirect(nextUrl);
    }

    public async Task<bool> IsSSOAuthenticated(string time)
    {
        var isMaintenance = await maintenanceService.IsUnderMaintenanceAsync();
        var currentTime = isMaintenance
        ? DateTime.Now.ToString()
        : await securityService.GetTimeAsync();
        var timeDecrypt = Cryptography.DecryptPassword(time.Replace(" ", "+"));
        var expiration = Convert.ToDateTime(currentTime) - Convert.ToDateTime(timeDecrypt);

        return expiration.TotalSeconds < TimeExpirationInSecond;
    }

    private async Task<bool> CreateSSOAuth(string userId, int onUserId)
    {
        var userIdDecrypt = Cryptography.DecryptPassword(userId.Replace(" ", "+"));

        var user = await userService.GetAsync(int.Parse(userIdDecrypt));
        napSession.UserId = user.Id;
        napSession.UserName = user.Username;
        napSession.OnUserId = onUserId;
        napSession.Level = await userService.GetUserLevelAsync(user.Id, napSettings.SiteId);

        if (napSettings.SiteId != 0)
        {
            napSession.MenuItemList = await userService.GetMenuItemAsync(user.Id,
                napSettings.SiteId,
                napSettings.Environment) as IList<MenuItem>;
        }

        await CreateAuthInfo(user);

        return true;
    }

    private SSOLoginParameter CreateSsoLoginParameter(string userId, string onuserId)
    {
        return new SSOLoginParameter
        {
            UserId = Convert.ToInt32(userId),
            SiteId = napSettings.SiteId,
            CheckOnUserId = Convert.ToInt32(onuserId),
            OnUserId = 0,
            LoginDate = DateTime.Now,
            NetSessionId = HttpContext.Session.Id,
            IP = GetClientIP(),
            BrowserName = Request.Headers["User-Agent"].ToString(),
            CountryCode = ResolveCountry(HttpContext.Request)?.GeoId.ToString(CultureInfo.CurrentCulture),
            Domain = HttpContext.Request.Host.ToString(),
            Platform = Environment.OSVersion.Platform.ToString()
        };
    }

    #endregion

    #region Propagation

    [HttpGet]
    [SkipUMFilter]
    public async Task<IActionResult> Propagation(string userId, string onuserId, string time, string redirectUrl = "")
    {
        return await SSO(userId, onuserId, time, redirectUrl);
    }

    #endregion

    #region LoginFlow

    private async Task<ActionResult> LoginFlow(User user)
    {
        var ignoredOtp = !napSettings.EnableOtp
                         || await userService.IgnoredOtpAsync(user.Id, OtpSite);

        if (ignoredOtp)
        {
            return await DefaultLoginFlow(user);
        }

        HttpContext.Session.SetInt32("OtpUserId", user.Id);
        return RedirectToAction("OtpLoginFlow");
    }

    private async Task<ActionResult> DefaultLoginFlow(User user)
    {
        var criteria = SetUserCheckInCriteria(user.Id);
        var onUserId = await maintenanceService.IsUnderMaintenanceAsync()
            ? user.Id
            : await securityService.LoginAsync(criteria);

        await SaveUserSession(user, user.Username, onUserId);
        return Redirect(SignInHelper.PathBase);
    }

    [HttpGet]
    public async Task<ActionResult> OtpLoginFlow()
    {
        var otpUserId = HttpContext.Session.GetInt32("OtpUserId");
        var userId = otpUserId ?? 0;
        if (userId == 0)
        {
            return Redirect(configuration.GetSection("AuthSettings:LoginPath").Value);
        }

        var ignoredOtp = !napSettings.EnableOtp
                         || await userService.IgnoredOtpAsync(userId, OtpSite);

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
        var userId = otpUserId ?? 0;
        var ignoredOtp = !napSettings.EnableOtp
                         || await userService.IgnoredOtpAsync(userId, OtpSite);
        if (userId == 0 || ignoredOtp)
        {
            HttpContext.Session.SetInt32("OtpUserId", 0);
            return RedirectToAction("Index");
        }

        var validOtp = !string.IsNullOrEmpty(otp)
                       && await userService.VerifyOtpAsync(userId, OtpSite, otp);

        if (validOtp)
        {
            HttpContext.Session.SetInt32("OtpUserId", 0);
            var user = await userService.GetAsync(userId);
            return await DefaultLoginFlow(user);
        }

        ViewBag.Message = "Invalid OTP. Please try again";

        return View();
    }

    #endregion

    public ActionResult WhoAmI()
    {
        return Content($"Environment:{Environment.GetEnvironmentVariable("ASPNETCORE_APPL_PATH")}");
    }
}
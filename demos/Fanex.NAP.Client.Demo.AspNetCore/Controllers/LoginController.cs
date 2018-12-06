namespace Fanex.Nap.Client.AspNetCore.DemoSite.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Common;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models.ParameterObject;
    using Fanex.NAP.Client.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class LoginController : Controller
    {
        private readonly INapSettings napSettings;
        private readonly IUserService userService;
        private readonly ISecurityService securityService;
        private readonly INapSession napSession;

        public LoginController(
            IUserService userService,
            ISecurityService securityService,
            INapSettings napSettings,
            INapSession napSession)
        {
            this.userService = userService;
            this.securityService = securityService;
            this.napSession = napSession;
            this.napSettings = napSettings;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
            {
                ViewBag.Message = "Invalid User name or password!";

                return View();
            }

            var user = userService.Get(userName?.Trim());

            if (user != null)
            {
                password = Cryptography.CfsEnCode(password);

                if (password == user.Password)
                {
                    var siteLevelList = userService.GetSiteLevel(user.Id, napSettings.Environment);

                    if (siteLevelList.Any(s => s.Site.Id == napSettings.SiteId))
                    {
                        var loginParam = new LoginParameter
                        {
                            UserId = user.Id,
                            SiteId = napSettings.SiteId,
                            NetSessionId = HttpContext.Session.Id,
                            Ip = HttpContext.Connection.RemoteIpAddress.ToString(),
                            LoginDate = DateTime.Now
                        };

                        var onlineUserId = securityService.Login(loginParam);

                        napSession.UserName = user.Username;
                        napSession.UserId = user.Id;
                        napSession.OnUserId = onlineUserId;
                        napSession.MenuItemList = userService.GetMenuItem(user.Id, napSettings.SiteId,
                            napSettings.Environment, true).ToList();

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Username),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identity));

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            napSession.ClearAll();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }
    }
}
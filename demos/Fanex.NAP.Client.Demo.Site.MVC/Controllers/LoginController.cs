namespace Fanex.NAP.Client.Demo.AspNetMvc.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.NAP.Client.AspNetMVC.Auth;
    using Fanex.NAP.Client.AspNetMVC.Context;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Common;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models.ParameterObject;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Web.Context;
    using Fanex.NAP.Core.Web.Session;

    public class LoginController : Controller
    {
        private readonly IUserService userService;
        private readonly ISecurityService securityService;
        private readonly INapSettings napSettings;
        private readonly INapSession napSession;
        private readonly ICurrentContext currentContext;

        public LoginController() : this(null, null, null, null)
        {
        }

        public LoginController(IUserService userService, INapSettings napSettings, INapSession napSession, ICurrentContext currentContext)
        {
            this.userService = userService ?? new UserService();
            this.napSettings = napSettings ?? new NapSettings();
            this.securityService = securityService ?? new SecurityService();
            this.napSession = napSession ?? new NapSession();
            this.currentContext = currentContext ?? new CurrentContext();
        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(string userName, string password)
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
                    var siteLevelList = await userService.GetSiteLevelAsync(user.Id, napSettings.Environment);

                    if (siteLevelList.Any(s => s.Site.Id == napSettings.SiteId))
                    {
                        var loginParam = new LoginParameter
                        {
                            UserId = user.Id,
                            SiteId = napSettings.SiteId,
                            NetSessionId = HttpContext.Session.SessionID,
                            Ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"],
                            LoginDate = DateTime.Now
                        };

                        var onlineUserId = await securityService.LoginAsync(loginParam);

                        napSession.UserName = user.Username;
                        napSession.UserId = user.Id;
                        napSession.OnUserId = onlineUserId;
                        napSession.MenuItemList = (await userService.GetMenuItemAsync(user.Id, napSettings.SiteId,
                            napSettings.Environment, true)).ToList();

                        string currentDomainPath = currentContext.CurrentDomain.Path;

                        var authInfo = new AuthInfo
                        {
                            UserId = user.Id,
                            OnUserId = onlineUserId
                        };

                        var authTicket = new FormsAuthenticationTicket(
                            1,
                            user.Username,
                            DateTime.Now,
                            DateTime.Now.AddMinutes(20000),
                            false,
                            authInfo.ToXml(),
                            currentDomainPath);

                        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                        {
                            HttpOnly = true,
                            Path = currentDomainPath,
                            Name = FormsAuthentication.FormsCookieName
                        };

                        Response.Cookies.Add(authCookie);

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View();
        }

        public async Task<ActionResult> Logout()
        {
            if (napSession != null && napSession.OnUserId != 0)
            {
                await securityService.LogoutAsync(napSession.OnUserId);
            }

            FormsAuthentication.SignOut();
            Response.Cookies.Clear();
            Session.Abandon();
            napSession.ClearAll();

            return Redirect(FormsAuthentication.LoginUrl);
        }
    }
}
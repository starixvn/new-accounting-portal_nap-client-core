using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Models;
using Fanex.NAP.Client.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

            var user = await userService.GetAsync(username.Trim());
            if (user == null)
            {
                return Content("Invalid Username or Password");
            }

            string hashPassword = SHA256(user.Password + loginToken);
            if (password != hashPassword)
            {
                return Content("Invalid Username or Password");
            }

            await SaveUserSession(user, username, int.MaxValue - 1);

            redirectUrl = WebUtility.UrlDecode(redirectUrl);
            return Redirect(redirectUrl);
        }

        #endregion sign in

        #region sign out

        [AllowAnonymous]
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

        private async Task SaveUserSession(User user, string userName, int onUserId)
        {
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
            var currentTime = await securityService.GetTimeAsync();
            var userIdDecrypt = Data.Cryptography.DecryptPassword(userId.Replace(" ", "+"));
            var onuserIdDecrypt = Data.Cryptography.DecryptPassword(onuserId.Replace(" ", "+"));
            var timeDecrypt = Data.Cryptography.DecryptPassword(time.Replace(" ", "+"));
            var expiration = Convert.ToDateTime(currentTime) - Convert.ToDateTime(timeDecrypt);

            if (expiration.TotalSeconds <= TimeExipration)
            {
                var user = await userService.GetAsync(Convert.ToInt32(userIdDecrypt));
                napSession.UserId = user.Id;
                napSession.UserName = user.Username;
                napSession.OnUserId = Convert.ToInt32(onuserIdDecrypt);
                napSession.MenuItemList = (await userService.GetMenuItemAsync(user.Id, napSettings.SiteId, napSettings.Environment, true)).ToList();

                await CreateAuthInfo(user);

                return true;

            }

            return false;
        }

        #endregion

    }
}

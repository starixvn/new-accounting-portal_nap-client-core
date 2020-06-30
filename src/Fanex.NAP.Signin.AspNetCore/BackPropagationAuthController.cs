using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace Fanex.NAP.Signin.AspNetCore
{
    public class BackPropagationAuthController : Controller
    {
        private readonly IConfiguration configuration;

        public BackPropagationAuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return Redirect($"{GetLegacyUrl()}/Auth/Index");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogOut()
        {
            return Redirect($"{GetLegacyUrl()}/Auth/LogOut");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult WhoAmI()
        {
            return Content($"{Request.PathBase}");
        }

        private string GetLegacyUrl()
        {
            var applicationPath = GetApplicationPath();
            var legacyAuthUrl = configuration.GetSection("AuthSettings:LegacyUrl").Value;
            legacyAuthUrl = string.IsNullOrEmpty(legacyAuthUrl)
                ? string.Empty
                : legacyAuthUrl.Replace("{ApplicationPath}", applicationPath);

            return legacyAuthUrl;
        }

        private string GetApplicationPath()
        {
            var fullUrl = Request.GetDisplayUrl();
            var regex = new Regex(@"NetCoreV\d", RegexOptions.IgnoreCase);
            var matches = regex.Matches(fullUrl);
            if (matches.Count >= 1)
            {
                var applicationPath = matches[0].Value;
                applicationPath = Regex.Replace(applicationPath, "NetCore", string.Empty, RegexOptions.IgnoreCase);

                return applicationPath.ToUpper();
            }

            return string.Empty;
        }
    }
}

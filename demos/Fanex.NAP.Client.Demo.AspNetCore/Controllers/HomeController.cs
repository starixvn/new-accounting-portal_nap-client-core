namespace Fanex.Nap.Client.AspNetCore.DemoSite.Controllers
{
    using System.Diagnostics;
    using Fanex.Nap.Client.AspNetCore.DemoSite.Models;
    using Fanex.NAP.Client.AspNetCore.Attributes;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.Core.Business;
    using Microsoft.AspNetCore.Mvc;

    [NapSuperAuth]
    public class HomeController : Controller
    {
        [NapAuth(FunctionKey = "Home", Policy = Constant.Policy.Read, PermissionCache = true)]
        public IActionResult Index()
        {
            var menuItem = new SiteService().GetMenuItemBySiteId(1, "UAT");
            var group = new GroupService().Get(23);

            return View();
        }

        [NapAuth(FunctionKey = "SourceCode", Policy = Constant.Policy.Host)]
        public IActionResult SourceCode()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [NapAuth(FunctionKey = "Contact", Policy = Constant.Policy.Host)]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
namespace Fanex.Nap.Client.AspNetCore.DemoSite.Controllers
{
    using System.Diagnostics;
    using Fanex.Nap.Client.AspNetCore.DemoSite.Models;
    using Fanex.NAP.Client.AspNetCore.Attributes;
    using Fanex.NAP.Core.Business;
    using Microsoft.AspNetCore.Mvc;

    [NapSuperAuth]
    public class HomeController : Controller
    {
        [NapAuth(FunctionKey = "Dashboard", Policy = Constant.Policy.Read, PermissionCache = true)]
        public IActionResult Index()
        {
            return View();
        }

        [NapAuth(FunctionKey = "Dashboard", Policy = Constant.Policy.Read)]
        public IActionResult SourceCode()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [NapAuth(FunctionKey = "Dashboard", Policy = Constant.Policy.Read)]
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
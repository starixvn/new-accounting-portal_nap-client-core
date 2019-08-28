namespace Fanex.NAP.Client.Demo.AspNetMvc.Controllers
{
    using System.Web.Mvc;
    using Fanex.NAP.Client.AspNetMVC.Attributes;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    [NapSuperAuth]
    public class HomeController : Controller
    {
        [NapAuth(FunctionKey = "Home", Policy = Constant.Policy.Read, PermissionCache = true)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
namespace Fanex.Nap.Client.AspNetCore.DemoSite.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class CommonController : Controller
    {
        public IActionResult Unauthorize()
        {
            return View();
        }
    }
}
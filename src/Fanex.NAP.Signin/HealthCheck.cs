namespace Fanex.NAP.SignIn;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ONEbook.UM.AspNetCore.Filters;

[AllowAnonymous]
public class HealthCheck : Controller
{
    [SkipUMFilter]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public ActionResult Index()
    {
        return Content("OK");
    }
}
namespace Fanex.NAP.SignIn;

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

    [SkipUMFilter]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public ActionResult Redis()
    {
        HttpContext.Session.SetString("Now", DateTimeOffset.Now.ToString());

        return Content(HttpContext.Session.GetString("Now"));
    }
}
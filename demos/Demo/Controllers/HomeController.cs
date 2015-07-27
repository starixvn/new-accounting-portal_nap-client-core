namespace Demo.Controllers;

using System.Diagnostics;
using Fanex.NAP.Client.Attributes;
using Fanex.NAP.Client.Helpers.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;

[Authorize]
[SessionAuthorize]
public class HomeController : Controller
{
    [NapAuth(FunctionKey = "Dashboard", Policy = Constant.Policy.Read)]
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
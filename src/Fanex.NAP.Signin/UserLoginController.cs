namespace Fanex.NAP.Signin.AspNetCore;

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class UserLoginController : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult SSO(string userId, string onuserId, string time, Uri redirectUrl = null)
    {
        return RedirectToAction("SSO", "Auth", new { userId, onuserId, time, redirectUrl });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        return RedirectToAction("Index", "Auth");
    }
}
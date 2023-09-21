namespace Fanex.NAP.Client.Demo.AspNetCore.Controllers;

using System;
using Client.Models.ParameterObject;
using Microsoft.AspNetCore.Mvc;
using Services;

public class SecurityClientController : Controller
{
    private readonly SecurityService securityService;

    public SecurityClientController()
    {
        securityService = new SecurityService();
    }

    public int Login(int userId, int siteId)
    {
        var onUserId = securityService.Login(new LoginParameter
        {
            UserId = userId,
            SiteId = siteId,
            LoginDate = DateTime.Now,
            NetSessionId = "123123",
            Ip = "10.19.10.49"
        });

        return onUserId;
    }

    public void Logout(int onlineUserId)
    {
        securityService.Logout(onlineUserId);
    }

    public int ReCheckIn(int onlineUserId, int siteId) => securityService.ReCheckIn(onlineUserId, siteId);

    public string GetTime() => securityService.GetTime();

    public int SSOLogin(int userId, int siteId)
    {
        var onUserId = securityService.SSOLogin(new SSOLoginParameter
        {
            UserId = userId,
            SiteId = siteId,
            LoginDate = DateTime.Now,
            NetSessionId = "123123",
            OnUserId = 233424
        });

        return onUserId;
    }

    public bool UpdatePassword(int siteId, int userId, string password) =>
        securityService.UpdatePassword(siteId, userId, password);
}
using System;
using Fanex.NAP.Core.Context;
using Fanex.NAP.Core.Models.ParameterObject;
using Fanex.NAP.Core.Service.Impl;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Nap.Client.AspNetCore.Controllers
{
    public class SecurityClientController : Controller
    {
        private readonly SecurityService _securityService;
        private readonly CurrentContext _currentContext;

        public SecurityClientController()
        {
            _securityService = new SecurityService();
            _currentContext = new CurrentContext();
        }

        public int Login(int userId, int siteId)
        {
            var onUserId = _securityService.Login(new LoginParameter
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
            _securityService.Logout(onlineUserId);
        }

        public int ReCheckIn(int onlineUserId, int siteId) => _securityService.ReCheckIn(onlineUserId, siteId);

        public string GetTime() => _securityService.GetTime();

        public int SSOLogin(int userId, int siteId)
        {
            var onUserId = _securityService.SSOLogin(new SSOLoginParameter
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
            _securityService.UpdatePassword(siteId, userId, password);
    }
}
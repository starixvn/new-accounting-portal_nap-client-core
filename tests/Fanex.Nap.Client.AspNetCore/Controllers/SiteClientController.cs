using System.Collections.Generic;
using Fanex.NAP.Core.Models;
using Fanex.NAP.Core.Models.ValueObject;
using Fanex.NAP.Core.Service.Impl;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Nap.Client.AspNetCore.Controllers
{
    public class SiteClientController : Controller
    {
        private readonly SiteService _siteService;

        public SiteClientController()
        {
            _siteService = new SiteService();
        }

        public bool IsActivated(int id) => _siteService.IsActivated(id);

        public IEnumerable<SiteUrl> GetUrlById(int id) => _siteService.GetUrlById(id, "LIVE");

        public IEnumerable<MenuItem> GetMenuItemBySiteId(int id) => _siteService.GetMenuItemBySiteId(id, "UAT");
    }
}
namespace Fanex.Nap.Client.AspNetCore.Controllers
{
    using System.Collections.Generic;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ValueObject;
    using Fanex.NAP.Client.Services;
    using Microsoft.AspNetCore.Mvc;

    public class SiteClientController : Controller
    {
        private readonly SiteService siteService;

        public SiteClientController()
        {
            siteService = new SiteService();
        }

        public bool IsActivated(int id) => siteService.IsActivated(id);

        public IEnumerable<SiteUrl> GetUrlById(int id) => siteService.GetUrlById(id, "LIVE");

        public IEnumerable<MenuItem> GetMenuItemBySiteId(int id) => siteService.GetMenuItemBySiteId(id, "UAT");
    }
}
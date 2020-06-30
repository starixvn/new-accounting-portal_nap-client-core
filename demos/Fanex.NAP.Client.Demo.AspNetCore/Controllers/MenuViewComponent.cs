namespace Fanex.Nap.Client.AspNetCore.DemoSite.Controllers
{
    using Fanex.Nap.Client.AspNetCore.DemoSite.Models;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    public class MenuViewComponent : ViewComponent
    {
        private readonly INapSession napSession;

        public MenuViewComponent(INapSession napSession)
        {
            this.napSession = napSession;
        }

        public IViewComponentResult Invoke()
        {
            var menuItems = napSession.MenuItemList?.OrderBy(item => item.DisplayOrder).ToList();
            var userName = napSession.UserName;

            return View(new MenuViewModel
            {
                MemuItems = menuItems ?? new List<MenuItem>(),
                UserName = userName
            });
        }
    }
}
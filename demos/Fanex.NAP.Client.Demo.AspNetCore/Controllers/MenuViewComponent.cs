namespace Fanex.Nap.Client.AspNetCore.DemoSite.Controllers
{
    using System.Linq;
    using Fanex.Nap.Client.AspNetCore.DemoSite.Models;
    using Fanex.NAP.Client.Helpers.Web;
    using Microsoft.AspNetCore.Mvc;

    public class MenuViewComponent : ViewComponent
    {
        private readonly INapSession napSession;

        public MenuViewComponent(INapSession napSession)
        {
            this.napSession = napSession;
        }

        public IViewComponentResult Invoke()
        {
            var menuItems = napSession.MenuItemList?.OrderBy(item => item.DisplayOrder);
            var userName = napSession.UserName;

            return View(new MenuViewModel
            {
                MemuItems = menuItems,
                UserName = userName
            });
        }
    }
}
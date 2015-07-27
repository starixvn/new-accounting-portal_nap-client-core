namespace Demo.Controllers;

using System.Linq;
using Fanex.NAP.Client;
using Microsoft.AspNetCore.Mvc;
using Models;

public class MenuViewComponent(INapSession napSession) : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var menuItems = napSession.MenuItemList?.OrderBy(item => item.DisplayOrder).ToList() ?? [];
        var userName = napSession.UserName;

        return View(new MenuViewModel
        {
            MenuItems = menuItems,
            UserName = userName
        });
    }
}
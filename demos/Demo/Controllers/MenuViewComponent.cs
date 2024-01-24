namespace Demo.Controllers;

using System.Collections.Generic;
using System.Linq;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Models;

public class MenuViewComponent : ViewComponent
{
    private readonly INapSession napSession;

    public MenuViewComponent(INapSession napSession)
    {
        this.napSession = napSession;
    }

    public IViewComponentResult Invoke()
    {
        var menuItems = napSession.MenuItemList?.OrderBy(item => item.DisplayOrder).ToList() ?? new List<MenuItem>();
        var userName = napSession.UserName;

        return View(new MenuViewModel
        {
            MenuItems = menuItems,
            UserName = userName
        });
    }
}
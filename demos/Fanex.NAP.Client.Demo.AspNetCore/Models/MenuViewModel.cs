namespace Fanex.NAP.Client.Demo.AspNetCore.Models;

using System.Collections.Generic;
using Client.Models;

public class MenuViewModel
{
    public IList<MenuItem> MenuItems { get; set; }

    public string UserName { get; set; }
}
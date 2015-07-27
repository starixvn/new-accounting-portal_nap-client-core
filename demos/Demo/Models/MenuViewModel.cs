namespace Demo.Models;

using System.Collections.Generic;
using Fanex.NAP.Client.Models;

public class MenuViewModel
{
    public IList<MenuItem> MenuItems { get; set; }

    public string UserName { get; set; }
}
namespace Fanex.Nap.Client.AspNetCore.DemoSite.Models
{
    using System.Collections.Generic;
    using Fanex.NAP.Client.Models;

    public class MenuViewModel
    {
        public IEnumerable<MenuItem> MemuItems { get; set; }

        public string UserName { get; set; }
    }
}
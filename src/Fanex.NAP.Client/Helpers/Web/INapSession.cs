namespace Fanex.NAP.Client.Helpers.Web
{
    using System.Collections.Generic;
    using Models;

    public interface INapSession
    {
        string UserName { get; set; }

        int UserId { get; set; }

        int Level { get; set; }

        int OnUserId { get; set; }

        IList<MenuItem> MenuItemList { get; set; }

        void ClearAll();
    }
}
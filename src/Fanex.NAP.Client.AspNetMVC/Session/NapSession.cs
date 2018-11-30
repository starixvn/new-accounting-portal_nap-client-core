namespace Fanex.NAP.Core.Web.Session
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Core.Configuration;

    public class NapSession : INapSession
    {
        #region const

        public const string InternalUsername = "InternalUsername";

        public const string InternalUserId = "InternalUserId";

        public const string InternalOnUserId = "InternalOnUserId";

        public const string InternalNetSessionId = "InternalNetSessionId";

        public const string InternalLevel = "InternalLevel";

        public const string InternalMenuItemList = "InternalMenuItemList";

        #endregion const

        private readonly INapSettings napSettings;

        public NapSession(INapSettings napSettings = null)
        {
            this.napSettings = napSettings ?? new NapSettings();
        }

        public string UserName
        {
            get
            {
                return
                    (HttpContext.Current != null)
                    ? Convert.ToString(HttpContext.Current.Session[InternalUsername])
                    : string.Empty;
            }

            set
            {
                HttpContext.Current.Session[InternalUsername] = value;
            }
        }

        public int UserId
        {
            get
            {
                return
                    (HttpContext.Current != null)
                    ? Convert.ToInt32(HttpContext.Current.Session[InternalUserId])
                    : 0;
            }

            set
            {
                HttpContext.Current.Session[InternalUserId] = value;
            }
        }

        public int OnUserId
        {
            get
            {
                return
                (HttpContext.Current != null)
                ? Convert.ToInt32(HttpContext.Current.Session[InternalOnUserId])
                : 0;
            }

            set
            {
                HttpContext.Current.Session[InternalOnUserId] = value;
            }
        }

        public string SessionId
        {
            get
            {
                return
                    (HttpContext.Current != null)
                    ? Convert.ToString(HttpContext.Current.Session[InternalNetSessionId])
                    : string.Empty;
            }

            set
            {
                HttpContext.Current.Session[InternalNetSessionId] = value;
            }
        }

        public IList<MenuItem> MenuItemList
        {
            get
            {
                var menuItemList = HttpContext.Current.Session[InternalMenuItemList] as Dictionary<int, List<MenuItem>>;

                if (menuItemList == null)
                {
                    // ensure menu should be empty instead of null
                    menuItemList = new Dictionary<int, List<MenuItem>>();
                    HttpContext.Current.Session[InternalMenuItemList] = menuItemList;
                }

                return menuItemList.ContainsKey(napSettings.SiteId) ? menuItemList[napSettings.SiteId] : null;
            }

            set
            {
                var menuItemList = HttpContext.Current.Session[InternalMenuItemList] as Dictionary<int, List<MenuItem>>;

                if (menuItemList == null)
                {
                    // ensure menu should be empty instead of null
                    menuItemList = new Dictionary<int, List<MenuItem>>();
                    HttpContext.Current.Session[InternalMenuItemList] = menuItemList;
                }

                var siteMenuItemList = value as List<MenuItem>;

                if (menuItemList.ContainsKey(napSettings.SiteId))
                {
                    menuItemList[napSettings.SiteId] = siteMenuItemList;
                }
                else
                {
                    menuItemList.Add(napSettings.SiteId, siteMenuItemList);
                }

                HttpContext.Current.Session[InternalMenuItemList] = menuItemList;
            }
        }

        public int Level
        {
            get
            {
                return
                    (HttpContext.Current != null)
                    ? Convert.ToInt32(HttpContext.Current.Session[InternalLevel])
                    : 0;
            }

            set
            {
                HttpContext.Current.Session[InternalLevel] = value;
            }
        }

        public void ClearAll()
        {
            if (!object.ReferenceEquals(HttpContext.Current, null))
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
            }
        }
    }
}
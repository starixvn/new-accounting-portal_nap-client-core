namespace Fanex.NAP.Client.Clients
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    internal interface ISiteClient
    {
        Task<IEnumerable<MenuItem>> GetMenuItemBySiteId(int siteId, string env, bool? isActive = null);
    }

    internal class SiteClient : BaseClient, ISiteClient
    {
        public async Task<IEnumerable<MenuItem>> GetMenuItemBySiteId(int siteId, string env, bool? isActive = null)
        {
            return await Get<IEnumerable<MenuItem>>(CreateRequestUri(string.Format(RelativePaths.GetMenuItemBySiteId, siteId), new { environment = env, isActive }));
        }

        private static class RelativePaths
        {
            public const string GetMenuItemBySiteId = "site/{0}/menuItem";
        }
    }
}
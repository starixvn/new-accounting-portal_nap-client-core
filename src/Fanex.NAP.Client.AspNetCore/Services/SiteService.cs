namespace Fanex.NAP.Client.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Clients;
    using Models;

    public interface ISiteService
    {
        IEnumerable<MenuItem> GetMenuItemBySiteId(int siteId, string environment, bool? isActive = null);

        Task<IEnumerable<MenuItem>> GetMenuItemBySiteIdAsync(int siteId, string environment, bool? isActive = null);
    }

    public class SiteService : ISiteService
    {
        public IEnumerable<MenuItem> GetMenuItemBySiteId(int siteId, string environment, bool? isActive = null)
            => GetMenuItemBySiteIdAsync(siteId, environment, isActive).Result;

        public async Task<IEnumerable<MenuItem>> GetMenuItemBySiteIdAsync(int siteId, string environment, bool? isActive = null)
        {
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            Check.Argument.IsNotEmpty(environment, "environment");
            var siteClient = new SiteClient();
            return await siteClient.GetMenuItemBySiteId(siteId, environment, isActive).ConfigureAwait(false);
        }
    }
}
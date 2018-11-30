namespace Fanex.NAP.Client.Clients
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Models.ValueObject;
    using Models;

    internal interface ISiteClient
    {
        Task<bool> IsActivated(int siteId);

        Task<IEnumerable<SiteUrl>> GetUrlById(int siteId, string env);

        Task<IEnumerable<MenuItem>> GetMenuItemBySiteId(int siteId, string env, bool? isActive = null);
    }

    internal class SiteClient : BaseClient, ISiteClient
    {
        public SiteClient() : base(null)
        {
        }

        public async Task<bool> IsActivated(int siteId)
            => await Get<bool>(CreateRequestUri(string.Format(RelativePaths.IsActivated, siteId))).ConfigureAwait(false);

        public async Task<IEnumerable<SiteUrl>> GetUrlById(int siteId, string env)
            => await Get<IEnumerable<SiteUrl>>(
                    CreateRequestUri(string.Format(RelativePaths.GetUrlById, siteId),
                    new { environment = env }))
                .ConfigureAwait(false);

        public async Task<IEnumerable<MenuItem>> GetMenuItemBySiteId(int siteId, string env, bool? isActive = null)
        {
            return await Get<IEnumerable<MenuItem>>(
                    CreateRequestUri(string.Format(RelativePaths.GetMenuItemBySiteId, siteId),
                    new { environment = env, isActive }))
                .ConfigureAwait(false);
        }

        private static class RelativePaths
        {
            public const string IsActivated = "/site/{0}/status";
            public const string GetUrlById = "/site/{0}/siteUrl";
            public const string GetMenuItemBySiteId = "/site/{0}/menuItem";
        }
    }
}
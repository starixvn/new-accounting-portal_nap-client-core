namespace Fanex.NAP.Client.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Clients;
    using Fanex.NAP.Client.Models.ValueObject;
    using Models;

    public interface ISiteService
    {
        IEnumerable<SiteUrl> GetUrlById(int siteId, string environment);

        Task<IEnumerable<SiteUrl>> GetUrlByIdAsync(int siteId, string environment);

        bool IsActivated(int siteId);

        Task<bool> IsActivatedAsync(int siteId);

        IEnumerable<MenuItem> GetMenuItemBySiteId(int siteId, string enviroment, bool? isActive = null);

        Task<IEnumerable<MenuItem>> GetMenuItemBySiteIdAsync(int siteId, string enviroment, bool? isActive = null);
    }

    public class SiteService : ISiteService
    {
        private readonly ISiteClient siteClient;

        public SiteService() : this(null)
        {
        }

        internal SiteService(ISiteClient siteClient)
        {
            this.siteClient = siteClient ?? new SiteClient();
        }

        public bool IsActivated(int siteId) => IsActivatedAsync(siteId).Result;

        public async Task<bool> IsActivatedAsync(int siteId)
        {
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");

            return await siteClient.IsActivated(siteId).ConfigureAwait(false);
        }

        public IEnumerable<SiteUrl> GetUrlById(int siteId, string environment) => GetUrlByIdAsync(siteId, environment).Result;

        public async Task<IEnumerable<SiteUrl>> GetUrlByIdAsync(int siteId, string environment)
        {
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            Check.Argument.IsNotEmpty(environment, "environment");

            return await siteClient.GetUrlById(siteId, environment).ConfigureAwait(false);
        }

        public IEnumerable<MenuItem> GetMenuItemBySiteId(int siteId, string enviroment, bool? isActive = null)
            => GetMenuItemBySiteIdAsync(siteId, enviroment, isActive).Result;

        public async Task<IEnumerable<MenuItem>> GetMenuItemBySiteIdAsync(int siteId, string enviroment, bool? isActive = null)
        {
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            Check.Argument.IsNotEmpty(enviroment, "enviroment");

            return await siteClient.GetMenuItemBySiteId(siteId, enviroment, isActive).ConfigureAwait(false);
        }
    }
}
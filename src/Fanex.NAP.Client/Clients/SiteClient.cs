namespace Fanex.NAP.Client.Clients;

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Models;

public interface ISiteClient
{
    Task<IEnumerable<MenuItem>> GetMenuItemBySiteId(int siteId, string env, bool? isActive = null);
}

public class SiteClient(HttpClient httpClient) : BaseClient(httpClient), ISiteClient
{
    public async Task<IEnumerable<MenuItem>> GetMenuItemBySiteId(int siteId, string env, bool? isActive = null)
    {
        return await Get<IEnumerable<MenuItem>>(CreateRequestPath(string.Format(RelativePaths.GetMenuItemBySiteId, siteId), new { environment = env, isActive }));
    }

    private static class RelativePaths
    {
        public const string GetMenuItemBySiteId = "site/{0}/menuItem";
    }
}
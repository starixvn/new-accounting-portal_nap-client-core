namespace Fanex.NAP.Client.Clients
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface ICheckinClient
    {
        Task<int> ReCheckIn(int onlineUserId, int siteId);
    }

    public class CheckinClient : BaseClient, ICheckinClient
    {
        public CheckinClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<int> ReCheckIn(int onlineUserId, int siteId)
        {
            return await Get<int>(CreateRequestPath(RelativePaths.ReCheckIn, new { onUserId = onlineUserId, siteId }));
        }

        private static class RelativePaths
        {
            public const string ReCheckIn = "recheckin";
        }
    }
}
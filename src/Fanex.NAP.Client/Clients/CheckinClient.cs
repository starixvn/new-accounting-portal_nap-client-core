namespace Fanex.NAP.Client.Clients
{
    using System.Threading.Tasks;

    internal interface ICheckinClient
    {
        Task<int> ReCheckIn(int onlineUserId, int siteId);
    }

    internal class CheckinClient : BaseClient, ICheckinClient
    {
        public async Task<int> ReCheckIn(int onlineUserId, int siteId)
        {
            return await Get<int>(CreateRequestUri(RelativePaths.ReCheckIn, new { onUserId = onlineUserId, siteId = siteId }));
        }

        private class RelativePaths
        {
            public const string ReCheckIn = "/recheckin/";
        }
    }
}
namespace Fanex.NAP.Client.Clients
{
    using System.Threading.Tasks;

    internal interface ICheckinClient
    {
        Task<int> ReCheckIn(int onlineUserId, int siteId);
    }

    internal class CheckinClient : BaseClient, ICheckinClient
    {
        public CheckinClient() : base(null)
        {
        }

        public async Task<int> ReCheckIn(int onlineUserId, int siteId)
            => await Get<int>(
                CreateRequestUri(RelativePaths.ReCheckIn, new { onUserId = onlineUserId, siteId }))
            .ConfigureAwait(false);

        private static class RelativePaths
        {
            public const string ReCheckIn = "/recheckin/";
        }
    }
}
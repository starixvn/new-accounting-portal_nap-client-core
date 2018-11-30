namespace Fanex.NAP.Client.Clients
{
    using System.Threading.Tasks;

    internal interface ILogoutClient
    {
        Task Logout(int onlineUserId);
    }

    internal class LogoutClient : BaseClient, ILogoutClient
    {
        public LogoutClient() : base(null)
        {
        }

        public async Task Logout(int onlineUserId)
            => await Post(CreateRequestUri(RelativePaths.Logout), onlineUserId).ConfigureAwait(false);

        private static class RelativePaths
        {
            public const string Logout = "/logout/";
        }
    }
}
namespace Fanex.NAP.Client.Clients
{
    using System.Net.Http;
    using System.Threading.Tasks;

    internal interface ILogoutClient
    {
        Task Logout(int onlineUserId);
    }

    internal class LogoutClient : BaseClient, ILogoutClient
    {
        public async Task Logout(int onlineUserId)
        {
            await Send<int>(CreateRequestUri(RelativePaths.Logout), HttpMethod.Post, onlineUserId);
        }

        private class RelativePaths
        {
            public const string Logout = "/logout/";
        }
    }
}
namespace Fanex.NAP.Client.Clients
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface ILogoutClient
    {
        Task Logout(int onlineUserId);
    }

    public class LogoutClient : BaseClient, ILogoutClient
    {
        public LogoutClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task Logout(int onlineUserId)
        {
            await Get<int>(CreateRequestPath(string.Format(RelativePaths.Logout, onlineUserId)));
        }

        private static class RelativePaths
        {
            public const string Logout = "logout/{0}";
        }
    }
}
namespace Fanex.NAP.Client.Clients
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models.ParameterObject;

    public interface ISSOClient
    {
        Task<string> GetTime();

        Task<int> Login(SSOLoginParameter parameter);
    }

    public class SSOClient : BaseClient, ISSOClient
    {
        public SSOClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<string> GetTime()
        {
            return await Get<string>(CreateRequestPath(RelativePaths.GetTime));
        }

        public async Task<int> Login(SSOLoginParameter parameter)
        {
            return await Post<SSOLoginParameter, int>(CreateRequestPath(RelativePaths.Login), parameter);
        }

        private static class RelativePaths
        {
            public const string GetTime = "ssologin";
            public const string Login = "ssologin";
        }
    }
}
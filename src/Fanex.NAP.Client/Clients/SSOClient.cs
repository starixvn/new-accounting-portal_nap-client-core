namespace Fanex.NAP.Client.Clients
{
    using Fanex.NAP.Client.Models.ParameterObject;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal interface ISSOClient
    {
        Task<string> GetTime();

        Task<int> Login(SSOLoginParameter parameter);
    }

    internal class SSOClient : BaseClient, ISSOClient
    {
        public async Task<string> GetTime()
        {
            return await Get<string>(CreateRequestUri(RelativePaths.GetTime));
        }

        public async Task<int> Login(SSOLoginParameter parameter)
        {
            return await Send<SSOLoginParameter, int>(CreateRequestUri(RelativePaths.Login), HttpMethod.Post, parameter);
        }

        private class RelativePaths
        {
            public const string GetTime = "/ssologin/";
            public const string Login = "/ssologin/";
        }
    }
}
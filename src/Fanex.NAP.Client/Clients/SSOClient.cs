namespace Fanex.NAP.Client.Clients
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Models.ParameterObject;

    internal interface ISSOClient
    {
        Task<string> GetTime();

        Task<int> Login(SSOLoginParameter parameter);
    }

    internal class SSOClient : BaseClient, ISSOClient
    {
        public SSOClient() : base(null)
        {
        }

        public async Task<string> GetTime()
        {
            return await Get<string>(CreateRequestUri(RelativePaths.GetTime)).ConfigureAwait(false);
        }

        public async Task<int> Login(SSOLoginParameter parameter)
        {
            return await Post<SSOLoginParameter, int>(CreateRequestUri(RelativePaths.Login), parameter).ConfigureAwait(false);
        }

        private static class RelativePaths
        {
            public const string GetTime = "/ssologin/";
            public const string Login = "/ssologin/";
        }
    }
}
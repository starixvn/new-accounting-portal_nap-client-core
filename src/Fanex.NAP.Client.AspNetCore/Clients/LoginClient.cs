namespace Fanex.NAP.Client.Clients
{
    using Fanex.NAP.Client.Models.ParameterObject;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal interface ILoginClient
    {
        Task<int> Login(LoginParameter parameter);
    }

    internal class LoginClient : BaseClient, ILoginClient
    {
        public async Task<int> Login(LoginParameter parameter)
        {
            return await Send<LoginParameter, int>(CreateRequestUri(RelativePaths.Login), HttpMethod.Post, parameter);
        }

        private class RelativePaths
        {
            public const string Login = "/login/";
        }
    }
}
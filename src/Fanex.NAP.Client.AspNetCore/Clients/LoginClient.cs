namespace Fanex.NAP.Client.Clients
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Models.ParameterObject;

    internal interface ILoginClient
    {
        Task<int> Login(LoginParameter parameter);
    }

    internal class LoginClient : BaseClient, ILoginClient
    {
        public async Task<int> Login(LoginParameter parameter)
        {
            return await Post<LoginParameter, int>(CreateRequestUri(RelativePaths.Login), parameter);
        }

        private class RelativePaths
        {
            public const string Login = "login";
        }
    }
}
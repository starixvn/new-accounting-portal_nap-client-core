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
        public LoginClient() : base(null)
        {
        }

        public async Task<int> Login(LoginParameter parameter)
            => await Post<LoginParameter, int>(CreateRequestUri(RelativePaths.Login), parameter).ConfigureAwait(false);

        private static class RelativePaths
        {
            public const string Login = "/login/";
        }
    }
}
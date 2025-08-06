namespace Fanex.NAP.Client.Clients
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models.ParameterObject;

    public interface ILoginClient
    {
        Task<int> Login(LoginParameter parameter);
    }

    public class LoginClient : BaseClient, ILoginClient
    {
        public LoginClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<int> Login(LoginParameter parameter)
        {
            return await Post<LoginParameter, int>(CreateRequestPath(RelativePaths.Login), parameter);
        }

        private static class RelativePaths
        {
            public const string Login = "login";
        }
    }
}
namespace Fanex.NAP.Client.Services
{
    using System.Threading.Tasks;
    using Clients;
    using Helpers.Common;
    using Models.ParameterObject;

    public interface ISecurityService
    {
        string GetTime();

        Task<string> GetTimeAsync();

        int Login(LoginParameter parameter);

        Task<int> LoginAsync(LoginParameter parameter);

        void Logout(int onlineUserId);

        Task LogoutAsync(int onlineUserId);

        int ReCheckIn(int onlineUserId, int siteId);

        Task<int> ReCheckInAsync(int onlineUserId, int siteId);

        int SSOLogin(SSOLoginParameter parameter);

        Task<int> SSOLoginAsync(SSOLoginParameter parameter);

        bool UpdatePassword(int siteId, int userId, string password);

        Task<bool> UpdatePasswordAsync(int siteId, int userId, string password);
    }

    public class SecurityService : ISecurityService
    {
        public int Login(LoginParameter parameter) => LoginAsync(parameter).Result;

        public async Task<int> LoginAsync(LoginParameter parameter)
        {
            Check.Argument.IsNotNull(parameter, "parameter");
            var loginClient = new LoginClient();
            return await loginClient.Login(parameter).ConfigureAwait(false);
        }

        public void Logout(int onlineUserId) => LogoutAsync(onlineUserId).Wait();

        public async Task LogoutAsync(int onlineUserId)
        {
            Check.Argument.IsNotNegativeOrZero(onlineUserId, "onlineUserId");
            var logoutClient = new LogoutClient();
            await logoutClient.Logout(onlineUserId).ConfigureAwait(false);
        }

        public int ReCheckIn(int onlineUserId, int siteId) => ReCheckInAsync(onlineUserId, siteId).Result;

        public async Task<int> ReCheckInAsync(int onlineUserId, int siteId)
        {
            Check.Argument.IsNotNegativeOrZero(onlineUserId, "onlineUserId");
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            var checkinClient = new CheckinClient();
            return await checkinClient.ReCheckIn(onlineUserId, siteId).ConfigureAwait(false);
        }

        public string GetTime() => GetTimeAsync().Result;

        public async Task<string> GetTimeAsync()
        {
            var ssoClient = new SSOClient();
            return await ssoClient.GetTime().ConfigureAwait(false);
        }

        public int SSOLogin(SSOLoginParameter parameter) => SSOLoginAsync(parameter).Result;

        public async Task<int> SSOLoginAsync(SSOLoginParameter parameter)
        {
            Check.Argument.IsNotNull(parameter, "parameter");
            var ssoClient = new SSOClient();
            return await ssoClient.Login(parameter).ConfigureAwait(false);
        }

        public bool UpdatePassword(int siteId, int userId, string password)
            => UpdatePasswordAsync(siteId, userId, password).Result;

        public async Task<bool> UpdatePasswordAsync(int siteId, int userId, string password)
        {
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            Check.Argument.IsNotNegativeOrZero(userId, "parameter");
            Check.Argument.IsNotEmpty(password, "password");
            var userInfoClient = new UserInfoClient();
            return await userInfoClient.UpdatePassword(siteId, userId, password).ConfigureAwait(false);
        }
    }
}
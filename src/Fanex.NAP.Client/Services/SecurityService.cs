namespace Fanex.NAP.Client.Services
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Clients;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Models.ParameterObject;

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

        bool UpdateProfile(int siteId, UpdateUserInfoParameter parameter);

        Task<bool> UpdateProfileAsync(int siteId, UpdateUserInfoParameter parameter);
    }

    public class SecurityService : ISecurityService
    {
        private readonly ILoginClient loginClient;
        private readonly ILogoutClient logoutClient;
        private readonly ICheckinClient checkinClient;
        private readonly ISSOClient ssoClient;
        private readonly IUserInfoClient userInfoClient;

        public SecurityService() : this(null, null, null, null, null)
        {
        }

        internal SecurityService(
            ILoginClient loginClient,
            ILogoutClient logoutClient,
            ICheckinClient checkinClient,
            ISSOClient ssoClient,
            IUserInfoClient userInfoClient)
        {
            this.loginClient = loginClient ?? new LoginClient();
            this.logoutClient = logoutClient ?? new LogoutClient();
            this.checkinClient = checkinClient ?? new CheckinClient();
            this.ssoClient = ssoClient ?? new SSOClient();
            this.userInfoClient = userInfoClient ?? new UserInfoClient();
        }

        public int Login(LoginParameter parameter) => LoginAsync(parameter).Result;

        public async Task<int> LoginAsync(LoginParameter parameter)
        {
            Check.Argument.IsNotNull(parameter, "parameter");

            return await loginClient.Login(parameter).ConfigureAwait(false);
        }

        public void Logout(int onlineUserId) => LogoutAsync(onlineUserId).Wait();

        public async Task LogoutAsync(int onlineUserId)
        {
            Check.Argument.IsNotNegativeOrZero(onlineUserId, "onlineUserId");

            await logoutClient.Logout(onlineUserId).ConfigureAwait(false);
        }

        public int ReCheckIn(int onlineUserId, int siteId) => ReCheckInAsync(onlineUserId, siteId).Result;

        public async Task<int> ReCheckInAsync(int onlineUserId, int siteId)
        {
            Check.Argument.IsNotNegativeOrZero(onlineUserId, "onlineUserId");
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");

            return await checkinClient.ReCheckIn(onlineUserId, siteId).ConfigureAwait(false);
        }

        public string GetTime() => GetTimeAsync().Result;

        public async Task<string> GetTimeAsync() => await ssoClient.GetTime().ConfigureAwait(false);

        public int SSOLogin(SSOLoginParameter parameter) => SSOLoginAsync(parameter).Result;

        public async Task<int> SSOLoginAsync(SSOLoginParameter parameter)
        {
            Check.Argument.IsNotNull(parameter, "parameter");

            return await ssoClient.Login(parameter).ConfigureAwait(false);
        }

        public bool UpdatePassword(int siteId, int userId, string password)
            => UpdatePasswordAsync(siteId, userId, password).Result;

        public async Task<bool> UpdatePasswordAsync(int siteId, int userId, string password)
        {
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            Check.Argument.IsNotNegativeOrZero(userId, "parameter");
            Check.Argument.IsNotEmpty(password, "password");

            return await userInfoClient.UpdatePassword(siteId, userId, password).ConfigureAwait(false);
        }

        public bool UpdateProfile(int siteId, UpdateUserInfoParameter parameter)
            => UpdateProfileAsync(siteId, parameter).Result;

        public async Task<bool> UpdateProfileAsync(int siteId, UpdateUserInfoParameter parameter)
        {
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            Check.Argument.IsNotNull(parameter, "parameter");

            return await userInfoClient.UpdateProfile(siteId, parameter).ConfigureAwait(false);
        }
    }
}
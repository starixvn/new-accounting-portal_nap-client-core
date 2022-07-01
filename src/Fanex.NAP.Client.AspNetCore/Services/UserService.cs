namespace Fanex.NAP.Client.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Clients;
    using Fanex.NAP.Core.Business;
    using Models;
    using Models.ValueObject;

    public interface IUserService
    {
        User Get(int userId);

        Task<User> GetAsync(int userId);

        User Get(string username);

        Task<User> GetAsync(string username);

        int GetUserLevel(int userId, int siteId);

        Task<int> GetUserLevelAsync(int userId, int siteId);

        IEnumerable<MenuItem> GetMenuItem(int userId, int siteId, string environment = Constant.UAT, bool? isActive = null);

        Task<IEnumerable<MenuItem>> GetMenuItemAsync(int userId, int siteId, string environment = Constant.UAT, bool? isActive = null);

        int GetPermission(int userId, int siteId, string functionKey);

        Task<int> GetPermissionAsync(int userId, int siteId, string functionKey);

        IEnumerable<SiteLevel> GetSiteLevel(int userId, string environment = Constant.UAT);

        Task<IEnumerable<SiteLevel>> GetSiteLevelAsync(int userId, string environment = Constant.UAT);

        IEnumerable<FunctionPolicy> GetPermissions(int userId, int siteId, string parentFunctionKey, string environment = Constant.UAT);

        Task<IEnumerable<FunctionPolicy>> GetPermissionsAsync(int userId, int siteId, string parentFunctionKey, string environment = Constant.UAT);

        bool IsHavingPermission(int userId, int siteId, string functionKey, Constant.Policy policy);

        Task<bool> IsHavingPermissionAsync(int userId, int siteId, string functionKey, Constant.Policy policy);

        bool IgnoredOtp(int userId, string system);

        Task<bool> IgnoredOtpAsync(int userId, string system);

        bool VerifyOtp(int userId, string system, string otp);

        Task<bool> VerifyOtpAsync(int userId, string system, string otp);
    }

    public class UserService : IUserService
    {
        public IEnumerable<MenuItem> GetMenuItem(int userId, int siteId, string environment = Constant.UAT, bool? isActive = null)
            => GetMenuItemAsync(userId, siteId, environment, isActive).Result;

        public async Task<IEnumerable<MenuItem>> GetMenuItemAsync(int userId, int siteId, string environment = Constant.UAT, bool? isActive = null)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            Check.Argument.IsNotEmpty(environment, "environment");
            var userClient = new UserClient();
            return await userClient.GetMenuItem(userId, siteId, environment, isActive).ConfigureAwait(false);
        }

        public IEnumerable<SiteLevel> GetSiteLevel(int userId, string environment = Constant.UAT)
            => GetSiteLevelAsync(userId, environment).Result;

        public async Task<IEnumerable<SiteLevel>> GetSiteLevelAsync(int userId, string environment = Constant.UAT)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");
            Check.Argument.IsNotEmpty(environment, "environment");
            var userClient = new UserClient();
            return await userClient.GetSiteLevel(userId, environment).ConfigureAwait(false);
        }

        public User Get(int userId) => GetAsync(userId).Result;

        public async Task<User> GetAsync(int userId)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");
            var userClient = new UserClient();
            return await userClient.Get(userId).ConfigureAwait(false);
        }

        public User Get(string username) => GetAsync(username).Result;

        public async Task<User> GetAsync(string username)
        {
            Check.Argument.IsNotEmpty(username, "username");
            var userClient = new UserClient();
            return await userClient.Get(username).ConfigureAwait(false);
        }

        public int GetUserLevel(int userId, int siteId) => GetUserLevelAsync(userId, siteId).Result;

        public async Task<int> GetUserLevelAsync(int userId, int siteId)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            var userClient = new UserClient();
            return await userClient.GetUserLevel(userId, siteId).ConfigureAwait(false);
        }

        public int GetPermission(int userId, int siteId, string functionKey) => GetPermissionAsync(userId, siteId, functionKey).Result;

        public async Task<int> GetPermissionAsync(int userId, int siteId, string functionKey)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            Check.Argument.IsNotEmpty(functionKey, "functionKey");
            var userClient = new UserClient();
            return await userClient.GetPermission(userId, siteId, functionKey).ConfigureAwait(false);
        }

        public IEnumerable<FunctionPolicy> GetPermissions(int userId, int siteId, string parentFunctionKey, string environment = Constant.UAT)
            => GetPermissionsAsync(userId, siteId, parentFunctionKey, environment).Result;

        public async Task<IEnumerable<FunctionPolicy>> GetPermissionsAsync(int userId, int siteId, string parentFunctionKey, string environment = Constant.UAT)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            var userClient = new UserClient();
            return await userClient.GetPermissions(userId, siteId, parentFunctionKey, environment).ConfigureAwait(false);
        }

        public bool IsHavingPermission(int userId, int siteId, string functionKey, Constant.Policy policy)
            => IsHavingPermissionAsync(userId, siteId, functionKey, policy).Result;

        public async Task<bool> IsHavingPermissionAsync(int userId, int siteId, string functionKey, Constant.Policy policy)
        {
            if (userId <= 0 || siteId <= 0)
            {
                return false;
            }

            var permission = await GetPermissionAsync(userId, siteId, functionKey);

            return permission >= (int)policy;
        }

        public bool IgnoredOtp(int userId, string system) => IgnoredOtpAsync(userId, system).Result;

        public async Task<bool> IgnoredOtpAsync(int userId, string system)
        {
            var userClient = new UserClient();
            return await userClient.IgnoredOtpAsync(userId, system);
        }

        public bool VerifyOtp(int userId, string system, string otp) => VerifyOtpAsync(userId, system, otp).Result;

        public async Task<bool> VerifyOtpAsync(int userId, string system, string otp)
        {
            var userClient = new UserClient();
            return await userClient.VerifyOtpAsync(userId, system, otp);
        }

    }
}
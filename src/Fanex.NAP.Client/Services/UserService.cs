namespace Fanex.NAP.Client.Services
{
    using Clients;
    using Fanex.NAP.Core.Business;
    using Models;
    using Models.ValueObject;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IUserService
    {
        User Get(int userId);

        Task<User> GetAsync(int userId);

        User Get(string username);

        Task<User> GetAsync(string username);

        IEnumerable<User> GetAll();

        Task<IEnumerable<User>> GetAllAsync();

        IEnumerable<MenuItem> GetMenuItem(int userId, int siteId, string environment = Constant.UAT, bool? isActive = null);

        Task<IEnumerable<MenuItem>> GetMenuItemAsync(int userId, int siteId, string environment = Constant.UAT, bool? isActive = null);

        int GetPermission(int userId, int siteId, string functionKey);

        Task<int> GetPermissionAsync(int userId, int siteId, string functionKey);

        IEnumerable<SiteLevel> GetSiteLevel(int userId, string environment = Constant.UAT);

        Task<IEnumerable<SiteLevel>> GetSiteLevelAsync(int userId, string environment = Constant.UAT);

        int GetUserLevel(int userId, int siteId);

        Task<int> GetUserLevelAsync(int userId, int siteId);

        bool HasAnyPermission(int userId, int siteId, string environment = Constant.UAT);

        Task<bool> HasAnyPermissionAsync(int userId, int siteId, string environment = Constant.UAT);

        bool IsHavingPermission(int userId, int siteId, string functionKey, Constant.Policy policy);

        IEnumerable<FunctionPolicy> GetPermissions(int userId, int siteId, string parentFunctionKey, string environment = Constant.UAT);
    }

    public class UserService : IUserService
    {
        public bool HasAnyPermission(int userId, int siteId, string environment = Constant.UAT)
            => HasAnyPermissionAsync(userId, siteId, environment).Result;

        public async Task<bool> HasAnyPermissionAsync(int userId, int siteId, string environment = Constant.UAT)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            Check.Argument.IsNotEmpty(environment, "environment");
            var userClient = new UserClient();
            return await userClient.HasAnyPermission(userId, siteId, environment).ConfigureAwait(false);
        }

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

        public IEnumerable<User> GetAll() => GetAllAsync().Result;

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var userClient = new UserClient();
            return await userClient.GetAll().ConfigureAwait(false);
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

        public bool IsHavingPermission(int userId, int siteId, string functionKey, Constant.Policy policy)
        {
            Check.Argument.IsNotNegativeOrZero(userId, "userId");
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            Check.Argument.IsNotEmpty(functionKey, "functionKey");

            int currentPolicy = GetPermission(userId, siteId, functionKey);

            return currentPolicy >= (int)policy;
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

        public IEnumerable<UserPolicy> GetUserPolicies(string functionKey, int siteId)
            => GetUserPoliciesAsync(functionKey, siteId).Result;

        public async Task<IEnumerable<UserPolicy>> GetUserPoliciesAsync(string functionKey, int siteId)
        {
            Check.Argument.IsNotEmpty(functionKey, "functionKey");
            Check.Argument.IsNotNegativeOrZero(siteId, "siteId");
            var userClient = new UserClient();
            return await userClient.GetUserPolicies(functionKey, siteId).ConfigureAwait(false);
        }
    }
}
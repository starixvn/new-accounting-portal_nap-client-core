namespace Fanex.NAP.Client.Clients
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ValueObject;
    using Fanex.NAP.Client.Web;
    using Fanex.NAP.Core.Business;

    internal interface IUserClient
    {
        Task<bool> HasAnyPermission(int userId, int siteId, string environment = Constant.UAT);

        Task<IEnumerable<MenuItem>> GetMenuItem(int userId, int siteId, string env = Constant.UAT, bool? isActive = null);

        Task<IEnumerable<SiteLevel>> GetSiteLevel(int userId, string env = Constant.UAT);

        Task<User> Get(int userId);

        Task<User> Get(string username);

        Task<IEnumerable<User>> GetAll();

        Task<int> GetUserLevel(int userId, int siteId);

        Task<int> GetPermission(int userId, int siteId, string functionKey);

        Task<IEnumerable<FunctionPolicy>> GetPermissions(int userId, int siteId, string parentFunctionKey, string environment = Constant.UAT);
        Task<IEnumerable<UserPolicy>> GetUserPolicies(string functionKey, int siteId);
    }

    internal class UserClient : BaseClient, IUserClient
    {
        public UserClient() : base(null)
        {
        }

        public async Task<bool> HasAnyPermission(int userId, int siteId, string environment = Constant.UAT)
            => await Get<bool>(
                    CreateRequestUri(string.Format(RelativePaths.HasAnyPermission, userId),
                    new { siteId, environment }))
                .ConfigureAwait(false);

        public async Task<IEnumerable<MenuItem>> GetMenuItem(int userId, int siteId, string env = Constant.UAT, bool? isActive = null)
            => await Get<IEnumerable<MenuItem>>(
                    CreateRequestUri(string.Format(RelativePaths.GetMenuItem, userId),
                    new { siteId, environment = env, isActive }))
                .ConfigureAwait(false);

        public async Task<IEnumerable<SiteLevel>> GetSiteLevel(int userId, string env = Constant.UAT)
            => await Get<IEnumerable<SiteLevel>>(
                    CreateRequestUri(string.Format(RelativePaths.GetSiteLevel, userId),
                    new { environment = env }))
                .ConfigureAwait(false);

        public async Task<User> Get(int userId)
            => await Get<User>(CreateRequestUri(string.Format(RelativePaths.GetById, userId))).ConfigureAwait(false);

        public async Task<User> Get(string username)
            => await Get<User>(CreateRequestUri(RelativePaths.GetByUsername, new { username })).ConfigureAwait(false);

        public async Task<IEnumerable<User>> GetAll()
            => await Get<IEnumerable<User>>(CreateRequestUri(RelativePaths.GetAll)).ConfigureAwait(false);

        public async Task<int> GetUserLevel(int userId, int siteId)
            => await Get<int>(CreateRequestUri(string.Format(RelativePaths.GetUserLevel, userId, siteId)))
            .ConfigureAwait(false);

        public async Task<int> GetPermission(int userId, int siteId, string functionKey)
            => await Get<int>(CreateRequestUri(string.Format(RelativePaths.GetPermission, userId), new { siteId, functionKey }))
            .ConfigureAwait(false);

        public async Task<IEnumerable<FunctionPolicy>> GetPermissions(int userId, int siteId, string parentFunctionKey, string environment = Constant.UAT)
            => await Get<IEnumerable<FunctionPolicy>>(
                CreateRequestUri(string.Format(RelativePaths.GetPermissions, userId),
                new { siteId, parentFunctionKey, environment }))
            .ConfigureAwait(false);

        public async Task<IEnumerable<UserPolicy>> GetUserPolicies(string functionKey, int siteId)
            => await Get<IEnumerable<UserPolicy>>(
                CreateRequestUri(RelativePaths.GetUserPolicies, new { functionKey, siteId }))
            .ConfigureAwait(false);

        private static class RelativePaths
        {
            public const string HasAnyPermission = "/user/{0}/functionPolicy";
            public const string GetMenuItem = "/user/{0}/menuItem";
            public const string GetSiteLevel = "/user/{0}/sitelevel";
            public const string GetById = "/user/{0}";
            public const string GetByUsername = "/user";
            public const string GetAll = "/user/";
            public const string GetUserLevel = "/user/{0}/levelonsite/{1}";
            public const string GetPermission = "/user/{0}/sitepolicy";
            public const string GetPermissions = "/user/{0}/functionPolicies";
            public const string GetUserPolicies = "/user/userPolicies";
        }
    }
}
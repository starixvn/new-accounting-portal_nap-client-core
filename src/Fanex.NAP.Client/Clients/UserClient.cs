namespace Fanex.NAP.Client.Clients
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Helpers.Business;
    using Models;
    using Models.ValueObject;

    internal interface IUserClient
    {
        Task<IEnumerable<MenuItem>> GetMenuItem(int userId, int siteId, string env = Constant.UAT, bool? isActive = null);

        Task<IEnumerable<SiteLevel>> GetSiteLevel(int userId, string env = Constant.UAT);

        Task<User> Get(int userId);

        Task<User> Get(string username);

        Task<int> GetUserLevel(int userId, int siteId);

        Task<int> GetPermission(int userId, int siteId, string functionKey);

        Task<IEnumerable<FunctionPolicy>> GetPermissions(int userId, int siteId, string parentFunctionKey,
            string environment = Constant.UAT);

        Task<bool> IgnoredOtpAsync(int userId, string system);

        Task<bool> VerifyOtpAsync(int userId, string system, string otp);
    }

    internal class UserClient : BaseClient, IUserClient
    {
        public async Task<IEnumerable<MenuItem>> GetMenuItem(int userId, int siteId, string env = Constant.UAT, bool? isActive = null)
        {
            return await Get<IEnumerable<MenuItem>>(CreateRequestUri(
                string.Format(RelativePaths.GetMenuItem, userId),
                new { siteId, environment = env, isActive }));
        }

        public async Task<IEnumerable<SiteLevel>> GetSiteLevel(int userId, string env = Constant.UAT)
        {
            return await Get<IEnumerable<SiteLevel>>(CreateRequestUri(string.Format(RelativePaths.GetSiteLevel, userId), new { environment = env }));
        }

        public async Task<User> Get(int userId)
        {
            return await Get<User>(CreateRequestUri(string.Format(RelativePaths.GetById, userId)));
        }

        public async Task<User> Get(string username)
        {
            return await Get<User>(CreateRequestUri(RelativePaths.GetByUsername, new { username }));
        }

        public async Task<int> GetUserLevel(int userId, int siteId)
        {
            return await Get<int>(CreateRequestUri(string.Format(RelativePaths.GetUserLevel, userId, siteId)));
        }

        public async Task<int> GetPermission(int userId, int siteId, string functionKey)
        {
            return await Get<int>(CreateRequestUri(string.Format(RelativePaths.GetPermission, userId), new { siteId, functionKey }));
        }

        public async Task<IEnumerable<FunctionPolicy>> GetPermissions(int userId, int siteId, string parentFunctionKey, string environment = Constant.UAT)
            => await Get<IEnumerable<FunctionPolicy>>(
                CreateRequestUri(string.Format(RelativePaths.GetPermissions, userId),
                new { siteId, parentFunctionKey, environment }));

        public async Task<bool> IgnoredOtpAsync(int userId, string system)
        {
            var path = string.Format(RelativePaths.IgnoredOtp, userId);
            return await Get<bool>(CreateRequestUri(path, new { system }));
        }

        public async Task<bool> VerifyOtpAsync(int userId, string system, string otp)
        {
            var path = string.Format(RelativePaths.VerifyOtp, userId);
            return await Get<bool>(CreateRequestUri(path, new { system, otp }));
        }

        private static class RelativePaths
        {
            public const string GetById = "user/{0}";
            public const string GetByUsername = "user";
            public const string GetPermission = "user/{0}/permission";
            public const string GetPermissions = "user/{0}/permissions";
            public const string GetMenuItem = "user/{0}/menuItem";
            public const string GetSiteLevel = "user/{0}/sitelevel";
            public const string GetUserLevel = "user/{0}/levelonsite/{1}";
            public const string IgnoredOtp = "user/{0}/ignored-otp";
            public const string VerifyOtp = "user/{0}/verify-otp";
        }
    }
}
namespace Fanex.NAP.Client.Clients
{
    using Models.ParameterObject;
    using System.Net.Http;
    using System.Threading.Tasks;

    internal interface IUserInfoClient
    {
        Task<bool> UpdatePassword(int siteId, int userId, string password);

        Task<bool> UpdateProfile(int siteId, UpdateUserInfoParameter parameter);
    }

    internal class UserInfoClient : BaseClient, IUserInfoClient
    {
        public async Task<bool> UpdatePassword(int siteId, int userId, string password)
        {
            return await Send<object, bool>(CreateRequestUri(RelativePaths.UpdatePassword, new { siteId = siteId, userId = userId, password = password }), HttpMethod.Post, null);
        }

        public async Task<bool> UpdateProfile(int siteId, UpdateUserInfoParameter parameter)
        {
            return await Send<UpdateUserInfoParameter, bool>(CreateRequestUri(RelativePaths.UpdateProfile, new { siteId = siteId }), HttpMethod.Post, parameter);
        }

        private class RelativePaths
        {
            public const string UpdatePassword = "/userinfo/updatePassword";
            public const string UpdateProfile = "/userinfo/updateProfile";
        }
    }
}
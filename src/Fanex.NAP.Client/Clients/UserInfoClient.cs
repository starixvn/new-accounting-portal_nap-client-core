namespace Fanex.NAP.Client.Clients
{
    using System.Threading.Tasks;
    using Models.ParameterObject;

    internal interface IUserInfoClient
    {
        Task<bool> UpdatePassword(int siteId, int userId, string password);

        Task<bool> UpdateProfile(int siteId, UpdateUserInfoParameter parameter);
    }

    internal class UserInfoClient : BaseClient, IUserInfoClient
    {
        public UserInfoClient() : base(null)
        {
        }

        public async Task<bool> UpdatePassword(int siteId, int userId, string password)
            => await Post<object, bool>(CreateRequestUri(RelativePaths.UpdatePassword, new { siteId, userId, password }), null)
            .ConfigureAwait(false);

        public async Task<bool> UpdateProfile(int siteId, UpdateUserInfoParameter parameter)
            => await Post<UpdateUserInfoParameter, bool>(CreateRequestUri(RelativePaths.UpdateProfile, new { siteId }), parameter)
            .ConfigureAwait(false);

        private static class RelativePaths
        {
            public const string UpdatePassword = "/userinfo/updatePassword";
            public const string UpdateProfile = "/userinfo/updateProfile";
        }
    }
}
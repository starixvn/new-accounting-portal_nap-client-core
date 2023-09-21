namespace Fanex.NAP.Client.Clients
{
    using System.Threading.Tasks;

    internal interface IUserInfoClient
    {
        Task<bool> UpdatePassword(int siteId, int userId, string password);

    }

    internal class UserInfoClient : BaseClient, IUserInfoClient
    {
        public async Task<bool> UpdatePassword(int siteId, int userId, string password)
        {
            var parameters = new
            {
                siteId,
                userId,
                password
            };
            return await Get<bool>(CreateRequestUri(RelativePaths.UpdatePassword, parameters));
        }

        private static class RelativePaths
        {
            public const string UpdatePassword = "userinfo/updatePassword";
        }
    }
}
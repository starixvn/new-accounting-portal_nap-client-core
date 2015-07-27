namespace Fanex.NAP.Client.Clients;

using System.Net.Http;
using System.Threading.Tasks;

public interface IUserInfoClient
{
    Task<bool> UpdatePassword(int siteId, int userId, string password);

}

public class UserInfoClient(HttpClient httpClient) : BaseClient(httpClient), IUserInfoClient
{
    public async Task<bool> UpdatePassword(int siteId, int userId, string password)
    {
        var parameters = new
        {
            siteId,
            userId,
            password
        };
        return await Get<bool>(CreateRequestPath(RelativePaths.UpdatePassword, parameters));
    }

    private static class RelativePaths
    {
        public const string UpdatePassword = "userinfo/updatePassword";
    }
}
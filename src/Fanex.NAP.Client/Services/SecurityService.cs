namespace Fanex.NAP.Client.Services;

using System;
using System.Threading.Tasks;
using Clients;
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

public class SecurityService(
    ILoginClient loginClient,
    ILogoutClient logoutClient,
    ICheckinClient checkinClient,
    ISSOClient ssoClient,
    IUserInfoClient userInfoClient) : ISecurityService
{
    public int Login(LoginParameter parameter) => LoginAsync(parameter).Result;

    public async Task<int> LoginAsync(LoginParameter parameter)
    {
        return await loginClient.Login(parameter).ConfigureAwait(false);
    }

    public void Logout(int onlineUserId) => LogoutAsync(onlineUserId).Wait();

    public async Task LogoutAsync(int onlineUserId)
    {
        await logoutClient.Logout(onlineUserId).ConfigureAwait(false);
    }

    public int ReCheckIn(int onlineUserId, int siteId) => ReCheckInAsync(onlineUserId, siteId).Result;

    public async Task<int> ReCheckInAsync(int onlineUserId, int siteId)
    {
        return await checkinClient.ReCheckIn(onlineUserId, siteId).ConfigureAwait(false);
    }

    public string GetTime() => GetTimeAsync().Result;

    public async Task<string> GetTimeAsync()
    {
        return await ssoClient.GetTime().ConfigureAwait(false);
    }

    public int SSOLogin(SSOLoginParameter parameter) => SSOLoginAsync(parameter).Result;

    public async Task<int> SSOLoginAsync(SSOLoginParameter parameter)
    {
        return await ssoClient.Login(parameter).ConfigureAwait(false);
    }

    public bool UpdatePassword(int siteId, int userId, string password)
        => UpdatePasswordAsync(siteId, userId, password).Result;

    public async Task<bool> UpdatePasswordAsync(int siteId, int userId, string password)
    {
        return await userInfoClient.UpdatePassword(siteId, userId, password).ConfigureAwait(false);
    }
}
namespace Fanex.Nap.Client.Integration.Tests;

using System.Net;
using System.Threading.Tasks;
using Xunit;

public class SecurityClientTests : IClassFixture<ServerFixture>
{
    private readonly ServerFixture _serverFixture;

    public SecurityClientTests(ServerFixture serverFixture)
    {
        _serverFixture = serverFixture;
    }

    [Fact]
    public async Task Login_Always_ReturnOnUserId()
    {
        // Arrange
        const string request = "/SecurityClient/Login?userId=8&&siteId=1";
        var response = await _serverFixture.Client.GetAsync(request);
        response.EnsureSuccessStatusCode();

        // Act
        var onUserId = CommonUtils.GetFromResponse<int>(response);

        // Assert
        Assert.True(onUserId > 0);
    }

    [Fact]
    public async Task Logout_Always_Success()
    {
        // Arrange
        var request = "/SecurityClient/Login?userId=8&&siteId=1";
        var response = await _serverFixture.Client.GetAsync(request);
        var onUserId = CommonUtils.GetFromResponse<int>(response);
        request = "/SecurityClient/Logout?onlineUserId=" + onUserId;

        // Act
        response = await _serverFixture.Client.GetAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ReCheckIn_Always_ReturnId()
    {
        // Arrange
        const string request = "/SecurityClient/RecheckIn?onlineUserId=23231&&siteId=1";
        var response = await _serverFixture.Client.GetAsync(request);
        response.EnsureSuccessStatusCode();

        // Act
        var onUserId = CommonUtils.GetFromResponse<int>(response);

        // Assert
        Assert.True(onUserId > 0);
    }
}
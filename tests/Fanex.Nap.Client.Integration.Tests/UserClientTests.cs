namespace Fanex.Nap.Client.AspNetCore.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ValueObject;
    using Xunit;

    public class UserClientTests : IClassFixture<ServerFixture>
    {
        private ServerFixture _serverFixture;

        public UserClientTests(ServerFixture serverFixture)
        {
            _serverFixture = serverFixture;
        }

        [Fact]
        public async Task GetUserById_Always_ReturnExpectedUserInfo()
        {
            // Arrange
            var request = "/UserClient/GetById?id=8";
            var response = await _serverFixture.Client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            // Act
            var user = CommonUtils.GetFromResponse<User>(response);

            // Assert
            Assert.Equal(8, user.Id);
        }

        [Fact]
        public async Task GetUserByName_Always_ReturnExpectedUserInfo()
        {
            // Arrange
            var request = "/UserClient/GetByName?name=sa";
            var response = await _serverFixture.Client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            // Act
            var user = CommonUtils.GetFromResponse<User>(response);

            // Assert
            Assert.Equal("sa", user.Username);
        }


        [Fact]
        public async Task GetPermission_Always_ReturnPermission()
        {
            // Arrange
            var request = "/UserClient/GetPermission?userId=8&&siteId=1&&functionKey=ManageUser";
            var response = await _serverFixture.Client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            // Act
            var permission = CommonUtils.GetFromResponse<int>(response);

            // Assert
            Assert.Equal(2, permission);
        }

        [Fact]
        public async Task GetPermissions_Always_ReturnPermissions()
        {
            // Arrange
            var request = "/UserClient/GetPermissions?userId=8&&siteId=1&&functionKey=ManageUser";
            var response = await _serverFixture.Client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            // Act
            var permissions = CommonUtils.GetFromResponse<IEnumerable<FunctionPolicy>>(response);

            // Assert
            Assert.Equal(1, permissions.Count());
        }
    }
}
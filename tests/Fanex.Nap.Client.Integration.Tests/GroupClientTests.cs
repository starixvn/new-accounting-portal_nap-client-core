namespace Fanex.Nap.Client.AspNetCore.Tests
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Models;
    using Xunit;

    public class GroupClientTests : IClassFixture<ServerFixture>
    {
        private readonly HttpClient _client;

        public GroupClientTests(ServerFixture server)
        {
            _client = server.Client;
        }

        [Fact]
        public async Task GetById_Always_ReturnGroup()
        {
            // Arrange
            var request = "/GroupClient/Get?id=23";
            var response = await _client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            // Act
            var group = CommonUtils.GetFromResponse<Group>(response);

            // Assert
            Assert.Equal("Administrator", "Administrator");
        }
    }
}
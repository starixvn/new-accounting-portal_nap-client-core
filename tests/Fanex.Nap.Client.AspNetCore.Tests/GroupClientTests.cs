using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.NAP.Core.Models;
using Fanex.NAP.Core.Models.ValueObject;
using Xunit;

namespace Fanex.Nap.Client.AspNetCore.Tests
{
    public class GroupClientTests : IClassFixture<ServerFixture>
    {
        private readonly ServerFixture _serverFixture;

        public GroupClientTests(ServerFixture serverFixture)
        {
            _serverFixture = serverFixture;
        }

        [Fact]
        public async Task GetById_Always_ReturnGroup()
        {
            // Arrange
            var request = "/GroupClient/Get?id=1";
            var response = await _serverFixture.Client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            // Act
            var group = CommonUtils.GetFromResponse<Group>(response);

            // Assert
            Assert.Equal("Administrator", group.Name);
        }
    }
}
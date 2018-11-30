using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fanex.NAP.Core.Models;
using Fanex.NAP.Core.Models.ValueObject;
using Xunit;

namespace Fanex.Nap.Client.AspNetCore.Tests
{
    public class SiteClientTests : IClassFixture<ServerFixture>
    {
        private readonly ServerFixture _serverFixture;

        public SiteClientTests(ServerFixture serverFixture)
        {
            _serverFixture = serverFixture;
        }
        
        [Fact]
        public async Task IsActivated_Always_ReturnSiteStatus()
        {
            // Arrange
            var request = "/SiteClient/IsActivated?id=1";
            var response = await _serverFixture.Client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            // Act
            var isActivated = CommonUtils.GetFromResponse<bool>(response);

            // Assert
            Assert.True(isActivated);
        }

        [Fact]
        public async Task GetUrlById_Always_ReturnSiteUrl()
        {
            // Arrange
            var request = "/SiteClient/GetUrlById?id=1";
            var response = await _serverFixture.Client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            // Act
            var siteUrls = CommonUtils.GetFromResponse<IEnumerable<SiteUrl>>(response);

            // Assert
            Assert.Equal(81, siteUrls.FirstOrDefault().SiteUrlId);
        }
        
        [Fact]
        public async Task GetMenuItemBySiteId_Always_ReturnMenuItems()
        {
            // Arrange
            var request = "/SiteClient/GetMenuItemBySiteId?id=1";
            var response = await _serverFixture.Client.GetAsync(request);
            response.EnsureSuccessStatusCode();

            // Act
            var menuItems = CommonUtils.GetFromResponse<IEnumerable<MenuItem>>(response);

            // Assert
            Assert.Equal(21, menuItems.Count());
        }
    }
}
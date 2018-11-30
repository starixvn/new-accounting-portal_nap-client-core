namespace Fanex.NAP.Core.Test.Clients
{
    using System.Linq;
    using Fanex.NAP.Core.Clients;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Exceptions;
    using NUnit.Framework;

    [TestFixture]
    public class ClientContextTests
    {
        [SetUp]
        public void SetUp()
        {
            ClientContext.Configuration = new NapSettings();
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase(" ")]
        public void GetServiceUrl_ServiceUrlsIsNullOrWhiteSpace_ThrowNAPAPIException(string serviceUrls)
        {
            //// Arrange
            ClientContext.Configuration.ServiceUrls = serviceUrls;

            //// Assert
            Assert.Throws<NAPApiException>(new TestDelegate(() => ClientContext.GetServiceUrl()));
        }

        [Test]
        public void GetServiceUrl_ServiceUrlsHasOneUrl_ReturnThatUrl()
        {
            //// Arrange
            var serviceUrl = "url";
            ClientContext.Configuration.ServiceUrls = serviceUrl;

            //// Act
            var actualUrl = ClientContext.GetServiceUrl();

            //// Assert
            Assert.AreEqual(serviceUrl, actualUrl);
        }

        [Test]
        public void GetServiceUrl_ServiceUrlsHasMoreThanOneUrl_ReturnRandomUrl()
        {
            //// Arrange
            var serviceUrls = "url,url2,url3";
            ClientContext.Configuration.ServiceUrls = serviceUrls;

            //// Act
            var actualUrl = ClientContext.GetServiceUrl();

            //// Assert
            Assert.IsTrue(serviceUrls.Split(',').Contains(actualUrl));
        }
    }
}
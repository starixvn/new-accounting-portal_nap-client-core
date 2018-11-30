namespace Fanex.NAP.Core.Test.Clients
{
    using Fanex.NAP.Core.Clients;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Net.Http;

    [TestFixture]
    public class CheckinClientTest
    {
        private const string BaseEndpoint = "http://10.18.200.209:1710/api";

        [Test]
        public void ReCheckIn_WhenCalled_RequestUri()
        {
            var respone = Substitute.For<HttpResponseMessage>();
            respone.Content = Substitute.For<HttpContent>();
            var httpClient = HttpClientFactory.Create(new FakeDelegatingHandler(respone));
            var client = new CheckinClient(new Uri(BaseEndpoint), httpClient);

            var onlineUserId = 124;
            var siteId = 8;

            client.ReCheckIn(onlineUserId, siteId);

            var expectedRequestUri = "http://10.18.200.209:1710/api/recheckin/?onUserId=124&siteId=8";
            Assert.AreEqual(expectedRequestUri, respone.RequestMessage.RequestUri.ToString());
        }
    }
}
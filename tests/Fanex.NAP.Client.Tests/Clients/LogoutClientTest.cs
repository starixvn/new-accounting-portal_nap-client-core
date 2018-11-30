namespace Fanex.NAP.Core.Test.Clients
{
    using Fanex.NAP.Core.Clients;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Net.Http;

    [TestFixture]
    public class LogoutClientTest
    {
        private const string BaseEndpoint = "http://10.18.200.209:1710/api";

        [Test]
        public void Logout_WhenCalled_RequestUri()
        {
            var respone = Substitute.For<HttpResponseMessage>();
            respone.Content = Substitute.For<HttpContent>();
            var httpClient = HttpClientFactory.Create(new FakeDelegatingHandler(respone));
            var client = new LogoutClient(new Uri(BaseEndpoint), httpClient);

            client.Logout(Arg.Any<int>());

            var expectedRequestUri = "http://10.18.200.209:1710/api/logout/";
            Assert.AreEqual(expectedRequestUri, respone.RequestMessage.RequestUri.ToString());
        }
    }
}
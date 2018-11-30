namespace Fanex.NAP.Core.Test.Clients
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.NAP.Core.Clients;
    using Fanex.NAP.Core.Configuration;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class GroupClientTest
    {
        private const string BaseEndpoint = "http://10.18.200.209:1710/api";

        [Test]
        public void Get_WhenCalled_RequestUri()
        {
            var respone = Substitute.For<HttpResponseMessage>();
            respone.Content = Substitute.For<HttpContent>();
            var httpClient = HttpClientFactory.Create(new FakeDelegatingHandler(respone));
            var client = new GroupClient(new Uri(BaseEndpoint), httpClient);

            var groupId = 124;

            client.Get(groupId);

            var expectedRequestUri = "http://10.18.200.209:1710/api/group/124";
            Assert.AreEqual(expectedRequestUri, respone.RequestMessage.RequestUri.ToString());
        }
    }
}

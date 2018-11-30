namespace Fanex.NAP.Core.Test.Clients
{
    using Fanex.NAP.Core.Clients;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    [TestFixture]
    public class SiteClientTest
    {
        private const string BaseEndpoint = "http://10.18.200.209:1710/api";
        private HttpResponseMessage _respone;
        private SiteClient _client;
        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            _respone = Substitute.For<HttpResponseMessage>();
            _respone.Content = Substitute.For<HttpContent>();
            _httpClient = HttpClientFactory.Create(new FakeDelegatingHandler(_respone));
            _client = new SiteClient(new Uri(BaseEndpoint), _httpClient);
        }

        [Test]
        public void IsActivated_WhenCalled_RequestUri()
        {
            var siteId = 1456;

            _client.IsActivated(siteId);

            var expectedRequestUri = "http://10.18.200.209:1710/api/site/1456/status";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        [Test]
        public void GetUrlById_WhenCalled_RequestUri()
        {
            var siteId = 1456;
            var environment = "UAT";

            _client.GetUrlById(siteId, environment);

            var expectedRequestUri = "http://10.18.200.209:1710/api/site/1456/siteUrl?environment=UAT";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        [Test]
        public void GetMenuItemBySiteId_WhenCalled_RequestUri()
        {
            var siteId = 1456;
            var environment = "UAT";
            var isActive = true;

            _client.GetMenuItemBySiteId(siteId, environment, isActive);

            var expectedRequestUri = "http://10.18.200.209:1710/api/site/1456/menuItem?environment=UAT&isActive=True";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        private Task<string> GetIsActivatedResult()
        {
            return Task.Factory.StartNew<string>(() => { return "A"; });
        }
    }
}
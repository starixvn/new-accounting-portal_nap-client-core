namespace Fanex.NAP.Core.Test.Clients
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.NAP.Core.Clients;
    using Fanex.NAP.Core.Configuration;
    using Fanex.NAP.Core.Models.ParameterObject;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class UserClientTest
    {
        private const string BaseEndpoint = "http://10.18.200.209:1710/api";
        private HttpResponseMessage _respone;
        private UserClient _client;
        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            _respone = Substitute.For<HttpResponseMessage>();
            _respone.Content = Substitute.For<HttpContent>();
            _httpClient = HttpClientFactory.Create(new FakeDelegatingHandler(_respone));
            _client = new UserClient(new Uri(BaseEndpoint), _httpClient);
        }

        [Test]
        public void HasAnyPermission_WhenCalled_RequestUri()
        {
            var userId = 1;
            var siteId = 2;
            var environment = "LIVE";

            _client.HasAnyPermission(userId, siteId, environment);

            var expectedRequestUri = "http://10.18.200.209:1710/api/user/1/functionPolicy?siteId=2&environment=LIVE";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        [Test]
        public void GetMenuItem_WhenCalled_RequestUri()
        {
            var userId = 1;
            var siteId = 2;
            var environment = "LIVE";
            var isActive = true;

            _client.GetMenuItem(userId, siteId, environment, isActive);

            var expectedRequestUri = "http://10.18.200.209:1710/api/user/1/menuItem?siteId=2&environment=LIVE&isActive=True";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        [Test]
        public void GetSiteLevel_WhenCalled_RequestUri()
        {
            var userId = 1;
            var environment = "LIVE";

            _client.GetSiteLevel(userId, environment);

            var expectedRequestUri = "http://10.18.200.209:1710/api/user/1/sitelevel?environment=LIVE";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        [Test]
        public void GetById_WhenCalled_RequestUri()
        {
            var userId = 1;

            _client.Get(userId);

            var expectedRequestUri = "http://10.18.200.209:1710/api/user/1";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        [Test]
        public void GetByUsername_WhenCalled_RequestUri()
        {
            var username = "edward.nguyen";

            _client.Get(username);

            var expectedRequestUri = "http://10.18.200.209:1710/api/user?username=edward.nguyen";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        [Test]
        public void GetAll_WhenCalled_RequestUri()
        {
            _client.GetAll();

            var expectedRequestUri = "http://10.18.200.209:1710/api/user/";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        [Test]
        public void GetUserLevel_WhenCalled_RequestUri()
        {
            var userId = 1;
            var siteId = 2;

            _client.GetUserLevel(userId, siteId);

            var expectedRequestUri = "http://10.18.200.209:1710/api/user/1/levelonsite/2";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        [Test]
        public void GetPermission_WhenCalled_RequestUri()
        {
            var userId = 1;
            var siteId = 2;
            var functionKey = "GetAllUser";

            _client.GetPermission(userId, siteId, functionKey);

            var expectedRequestUri = "http://10.18.200.209:1710/api/user/1/sitepolicy?siteId=2&functionKey=GetAllUser";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        [Test]
        public void GetPermissions_WhenCalled_RequestUri()
        {
            var userId = 1;
            var siteId = 2;
            var parentFunctionKey = "ParentKey";

            _client.GetPermissions(userId, siteId, parentFunctionKey);

            var expectedRequestUri = "http://10.18.200.209:1710/api/user/1/functionPolicies?siteId=2&parentFunctionKey=ParentKey&environment=UAT";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }
    }
}
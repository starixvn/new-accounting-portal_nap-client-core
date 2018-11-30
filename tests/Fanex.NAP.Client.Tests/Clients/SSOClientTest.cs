namespace Fanex.NAP.Core.Test.Clients
{
    using Fanex.NAP.Core.Clients;
    using Fanex.NAP.Core.Models.ParameterObject;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Net.Http;

    [TestFixture]
    public class SSOClientTest
    {
        private const string BaseEndpoint = "http://10.18.200.209:1710/api";
        private HttpResponseMessage _respone;
        private SSOClient _client;
        private HttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            _respone = Substitute.For<HttpResponseMessage>();
            _respone.Content = Substitute.For<HttpContent>();
            _httpClient = HttpClientFactory.Create(new FakeDelegatingHandler(_respone));
            _client = new SSOClient(new Uri(BaseEndpoint), _httpClient);
        }

        [Test]
        public void GetTime_WhenCalled_RequestUri()
        {
            _client.GetTime();

            var expectedRequestUri = "http://10.18.200.209:1710/api/ssologin/";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }

        [Test]
        public void Login_WhenCalled_RequestUri()
        {
            _client.Login(Arg.Any<SSOLoginParameter>());

            var expectedRequestUri = "http://10.18.200.209:1710/api/ssologin/";
            Assert.AreEqual(expectedRequestUri, _respone.RequestMessage.RequestUri.ToString());
        }
    }
}
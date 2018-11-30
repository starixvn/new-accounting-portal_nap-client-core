namespace Fanex.NAP.Core.Test.Clients
{
    using Fanex.NAP.Core.Clients;
    using Fanex.NAP.Core.Models.ParameterObject;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.Net.Http;

    [TestFixture]
    public class LoginClientTest
    {
        private const string BaseEndpoint = "http://10.18.200.209:1710/api";

        [Test]
        public void Login_WhenCalled_RequestUri()
        {
            var respone = Substitute.For<HttpResponseMessage>();
            respone.Content = Substitute.For<HttpContent>();
            var httpClient = HttpClientFactory.Create(new FakeDelegatingHandler(respone));
            var client = new LoginClient(new Uri(BaseEndpoint), httpClient);

            client.Login(Arg.Any<LoginParameter>());

            var expectedRequestUri = "http://10.18.200.209:1710/api/login/";
            Assert.AreEqual(expectedRequestUri, respone.RequestMessage.RequestUri.ToString());
        }
    }
}
namespace Fanex.NAP.Core.Test.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using Fanex.NAP.Core.Clients;
    using Fanex.NAP.Core.Configuration;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class BaseClientTest
    {
        [Test]
        public void CreateRequestUri_QueryStringIsString_ReturnAbsoluteUri()
        {
            var baseClient = CreateBaseClient();

            var actualUri = baseClient.CreateRequestUri("/group", "username=edward.nguyen");

            var expectedUri = new Uri("http://10.18.200.209:1710/api/group?username=edward.nguyen");
            Assert.AreEqual(expectedUri, actualUri);
        }

        [Test]
        public void CreateRequestUri_QueryStringIsObject_ReturnAbsoluteUri()
        {
            var baseClient = CreateBaseClient();

            var actualUri = baseClient.CreateRequestUri("/group", new { username = "edward.nguyen" });

            var expectedUri = new Uri("http://10.18.200.209:1710/api/group?username=edward.nguyen");
            Assert.AreEqual(expectedUri, actualUri);
        }

        [Test]
        public void CreateRequestUri_QueryStringIsKeyValuePair_ReturnAbsoluteUri()
        {
            var baseClient = CreateBaseClient();

            var actualUri = baseClient.CreateRequestUri("/group", new KeyValuePair<string, string>("username", "edward.nguyen"));

            var expectedUri = new Uri("http://10.18.200.209:1710/api/group?username=edward.nguyen");
            Assert.AreEqual(expectedUri, actualUri);
        }

        private BaseClientUnderTest CreateBaseClient()
        {
            return new BaseClientUnderTest();
        }

        private class BaseClientUnderTest : BaseClient
        {
            public BaseClientUnderTest()
                : base(new Uri("http://10.18.200.209:1710/api"), HttpClientFactory.Create(Substitute.For<DelegatingHandler>()))
            {
            }

            public new Uri CreateRequestUri(string relativePath, string queryString)
            {
                return base.CreateRequestUri(relativePath, queryString);
            }

            [SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification = "Reviewed!")]
            public new Uri CreateRequestUri(string relativePath, object queryStringParameters)
            {
                return base.CreateRequestUri(relativePath, queryStringParameters);
            }

            public new Uri CreateRequestUri(string relativePath, params KeyValuePair<string, string>[] queryStringParameters)
            {
                return base.CreateRequestUri(relativePath, queryStringParameters);
            }
        }
    }
}

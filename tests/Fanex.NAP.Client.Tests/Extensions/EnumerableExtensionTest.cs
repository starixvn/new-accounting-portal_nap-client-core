namespace Fanex.NAP.Core.Test.Extensions
{
    using System.Collections.Generic;
    using Fanex.NAP.Core.Extensions;
    using NUnit.Framework;

    [TestFixture]
    public class EnumerableExtensionTest
    {
        [Test]
        public void ToDictionary_WhenCalled_ReturnDictionary()
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("name", "edward.nguyen"),
                new KeyValuePair<string, string>("id", "1")
            };

            var actualDictionary = keyValuePairs.ToDictionary();

            var expectedDictionary = new Dictionary<string, string>
            {
                { "name", "edward.nguyen" },
                { "id", "1" }
            };

            Assert.AreEqual(expectedDictionary, actualDictionary);
        }
    }
}

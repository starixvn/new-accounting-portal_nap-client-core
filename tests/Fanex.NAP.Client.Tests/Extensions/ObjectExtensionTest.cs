namespace Fanex.NAP.Core.Test.Extensions
{
    using System.Collections.Generic;
    using Fanex.NAP.Core.Extensions;
    using NUnit.Framework;

    [TestFixture]
    public class ObjectExtensionTest
    {
        [Test]
        public void ToDictionary_WhenCalled_ReturnDictionary()
        {
            var user = new
            {
                id = "1",
                name = "edward.nguyen"
            };

            var actualDictionary = user.ToPropertyDictionary();

            var expectedDictionary = new Dictionary<string, string>
            {
                { "name", "edward.nguyen" },
                { "id", "1" }
            };

            Assert.AreEqual(expectedDictionary, actualDictionary);
        }

        [Test]
        public void ToDictionary_ObjectIsNull_ReturnNull()
        {
            Assert.IsNull(ObjectExtension.ToPropertyDictionary(null));
        }

        [Test]
        public void ToNullOrString_WhenCalled_ReturnString()
        {
            var user = new
            {
                id = "1",
                name = "edward.nguyen"
            };

            var actualString = user.ToNullOrString();

            var expectedString = "{ id = 1, name = edward.nguyen }";

            Assert.AreEqual(expectedString, actualString);
        }

        [Test]
        public void ToNullOrString_ObjectIsNull_ReturnNull()
        {
            Assert.IsNull(ObjectExtension.ToNullOrString(null));
        }
    }
}

namespace Fanex.NAP.Client.AspNetMVC.Auth
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    public class AuthInfo
    {
        private int userId;

        private int onuserId;

        public int UserId
        {
            get
            {
                return userId;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", "userid");
                }

                userId = value;
            }
        }

        public int OnUserId
        {
            get
            {
                return onuserId;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", "onUserid");
                }

                onuserId = value;
            }
        }

        public static AuthInfo CreateUserData(string authInfoXml)
        {
            var serializer = new XmlSerializer(typeof(AuthInfo));

            return (AuthInfo)serializer.Deserialize(new StringReader(authInfoXml));
        }

        public string ToXml()
        {
            var xml = string.Empty;

            using (var sw = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(AuthInfo));

                serializer.Serialize(sw, this);
                sw.Close();
                xml = sw.ToString();
            }

            return xml;
        }
    }
}
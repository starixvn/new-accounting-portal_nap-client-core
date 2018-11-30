namespace Fanex.NAP.Client.Transport
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using Fanex.NAP.Client.Configuration;
    using RestSharp;
    using RestSharp.Authenticators;

    internal class HmacAuthenticator : IAuthenticator
    {
        private readonly INapSettings configuration;

        public HmacAuthenticator() : this(null)
        {
        }

        public HmacAuthenticator(INapSettings configuration)
        {
            this.configuration = configuration ?? NapClientManager.NapSettings;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client), "Rest Client cannot be null");
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Rest Request cannot be null");
            }

            string requestContentBase64String = string.Empty;

            string requestUri = HttpUtility.UrlEncode(client.BuildUri(request).OriginalString.ToLowerInvariant());

            string requestHttpMethod = request.Method.ToString();

            // Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            // create random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");

            // Checking if the request contains body, usually will be null wiht HTTP GET and DELETE
            var body = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);

            if (body != null)
            {
                byte[] content = ObjectToByteArray(body.Value);

                // Hashing the request body, any change in request body will result in different hash, we'll incure message integrity
                var md5 = MD5.Create();
                byte[] requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            // Creating the raw signature string
            string signatureRawData = string.Format("{0}{1}{2}{3}{4}{5}", configuration.SiteId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            var secretKeyByteArray = Convert.FromBase64String(configuration.AccessKey);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                // Setting the values in the Authorization header using custom scheme (amx)
                string authHeader = string.Format("amx {0}", string.Format("{0}:{1}:{2}:{3}", configuration.SiteId, requestSignatureBase64String, nonce, requestTimeStamp));
                request.AddParameter("Authorization", authHeader, ParameterType.HttpHeader);
            }
        }

        private static byte[] ObjectToByteArray(object obj)
        {
            var formatter = new BinaryFormatter();

            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, obj);

                return ms.ToArray();
            }
        }
    }
}
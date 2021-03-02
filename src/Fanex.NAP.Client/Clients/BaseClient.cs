namespace Fanex.NAP.Client.Clients
{
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Exceptions;
    using Fanex.NAP.Client.Resources;
    using Fanex.NAP.Core.Extensions;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    internal class BaseClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private static readonly Random randomHandler = new Random();

        private readonly INapSettings napSettings;

        public BaseClient()
        {
            napSettings = NapClientManager.NapSettings;
            BaseAddress = new Uri(GetServiceUrl());
        }

        public Uri BaseAddress { get; set; }

        protected async Task<T> Get<T>(Uri requestUri)
        {
            T result = default(T);

            using (var message = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                using (var response = await SendAsync(message))
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        throw new NAPApiException(string.Format(ViewResources.Dont_Have_Permission, requestUri));
                    }
                    else if (response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        throw new NAPApiException(content);
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();

                        result = DeserializeResult(result, content);
                    }
                }
            }

            return result;
        }

        private static T DeserializeResult<T>(T result, string content)
        {
            if (!string.IsNullOrWhiteSpace(content))
            {
                try
                {
                    result = JsonConvert.DeserializeObject<T>(content);
                }
                catch
                {
                    throw new NAPApiException(content);
                }
            }

            return result;
        }

        protected async Task Send<TInput>(Uri requestUri, HttpMethod httpMethod, TInput body)
        {
            await Send<TInput, object>(requestUri, httpMethod, body);
        }

        protected async Task<TOutput> Send<TInput, TOutput>(Uri requestUri, HttpMethod httpMethod, TInput body)
        {
            using (var request = new HttpRequestMessage(httpMethod, requestUri))
            {
                var json = JsonConvert.SerializeObject(body);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;
                    using (var response = await SendAsync(request))
                    {
                        ThrowIfResponseNotSuccessfulAsync(response);

                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<TOutput>(jsonResponse);
                    }
                }
            }
        }

        protected Uri CreateRequestUri(string relativePath, params KeyValuePair<string, string>[] queryStringParameters)
        {
            var queryString = string.Empty;

            if (queryStringParameters != null && queryStringParameters.Length > 0)
            {
                var queryStringProperties = ParseQueryString(BaseAddress.Query);
                foreach (var queryStringParameter in queryStringParameters)
                {
                    queryStringProperties[queryStringParameter.Key] = queryStringParameter.Value;
                }

                queryString = ToQueryString(queryStringProperties);
            }

            return CreateRequestUri(relativePath, queryString);
        }

        protected Uri CreateRequestUri(string relativePath, object queryStringParameters)
        {
            var parameters = queryStringParameters.ToPropertyDictionary()
                    .Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToNullOrString()))
                    .ToArray();
            return CreateRequestUri(relativePath, parameters);
        }

        protected Uri CreateRequestUri(string relativePath, string queryString)
        {
            var uriBuilder = new UriBuilder(BaseAddress + relativePath) { Query = queryString };

            return uriBuilder.Uri;
        }

        protected async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            string requestContentBase64String = string.Empty;

            string requestUri = HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());

            string requestHttpMethod = request.Method.Method;

            // Calculate UNIX time
            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = DateTime.UtcNow - epochStart;
            string requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

            // create random nonce for each request
            string nonce = Guid.NewGuid().ToString("N");

            // Checking if the request contains body, usually will be null wiht HTTP GET and DELETE
            if (request.Content != null)
            {
                byte[] content = request.Content.ReadAsByteArrayAsync().Result;
                MD5 md5 = MD5.Create();

                // Hashing the request body, any change in request body will result in different hash, we'll incure message integrity
                byte[] requestContentHash = md5.ComputeHash(content);
                requestContentBase64String = Convert.ToBase64String(requestContentHash);
            }

            // Creating the raw signature string
            string signatureRawData = string.Format("{0}{1}{2}{3}{4}{5}", napSettings.SiteId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            var secretKeyByteArray = Convert.FromBase64String(napSettings.AccessKey);

            byte[] signature = Encoding.UTF8.GetBytes(signatureRawData);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyByteArray))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);
                string requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

                // Setting the values in the Authorization header using custom scheme (amx)
                request.Headers.Authorization = new AuthenticationHeaderValue(
                    "amx",
                    string.Format("{0}:{1}:{2}:{3}", napSettings.SiteId, requestSignatureBase64String, nonce, requestTimeStamp));
            }

            var response = await httpClient.SendAsync(request);

            return response;
        }

        protected string GetServiceUrl()
        {
            if (string.IsNullOrWhiteSpace(napSettings.ServiceUrls))
            {
                throw new NAPApiException("Service Urls cannot be null or white space");
            }

            var serviceUrls = napSettings.ServiceUrls.Split(',');
            var numOfUrl = serviceUrls.Count();

            if (numOfUrl == 1)
            {
                return serviceUrls[0];
            }

            var selectedServiceUrl = serviceUrls[randomHandler.Next(0, numOfUrl)];

            return selectedServiceUrl;
        }

        private void ThrowIfResponseNotSuccessfulAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                // Do not treat not found as exception, but rather as null result
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        private static NameValueCollection ParseQueryString(string queryString)
        {
            var queryParameters = new NameValueCollection();
            string[] querySegments = queryString.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (string segment in querySegments)
            {
                string[] parts = segment.Split('=');
                if (parts.Length > 0)
                {
                    string key = parts[0].Trim(new char[] { '?', ' ' });
                    string val = parts[1].Trim();

                    queryParameters.Add(key, val);
                }
            }

            return queryParameters;
        }

        private static string ToQueryString(NameValueCollection nvc)
        {
            if (nvc == null) return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (string key in nvc.Keys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;

                string[] values = nvc.GetValues(key);
                if (values == null) continue;

                foreach (string value in values)
                {
                    if (value == null)
                    {
                        continue;
                    }
                    sb.Append(sb.Length == 0 ? "?" : "&");
                    sb.AppendFormat("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value));
                }
            }

            return sb.ToString();
        }
    }
}
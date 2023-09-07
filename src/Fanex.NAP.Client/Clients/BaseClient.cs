namespace Fanex.NAP.Client.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Configuration;
    using Exceptions;
    using Helpers.Extensions;
    using Newtonsoft.Json;

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
            using (var message = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                using (var response = await httpClient.SendAsync(message))
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            throw new NAPApiException("No permission");
                        case HttpStatusCode.InternalServerError:
                            throw new NAPApiException(await response.Content.ReadAsStringAsync());
                        default:
                        {
                            var receivedContent = await response.Content.ReadAsStringAsync();

                            return DeserializeObject<T>(requestUri, receivedContent);
                        }
                    }
                }
            }
        }

        private static T DeserializeObject<T>(Uri requestUri, string content)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception e)
            {
                throw new Exception($"=====RequestUri:{requestUri}=======Received:===\r\n {content}\r\n========", e);
            }
        }

        protected async Task<TOutput> Post<TInput, TOutput>(Uri requestUri, TInput body)
        {
            var json = JsonConvert.SerializeObject(body);
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                using (var response = await httpClient.PostAsync(requestUri, content))
                {
                    ThrowIfResponseNotSuccessfulAsync(response);
                    var receivedContent = await response.Content.ReadAsStringAsync();
                    try
                    {
                        return JsonConvert.DeserializeObject<TOutput>(receivedContent);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"=====RequestUri:{requestUri}=======Received:===\r\n {receivedContent}\r\n========", e);
                    }
                }
            }
        }

        protected Uri CreateRequestUri(string relativePath, params KeyValuePair<string, string>[] queryStringParameters)
        {
            var queryString = string.Empty;

            if (queryStringParameters == null || queryStringParameters.Length <= 0)
            {
                return CreateRequestUri(relativePath, queryString);
            }
                
            var queryStringProperties = ParseQueryString(BaseAddress.Query);
            foreach (var queryStringParameter in queryStringParameters)
            {
                queryStringProperties[queryStringParameter.Key] = queryStringParameter.Value;
            }

            queryString = ToQueryString(queryStringProperties);

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

        protected string GetServiceUrl()
        {
            if (string.IsNullOrWhiteSpace(napSettings.ServiceUrls))
            {
                throw new NAPApiException("Service Urls cannot be null or white space");
            }

            var serviceUrls = napSettings.ServiceUrls.Split(',');
            var numOfUrl = serviceUrls.Length;

            if (numOfUrl == 1)
            {
                return serviceUrls[0];
            }

            var selectedServiceUrl = serviceUrls[randomHandler.Next(0, numOfUrl)];

            return selectedServiceUrl;
        }

        private static void ThrowIfResponseNotSuccessfulAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            switch (response.StatusCode)
            {
                // Do not treat not found as exception, but rather as null result
                case HttpStatusCode.NotFound:
                    return;
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedAccessException();
            }
        }

        private static NameValueCollection ParseQueryString(string queryString)
        {
            var queryParameters = new NameValueCollection();
            var querySegments = queryString.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in querySegments)
            {
                var parts = segment.Split('=');
                if (parts.Length <= 0)
                {
                    continue;
                }

                var key = parts[0].Trim('?', ' ');
                var val = parts[1].Trim();

                queryParameters.Add(key, val);
            }

            return queryParameters;
        }

        private static string ToQueryString(NameValueCollection nvc)
        {
            if (nvc == null) return string.Empty;

            var sb = new StringBuilder();

            foreach (string key in nvc.Keys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;

                var values = nvc.GetValues(key);
                if (values == null) continue;

                foreach (var value in values)
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
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
    using Exceptions;
    using Helpers.Extensions;
    using Newtonsoft.Json;

    public class BaseClient
    {
        private readonly HttpClient httpClient;
        public BaseClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        protected async Task<T> Get<T>(string requestUri)
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

        private static T DeserializeObject<T>(string requestUri, string content)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception e)
            {
                throw new ArgumentException($"=====RequestUri:{requestUri}=======Received:===\r\n {content}\r\n========", e);
            }
        }

        protected async Task<TOutput> Post<TInput, TOutput>(string requestUri, TInput body)
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
                        throw new ArgumentException($"=====RequestUri:{requestUri}=======Received:===\r\n {receivedContent}\r\n========", e);
                    }
                }
            }

        }

        protected static string CreateRequestPath(string relativePath, params KeyValuePair<string, string>[] queryStringParameters)
        {
            var queryString = string.Empty;

            if (queryStringParameters == null || queryStringParameters.Length == 0)
            {
                return CreateRequestPath(relativePath, queryString);
            }

            var queryStringProperties = new NameValueCollection();
            foreach (var queryStringParameter in queryStringParameters)
            {
                queryStringProperties[queryStringParameter.Key] = queryStringParameter.Value;
            }

            queryString = ToQueryString(queryStringProperties);

            return CreateRequestPath(relativePath, queryString);
        }

        protected static string CreateRequestPath(string relativePath, object queryStringParameters)
        {
            var parameters = queryStringParameters.ToPropertyDictionary()
                .Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToNullOrString()))
                .ToArray();
            return CreateRequestPath(relativePath, parameters);
        }

        protected static string CreateRequestPath(string relativePath, string queryString)
        {
            return $"{relativePath}{queryString}";
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
                    sb.Append($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
                }
            }

            return sb.ToString();
        }
    }
}
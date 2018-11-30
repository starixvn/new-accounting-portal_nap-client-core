namespace Fanex.NAP.Client.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Web;
    using Fanex.NAP.Core.Extensions;

    internal class BaseClient
    {
        private readonly IRestClient restClient;

        public BaseClient(IRestClient restClient)
        {
            this.restClient = restClient ?? NapClientManager.RestClient;
        }

        protected async Task<T> Get<T>(Uri requestUri)
        {
            var result = await restClient.ExecuteGetAsync<T>(requestUri?.ToString()).ConfigureAwait(false);

            return result;
        }

        protected async Task Post<TInput>(Uri requestUri, TInput body)
        {
            await Post<TInput, object>(requestUri, body).ConfigureAwait(false);
        }

        protected async Task<TOutput> Post<TInput, TOutput>(Uri requestUri, TInput body)
        {
            var result = await restClient.ExecutePostAsync<TOutput>(requestUri?.ToString(), body).ConfigureAwait(false);

            return result;
        }

        protected Uri CreateRequestUri(string relativePath, params KeyValuePair<string, string>[] queryStringParameters)
        {
            var queryString = new StringBuilder();

            if (queryStringParameters != null && queryStringParameters.Length > 0)
            {
                foreach (var queryStringParameter in queryStringParameters)
                {
                    queryString
                        .Append(queryStringParameter.Key)
                        .Append("=")
                        .Append(queryStringParameter.Value)
                        .Append("&");
                }
            }

            return CreateRequestUri(relativePath, queryString.ToString().TrimEnd('&'));
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
            return new Uri($"{relativePath}?{queryString}", UriKind.Relative);
        }
    }
}
namespace Fanex.NAP.Core.Test.Clients
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class FakeDelegatingHandler : DelegatingHandler
    {
        private HttpResponseMessage _response;

        public FakeDelegatingHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var responseTask = new TaskCompletionSource<HttpResponseMessage>();

            _response.RequestMessage = request;
            responseTask.SetResult(_response);

            return responseTask.Task;
        }
    }
}

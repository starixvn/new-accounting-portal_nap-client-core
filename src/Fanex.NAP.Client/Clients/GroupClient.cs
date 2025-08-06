namespace Fanex.NAP.Client.Clients
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Models;

    public interface IGroupClient
    {
        Task<Group> Get(int groupId);
    }

    public class GroupClient : BaseClient, IGroupClient
    {
        public GroupClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<Group> Get(int groupId)
        {
            return await Get<Group>(CreateRequestPath(string.Format(RelativePaths.Get, groupId)));
        }

        private static class RelativePaths
        {
            public const string Get = "group/{0}";
        }
    }
}
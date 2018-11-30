namespace Fanex.NAP.Client.Clients
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client.Models;

    public interface IGroupClient
    {
        Task<Group> Get(int groupId);
    }

    internal class GroupClient : BaseClient, IGroupClient
    {
        public GroupClient() : base(null)
        {
        }

        public async Task<Group> Get(int groupId)
            => await Get<Group>(CreateRequestUri(string.Format(RelativePaths.Get, groupId))).ConfigureAwait(false);

        private static class RelativePaths
        {
            public const string Get = "/group/{0}";
        }
    }
}
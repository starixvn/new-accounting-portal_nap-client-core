namespace Fanex.NAP.Client.Clients
{
    using Fanex.NAP.Client.Models;
    using System.Threading.Tasks;

    public interface IGroupClient
    {
        Task<Group> Get(int groupId);
    }

    internal class GroupClient : BaseClient, IGroupClient
    {
        public async Task<Group> Get(int groupId)
        {
            return await Get<Group>(CreateRequestUri(string.Format(RelativePaths.Get, groupId)));
        }

        private class RelativePaths
        {
            public const string Get = "group/{0}";
        }
    }
}
namespace Fanex.NAP.Client.Clients
{
    using System.Threading.Tasks;
    using Models;

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

        private static class RelativePaths
        {
            public const string Get = "group/{0}";
        }
    }
}
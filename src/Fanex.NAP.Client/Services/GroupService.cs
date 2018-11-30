namespace Fanex.NAP.Client.Services
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Clients;
    using Fanex.NAP.Client.Models;

    public interface IGroupService
    {
        Group Get(int groupId);

        Task<Group> GetAsync(int groupId);
    }

    public class GroupService : IGroupService
    {
        private readonly IGroupClient groupClient;

        public GroupService() : this(null)
        {
        }

        public GroupService(IGroupClient groupClient)
        {
            this.groupClient = groupClient ?? new GroupClient();
        }

        public Group Get(int groupId) => GetAsync(groupId).Result;

        public async Task<Group> GetAsync(int groupId)
        {
            Check.Argument.IsNotNegativeOrZero(groupId, "groupId");

            return await groupClient.Get(groupId);
        }
    }
}
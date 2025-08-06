namespace Fanex.NAP.Client.Services
{
    using System.Threading.Tasks;
    using Clients;
    using Models;

    public interface IGroupService
    {
        Group Get(int groupId);

        Task<Group> GetAsync(int groupId);
    }

    public class GroupService : IGroupService
    {
        private readonly IGroupClient groupClient;
        public GroupService(IGroupClient groupClient)
        {
            this.groupClient = groupClient;
        }

        public Group Get(int groupId) => GetAsync(groupId).Result;

        public async Task<Group> GetAsync(int groupId)
        {
            return await groupClient.Get(groupId);
        }
    }
}
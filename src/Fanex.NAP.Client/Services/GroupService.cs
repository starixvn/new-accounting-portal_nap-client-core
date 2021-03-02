namespace Fanex.NAP.Client.Services
{
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.Clients;
    using Fanex.NAP.Client.Models;
    using System.Threading.Tasks;

    public interface IGroupService
    {
        Group Get(int groupId);

        Task<Group> GetAsync(int groupId);
    }

    public class GroupService : IGroupService
    {
        public Group Get(int groupId) => GetAsync(groupId).Result;

        public async Task<Group> GetAsync(int groupId)
        {
            Check.Argument.IsNotNegativeOrZero(groupId, "groupId");
            var groupClient = new GroupClient();

            return await groupClient.Get(groupId);
        }
    }
}
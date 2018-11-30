
namespace Fanex.Nap.Client.AspNetCore.Controllers
{
    using Fanex.NAP.Core.Models;
    using Fanex.NAP.Core.Service.Impl;
    using Microsoft.AspNetCore.Mvc;

    public class GroupClientController : Controller
    {
        private readonly GroupService _groupService;

        public GroupClientController()
        {
            _groupService = new GroupService();
        }

        public Group Get(int id) => _groupService.Get(id);
    }
}
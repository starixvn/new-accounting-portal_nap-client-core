namespace Fanex.Nap.Client.AspNetCore.Controllers
{
    using System.Collections.Generic;
    using Fanex.NAP.Client.Models;
    using Fanex.NAP.Client.Models.ValueObject;
    using Fanex.NAP.Client.Services;
    using Microsoft.AspNetCore.Mvc;
    using NAP.Core.Business;

    public class UserClientController : Controller
    {
        private readonly UserService _userService;

        public UserClientController()
        {
            _userService = new UserService();
        }

        public User GetById(int id) => _userService.Get(id);

        public User GetByName(string name) => _userService.Get(name);

        public IEnumerable<User> GetAll() => _userService.GetAll();

        public int GetUserLevel(int userId, int siteId) => _userService.GetUserLevel(userId, siteId);

        public int GetPermission(int userId, int siteId, string functionKey) => _userService.GetPermission(userId, siteId, functionKey);

        public bool IsHavingPermission(int userId, int siteId, string functionKey, Constant.Policy policy) => _userService.IsHavingPermission(8, 1, "ManageUser", Constant.Policy.Host);

        public IEnumerable<FunctionPolicy> GetPermissions(int userId, int siteId, string functionKey) => _userService.GetPermissions(userId, siteId, functionKey);
    }
}
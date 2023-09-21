namespace Fanex.NAP.Client.Demo.AspNetCore.Controllers;

using System.Collections.Generic;
using Client.Models;
using Client.Models.ValueObject;
using Microsoft.AspNetCore.Mvc;
using Services;

public class UserClientController : Controller
{
    private readonly UserService _userService;

    public UserClientController()
    {
        _userService = new UserService();
    }

    public User GetById(int id) => _userService.Get(id);

    public User GetByName(string name) => _userService.Get(name);

    public int GetPermission(int userId, int siteId, string functionKey) => _userService.GetPermission(userId, siteId, functionKey);

    public IEnumerable<FunctionPolicy> GetPermissions(int userId, int siteId, string functionKey) => _userService.GetPermissions(userId, siteId, functionKey);
}
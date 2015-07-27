namespace Demo.Controllers;

using System.Collections.Generic;
using Fanex.NAP.Client.Models;
using Fanex.NAP.Client.Models.ValueObject;
using Fanex.NAP.Client.Services;
using Microsoft.AspNetCore.Mvc;

public class UserClientController(IUserService userService) : Controller
{
    public User GetById(int id) => userService.Get(id);

    public User GetByName(string name) => userService.Get(name);

    public int GetPermission(int userId, int siteId, string functionKey) => userService.GetPermission(userId, siteId, functionKey);

    public IEnumerable<FunctionPolicy> GetPermissions(int userId, int siteId, string functionKey) => userService.GetPermissions(userId, siteId, functionKey);
}
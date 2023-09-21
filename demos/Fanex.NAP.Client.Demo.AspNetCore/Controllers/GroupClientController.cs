namespace Fanex.NAP.Client.Demo.AspNetCore.Controllers;

using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

public class GroupClientController : Controller
{
    private readonly GroupService groupService;

    public GroupClientController()
    {
        groupService = new GroupService();
    }

    public Group Get(int id) => groupService.Get(id);
}
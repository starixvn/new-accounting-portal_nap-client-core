namespace Demo.Controllers;

using Fanex.NAP.Client.Models;
using Fanex.NAP.Client.Services;
using Microsoft.AspNetCore.Mvc;

public class GroupClientController(IGroupService groupService) : Controller
{
    public Group Get(int id) => groupService.Get(id);
}
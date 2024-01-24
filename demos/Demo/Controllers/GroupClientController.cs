namespace Demo.Controllers;

using Fanex.NAP.Client.Models;
using Fanex.NAP.Client.Services;
using Microsoft.AspNetCore.Mvc;

public class GroupClientController : Controller
{
    private readonly GroupService groupService = new();

    public Group Get(int id) => groupService.Get(id);
}
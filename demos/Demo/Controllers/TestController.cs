namespace Demo.Controllers;

using System.IO;
using Fanex.NAP.Client.Helpers.Business;
using Fanex.NAP.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[AllowAnonymous]
public class TestController : Controller
{
    private const string TestFolder = @"F:\temp\napapinew";

    public IActionResult Index()
    {
        CleanTestFolder();
        TestSiteService();
        TestGroupService();
        TestUserService();
        return Content("OK");
    }

    private void CleanTestFolder()
    {
        if (Directory.Exists(TestFolder))
        {
            Directory.Delete(TestFolder, true);
        }
        
        Directory.CreateDirectory(TestFolder);
    }
    private void TestSiteService()
    {
        var siteService = new SiteService();
        var menus = siteService.GetMenuItemBySiteId(99, "UAT", null);
        var json = JsonConvert.SerializeObject(menus, Formatting.Indented);
        System.IO.File.WriteAllText(@$"{TestFolder}\\site.json", json);
    }

    private void TestGroupService()
    {
        var groupService = new GroupService();
        var group = groupService.Get(2);
        var json = JsonConvert.SerializeObject(group, Formatting.Indented);
        System.IO.File.WriteAllText(@$"{TestFolder}\\group.json", json);
    }

    private void TestUserService()
    {
        var service = new UserService();

        var users = service.GetBySite(99, true);
        var json = JsonConvert.SerializeObject(users, Formatting.Indented);
        System.IO.File.WriteAllText(@$"{TestFolder}\\user.json", json + "\r\n");

        var user = service.Get(8);
        json = JsonConvert.SerializeObject(user, Formatting.Indented);
        System.IO.File.AppendAllText(@$"{TestFolder}\\user.json", json + "\r\n");

        user = service.Get("starix1");
        json = JsonConvert.SerializeObject(user, Formatting.Indented);
        System.IO.File.AppendAllText(@$"{TestFolder}\\user.json", json + "\r\n");

        var menuItems =service.GetMenuItem(user.Id, 99);
        json = JsonConvert.SerializeObject(menuItems, Formatting.Indented);
        System.IO.File.AppendAllText(@$"{TestFolder}\\user.json", json + "\r\n");

        var permission =service.GetPermission(user.Id, 99, "LAKpi");
        json = JsonConvert.SerializeObject(permission, Formatting.Indented);
        System.IO.File.AppendAllText(@$"{TestFolder}\\user.json", json + "\r\n");

        var functions = service.GetPermissions(user.Id, 99, "LeagueManagement");
        json = JsonConvert.SerializeObject(functions, Formatting.Indented);
        System.IO.File.AppendAllText(@$"{TestFolder}\\user.json", json + "\r\n");

        var siteLevels = service.GetSiteLevel(user.Id);
        json = JsonConvert.SerializeObject(siteLevels, Formatting.Indented);
        System.IO.File.AppendAllText(@$"{TestFolder}\\user.json", json + "\r\n");

        var boolVar= service.IgnoredOtp(15512817, "NAP");
        json = JsonConvert.SerializeObject(boolVar, Formatting.Indented);
        System.IO.File.AppendAllText(@$"{TestFolder}\\user.json", json + "\r\n");

        boolVar = service.IsHavingPermission(user.Id, 99, "LAKpi", Constant.Policy.Host);
        json = JsonConvert.SerializeObject(boolVar, Formatting.Indented);
        System.IO.File.AppendAllText(@$"{TestFolder}\\user.json", json + "\r\n");

        boolVar = service.VerifyOtp(15512817, "NAP", "123456");
        json = JsonConvert.SerializeObject(boolVar, Formatting.Indented);
        System.IO.File.AppendAllText(@$"{TestFolder}\\user.json", json + "\r\n");

        var level = service.GetUserLevel(user.Id, 99);
        json = JsonConvert.SerializeObject(level, Formatting.Indented);
        System.IO.File.AppendAllText(@$"{TestFolder}\\user.json", json + "\r\n");
    }
}
namespace Demo.Controllers;

using System.IO;
using System.Linq;
using Fanex.NAP.Client.Helpers.Business;
using Fanex.NAP.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[AllowAnonymous]
public class TestController : Controller
{
    //private const string TestFolder = @"F:\temp\napapinew";
    private const string TestFolder = @"F:\temp\napapiold";

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
        var filename = @$"{TestFolder}\\user.json";
        var users = service.GetBySite(99, true).OrderBy(u => u.Id).ToList();
        var json = JsonConvert.SerializeObject(users, Formatting.Indented);
        System.IO.File.WriteAllText(filename, "Test 1:\r\n");
        System.IO.File.AppendAllText(filename, json + "\r\n");

        var user = service.Get(8);
        json = JsonConvert.SerializeObject(user, Formatting.Indented);
        System.IO.File.AppendAllText(filename, "Test 2:\r\n");
        System.IO.File.AppendAllText(filename, json + "\r\n");

        user = service.Get("starix1");
        json = JsonConvert.SerializeObject(user, Formatting.Indented);
        System.IO.File.AppendAllText(filename, "Test 3:\r\n");
        System.IO.File.AppendAllText(filename, json + "\r\n");

        var menuItems = service.GetMenuItem(user.Id, 99);
        System.IO.File.AppendAllText(filename, "Test 4:\r\n");
        json = JsonConvert.SerializeObject(menuItems, Formatting.Indented);
        System.IO.File.AppendAllText(filename, json + "\r\n");

        var permission = service.GetPermission(user.Id, 99, "LAKpi");
        json = JsonConvert.SerializeObject(permission, Formatting.Indented);
        System.IO.File.AppendAllText(filename, "Test 5:\r\n");
        System.IO.File.AppendAllText(filename, json + "\r\n");

        var functions = service.GetPermissions(user.Id, 99, "LeagueManagement");
        json = JsonConvert.SerializeObject(functions, Formatting.Indented);
        System.IO.File.AppendAllText(filename, "Test 6:\r\n");
        System.IO.File.AppendAllText(filename, json + "\r\n");

        var siteLevels = service.GetSiteLevel(user.Id).OrderBy(s=>s.SiteId).ToList();
        json = JsonConvert.SerializeObject(siteLevels, Formatting.Indented);
        System.IO.File.AppendAllText(filename, "Test 7:\r\n");
        System.IO.File.AppendAllText(filename, json + "\r\n");

        var boolVar = service.IgnoredOtp(15512817, "NAP");
        json = JsonConvert.SerializeObject(boolVar, Formatting.Indented);
        System.IO.File.AppendAllText(filename, "Test 8:\r\n");
        System.IO.File.AppendAllText(filename, json + "\r\n");

        boolVar = service.IsHavingPermission(user.Id, 99, "LAKpi", Constant.Policy.Host);
        json = JsonConvert.SerializeObject(boolVar, Formatting.Indented);
        System.IO.File.AppendAllText(filename, "Test 9:\r\n");
        System.IO.File.AppendAllText(filename, json + "\r\n");

        boolVar = service.VerifyOtp(15512817, "NAP", "123456");
        json = JsonConvert.SerializeObject(boolVar, Formatting.Indented);
        System.IO.File.AppendAllText(filename, "Test 10:\r\n");
        System.IO.File.AppendAllText(filename, json + "\r\n");

        var level = service.GetUserLevel(user.Id, 99);
        json = JsonConvert.SerializeObject(level, Formatting.Indented);
        System.IO.File.AppendAllText(filename, "Test 11:\r\n");
        System.IO.File.AppendAllText(filename, json + "\r\n");
    }
}
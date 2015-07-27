namespace Fanex.NAP.SignIn;

using System;
using System.Diagnostics;
using System.Net;
using System.Runtime;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ONEbook.UM.AspNetCore.Filters;

[AllowAnonymous]
public class HealthCheck : Controller
{
    private static bool alive = true;

    [HttpGet]
    [SkipUMFilter]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public ActionResult Index()
    {
        return alive
            ? Ok(Environment.MachineName)
            : Problem(
                "Alive is false",
                Environment.MachineName,
                (int)HttpStatusCode.ServiceUnavailable,
                "Alive is false",
                "Alive is false");
    }

    [HttpGet("kill")]
    [SkipUMFilter]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Kill()
    {
        alive = false;
        return Ok($"{Environment.MachineName} - Alive: {alive}");
    }

    [HttpGet]
    [SkipUMFilter]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public ActionResult Redis()
    {
        HttpContext.Session.SetString("Now", DateTimeOffset.Now.ToString());

        return Content(HttpContext.Session.GetString("Now"));
    }

    [HttpGet]
    [SkipUMFilter]
    public IActionResult Gc()
    {
        const int megabyte = 1024 * 1024;
        var stringBuilder = new StringBuilder();
        var sw = Stopwatch.StartNew();
        sw.Start();

        using var currentProcess = Process.GetCurrentProcess();
        currentProcess.Refresh();
        var memory = currentProcess.WorkingSet64 / megabyte;

        stringBuilder.Append($"Before GC.Collect: {memory} Mb.\r\n");

        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, true, true);
        GC.WaitForPendingFinalizers();
        GC.WaitForFullGCComplete();

        currentProcess.Refresh();
        memory = currentProcess.WorkingSet64 / megabyte;

        var duration = sw.ElapsedMilliseconds;
        stringBuilder.Append($"After GC.Collect. Usage memory = {memory}.\r\n");
        stringBuilder.Append($"Duration: {duration} milliseconds.\r\n");

        return Ok(stringBuilder.ToString());
    }
}
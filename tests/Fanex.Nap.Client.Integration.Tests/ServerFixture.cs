namespace Fanex.Nap.Client.Integration.Tests;

using System;
using System.IO;
using System.Net.Http;
using Demo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

public class ServerFixture
{
    public ServerFixture()
    {
        var server = new TestServer(
            new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\")))
                    .AddJsonFile("appsettings.json")
                    .Build()
                )
                .UseStartup<Startup>());

        Client = server.CreateClient();
    }

    public HttpClient Client { get; }
}
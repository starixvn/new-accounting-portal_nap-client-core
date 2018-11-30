using System;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Fanex.Nap.Client.AspNetCore.Tests
{
    public class ServerFixture
    {
        private readonly TestServer _server;
        
        public ServerFixture()
        {
            _server = new TestServer(
                new WebHostBuilder() 
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseConfiguration(new ConfigurationBuilder()
                        .SetBasePath(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,"..\\..\\..\\")))
                        .AddJsonFile("appsettings.json")
                        .Build()
                    )
                    .UseStartup<Startup>());
            Client = _server.CreateClient();
        }

        public HttpClient Client { get; }
    }
}
using System;
using System.Threading.Tasks;
using Fanex.NAP.Client;
using Fanex.NAP.Client.Configuration;
using Fanex.NAP.Client.Helpers.Web;
using Fanex.NAP.Client.Services;
using Fanex.NAP.Client.Session;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddControllersWithViews();
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = configuration.GetSection("AuthSettings:LoginPath").Value;
        options.LogoutPath = configuration.GetSection("AuthSettings:LogoutPath").Value;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(int.Parse(configuration.GetSection("AuthSettings:CookieTimeOut").Value));
        options.Cookie = new CookieBuilder
        {
            Name = "Fanex.Demo.Cookie",
            SecurePolicy = CookieSecurePolicy.SameAsRequest
        };
        options.Events.OnSigningOut = context =>
        {
            context.Response.Redirect(options.LoginPath);
            return Task.CompletedTask;
        };
    });

services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
});

services.AddSingleton(new UMClientConfiguration().Initialize(configuration));

services.AddSession();
services.AddSingleton<INapSession, NapSession>();
services.AddSingleton<INapSettings>(new NapSettings(configuration));
services.AddSingleton<IGroupService, GroupService>();
services.AddSingleton<IUserService, UserService>();
services.AddSingleton<ISecurityService, SecurityService>();
services.AddSingleton<ISiteService, SiteService>();

services.AddSingleton<IMaintenanceService, MaintenanceService>();
services.AddSingleton<IUMCacheService, UMCacheService>();
services.AddSingleton<IUMRestClient, UMRestClient>();
services.AddSingleton<IHttpContextHelper, HttpContextHelper>();
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


var app = builder.Build();

NapClientManager
    .UseConfig(app.Services.GetService<INapSettings>())
    .UseSession(app.Services.GetService<INapSession>());


app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
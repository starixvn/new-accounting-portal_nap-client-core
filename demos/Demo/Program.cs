using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Demo;
using Fanex.NAP.Client;
using ONEbook.UM.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

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

services.AddOneBookUmForWeb(configuration);
services.AddAuthenticationRedis(configuration);
services.AddSession();
services.AddNapClient(configuration);

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
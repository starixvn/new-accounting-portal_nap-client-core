namespace Fanex.Nap.Client.AspNetCore.DemoSite
{
    using System.Threading.Tasks;
    using Fanex.NAP.Client;
    using Fanex.NAP.Client.AspNetCore.Configuration;
    using Fanex.NAP.Client.AspNetCore.Session;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Services;
    using Fanex.NAP.Client.Transport;
    using Fanex.NAP.Client.Web;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login/Index";
                    options.LogoutPath = "/Login/Logout";
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

            services.AddSession();
            services.AddSingleton<INapSettings>(new NapSettings(Configuration));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<INapSession, NapSession>();
            services.AddSingleton<IRestClient, RestClient>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ISecurityService, SecurityService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Login}/{action=Index}/{id?}");
            });

            NapClientManager
                .UseConfig(app.ApplicationServices.GetService<INapSettings>())
                .UseRestClient(app.ApplicationServices.GetService<IRestClient>())
                .UseSession(app.ApplicationServices.GetService<INapSession>());
        }
    }
}
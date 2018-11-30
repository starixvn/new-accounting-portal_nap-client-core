ASP.NET Core
==============

Install
-------

`Fanex.NAP.Client.AspNetCore`_

`Fanex.NAP.Client.Transport`_


Configuration
-------------

Please add to your *appsettings.json* file

.. code:: json

       "Fanex.NAP.Client": {
           "Environment": "UAT",
           "SiteId": 105,
           "UnauthorizeUrl": "~/Common/Unauthorize",
           "ServiceUrls": "http://ha.nexdev.net:1710/api",
           "AccessKey": "Czl6CxZgAX8e6h+Tqf6InhfilmoHEO03XeS7qQgVmkE=",
           "CacheDuration": 20,
           "EnableDirectLogin": false,
           "EnableReCheckIn": false,
            "HubUrl": ""
       }

Integration
-----------

Please add these code lines to your *Startup.cs* file

.. code:: csharp

       public void ConfigureServices(IServiceCollection services){
           ...
            services.AddSession();
            services.AddSingleton<INapSettings>(new NapSettings(Configuration));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<INapSession, NapSession>();
            services.AddSingleton<IRestClient, RestClient>();
           ...
       }

       public void Configure(IApplicationBuilder app, IHostingEnvironment env){
           ...
           app.UseSession();
           NapClientManager
                .UseConfig(app.ApplicationServices.GetService<INapSettings>())
                .UseRestClient(app.ApplicationServices.GetService<IRestClient>())
                .UseSession(app.ApplicationServices.GetService<INapSession>());
       }



Usage
--------

1) Using client to get information
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code:: csharp

       var userService = new UserService(); 
       var user = userService.Get(userName);

2)  Using NAP Auth Attribute
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

.. code:: csharp

    [NapSuperAuth]
    public class HomeController : Controller
    {
        [NapAuth(FunctionKey = "Home", Policy = Constant.Policy.Read, PermissionCache = true)]
        public IActionResult Index()
        {
            return View();
        }
    }



.. _Fanex.NAP.Client.AspNetCore: http://nuget.nexdev.net/packages/Fanex.NAP.Client.AspNetCore/
.. _Fanex.NAP.Client.Transport: http://nuget.nexdev.net/packages/Fanex.NAP.Client.Transport/
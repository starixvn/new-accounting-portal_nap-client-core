ASP.NET Core
==============

Install
-------

`Fanex.NAP.Client.AspNetMVC`_

`Fanex.NAP.Client.Transport`_


Configuration
-------------

Please add to your *web.config* file

.. code:: xml

    <add key="Fanex.NAP.Client:Environment" value="UAT" />
    <add key="Fanex.NAP.Client:SiteId" value="105" />
    <add key="Fanex.NAP.Client:UnauthorizeUrl" value="~/Common/Unauthorize" />
    <add key="Fanex.NAP.Client:ServiceUrls" value="http://ha.nexdev.net:1710/api" />
    <add key="Fanex.NAP.Client:AccessKey" value="Czl6CxZgAX8e6h+Tqf6InhfilmoHEO03XeS7qQgVmkE=" />
    <add key="Fanex.NAP.Client:CacheDuration" value="20" />
    <add key="Fanex.NAP.Client:EnableDirectLogin" value="false" />
    <add key="Fanex.NAP.Client:EnableReCheckIn" value="false" />
    <add key="Fanex.NAP.Client:HubUrl" value="http://ha.nexdev.net:1710/api" />

Integration
-----------

Please add these code lines to your *Global.asax* file

.. code:: csharp

       
       protected void Application_Start() {
           ...
           var napSettings = new NapSettings();
           NapClientManager
                .UseConfig(napSettings)
                .UseRestClient(new RestClient(napSettings))
                .UseSession(new NapSession());
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



.. _Fanex.NAP.Client.AspNetMVC: http://nuget.nexdev.net/packages/Fanex.NAP.Client.AspNetMVC/
.. _Fanex.NAP.Client.Transport: http://nuget.nexdev.net/packages/Fanex.NAP.Client.Transport/
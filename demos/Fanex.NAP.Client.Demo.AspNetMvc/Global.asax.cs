using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Fanex.NAP.Client.Transport;
using Fanex.NAP.Core.Configuration;
using Fanex.NAP.Core.Web.Session;

namespace Fanex.NAP.Client.Demo.AspNetMvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var napSettings = new NapSettings();
            NapClientManager
                .UseConfig(napSettings)
                .UseRestClient(new RestClient(napSettings))
                .UseSession(new NapSession());
        }
    }
}
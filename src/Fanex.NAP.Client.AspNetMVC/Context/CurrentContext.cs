namespace Fanex.NAP.Client.AspNetMVC.Context
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Web;
    using Fanex.NAP.Core.Web.Context;

    public class CurrentContext : ICurrentContext
    {
        public string IP
        {
            get
            {
                string ip = string.Empty;

                foreach (IPAddress itemIPA in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
                {
                    if (itemIPA.AddressFamily.ToString().Equals("InterNetwork"))
                    {
                        ip = itemIPA.ToString();
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(ip))
                {
                    return ip;
                }

                foreach (IPAddress itemIPA in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (itemIPA.AddressFamily.ToString().Equals("InterNetwork"))
                    {
                        ip = itemIPA.ToString();
                        break;
                    }
                }

                return ip?.Trim();
            }
        }

        public string DomainName
            => HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

        public string BrowserName
            => HttpContext.Current.Request.Browser.Browser + "/" + HttpContext.Current.Request.Browser.Version;

        public string Platform => HttpContext.Current.Request.UserAgent;

        public string CountryCode
            => ResolveCountry(HttpContext.Current.Request)?.GeoId.ToString(CultureInfo.CurrentCulture);

        public DomainElement CurrentDomain
        {
            get
            {
                string currentDomain = HttpContext.Current.Request.Url.Authority;
                string currentPath = HttpContext.Current.Request.ApplicationPath;

                return new DomainElement
                {
                    Name = currentDomain,
                    Path = currentPath
                };
            }
        }

        private RegionInfo ResolveCountry(HttpRequest request)
        {
            var culture = ResolveCulture(request);

            if (culture != null)
            {
                return new RegionInfo(culture.LCID);
            }

            return null;
        }

        private CultureInfo ResolveCulture(HttpRequest request)
        {
            string[] languages = request.UserLanguages;

            if (languages == null || languages.Length == 0)
            {
                return null;
            }

            try
            {
                string language = languages[0].ToLowerInvariant().Trim();

                return CultureInfo.CreateSpecificCulture(language);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
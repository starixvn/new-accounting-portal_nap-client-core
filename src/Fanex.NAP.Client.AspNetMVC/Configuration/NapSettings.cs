namespace Fanex.NAP.Core.Configuration
{
    using System.ComponentModel;
    using System.Configuration;
    using Fanex.NAP.Client.Configuration;

    public class NapSettings : INapSettings
    {
        public NapSettings()
        {
            SiteId = GetValue<int>("Fanex.NAP.Client:SiteId");
            EnableDirectLogin = GetValue<bool>("Fanex.NAP.Client:EnableDirectLogin");
            EnableReCheckIn = GetValue<bool>("Fanex.NAP.Client:EnableReCheckIn");
            HubUrl = GetValue<string>("Fanex.NAP.Client:HubUrl");
            Environment = GetValue<string>("Fanex.NAP.Client:Environment");
            UnauthorizeUrl = GetValue<string>("Fanex.NAP.Client:UnauthorizeUrl");
            ServiceUrls = GetValue<string>("Fanex.NAP.Client:ServiceUrls");
            AccessKey = GetValue<string>("Fanex.NAP.Client:AccessKey");
            CacheDuration = GetValue<int>("Fanex.NAP.Client:CacheDuration");
            HubUrl = GetValue<string>("Fanex.NAP.Client:HubUrl");
            ByPassProxyLocal = GetValue<bool?>("Fanex.NAP.Client:ByPassProxyLocal");
            IsNapHub = SiteId == 0;
        }

        public bool IsNapHub { get; }

        public bool EnableDirectLogin { get; }

        public bool EnableReCheckIn { get; }

        public int SiteId { get; }

        public string HubUrl { get; }

        public string Environment { get; }

        public string UnauthorizeUrl { get; }

        public string ServiceUrls { get; }

        public string AccessKey { get; }

        public int CacheDuration { get; }

        public bool? ByPassProxyLocal { get; }

        private static T GetValue<T>(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
    }
}
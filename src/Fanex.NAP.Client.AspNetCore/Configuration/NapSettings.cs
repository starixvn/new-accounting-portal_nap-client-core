namespace Fanex.NAP.Client.AspNetCore.Configuration
{
    using System;
    using System.ComponentModel;
    using Fanex.NAP.Client.Configuration;
    using Microsoft.Extensions.Configuration;

    public class NapSettings : INapSettings
    {
        private readonly IConfiguration configuration;

        public NapSettings(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

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

        private T GetValue<T>(string key)
        {
            var value = configuration[key];

            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            return (T)typeConverter.ConvertFromString(value);
        }
    }
}
namespace Fanex.NAP.Client.Configuration
{
    public interface INapSettings
    {
        bool IsNapHub { get; }

        bool EnableDirectLogin { get; }

        bool EnableReCheckIn { get; }

        int SiteId { get; }

        string HubUrl { get; }

        string Environment { get; }

        string UnauthorizeUrl { get; }

        string ServiceUrls { get; }

        string AccessKey { get; }

        int CacheDuration { get; }
    }
}
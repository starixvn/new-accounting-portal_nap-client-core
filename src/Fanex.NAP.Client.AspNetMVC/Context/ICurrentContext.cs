namespace Fanex.NAP.Core.Web.Context
{
    using Fanex.NAP.Client.AspNetMVC.Context;

    public interface ICurrentContext
    {
        string IP { get; }

        string DomainName { get; }

        string BrowserName { get; }

        string Platform { get; }

        string CountryCode { get; }

        DomainElement CurrentDomain { get; }
    }
}
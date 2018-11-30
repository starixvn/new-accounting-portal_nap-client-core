namespace Fanex.NAP.Client.Models.ParameterObject
{
    using System;

    public class LoginParameter
    {
        public int UserId { get; set; }

        public int SiteId { get; set; }

        public string NetSessionId { get; set; }

        public string Ip { get; set; }

        public DateTime LoginDate { get; set; }

        public string BrowserName { get; set; }

        public string CountryCode { get; set; }

        public string Domain { get; set; }

        public string Platform { get; set; }
    }
}
namespace Fanex.NAP.Client.Models.ParameterObject
{
    using System;

    public class SSOLoginParameter
    {
        public int UserId { get; set; }

        public int SiteId { get; set; }

        public int CheckOnUserId { get; set; }

        public int OnUserId { get; set; }

        public string IP { get; set; }

        public string NetSessionId { get; set; }

        public DateTime LoginDate { get; set; }

        public string BrowserName { get; set; }

        public string CountryCode { get; set; }

        public string Domain { get; set; }

        public string Platform { get; set; }
    }
}
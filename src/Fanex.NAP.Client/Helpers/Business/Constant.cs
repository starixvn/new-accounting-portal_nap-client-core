namespace Fanex.NAP.Client.Helpers.Business
{
    public static class Constant
    {
        public const string UAT = "UAT";

        public const string LIVE = "LIVE";

        public const string Environment = "Environment";

        public enum Policy
        {
            /// <summary>
            /// The none policy.
            /// </summary>
            None = 0,

            /// <summary>
            /// The read policy.
            /// </summary>
            Read = 1,

            /// <summary>
            /// The host policy.
            /// </summary>
            Host = 2
        }

        public enum Level
        {
            /// <summary>
            /// The user level.
            /// </summary>
            User = 0,

            /// <summary>
            /// The site admin level.
            /// </summary>
            SiteAdmin = 50,

            /// <summary>
            /// The portal admin level.
            /// </summary>
            PortalAdmin = 100
        }
    }
}
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Fanex.NAP.Client.Tests")]

namespace Fanex.NAP.Client
{
    using System;
    using Configuration;
    using Helpers.Web;

    public class NapClientManager
    {
        private INapSettings napSettings;
        private INapSession napSession;

        private static readonly NapClientManager CurrentInstance = new NapClientManager();

        public static NapClientManager UseConfig(INapSettings napSettings)
        {
            CurrentInstance.napSettings = napSettings ?? throw new ArgumentNullException(nameof(napSettings));

            return CurrentInstance;
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public NapClientManager UseSession(INapSession session)
        {
            CurrentInstance.napSession = session ?? throw new ArgumentNullException(nameof(session));

            return CurrentInstance;
        }

        public static INapSettings NapSettings
            => CurrentInstance.napSettings ?? throw new InvalidOperationException(nameof(CurrentInstance.napSettings));

        public static INapSession NapSession
            => CurrentInstance.napSession ?? throw new InvalidOperationException(nameof(CurrentInstance.napSession));
    }
}
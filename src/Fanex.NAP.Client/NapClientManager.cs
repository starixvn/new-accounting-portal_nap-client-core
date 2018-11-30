namespace Fanex.NAP.Client
{
    using System;
    using Fanex.NAP.Client.Configuration;
    using Fanex.NAP.Client.Helpers.Web;
    using Fanex.NAP.Client.Web;

    public class NapClientManager
    {
        private INapSettings napSettings;
        private IRestClient restClient;
        private INapSession napSession;

        private static NapClientManager CurrentInstance = new NapClientManager();

        public static NapClientManager UseConfig(INapSettings napSettings)
        {
            CurrentInstance.napSettings = napSettings ?? throw new ArgumentNullException(nameof(napSettings));

            return CurrentInstance;
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static

        public NapClientManager UseRestClient(IRestClient restClient)
        {
            CurrentInstance.restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));

            return CurrentInstance;
        }

        public NapClientManager UseSession(INapSession napSession)
        {
            CurrentInstance.napSession = napSession ?? throw new ArgumentNullException(nameof(napSession));

            return CurrentInstance;
        }

        public static INapSettings NapSettings
            => CurrentInstance.napSettings ?? throw new InvalidOperationException(nameof(CurrentInstance.napSettings));

        public static IRestClient RestClient
            => CurrentInstance.restClient ?? throw new InvalidOperationException(nameof(CurrentInstance.restClient));

        public static INapSession NapSession
            => CurrentInstance.napSession ?? throw new InvalidOperationException(nameof(CurrentInstance.napSession));
    }
}
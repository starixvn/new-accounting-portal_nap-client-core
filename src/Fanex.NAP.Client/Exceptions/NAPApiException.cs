namespace Fanex.NAP.Client.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NAPApiException : Exception
    {
        public NAPApiException()
        {
        }

        public NAPApiException(string message)
            : base(message)
        {
        }

        public NAPApiException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NAPApiException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Add implementation.
        }
    }
}
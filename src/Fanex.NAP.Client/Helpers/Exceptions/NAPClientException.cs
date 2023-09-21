namespace Fanex.NAP.Client.Helpers.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class NapClientException : Exception, IEquatable<NapClientException>
    {
        public NapClientException()
        {
        }

        public NapClientException(string message)
            : base(message)
        {
        }

        public NapClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NapClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Add implementation.
        }

        public bool Equals(NapClientException other)
        {
            throw new NotImplementedException();
        }
    }
}
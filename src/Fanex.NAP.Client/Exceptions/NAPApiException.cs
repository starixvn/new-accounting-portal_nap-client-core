namespace Fanex.NAP.Client.Exceptions;

using System;

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
}
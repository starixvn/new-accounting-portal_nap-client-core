namespace Fanex.NAP.Client.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class NapSuperAuthAttribute : SessionAuthorizeAttribute
    {
    }
}
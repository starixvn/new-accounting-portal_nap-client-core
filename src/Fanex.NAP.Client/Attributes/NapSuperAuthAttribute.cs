namespace Fanex.NAP.Client.Attributes;

using System;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
[Obsolete("This attribute is obsolete. Use SessionAuthorizeAttribute instead.", true)]
public class NapSuperAuthAttribute : SessionAuthorizeAttribute;
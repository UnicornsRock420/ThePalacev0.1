using System;

namespace ThePalace.Core.Server.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class AdminOnlyProtocolAttribute : Attribute
    {
    }
}

using System.Collections.Generic;

namespace System
{
    public static class GuidExts
    {
        public static class Types
        {
            public static readonly Type Guid = typeof(Guid);
            public static readonly Type GuidArray = typeof(Guid[]);
            public static readonly Type GuidList = typeof(List<Guid>);
        }

        //static GuidExts() { }
    }
}

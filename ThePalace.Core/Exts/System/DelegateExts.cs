using System.Collections.Generic;

namespace System
{
    public static class DelegateExts
    {
        public static class Types
        {
            public static readonly Type Delegate = typeof(Delegate);
            public static readonly Type DelegateArray = typeof(Delegate[]);
            public static readonly Type DelegateList = typeof(List<Delegate>);
            public static readonly Type MulticastDelegate = typeof(MulticastDelegate);
        }

        //static DelegateExts() { }
    }
}

using System.Collections.Generic;

namespace System
{
    public static class ExceptionExts
    {
        public static class Types
        {
            public static readonly Type Exception = typeof(Exception);
            public static readonly Type ExceptionArray = typeof(Exception[]);
            public static readonly Type ExceptionList = typeof(List<Exception>);
        }

        //static ExceptionExts() { }

        public static IEnumerable<TException> GetInnerExceptions<TException>(this TException ex, Func<TException, TException> next = null, Func<TException, bool> @continue = null)
            where TException : Exception
        {
            if (next == null)
                next = e => (TException)e.InnerException;
            return ex.FromHierarchy(next);
        }
    }
}

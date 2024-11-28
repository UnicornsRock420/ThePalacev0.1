using System;

namespace ThePalace.Core.Factories
{
    public class Singleton<T>
        where T : class, new()
    {
        private static Lazy<T> _current =
            new Lazy<T>();

        public static T Current =>
            _current.Value;
    }
}

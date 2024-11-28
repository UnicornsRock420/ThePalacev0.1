using Microsoft.EntityFrameworkCore;

namespace ThePalace.Core.Database.Core.Utility
{
    public static class DbConnection
    {
        public static T For<T>()
            where T : DbContext, new() =>
            new();
    }
}

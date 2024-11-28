using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ThePalace.Core.ExtensionMethods
{
    public static class DatabaseExts
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }

        public static bool HasUnsavedChanges(this DbContext dbContext)
        {
            return dbContext.ChangeTracker.HasChanges() ||
                dbContext.ChangeTracker.Entries().Any(e =>
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified ||
                    e.State == EntityState.Deleted);
        }

        public static DbCommand SqlCmd(this DbContext dbContext) =>
            dbContext.Database.GetDbConnection().CreateCommand();

        public static void ExecSql(this DbContext dbContext, string text, params SqlParameter[] args)
        {
            using (var sqlCmd = dbContext.SqlCmd())
            {
                var sb = new StringBuilder(text);

                foreach (var param in args)
                {
                    sb.Append($" {param.ParameterName}");
                }

                sqlCmd.CommandText = sb.ToString();
                sqlCmd.CommandType = CommandType.Text;
                if (args.Length > 0)
                {
                    sqlCmd.Parameters.AddRange(args);
                }

                sqlCmd.ExecuteNonQuery();
            }
        }

        public static void ExecStoredProcedure(this DbContext dbContext, string text, params SqlParameter[] args)
        {
            using (var sqlCmd = dbContext.SqlCmd())
            {
                var sb = new StringBuilder(text);

                foreach (var param in args)
                {
                    sb.Append($" {param.ParameterName}");
                }

                sqlCmd.CommandText = sb.ToString();
                sqlCmd.CommandType = CommandType.StoredProcedure;
                if (args.Length > 0)
                {
                    sqlCmd.Parameters.AddRange(args);
                }

                sqlCmd.ExecuteNonQuery();
            }
        }
    }
}

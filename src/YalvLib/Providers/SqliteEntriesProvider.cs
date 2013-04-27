using System.Collections.Generic;
using YalvLib.Domain;
using YalvLib.Model;

namespace YalvLib.Providers
{
    using System.Data;
    using System.Data.SQLite;

    public class SqliteEntriesProvider : ORMEntriesProvider
    {
        protected override IDbConnection CreateConnection(string dataSource)
        {
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder { DataSource = dataSource, FailIfMissing = true };
            string connectionString = sb.ConnectionString;
            return new SQLiteConnection(connectionString);
        }
    }
}
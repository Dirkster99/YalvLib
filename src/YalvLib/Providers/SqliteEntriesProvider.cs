namespace YalvLib.Providers
{
    using System.Data;
    using System.Data.SQLite;

    /// <summary>
    /// Base class for Sqlite based log file providers.
    /// 
    /// https://sqlite.org/index.html
    /// </summary>
    public class SqliteEntriesProvider : ORMEntriesProvider
    {
        /// <summary>
        /// Create a database connection based on a string connection descriptor.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        protected override IDbConnection CreateConnection(string dataSource)
        {
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder { DataSource = dataSource, FailIfMissing = true };

            string connectionString = sb.ConnectionString;

            return new SQLiteConnection(connectionString);
        }
    }
}
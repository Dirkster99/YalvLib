namespace YalvLib.Providers
{
    using System.Data;
    using System.Data.SQLite;

    public class SqliteEntriesProvider : AbstractEntriesProviderBase
    {
      protected override IDbConnection CreateConnection(string dataSource)
      {
        SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder { DataSource = dataSource, FailIfMissing = true };
        string connectionString = sb.ConnectionString;
        return new SQLiteConnection(connectionString);
      }
    }
}
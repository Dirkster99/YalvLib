namespace YalvLib.Providers
{
  /*** Dirkster Deactivated due to unavalability of AnyCPU implementation of SQLite
    using System.Data;
    using System.Data.SQLite;
    using YalvLib.Providers;

    public class SqliteEntriesProvider : AbstractEntriesProviderBase
    {
      protected override IDbConnection CreateConnection(string dataSource)
      {
        SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder { DataSource = dataSource, FailIfMissing = true };
        string connectionString = sb.ConnectionString;
        return new SQLiteConnection(connectionString);
      }
    }
   ***/
}
namespace YalvLib.Providers
{
  using System.Data;
  using System.Data.SqlClient;
  using YalvLib.Providers;

  public class MsSqlServerEntriesProvider : AbstractEntriesProviderBase
  {
    protected override IDbConnection CreateConnection(string dataSource)
    {
      return new SqlConnection(dataSource);
    }
  }
}
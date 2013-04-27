using System.Collections.Generic;
using YalvLib.Domain;
using YalvLib.Model;

namespace YalvLib.Providers
{
    using System.Data;
    using System.Data.SqlClient;
    using YalvLib.Providers;

    public class MsSqlServerEntriesProvider : ORMEntriesProvider
    {
        protected override IDbConnection CreateConnection(string dataSource)
        {
            return new SqlConnection(dataSource);
        }
    }
}
namespace YalvLib.Providers
{
    using System.Data;
    using System.Data.SqlClient;

    /// <summary>
    /// Base class for MS SQL Server based log file providers. 
    /// </summary>
    public class MsSqlServerEntriesProvider : ORMEntriesProvider
    {
        /// <summary>
        /// Create a database connection based on a string connection descriptor.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        protected override IDbConnection CreateConnection(string dataSource)
        {
            return new SqlConnection(dataSource);
        }
    }
}
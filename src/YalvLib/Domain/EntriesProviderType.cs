namespace YalvLib.Domain
{
    /// <summary>
    /// Enumeration to determine type of data import provider for log file
    /// </summary>
    public enum EntriesProviderType
    {
      /// <summary>
      /// Provide log file via XML persistence
      /// </summary>
      Xml,

      /// <summary>
      /// Provide log file via Sqlite persistence
      /// </summary>
      Sqlite,
      
      /// <summary>
      /// Provide log file via SQL Server persistence
      /// </summary>
      MsSqlServer
    }
}
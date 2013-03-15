namespace YalvLib.Providers
{
  using System;

  using YalvLib.Domain;
  using YalvLib.Providers;

  public static class EntriesProviderFactory
  {
    public static AbstractEntriesProvider GetProvider(EntriesProviderType type = EntriesProviderType.Xml)
    {
      switch (type)
      {
        case EntriesProviderType.Xml:            // Default type of data provider
          return new XmlEntriesProvider();

        ////case EntriesProviderType.Sqlite:       // Dirkster Deactivated due to unavalability of AnyCPU implementation of SQLite
        ////  return new SqliteEntriesProvider();

        case EntriesProviderType.MsSqlServer:
          return new MsSqlServerEntriesProvider();

        default:
          var message = string.Format("Type {0} not supported", type);
          throw new NotImplementedException(message);
      }
    }
  }
}
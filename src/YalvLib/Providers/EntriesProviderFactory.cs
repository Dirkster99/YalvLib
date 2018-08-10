namespace YalvLib.Providers
{
    using System;

    /// <summary>
    /// Abstract factory to provide an intitialized entries provider
    /// from XML, Sqlite, or SQL Server etc as requested in
    /// <see cref="GetProvider(EntriesProviderType)"/>
    /// </summary>
    public static class EntriesProviderFactory
    {
        /// <summary>
        /// Gets a new intitalzed entries provider from XML, Sqlite, or SQL Server etc 
        /// as requested in <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>The new requested type of entries provider</returns>
        public static AbstractEntriesProviderBase GetProvider(EntriesProviderType type = EntriesProviderType.Xml)
        {
            switch (type)
            {
                case EntriesProviderType.Xml:            // Default type of data provider
                    return new XmlEntriesProvider();

                case EntriesProviderType.Sqlite:
                    return new SqliteEntriesProvider();

                case EntriesProviderType.MsSqlServer:
                    return new MsSqlServerEntriesProvider();

                default:
                    var message = string.Format("Type {0} not supported", type);
                    throw new NotImplementedException(message);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YalvLib.Domain;
using YalvLib.Providers;

namespace YalvLib.Model
{

    public class LogEntrySqliteRepository : LogEntryRepository
    {
        public LogEntrySqliteRepository(String databasePath)
        {
            AbstractEntriesProvider provider = EntriesProviderFactory.GetProvider(EntriesProviderType.Sqlite);
            _logEntries = provider.GetEntries(databasePath).ToList();
        }
    }

}

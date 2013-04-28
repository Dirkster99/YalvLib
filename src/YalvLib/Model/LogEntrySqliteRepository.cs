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
        public LogEntrySqliteRepository(string path)
        {
            AbstractEntriesProviderBase provider = EntriesProviderFactory.GetProvider(EntriesProviderType.Sqlite);
            AddLogEntries(provider.GetEntries(path));
        }
    }

}

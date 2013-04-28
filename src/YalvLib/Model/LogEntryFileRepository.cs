using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YalvLib.Domain;
using YalvLib.Providers;
using YalvLib.ViewModel;

namespace YalvLib.Model
{

    public class LogEntryFileRepository : LogEntryRepository
    {
        public LogEntryFileRepository(string path)
        {
            AbstractEntriesProviderBase provider = EntriesProviderFactory.GetProvider(EntriesProviderType.Xml);
            AddLogEntries(provider.GetEntries(path));
        }
    }

}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using YalvLib.Providers;
using YalvLib.ViewModel;

namespace YalvLib.Model
{

    public class LogEntryFileRepository : LogEntryRepository
    {
        public LogEntryFileRepository(String path)
        {
            AbstractEntriesProviderBase provider = EntriesProviderFactory.GetProvider();
            _logEntries = provider.GetEntries(path).ToList();
        }
    }

}

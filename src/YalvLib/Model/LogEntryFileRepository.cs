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

        public LogEntryFileRepository()
        {}

        public LogEntryFileRepository(string path)
        {
            Path = path;
            AbstractEntriesProviderBase provider = EntriesProviderFactory.GetProvider(EntriesProviderType.Xml);
            AddLogEntries(provider.GetEntries(path));
        }

        public override bool Equals(object repo)
        {
            var rep = repo as LogEntryFileRepository;
            if (rep == null)
                return false;
            return this.Path.Equals(rep.Path);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}

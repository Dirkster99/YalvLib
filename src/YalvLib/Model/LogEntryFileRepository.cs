using YalvLib.Providers;

namespace YalvLib.Model
{
    /// <summary>
    /// This class represent a repository based on a xml typed file
    /// </summary>
    public class LogEntryFileRepository : LogEntryRepository
    {
        /// <summary>
        /// Empty constructor for the sql mapping
        /// </summary>
        public LogEntryFileRepository()
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">path of the file</param>
        public LogEntryFileRepository(string path)
        {
            Path = path;
            AbstractEntriesProviderBase provider = EntriesProviderFactory.GetProvider();
            AddLogEntries(provider.GetEntries(path));
        }

        public override bool Equals(object repo)
        {
            var rep = repo as LogEntryFileRepository;
            if (rep == null)
                return false;
            return Path.Equals(rep.Path);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
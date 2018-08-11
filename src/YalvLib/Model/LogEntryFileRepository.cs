namespace YalvLib.Model
{
    using YalvLib.Providers;

    /// <summary>
    /// This class represent a repository based on a xml typed file
    /// </summary>
    public class LogEntryFileRepository : LogEntryRepository
    {
        #region constructors
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
        #endregion constructors

        #region methods
        /// <summary>
        /// Determines whether this object is equal to <paramref name="repo"/> or not.
        /// </summary>
        /// <param name="repo"></param>
        /// <returns></returns>
        public override bool Equals(object repo)
        {
            var rep = repo as LogEntryFileRepository;
            if (rep == null)
                return false;
            return Path.Equals(rep.Path);
        }

        /// <summary>
        /// Gets the hash code for this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
        #endregion methods
    }
}
namespace YalvLib.Infrastructure.Mappings
{
    using FluentNHibernate.Mapping;
    using YalvLib.Model;

    /// <summary>
    /// Defines a mapping for an entity. Class is derived from
    /// FluentNHibernate <see cref="ClassMap{T}"/> to create a mapping,
    /// and use the constructor to control how your entity is persisted.
    /// </summary>
    public class LogEntryMapping : ClassMap<LogEntry>
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public LogEntryMapping()
        {
            Not.LazyLoad();
            Id(x => x.Uid).GeneratedBy.Guid();
            Map(x => x.Id);
            Map(x => x.GuId);
            Map(x => x.TimeStamp);
            Map(x => x.LevelIndex);
            Map(x => x.Message);
            Map(x => x.Logger);
            Map(x => x.Class);
            Map(x => x.File);
            Map(x => x.Method);
            Map(x => x.Line);
            Map(x => x.Thread);
            Map(x => x.Throwable);
            Map(x => x.Delta);
            Map(x => x.Path);
            Map(x => x.App);
            Map(x => x.HostName);
            Map(x => x.MachineName);
            Map(x => x.UserName);
        }
    }
}
namespace YalvLib.Infrastructure.Mappings
{
    using FluentNHibernate.Mapping;
    using YalvLib.Model;

    /// <summary>
    /// Defines a mapping for an entity. Class is derived from
    /// FluentNHibernate <see cref="ClassMap{T}"/> to create a mapping,
    /// and use the constructor to control how your entity is persisted.
    /// </summary>
    public class LogEntryRepositoryMapping : ClassMap<LogEntryRepository>
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public LogEntryRepositoryMapping()
        {
            Not.LazyLoad();
            Id(x => x.Uid).GeneratedBy.Guid();
            HasMany(x => x.LogEntries)
                .Cascade.All()
                .Not.LazyLoad();
            Map(x => x.Path);

        }

    }

}

namespace YalvLib.Infrastructure.Mappings
{
    using FluentNHibernate.Mapping;
    using YalvLib.Model;

    /// <summary>
    /// Defines a mapping for an entity. Class is derived from
    /// FluentNHibernate <see cref="ClassMap{T}"/> to create a mapping,
    /// and use the constructor to control how your entity is persisted.
    /// </summary>
    public class LogAnalysisMapping: ClassMap<LogAnalysis>
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public LogAnalysisMapping()
        {
            Not.LazyLoad();
            Id(x => x.Uid).GeneratedBy.Guid();

            HasMany(x => x.TextMarkers).Not.LazyLoad()
                .Cascade.All();

            HasMany(x => x.ColorMarkers).Not.LazyLoad()
                .Cascade.All();

            HasMany(x => x.Filters).Not.LazyLoad()
                .Cascade.All();
        }
    }
}

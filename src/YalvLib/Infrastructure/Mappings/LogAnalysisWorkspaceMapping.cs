namespace YalvLib.Infrastructure.Mappings
{
    using FluentNHibernate.Mapping;
    using YalvLib.Model;

    /// <summary>
    /// Defines a mapping for an entity. Class is derived from
    /// FluentNHibernate <see cref="ClassMap{T}"/> to create a mapping,
    /// and use the constructor to control how your entity is persisted.
    /// </summary>
    public class LogAnalysisWorkspaceMapping : ClassMap<LogAnalysisWorkspace>
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public LogAnalysisWorkspaceMapping()
        {
            Not.LazyLoad();
            Id(x => x.Uid).GeneratedBy.Guid();
            HasMany(x => x.SourceRepositories)
                .Cascade.All()
                .Not.LazyLoad();
            HasMany(x => x.Analyses).Cascade.All().Not.LazyLoad();
        }
    }
}

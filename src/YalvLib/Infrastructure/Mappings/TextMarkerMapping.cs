namespace YalvLib.Infrastructure.Mappings
{
    using FluentNHibernate.Mapping;
    using YalvLib.Model;

    /// <summary>
    /// Defines a mapping for an entity. Class is derived from
    /// FluentNHibernate <see cref="ClassMap{T}"/> to create a mapping,
    /// and use the constructor to control how your entity is persisted.
    /// </summary>
    public class TextMarkerMapping : ClassMap<TextMarker>
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public TextMarkerMapping()
        {
            Not.LazyLoad();
            Id(x => x.Uid).GeneratedBy.Guid();
            HasManyToMany(x => x.LogEntries);   // Can't be done with interfaces :-(
            Map(x => x.DateCreation);
            Map(x => x.DateLastModification);
            Map(x => x.Author);
            Map(x => x.Message);
        }
    }
}

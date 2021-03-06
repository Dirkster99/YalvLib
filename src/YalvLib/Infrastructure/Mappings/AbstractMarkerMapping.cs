﻿namespace YalvLib.Infrastructure.Mappings
{
    using FluentNHibernate.Mapping;
    using YalvLib.Model;

    /// <summary>
    /// Defines a mapping for an entity. Class is derived from
    /// FluentNHibernate <see cref="ClassMap{T}"/> to create a mapping,
    /// and use the constructor to control how your entity is persisted.
    /// </summary>
    class AbstractMarkerMapping : ClassMap<AbstractMarker>
    {
        /// <summary>
        /// Class constructor
        /// </summary>
        public AbstractMarkerMapping()
        {
            Not.LazyLoad();
            Id(x => x.Uid).GeneratedBy.Guid();
            HasManyToMany(x => x.LogEntries);
            Map(x => x.DateCreation);
            Map(x => x.DateLastModification);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Mappings
{

    public class LogAnalysisWorkspaceMapping : ClassMap<LogAnalysisWorkspace>
    {

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

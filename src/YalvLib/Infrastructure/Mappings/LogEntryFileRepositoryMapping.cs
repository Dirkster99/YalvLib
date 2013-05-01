using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Mappings
{

    public class LogEntryFileRepositoryMapping : SubclassMap<LogEntryFileRepository>
    {

        public LogEntryFileRepositoryMapping()
        {
            Not.LazyLoad();
            Map(x => x.Path);
        }

    }

}

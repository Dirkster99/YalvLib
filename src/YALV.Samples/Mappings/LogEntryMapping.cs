using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using YalvLib.Model;

namespace YALV.Samples.Mappings
{

    public class LogEntryMapping : ClassMap<SqliteLogEntry>
    {

        public LogEntryMapping()
        {
            Not.LazyLoad();
            Id(x => x.Id);
            Map(x => x.Caller);
            Map(x => x.Date);
            Map(x => x.Level);
            Map(x => x.Logger);
            Map(x => x.Thread);
            Map(x => x.Message);
            Map(x => x.Exception);
        }

    }

}

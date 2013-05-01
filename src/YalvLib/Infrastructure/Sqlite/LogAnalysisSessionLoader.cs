using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using YalvLib.Infrastructure.Mappings;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Sqlite
{

    public class LogAnalysisSessionLoader
    {

        private String _path;

        public LogAnalysisSessionLoader(String path)
        {
            _path = path;
        }

        public LogAnalysisSession Load()
        {
            var sessionFactory = Fluently.Configure()
                                        .Database(SQLiteConfiguration.Standard.UsingFile(_path))
                                      .Mappings(m =>
                                      {
                                          m.FluentMappings.Add<LogAnalysisSessionMapping>();
                                          m.FluentMappings.Add<LogEntryFileRepositoryMapping>();
                                          m.FluentMappings.Add<LogEntryRepositoryMapping>();
                                          m.FluentMappings.Add<LogEntryMapping>();
                                      })
                                      .ExposeConfiguration(BuildSchema)
                                      .BuildSessionFactory();

            LogAnalysisSession logAnalysisSession = null;
            using (ISession session = sessionFactory.OpenSession())
            {
                using(ITransaction transaction = session.BeginTransaction())
                {
                    IList l = session.CreateCriteria<LogAnalysisSession>().List();
                    if (l.Count > 0)
                        logAnalysisSession = l[0] as LogAnalysisSession;
                    transaction.Commit();
                }
            }
            return logAnalysisSession;
        }

        private void BuildSchema(Configuration config)
        {
            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
              .Create(false, false);
        }

    }

}

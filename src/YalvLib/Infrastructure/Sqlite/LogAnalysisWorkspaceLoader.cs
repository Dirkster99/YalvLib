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

    public class LogAnalysisWorkspaceLoader
    {

        private String _path;

        public LogAnalysisWorkspaceLoader(String path)
        {
            _path = path;
        }

        public LogAnalysisWorkspace Load()
        {
            var sessionFactory = Fluently.Configure()
                                        .Database(SQLiteConfiguration.Standard.UsingFile(_path))
                                      .Mappings(m =>
                                      {
                                          m.FluentMappings.Add<LogAnalysisWorkspaceMapping>();
                                          m.FluentMappings.Add<LogAnalysisMapping>();
                                          m.FluentMappings.Add<LogEntryFileRepositoryMapping>();
                                          m.FluentMappings.Add<LogEntryRepositoryMapping>();
                                          m.FluentMappings.Add<LogEntryMapping>();
                                          m.FluentMappings.Add<TextMarkerMapping>();
                                         
                                          
                                      })
                                      .ExposeConfiguration(BuildSchema)
                                      .BuildSessionFactory();

            LogAnalysisWorkspace logAnalysisWorkspace = null;
            using (ISession session = sessionFactory.OpenSession())
            {
                using(ITransaction transaction = session.BeginTransaction())
                {
                    IList l = session.CreateCriteria<LogAnalysisWorkspace>().List();
                    if (l.Count > 0)
                        logAnalysisWorkspace = l[0] as LogAnalysisWorkspace;
                    transaction.Commit();
                }
            }
            return logAnalysisWorkspace;
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

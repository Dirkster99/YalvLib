using System;
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

    public class ExportSession
    {

        private String _path;

        public ExportSession(String path)
        {
            _path = path;            
        }

        public void Export(LogAnalysisSession logSession)
        {
            var sessionFactory = Fluently.Configure()
                                        .Database(SQLiteConfiguration.Standard.UsingFile(_path))
                                      .Mappings(m =>
                                      {
                                          m.FluentMappings.Add<LogAnalysisSessionMapping>();
                                          m.FluentMappings.Add<LogEntryRepositoryMapping>();
                                          m.FluentMappings.Add<LogEntryMapping>();
                                      })
                                      .ExposeConfiguration(BuildSchema)
                                      .BuildSessionFactory();

            using (ISession session = sessionFactory.OpenSession())
            {
                using(ITransaction transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(logSession);
                    transaction.Commit();
                }
            }
        }

        private void BuildSchema(Configuration config)
        {
            // delete the existing db on each run
            if (File.Exists(_path))
                File.Delete(_path);

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
              .Create(false, true);
        }

    }

}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using YalvLib.Infrastructure.Mappings;
using YalvLib.Model;

namespace YalvLib.Infrastructure.Sqlite
{

    public class LogAnalysisWorkspaceExporter
    {

        private String _path;

        public LogAnalysisWorkspaceExporter(String path)
        {
            _path = path;            
        }

        public void Export(LogAnalysisWorkspace logWorkspace)
        {
            var sessionFactory = Fluently.Configure()
                                        .Database(SQLiteConfiguration.Standard.UsingFile(_path))
                                      .Mappings(m =>
                                      {
                                          m.FluentMappings.Add<LogAnalysisWorkspaceMapping>();
                                          m.FluentMappings.Add<LogAnalysisMapping>();
                                          m.FluentMappings.Add<LogEntryFileRepositoryMapping>();
                                          m.FluentMappings.Add<LogEntryRepositoryMapping>();
                                          m.FluentMappings.Add<TextMarkerMapping>();
                                          m.FluentMappings.Add<LogEntryMapping>();
                                      })
                                      .ExposeConfiguration(BuildSchema)
                                      .BuildSessionFactory();

            using (ISession session = sessionFactory.OpenSession())
            {
                using(ITransaction transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(logWorkspace);
                    transaction.Commit();
                }
            }
            sessionFactory.Close();
            MessageBox.Show("Export Done.");
        }

        private void BuildSchema(Configuration config)
        {
            // delete the existing db on each run
            if (File.Exists(_path))
                File.Delete(_path);

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
              .Create(true, true);
        }

    }

}

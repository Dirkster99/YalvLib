using System;
using System.IO;
using System.Windows.Forms;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using YalvLib.Infrastructure.Mappings;
using YalvLib.Model;
using YalvLib.Strings;

namespace YalvLib.Infrastructure.Sqlite
{
    /// <summary>
    /// This class export the LogAnalysis workspace to a SQLite file
    /// </summary>
    public class LogAnalysisWorkspaceExporter
    {
        private readonly String _path;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path of the SQLite file</param>
        public LogAnalysisWorkspaceExporter(String path)
        {
            _path = path;
        }

        /// <summary>
        /// Map and export the logAnalysis workspaces to a SQLite database
        /// </summary>
        /// <param name="logWorkspace">Log Analysis workspace to export</param>
        public void Export(LogAnalysisWorkspace logWorkspace)
        {
            ISessionFactory sessionFactory = Fluently.Configure()
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
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(logWorkspace);
                    transaction.Commit();
                }
            }
            sessionFactory.Close();
            MessageBox.Show(Resources.GlobalHelper_ExportDone_Text);
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
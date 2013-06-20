using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using YalvLib.Infrastructure.Mappings;
using YalvLib.Model;

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

        public string Directory
        {
            get { return Path.GetDirectoryName(_path); }
        }

        /// <summary>
        /// Build a session based on mapping
        /// </summary>
        /// <returns></returns>
        public ISessionFactory BuildFactory()
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
            return sessionFactory;
        }

        /// <summary>
        /// Map and export the logAnalysis workspaces to a SQLite database
        /// </summary>
        /// <param name="logWorkspace">Log Analysis workspace to export</param>
        public void Export(LogAnalysisWorkspace logWorkspace)
        {
            var cancelToken = new CancellationToken(true);
            Task taskToComplete = Task.Factory.StartNew(obj =>
            {
                ISessionFactory factory = BuildFactory();
                using (ISession session = factory.OpenSession())
                {
                    using (
                        ITransaction transaction =
                            session.BeginTransaction())
                    {
                        session.SaveOrUpdate(logWorkspace);
                        transaction.Commit();
                    }
                }
                factory.Close();
            }, cancelToken).ContinueWith(ant => ReportExportComplete());
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

        /// <summary>
        /// Event raised when the export is done
        /// </summary>
        public event EventHandler ExportResultEvent;

        /// <summary>
        /// Report the asynchronous task as having completed
        /// </summary>
        private void ReportExportComplete()
        {
            if (ExportResultEvent != null)
            {
                ExportResultEvent(this, null);
            }
        }
    }
}
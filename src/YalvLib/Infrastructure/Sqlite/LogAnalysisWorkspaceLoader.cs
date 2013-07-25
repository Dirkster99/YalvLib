using System;
using System.Collections;
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
    /// This class loads a SQLite database containing a logAnalysis Workspace
    /// </summary>
    public class LogAnalysisWorkspaceLoader
    {
        private readonly String _path;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">path of the sqlite database</param>
        public LogAnalysisWorkspaceLoader(String path)
        {
            _path = path;
        }

        /// <summary>
        /// Load a workspace from a salite database
        /// </summary>
        /// <returns></returns>
        public LogAnalysisWorkspace Load()
        {
            ISessionFactory sessionFactory = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.UsingFile(_path))
                .Mappings(m =>
                              {
                                  m.FluentMappings.Add<LogAnalysisWorkspaceMapping>();
                                  m.FluentMappings.Add<LogAnalysisMapping>();
                                  m.FluentMappings.Add<LogEntryRepositoryMapping>();
                                  m.FluentMappings.Add<TextMarkerMapping>();
                                  m.FluentMappings.Add<LogEntryMapping>();
                                  m.FluentMappings.Add<CustomFilterMapping>();
                              })
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
                LogAnalysisWorkspace logAnalysisWorkspace = null;
            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
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
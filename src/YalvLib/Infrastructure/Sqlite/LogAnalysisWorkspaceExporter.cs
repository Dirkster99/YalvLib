﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        /// <summary>
        /// Get the directory name of the chosen file
        /// </summary>
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
            try
            {       
                ISessionFactory sessionFactory = Fluently.Configure()
                    .Database(SQLiteConfiguration.Standard.UsingFile(_path))
                    .Mappings(m =>
                                  {
                                      m.FluentMappings.Add<LogAnalysisWorkspaceMapping>();
                                      m.FluentMappings.Add<LogAnalysisMapping>();
                                      m.FluentMappings.Add<LogEntryRepositoryMapping>();
                                      m.FluentMappings.Add<LogEntryMapping>();
                                      m.FluentMappings.Add<TextMarkerMapping>();
                                      m.FluentMappings.Add<ColorMarkerMapping>();
                                      m.FluentMappings.Add<CustomFilterMapping>();
                                  })
                    .ExposeConfiguration(BuildSchema)
                    .BuildSessionFactory();
                return sessionFactory;
            }catch(Exception e)
            {
                throw e.InnerException;
            }
        }

        /// <summary>
        /// Map and export the logAnalysis workspaces to a SQLite database
        /// </summary>
        /// <param name="logWorkspace">Log Analysis workspace to export</param>
        public void Export(LogAnalysisWorkspace logWorkspace)
        {
            try
            {
                var cancelToken = new CancellationToken(true);
                Task.Factory.StartNew(obj =>
                {
                    ISessionFactory factory = BuildFactory();
                    NHibernateUtil.Initialize(factory);
                    
                    using (ISession session = factory.OpenSession())
                    {
                        using (
                            ITransaction transaction =
                                session.BeginTransaction())
                        {
                            try
                            {
                                session.SaveOrUpdate(logWorkspace);
                                transaction.Commit();
                            }
                            catch (Exception e)
                            {
                                throw e.InnerException;
                            }finally
                            {
                                factory.Close();
                            }
                        }
                    }
                    factory.Close();
                }, cancelToken).ContinueWith(
                    ant => ReportExportComplete());
            }catch(Exception e)
            {
                throw e.InnerException;
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
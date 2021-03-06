﻿namespace YalvViewModelsLib.ViewModels
{
    using System.Collections.Generic;
    using Microsoft.Win32;
    using YalvLib.Infrastructure.Sqlite;
    using YalvLib.Model;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Shell;
    using Common;
    using Interfaces;
    using YalvLib.ViewModels;
    using EditableListBoxModelsLib.ViewModels.Base;
    using System.Windows.Input;

    internal class MainWindowVM : ViewModelBase, IMainWindowVM
    {
        #region fields
        public const string PropTitle = "Title";
        public const string PropRecentFileList = "RecentFileList";

        private readonly string _layoutFileName;

        private readonly YalvViewModel _yalvLogViewModel;
        private readonly LogAnalysisWorkspace _workspace;
        private RecentFileList _recentFileList;
        private bool _Disposed = false;

        /// <summary>
        /// Window to which the viewmodel is attached to
        /// </summary>
        private readonly IWinSimple _callingWin;
        #endregion fields

        #region constructor
        public MainWindowVM(IWinSimple win,
                            RecentFileList recentFileList)
        {
            _layoutFileName = Path.Combine(AppDataDirectoryPath,
                                           Assembly.GetEntryAssembly().GetName().Name + ".ColLayout");

            _callingWin = win;

            _workspace = new LogAnalysisWorkspace();
            YalvRegistry.Instance.SetActualLogAnalysisWorkspace(_workspace);

            _yalvLogViewModel = new YalvViewModel();

            CommandCancelProcessing = YalvLogViewModel.CommandCancelProcessing;

            if (CreateAppDataDir())
            {
                _yalvLogViewModel.LogEntryRows.LoadColumnsLayout(_layoutFileName);
            }

            RecentFileList = recentFileList;

            CommandExit = new RelayCommand<object>((p) => CommandExitExecute(p), p => true);
            CommandOpenFile = new RelayCommand<object>((p) => CommandOpenFileExecute(p), CommandOpenFileCanExecute);

            CommandExport = new RelayCommand<object>((p) => CommandExportExecute(p), p => true);
            CommandOpenSqliteDatabase = new RelayCommand<object>((p) => CommandOpenSqliteDatabaseExecute(p), p => true);
            CommandOpenLogAnalysisSession = new RelayCommand<object>((p) => CommandOpenLogAnalysisSessionExecute(p), p => true);
        }
        #endregion constructor

        #region static properties
        /// <summary>
        /// Get the application data directory where session and config data is to be stored.
        /// </summary>
        public static string AppDataDirectoryPath
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                                                 + Path.DirectorySeparatorChar + "Yalv";
            }
        }
        #endregion static properties

        #region Commands

        /// <summary>
        /// Exit Command
        /// </summary>
        public ICommand CommandExit { get; protected set; }

        /// <summary>
        /// OpenFile Command
        /// </summary>
        public ICommand CommandOpenFile { get; protected set; }

        public ICommand CommandExport { get; protected set; }
        public ICommand CommandOpenSqliteDatabase { get; protected set; }
        public ICommand CommandOpenLogAnalysisSession { get; protected set; }

        public ICommand CommandCancelProcessing { get; protected set; }
        #endregion

        #region Public Properties
        /// <summary>
        /// Get title of application to be displayed in the header of the window
        /// </summary>
        public string Title
        {
            get
            {
                string sFile = (YalvLogViewModel.FilePaths.Count == 0? string.Empty :
                                                                                        " - " + _yalvLogViewModel.FilePaths[0]);

                return string.Format("{0}{1}", log4netLib.Strings.Resources.MainWindow_Title, sFile);
            }
        }

        /// <summary>
        /// ViewModel container of logging related data
        /// </summary>
        public YalvViewModel YalvLogViewModel
        {
            get
            {
                return _yalvLogViewModel;
            }
        }

        /// <summary>
        /// RecentFileList Manager
        /// </summary>
        public RecentFileList RecentFileList
        {
            get
            {
                return _recentFileList;
            }

            set
            {
                _recentFileList = value;

                if (_recentFileList != null)
                {
                    _recentFileList.MenuClick += (s, e) => LoadLog4NetFile(e.Filepath);
                    UpdateJumpList();
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create user data directory for storing application session data and
        /// return true/false to indicate whether directory was already there or not.
        /// </summary>
        /// <returns></returns>
        public static bool CreateAppDataDir()
        {
            try
            {
                if (Directory.Exists(AppDataDirectoryPath) == false)
                {
                    Directory.CreateDirectory(AppDataDirectoryPath);
                    return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Load a log4net file to display its contents
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadLog4NetFile(string filePath)
        {
            try
            {
                _yalvLogViewModel.LoadFiles(new List<string>{filePath});
            }
            finally
            {
                NotifyPropertyChanged(PropTitle);
            }
        }
        #endregion

        #region Privates
        /// <summary>
        /// Save the column layout of the main data grid control.
        /// </summary>
        public void SaveColumnLayout()
        {
            try
            {
                _yalvLogViewModel.LogEntryRows.SaveColumnsLayout(_layoutFileName);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, exp.StackTrace, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Commands
        protected virtual object CommandExitExecute(object parameter)
        {
            _callingWin.Close();
            return null;
        }

        protected virtual object CommandOpenFileExecute(object parameter)
        {
            var dlg = new OpenFileDialog();

            bool addFile = parameter != null && parameter.Equals("ADD");

            dlg.Filter = YalvViewModel.FileExtensionDialogFilter;
            dlg.DefaultExt = "*.log4j";
            dlg.Multiselect = true;
            dlg.Title = addFile ? log4netLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Add_Log_File :
                                  log4netLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Open_Log_File;

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                _yalvLogViewModel.LoadFiles(new List<string>(dlg.FileNames));
         

                RecentFileList.InsertFile(dlg.FileName);

                UpdateJumpList();
            }

            return null;
        }

        protected virtual bool CommandOpenFileCanExecute(object parameter)
        {
            return true;
        }

        protected virtual object CommandExportExecute(object parameter)
        {
            LogAnalysisWorkspace workspace = YalvRegistry.Instance.ActualWorkspace;
            if (workspace != null && YalvLogViewModel.HasData)
            {
                var saveFileDialog = new SaveFileDialog {Filter = "Yalv file | *.yalv", Title = "Save a workspace"};
                saveFileDialog.ShowDialog();
                if(saveFileDialog.FileName != string.Empty)
                {
                    YalvLogViewModel.ManageRepositoriesViewModel.IsLoading = true;
                    var exporter = new LogAnalysisWorkspaceExporter(saveFileDialog.FileName);
                    exporter.ExportResultEvent += ExporterResultEvent;
                    exporter.Export(workspace);
                }
               
            }else
                MessageBox.Show("No Data to be exported !");
            
            return null;
        }

        private void ExporterResultEvent(object sender, EventArgs e)
        {
            var exporter = ((LogAnalysisWorkspaceExporter) sender);
            exporter.ExportResultEvent -= ExporterResultEvent;
            YalvLogViewModel.ManageRepositoriesViewModel.IsLoading = false;
        }

        protected virtual object CommandOpenSqliteDatabaseExecute(object parameter)
        {
            var dlg = new OpenFileDialog();

            bool addFile = parameter != null && parameter.Equals("ADD");

            dlg.Filter = YalvViewModel.DatabaseExtensionDialogFilter;
            dlg.DefaultExt = "*.db3";
            dlg.Multiselect = false;
            dlg.Title = addFile ? log4netLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Add_Log_File :
                                  log4netLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Open_Log_File;

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                _yalvLogViewModel.LoadSqliteDatabase(dlg.FileName);
                RecentFileList.InsertFile(dlg.FileName);
                UpdateJumpList();
            }
            return null;
        }


        protected virtual object CommandOpenLogAnalysisSessionExecute(object parameter)
        {
            var dlg = new OpenFileDialog();

            var addFile = parameter != null && parameter.Equals("ADD");

            dlg.Filter = YalvViewModel.LogAnalysisExtensionDialogFilter;
            dlg.DefaultExt = "*.yalv";
            dlg.Multiselect = false;
            dlg.Title = addFile ? log4netLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Add_Log_File :
                                  log4netLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Open_Log_File;

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                _yalvLogViewModel.LoadLogAnalysisSession(dlg.FileName);
                RecentFileList.InsertFile(dlg.FileName);
                UpdateJumpList();
            }
            return null;
        }
        #endregion Commands

        #region IDisposable
        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers.
        /// (required by IDisposable interface)
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern -
        /// to enable inheriting classes to participate in this pattern.
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.
            if (!this._Disposed)
            {
                if (disposing)
                {
////                    if (_yalvLogViewModel != null)
////                        _yalvLogViewModel.Dispose();

                    this.OnDispose();
                }

                // Indicate that the instance has been disposed.
                this._Disposed = true;
            }
        }

        /// <summary>
        /// Protected override-able implementation of Dispose pattern -
        /// to enable inheriting classes to participate in this pattern
        /// by overriding the original dispose implementation.
        /// </summary>
        protected virtual void OnDispose()
        {
        }
        #endregion IDisposable

        private void UpdateJumpList()
        {
            var myJumpList = JumpList.GetJumpList(Application.Current);

            if (myJumpList == null)
            {
                myJumpList = new JumpList();
                JumpList.SetJumpList(Application.Current, myJumpList);
            }

            myJumpList.JumpItems.Clear();
            if (RecentFileList != null && RecentFileList.RecentFiles != null)
            {
                foreach (string item in RecentFileList.RecentFiles)
                {
                    try
                    {
                        var myJumpTask = new JumpTask
                        {
                            CustomCategory =
                                log4netLib.Strings.Resources.
                                MainWindowVM_updateJumpList_CustomCategoryName,
                            Title = Path.GetFileName(item),
                            ApplicationPath =
                                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                            AppDomain.CurrentDomain.FriendlyName),
                            Arguments = item
                        };

                        ////myJumpTask.Description = "";
                        myJumpList.JumpItems.Add(myJumpTask);
                    }
                    catch (Exception e)
                    {
                        throw e.InnerException;
                        ////throw;
                    }
                }
            }

            myJumpList.Apply();
        }
        #endregion
    }
}

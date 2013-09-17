using System.Collections.Generic;
using Microsoft.Win32;
using YalvLib.Infrastructure.Sqlite;
using YalvLib.Model;

namespace YALV.ViewModel
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Shell;

    using Common;
    using Interfaces;
    using YalvLib.Common;
    using YalvLib.Common.Interfaces;
    using YalvLib.ViewModel;

    internal class MainWindowVM : BindableObject
    {
        #region fields
        public const string PropTitle = "Title";
        public const string PropRecentFileList = "RecentFileList";

        private readonly string _layoutFileName;

        private readonly YalvViewModel _yalvLogViewModel;
        private readonly LogAnalysisWorkspace _workspace;
        private RecentFileList _recentFileList;

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

            CommandExit = new CommandRelay(CommandExitExecute, p => true);
            CommandOpenFile = new CommandRelay(CommandOpenFileExecute, CommandOpenFileCanExecute);

            CommandAbout = new CommandRelay(CommandAboutExecute, p => true);
            CommandExport = new CommandRelay(CommandExportExecute, p => true);
            CommandOpenSqliteDatabase = new CommandRelay(CommandOpenSqliteDatabaseExecute, p => true);
            CommandOpenLogAnalysisSession = new CommandRelay(CommandOpenLogAnalysisSessionExecute, p => true);
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
        public ICommandAncestor CommandExit { get; protected set; }

        /// <summary>
        /// OpenFile Command
        /// </summary>
        public ICommandAncestor CommandOpenFile { get; protected set; }

        /// <summary>
        /// About Command
        /// </summary>
        public ICommandAncestor CommandAbout { get; protected set; }

        public ICommandAncestor CommandExport { get; protected set; }
        public ICommandAncestor CommandOpenSqliteDatabase { get; protected set; }
        public ICommandAncestor CommandOpenLogAnalysisSession { get; protected set; }

        public ICommandAncestor CommandCancelProcessing { get; protected set; }
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

                return string.Format("{0}{1}", YalvLib.Strings.Resources.MainWindow_Title, sFile);
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
                RaisePropertyChanged(PropTitle);
            }
        }
        #endregion

        #region Privates
        /// <summary>
        /// Save the column layout of the main data grid control.
        /// </summary>
        internal void SaveColumnLayout()
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
            dlg.Title = addFile ? YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Add_Log_File :
                                  YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Open_Log_File;

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

        protected virtual object CommandAboutExecute(object parameter)
        {
            var win = new View.About{ Owner = _callingWin as Window };
            win.ShowDialog();

            return null;
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
            dlg.Title = addFile ? YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Add_Log_File :
                                  YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Open_Log_File;

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
            dlg.Title = addFile ? YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Add_Log_File :
                                  YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Open_Log_File;

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                _yalvLogViewModel.LoadLogAnalysisSession(dlg.FileName);
                RecentFileList.InsertFile(dlg.FileName);
                UpdateJumpList();
            }
            return null;
        }
        #endregion Commands

        protected override void OnDispose()
        {
            if (_yalvLogViewModel != null)
                _yalvLogViewModel.Dispose();

            base.OnDispose();
        }

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
                                                     YalvLib.Strings.Resources.
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

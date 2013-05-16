using YalvLib.Infrastructure.Sqlite;
using YalvLib.Model;

namespace YALV.ViewModel
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Shell;

    using YALV.Common;
    using YALV.Interfaces;
    using YalvLib.Common;
    using YalvLib.Common.Interfaces;
    using YalvLib.ViewModel;

    internal class MainWindowVM : BindableObject
    {
        #region fields
        public const string PropTitle = "Title";
        public const string PropRecentFileList = "RecentFileList";

        private readonly string mLayoutFileName;

        private readonly YalvViewModel mYalvLogViewModel = null;
        private RecentFileList mRecentFileList;

        /// <summary>
        /// Window to which the viewmodel is attached to
        /// </summary>
        private IWinSimple mCallingWin;
        #endregion fields

        #region constructor
        public MainWindowVM(IWinSimple win,
                            RecentFileList recentFileList)
        {
            this.mLayoutFileName = System.IO.Path.Combine(MainWindowVM.AppDataDirectoryPath,
                                                          Assembly.GetEntryAssembly().GetName().Name + ".ColLayout");

            this.mCallingWin = win;

            this.mYalvLogViewModel = new YalvViewModel();

            if (MainWindowVM.CreateAppDataDir() == true)
            {
                this.mYalvLogViewModel.LogItems.LoadColumnsLayout(this.mLayoutFileName);
            }

            this.RecentFileList = recentFileList;

            this.CommandExit = new CommandRelay(this.CommandExitExecute, p => true);
            this.CommandOpenFile = new CommandRelay(this.CommandOpenFileExecute, this.CommandOpenFileCanExecute);

            this.CommandAbout = new CommandRelay(this.CommandAboutExecute, p => true);
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
                                                 + System.IO.Path.DirectorySeparatorChar + "Yalv";
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
        #endregion

        #region Public Properties
        /// <summary>
        /// Get title of application to be displayed in the header of the window
        /// </summary>
        public string Title
        {
            get
            {
                string sFile = (this.YalvLogViewModel.FilePaths.Length == 0? string.Empty :
                                                                                        " - " + this.mYalvLogViewModel.FilePaths[0]);

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
                return this.mYalvLogViewModel;
            }
        }

        /// <summary>
        /// RecentFileList Manager
        /// </summary>
        public RecentFileList RecentFileList
        {
            get
            {
                return this.mRecentFileList;
            }

            set
            {
                this.mRecentFileList = value;

                if (this.mRecentFileList != null)
                {
                    this.mRecentFileList.MenuClick += (s, e) => this.LoadLog4NetFile(e.Filepath);
                    this.UpdateJumpList();
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
                if (System.IO.Directory.Exists(MainWindowVM.AppDataDirectoryPath) == false)
                {
                    System.IO.Directory.CreateDirectory(MainWindowVM.AppDataDirectoryPath);
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
                this.mYalvLogViewModel.LoadFiles(new string[]{filePath});
            }
            finally
            {
                this.RaisePropertyChanged(PropTitle);
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
                this.mYalvLogViewModel.LogItems.SaveColumnsLayout(this.mLayoutFileName);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, exp.StackTrace.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Commands
        protected virtual object CommandExitExecute(object parameter)
        {
            this.mCallingWin.Close();
            return null;
        }

        protected virtual object CommandOpenFileExecute(object parameter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            bool addFile = parameter != null && parameter.Equals("ADD");

            dlg.Filter = YalvViewModel.FileExtensionDialogFilter;
            dlg.DefaultExt = "*.log4j";
            dlg.Multiselect = true;
            dlg.Title = addFile ? YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Add_Log_File :
                                  YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Open_Log_File;

            if (dlg.ShowDialog().GetValueOrDefault() == true)
            {
                this.mYalvLogViewModel.LoadFiles(dlg.FileNames);
         

                this.RecentFileList.InsertFile(dlg.FileName);

                this.UpdateJumpList();
            }

            return null;
        }

        protected virtual bool CommandOpenFileCanExecute(object parameter)
        {
            return true;
        }

        protected virtual object CommandAboutExecute(object parameter)
        {
            var win = new View.About() { Owner = this.mCallingWin as Window };
            win.ShowDialog();

            return null;
        }

        protected virtual object CommandExportExecute(object parameter)
        {
            LogAnalysisWorkspace workspace = YalvRegistry.Instance.ActualWorkspace;
            if (workspace != null)
            {
                new LogAnalysisSessionExporter("LogAnalysisSession.yalv").Export(workspace);
            }
            return null;
        }

        protected virtual object CommandOpenSqliteDatabaseExecute(object parameter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            bool addFile = parameter != null && parameter.Equals("ADD");

            dlg.Filter = YalvViewModel.DatabaseExtensionDialogFilter;
            dlg.DefaultExt = "*.db3";
            dlg.Multiselect = false;
            dlg.Title = addFile ? YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Add_Log_File :
                                  YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Open_Log_File;

            if (dlg.ShowDialog().GetValueOrDefault() == true)
            {
                this.mYalvLogViewModel.LoadSqliteDatabase(dlg.FileName);
                this.RecentFileList.InsertFile(dlg.FileName);
                this.UpdateJumpList();
            }
            return null;
        }


        protected virtual object CommandOpenLogAnalysisSessionExecute(object parameter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            bool addFile = parameter != null && parameter.Equals("ADD");

            dlg.Filter = YalvViewModel.LogAnalysisExtensionDialogFilter;
            dlg.DefaultExt = "*.yalv";
            dlg.Multiselect = false;
            dlg.Title = addFile ? YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Add_Log_File :
                                  YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Open_Log_File;

            if (dlg.ShowDialog().GetValueOrDefault() == true)
            {
                this.mYalvLogViewModel.LoadLogAnalysisSession(dlg.FileName);
                this.RecentFileList.InsertFile(dlg.FileName);
                this.UpdateJumpList();
            }
            return null;
        }
        #endregion Commands

        protected override void OnDispose()
        {
            if (this.mYalvLogViewModel != null)
                this.mYalvLogViewModel.Dispose();

            base.OnDispose();
        }

        private void UpdateJumpList()
        {
            JumpList myJumpList = JumpList.GetJumpList(Application.Current);

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
                        JumpTask myJumpTask = new JumpTask();
                        myJumpTask.CustomCategory = YalvLib.Strings.Resources.MainWindowVM_updateJumpList_CustomCategoryName;
                        myJumpTask.Title = Path.GetFileName(item);
                        ////myJumpTask.Description = "";
                        myJumpTask.ApplicationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.AppDomain.CurrentDomain.FriendlyName);
                        myJumpTask.Arguments = item;
                        myJumpList.JumpItems.Add(myJumpTask);
                    }
                    catch (Exception)
                    {
                        ////throw;
                    }
                }
            }

            myJumpList.Apply();
        }
        #endregion
    }
}

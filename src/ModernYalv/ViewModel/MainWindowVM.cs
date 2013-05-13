namespace ModernYalv.ViewModel
{
  using System;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Shell;

  using MRU.ViewModel;
  using ModernYalv.Interfaces;
  using ModernYalv.Settings;
  using YalvLib.Common;
  using YalvLib.Common.Interfaces;
  using YalvLib.ViewModel;

  internal class MainWindowVM : BindableObject
  {
    #region fields
    public const string PropTitle = "Title";

    private readonly YalvViewModel mYalvLogViewModel = null;

    private readonly string mLayoutFileName;

    /// <summary>
    /// Point to the last file that has been loaded into the software
    /// </summary>
    private string mLastFileLoad;

    /// <summary>
    /// Remember the default file filter index to display in File Open dialog
    /// </summary>
    private int mLastFileExtensionFilterIndex = 0;

    /// <summary>
    /// Window to which the viewmodel is attached
    /// </summary>
    private IWinSimple mCallingWin;

    private MRUListVM mRecentFiles = null;
    #endregion fields

    #region constructor
    public MainWindowVM(IWinSimple win)
    {
      this.mLayoutFileName = System.IO.Path.Combine(MainWindowVM.AppDataDirectoryPath,
                                                    Assembly.GetEntryAssembly().GetName().Name + ".session");

      this.mCallingWin = win;

      this.mYalvLogViewModel = new YalvViewModel();
      this.mRecentFiles = null;

      this.CommandExit = new CommandRelay(this.commandExitExecute, p => true);
      this.CommandOpenFile = new CommandRelay(this.commandOpenFileExecute, this.commandOpenFileCanExecute);
      this.CommandLoadFile = new CommandRelay(this.commandLoadFileExecute, this.commandOpenFileCanExecute);

      this.mLastFileLoad = string.Empty;
    }
    #endregion constructor

    #region Public Properties
    public static string AppDataDirectoryPath
    {
      get
      {
        return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                                         + System.IO.Path.DirectorySeparatorChar + "Yalv";
      }
    }

    public static string UserDataDirectoryPath
    {
      get
      {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      }
    }

    public string Title
    {
        get
        {
            string sFile = (this.YalvLogViewModel.FilePaths.Length == 0 ? string.Empty :
                                                                                    " - " + this.mYalvLogViewModel.FilePaths[0]);

            return string.Format("{0}{1}", YalvLib.Strings.Resources.MainWindow_Title, sFile);
        }
    }

    #region Commands
    /// <summary>
    /// Exit Command
    /// </summary>
    public ICommandAncestor CommandExit { get; protected set; }

    /// <summary>
    /// Open log file command
    /// </summary>
    public ICommandAncestor CommandOpenFile { get; protected set; }

    /// <summary>
    /// Load log file command
    /// </summary>
    public ICommandAncestor CommandLoadFile { get; protected set; }
    #endregion

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
    /// This property manages the data visible in the Recent Files View
    /// based on the <seealso cref="RecentFilesViewModel"/>.
    /// </summary>
    public MRUListVM RecentFiles
    {
      get
      {
        if (this.mRecentFiles == null)
        {
          this.mRecentFiles = new MRUListVM();
          this.RecentFiles.BindLoadFileCommand(this.commandLoadFileExecute, this.commandOpenFileCanExecute);
        }

        return this.mRecentFiles;
      }

      private set
      {
        if (this.mRecentFiles != value)
        {
          if (value == null)
          {
            this.mRecentFiles = new MRUListVM();
            this.RecentFiles.BindLoadFileCommand(this.commandLoadFileExecute, this.commandOpenFileCanExecute);

            this.RaisePropertyChanged("RecentFiles");
          }
          else
          {
            this.mRecentFiles = value;
            this.RecentFiles.BindLoadFileCommand(this.commandLoadFileExecute, this.commandOpenFileCanExecute);

            this.RaisePropertyChanged("RecentFiles");
          }
        }
      }
    }
    #endregion

    #region methods
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

    public void LoadFileList(string filePath)
    {
      try
      {
          this.mYalvLogViewModel.LoadFiles(new string[] { filePath });
      }
      finally
      {
        this.RaisePropertyChanged(PropTitle);
      }
    }
    #endregion

    #region internal Methods
    /// <summary>
    /// Load User Session Data such as last active directory etc..
    /// </summary>
    internal void LoadSession()
    {
      if (MainWindowVM.CreateAppDataDir() == true)
      {
        try
        {
          Session s = Session.LoadSession(this.mLayoutFileName);

          if (s != null)
          {
            // Restore MRU List entries
            if (s.MRU != null)
            {
              this.RecentFiles = s.MRU;
            }

            // Restore Data Grid columns
            if (s.DataGridColumns != null)
            {
              if (this.mYalvLogViewModel.LogItems != null)
              {
                this.mYalvLogViewModel.LogItems.SetColumnsLayout(s.DataGridColumns);
              }
            }

            // Restore window psoition
            if (s.MainWindowPosSz == null)
              s.MainWindowPosSz = new ViewPosSize();

            s.MainWindowPosSz.SetPos(this.mCallingWin);

            if (string.IsNullOrEmpty(s.LastFileLoad) == false)
            {
              this.mLastFileLoad = s.LastFileLoad;
            }

            this.mLastFileExtensionFilterIndex = s.DefaultFileExtensionIndex;
          }
        }
        catch (Exception)
        {
        }
      }
    }

    /// <summary>
    /// Save program settings for retrieval in next user session
    /// </summary>
    internal void SaveSession(ViewPosSize mainWindowSz)
    {
      try
      {
        Session set = new Session(this.mYalvLogViewModel.LogItems.DataGridColumns.DataGridColumns,
                                  this.mRecentFiles, mainWindowSz);

        set.LastFileLoad = this.mLastFileLoad;

        set.DefaultFileExtensionIndex = this.mLastFileExtensionFilterIndex;

        set.SaveSession(this.mLayoutFileName);
      }
      catch (Exception exp)
      {
        MessageBox.Show(exp.StackTrace.ToString(), exp.Message, MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
    #endregion internal Methods

    #region Commands
    protected virtual object commandExitExecute(object parameter)
    {
      this.mCallingWin.Close();
      return null;
    }

    protected virtual object commandOpenFileExecute(object parameter)
    {
      Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

      dlg.InitialDirectory = MainWindowVM.GetFileOpenDefaultDirectory(this.mLastFileLoad);

      bool addFile = parameter != null && parameter.Equals("ADD");

      dlg.Filter = YalvViewModel.FileExtensionDialogFilter;
      dlg.FilterIndex = this.mLastFileExtensionFilterIndex;

      dlg.Multiselect = false;
      dlg.Title = addFile ? YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Add_Log_File :
                            YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Open_Log_File;

      if (dlg.ShowDialog().GetValueOrDefault() == true)
      {
        this.mLastFileExtensionFilterIndex = dlg.FilterIndex;
        this.LoadLog4NetFile(dlg.FileName);
      }

      return null;
    }

    protected virtual bool commandOpenFileCanExecute(object parameter)
    {
      return true;
    }

    protected object commandLoadFileExecute(object arg)
    {
      string fname = arg as string;

      if (fname != null)
      {
        this.LoadLog4NetFile(fname);
      }

      return null;
    }
    #endregion Commands

    #region Privates
    protected override void OnDispose()
    {
      if (this.mYalvLogViewModel != null)
        this.mYalvLogViewModel.Dispose();

      base.OnDispose();
    }

    protected static string GetFileOpenDefaultDirectory(string lastFileLoaded)
    {
      if (string.IsNullOrEmpty(lastFileLoaded))
      {
        return MainWindowVM.UserDataDirectoryPath;
      }
      else
      {
        try
        {
          string dir = System.IO.Path.GetDirectoryName(lastFileLoaded);

          if (System.IO.Directory.Exists(dir) == true)
            return dir;
          else
            return MainWindowVM.UserDataDirectoryPath;
        }
        catch
        {
          return MainWindowVM.UserDataDirectoryPath;
        }
      }
    }

    private void updateJumpList()
    {
      JumpList myJumpList = JumpList.GetJumpList(Application.Current);

      if (myJumpList == null)
      {
        myJumpList = new JumpList();
        JumpList.SetJumpList(Application.Current, myJumpList);
      }

      myJumpList.JumpItems.Clear();
      /***
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
       ***/

      myJumpList.Apply();
    }

    private bool LoadLog4NetFile(string sfileName)
    {
      try
      {
          this.mYalvLogViewModel.LoadFiles(new string[] { sfileName });

        this.mLastFileLoad = sfileName;

        ////this.mYalvLogViewModel.LogItems.LoadFileList(dlg.FileName);

        if (this.RecentFiles != null)
          this.RecentFiles.AddNewEntryIntoMRU(sfileName);

        this.updateJumpList();

        return true;
      }
      catch (Exception exp)
      {
        System.Console.WriteLine(exp.ToString());
      }

      return false;
    }
    #endregion
    #endregion methods
  }
}

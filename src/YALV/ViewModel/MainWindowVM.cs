namespace YALV.ViewModel
{
  using System;
  using System.IO;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Shell;

  using YALV.Common;
  using YALV.Interfaces;
  using YALV.View.Components;
  using YalvLib.Common;
  using YalvLib.Common.Interfaces;
  using YalvLib.ViewModel;

  internal class MainWindowVM : BindableObject
  {
    #region fields
    public const string PropTitle = "Title";
    public const string PropRecentFileList = "RecentFileList";

    private readonly YalvViewModel mYalvLogViewModel = null;
    private RecentFileList mRecentFileList;

    /// <summary>
    /// Window to which the viewmodel is attached
    /// </summary>
    private IWinSimple mCallingWin;
    #endregion fields

    #region constructor
    public MainWindowVM(IWinSimple win,
                        RecentFileList recentFileList)
    {
      this.mCallingWin = win;

      this.mYalvLogViewModel = new YalvViewModel();

      this.RecentFileList = recentFileList;

      this.CommandExit = new CommandRelay(this.commandExitExecute, p => true);
      this.CommandOpenFile = new CommandRelay(this.commandOpenFileExecute, this.commandOpenFileCanExecute);

      this.CommandAbout = new CommandRelay(this.commandAboutExecute, p => true);
    }
    #endregion constructor

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
    #endregion

    #region Public Properties
    public string Title
    {
      get
      {
        string sFile = (string.IsNullOrEmpty(this.mYalvLogViewModel.FilePath) ? string.Empty :
                                                                                " - " + this.mYalvLogViewModel.FilePath);

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
          this.mRecentFileList.MenuClick += (s, e) =>
          {
            this.LoadFileList(e.Filepath);
          };

          this.updateJumpList();
        }
      }
    }
    #endregion

    #region Public Methods
    public void LoadFileList(string filePath)
    {
      try
      {
        this.mYalvLogViewModel.LoadFile(filePath);
      }
      finally
      {
        this.RaisePropertyChanged(PropTitle);
      }
    }
    #endregion

    #region Privates
    #region Commands
    protected virtual object commandExitExecute(object parameter)
    {
      this.mCallingWin.Close();
      return null;
    }

    protected virtual object commandOpenFileExecute(object parameter)
    {
      Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

      bool addFile = parameter != null && parameter.Equals("ADD");
      dlg.Filter = string.Format("{0} (*.xml)|*.xml|{1} (*.*)|*.*", YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_XmlFilesCaption,
                                                                    YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_AllFilesCaption);
      dlg.DefaultExt = "xml";
      dlg.Multiselect = false;
      dlg.Title = addFile ? YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Add_Log_File :
                            YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_Open_Log_File;

      if (dlg.ShowDialog().GetValueOrDefault() == true)
      {
        this.mYalvLogViewModel.LoadFile(dlg.FileName);
        ////this.mYalvLogViewModel.LogItems.LoadFileList(dlg.FileName);

        this.RecentFileList.InsertFile(dlg.FileName);

        this.updateJumpList();
      }

      return null;
    }

    protected virtual bool commandOpenFileCanExecute(object parameter)
    {
      return true;
    }

    protected virtual object commandAboutExecute(object parameter)
    {
      var win = new View.About() { Owner = this.mCallingWin as Window };
      win.ShowDialog();

      return null;
    }
    #endregion Commands

    protected override void OnDispose()
    {
      if (this.mYalvLogViewModel != null)
        this.mYalvLogViewModel.Dispose();

      base.OnDispose();
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

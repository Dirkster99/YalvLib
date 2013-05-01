using YalvLib.Domain;
using YalvLib.Providers;

namespace YalvLib.ViewModel
{
  using YalvLib.Common;
  using YalvLib.Common.Interfaces;

  /// <summary>
  /// Main ViewModel of Valv Lib control
  /// </summary>
  public class YalvViewModel : BindableObject
  {
    #region fields
    private const string PROP_FilePath = "FilePath";

    private DisplayLogVM mLogItems = null;
    #endregion fields

    public static string FileExtensionDialogFilter
    {
      get
      {
        return string.Format("*.log4j,*.log,*.txt,*.xml|*.log4j;*.log;*.txt;*.xml" +
                             "|{0} (*.log4j)|*.log4j" +
                             "|{1} (*.log)|*.log" +
                             "|{2} (*.txt)|*.txt" +
                             "|{3} (*.xml)|*.xml" +
                             "|{4} (*.*)|*.*", "Log4j files",
                                               "Log files",
                                               "Text files",
                                               YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_XmlFilesCaption,
                                               YalvLib.Strings.Resources.MainWindowVM_commandOpenFileExecute_AllFilesCaption);
      }
    }

    #region constructor
    /// <summary>
    /// Standard constructor
    /// </summary>
    public YalvViewModel()
    {
      this.mLogItems = new DisplayLogVM();

      this.CommandRefresh = new CommandRelay(this.commandRefreshExecute, this.commandRequiresDataCanExecute);
      this.CommandDelete = new CommandRelay(this.LogItems.commandDeleteExecute, this.LogItems.commandDeleteCanExecute);
      this.FilterYalvView = new CommandRelay(this.commandFilterYalvView, this.commandRequiresDataCanExecute);
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get property to determine the name of
    /// the currently viewed log4net file or
    /// empty string if there is no file being viewed at present.
    /// </summary>
    public string FilePath
    {
      get
      {
        if (this.LogItems != null)
        {
          if (this.LogItems.LogFile != null)
          {
            return this.LogItems.LogFile.FilePath;
          }
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Get a list of filter columns and data items (for display in a DataGridView)
    /// </summary>
    public DisplayLogVM LogItems
    {
      get
      {
        return this.mLogItems;
      }
    }

    /// <summary>
    /// Get whether there are data items in the collection or not
    /// (there may be no items to display if filter is applied but thats a different issue)
    /// </summary>
    public bool HasData
    {
      get
      {
        return (this.LogItems == null ? false : this.LogItems.HasData);
      }
    }

    #region Command
    /// <summary>
    /// FilterYalvView command to switches a filtered view on or off
    /// </summary>
    public ICommandAncestor FilterYalvView { get; protected set; }

    /// <summary>
    /// Refresh Command
    /// </summary>
    public ICommandAncestor CommandRefresh { get; protected set; }

    /// <summary>
    /// Delete Command
    /// </summary>
    public ICommandAncestor CommandDelete { get; protected set; }
    #endregion Command
    #endregion properties

    #region methods
    /// <summary>
    /// Load a log4nez log file to display its content through this ViewModel.
    /// </summary>
    /// <param name="path"></param>
    public void LoadFile(string path)
    {
        this.mLogItems.LoadFile(path, EntriesProviderType.Xml, this.loadFinishedEvent);
    }
    public void LoadSqliteDatabase(string path)
    {
        this.mLogItems.LoadFile(path, EntriesProviderType.Sqlite, this.loadFinishedEvent);
    }
    public void LoadLogAnalysisSession(string path)
    {
        this.mLogItems.LoadFile(path, EntriesProviderType.Yalv, this.loadFinishedEvent);
    }
    /// <summary>
    /// Implementation of the Refresh command (reload data and apply filters)
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    internal virtual object commandRefreshExecute(object parameter)
    {
      this.LogItems.commandRefreshExecute(this.loadFinishedEvent);

      return null;
    }

    internal virtual bool commandRequiresDataCanExecute(object parameter)
    {
      return this.HasData;
    }

    /// <summary>
    /// Default command method for applying column filters.
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    protected virtual object commandFilterYalvView(object parameter)
    {
      if (this.mLogItems != null)
      {
        this.mLogItems.ApplyFilter();
      }

      return null;
    }

    /// <summary>
    /// This is a callback method that is always called when
    /// the internal load process is finished
    /// (even when it failed to finish after initialization).
    /// </summary>
    /// <param name="loadWasSuccessful"></param>
    private void loadFinishedEvent(bool loadWasSuccessful)
    {
      this.refreshCommandsCanExecute();
    }

    private void refreshCommandsCanExecute()
    {
      this.CommandRefresh.OnCanExecuteChanged();
      this.CommandDelete.OnCanExecuteChanged();
      this.FilterYalvView.OnCanExecuteChanged();

      this.RaisePropertyChanged("HasData");
    }
    #endregion methods
  }
}

namespace YalvLib.ViewModel
{
  using System;
  using System.IO;
  using System.Windows;
  using YalvLib.Common;

  public class LogFileVM : BindableObject
  {
    #region fields
    public const string PROP_IsLoading = "IsLoading";
    public const string PROP_FilePath = "FilePath";
    public const string PROP_IsFileLoaded = "IsFileLoaded";

    private bool mIsLoading;
    #endregion fields

    private string mFilePath;
    private bool mIsFileLoaded;

    #region Constructors
    /// <Summary>
    /// Standard constructor of the <seealso cref="LogFileVM"/> class
    /// </Summary>
    public LogFileVM()
    {
      this.mFilePath = string.Empty;

      this.mIsFileLoaded = false;
      this.mIsLoading = false;
    }
    #endregion Constructors

    #region Properties
    /// <summary>
    /// Get/set property to indicate whether a logfile is currently being parsed and loaded or not.
    /// </summary>
    public bool IsLoading
    {
      get
      {
        return this.mIsLoading;
      }

      internal set
      {
        if (this.mIsLoading != value)
        {
          this.mIsLoading = value;
          this.RaisePropertyChanged(PROP_IsLoading);
        }
      }
    }

    public bool IsFileLoaded
    {
      get
      {
        return this.mIsFileLoaded;
      }

      internal set
      {
        if (this.mIsFileLoaded != value)
        {
          this.mIsFileLoaded = value;
          this.RaisePropertyChanged(PROP_IsFileLoaded);
        }
      }
    }

    public string FilePath
    {
      get
      {
        return (this.mFilePath == null ? string.Empty : this.mFilePath);
      }

      internal set
      {
        if (this.mFilePath != value)
        {
          this.mFilePath = value;
          this.RaisePropertyChanged(PROP_FilePath);
        }
      }
    }
    #endregion Properties

    #region Methods
    #region commandDelete
    internal virtual void commandDeleteExecute()
    {
      if (string.IsNullOrEmpty(this.FilePath) == false && this.IsFileLoaded == true)
      {
        if (MessageBox.Show(YalvLib.Strings.Resources.MainWindowVM_commandDeleteExecute_DeleteCheckedFiles_ConfirmText,
                            YalvLib.Strings.Resources.MainWindowVM_commandDeleteExecute_DeleteCheckedFiles_ConfirmTitle, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
          return;

        // Delete all selected file
        if (this.deleteFile(this.FilePath) == true)
        {
          this.FilePath = string.Empty;
          this.IsFileLoaded = false;
        }
      }

      return;
    }

    internal virtual bool commandDeleteCanExecute()
    {
      return (string.IsNullOrEmpty(this.FilePath) == false && this.IsFileLoaded == true);
    }

    /// <summary>
    /// Physically delete a file in the file system.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private bool deleteFile(string path)
    {
      try
      {
        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo != null)
          fileInfo.Delete();

        return true;
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format(YalvLib.Strings.Resources.MainWindowVM_deleteFile_ErrorMessage_Text, path, ex.Message),
                                      YalvLib.Strings.Resources.MainWindowVM_deleteFile_ErrorMessage_Title, MessageBoxButton.OK, MessageBoxImage.Error);
        return false;
      }
    }
    #endregion commandDelete

    #endregion Methods
  }
}

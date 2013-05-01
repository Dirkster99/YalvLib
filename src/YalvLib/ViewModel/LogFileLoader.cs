using YalvLib.Infrastructure.Sqlite;
using YalvLib.Model;

namespace YalvLib.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Threading;
  using YalvLib.Domain;
  using YalvLib.Providers;

  /// <summary>
  /// This class implements a task based log4net loader
  /// that can run in an async task and generate an event
  /// when loading is done.
  /// </summary>
  internal class LogFileLoader
  {
    #region fields
    public const string KeyLogItems = "LogItems";

    private LogFileVM mLogFile;

    private bool mAbortedWithErrors;
    private bool mAbortedWithCancel;

    private Exception mInnerException;
    private Dictionary<string, object> mObjColl = null;

    /// <summary>
    /// This property is used to tell the context of the thread that started this thread
    /// originally. We use this to generate callbacks in the correct context when the long
    /// running task is done
    /// 
    /// Quote:
    /// One of the most important parts of this pattern is calling the MethodNameCompleted
    /// method on the same thread that called the MethodNameAsync method to begin with. You
    /// could do this using WPF fairly easily, by storing CurrentDispatcher—but then the
    /// nongraphical component could only be used in WPF applications, not in Windows Forms
    /// or ASP.NET programs. 
    /// 
    /// The DispatcherSynchronizationContext class addresses this need—think of it as a
    /// simplified version of Dispatcher that works with other UI frameworks as well.
    /// 
    /// http://msdn.microsoft.com/en-us/library/ms741870.aspx
    /// </summary>
    private DispatcherSynchronizationContext mRequestingContext = null;
    #endregion fields

    #region Constructor
    public LogFileLoader()
    {
      this.mLogFile = new LogFileVM();

      this.mAbortedWithErrors = this.mAbortedWithCancel = false;
      this.mInnerException = null;
      this.mObjColl = new Dictionary<string, object>();
    }
    #endregion Constructor

    public event EventHandler<ResultEvent> loadResultEvent;

    #region Properties
    public Dictionary<string, object> ResultObjects
    {
      get
      {
        return (this.mObjColl == null ? new Dictionary<string, object>() :
                                        new Dictionary<string, object>(this.mObjColl));
      }
    }

    protected Exception InnerException
    {
      get { return this.mInnerException; }
      set { this.mInnerException = value; }
    }

      public EntriesProviderType ProviderType { get; set; }

      #endregion Properties

    #region Methods
    /// <summary>
    /// Parse a log4net log file via abstract parser provider class for SQL, Sqlite, XML file etc...
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    internal LogAnalysisSession CreateLogAnalysisSession(string path)
    {
        LogAnalysisSession session = new LogAnalysisSession();
        try
        {
            if (ProviderType.Equals(EntriesProviderType.Yalv))
            {
                LogAnalysisSessionLoader loader = new LogAnalysisSessionLoader(path);
                session = loader.Load();
            } else
            {
                LogEntryRepository repository = CreateLogFileEntryRepository(path);
                session.AddSourceRepository(repository);                
            }
        }
        catch (Exception exception)
        {
            string message = string.Format(YalvLib.Strings.Resources.GlobalHelper_ParseLogFile_Error_Text, path, exception.Message);
            MessageBox.Show(message, YalvLib.Strings.Resources.GlobalHelper_ParseLogFile_Error_Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        return session;
    }

    private LogEntryRepository CreateLogFileEntryRepository(string path)
    {
        switch (ProviderType)
        {
            case EntriesProviderType.Sqlite:
                return new LogEntrySqliteRepository(path);
            case EntriesProviderType.Xml:
                return new LogEntryFileRepository(path);
        }
        throw new NotImplementedException();
    }

      /// <summary>
    /// load the contents of a log file in an async task and return
    /// the result through an event object (event is setup prior to this call).
    /// </summary>
    /// <param name="path"></param>
    /// <param name="async"></param>
    internal void LoadFile(string path, bool async)
    {
      this.SaveThreadContext(async);

      CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
      CancellationToken cancelToken = cancelTokenSource.Token;

      Task taskToProcess = null;
      taskToProcess = Task.Factory.StartNew<ObservableCollection<string>>((stateObj) =>
      {
        this.mLogFile = new LogFileVM();
        this.mAbortedWithErrors = this.mAbortedWithCancel = false;
        this.mObjColl = new Dictionary<string, object>();

        // This is not run on the UI thread.
        ObservableCollection<string> Results = new ObservableCollection<string>();

        try
        {
          cancelToken.ThrowIfCancellationRequested();

          if (System.IO.File.Exists(path) == false)
          {
            MessageBox.Show(string.Format("Cannot access file '{0}'.", path));
            return Results;
          }

          this.mLogFile.FilePath = path;
            
          LogAnalysisSession session = CreateLogAnalysisSession(path);
            YalvRegistry.Instance.SetActualLogAnalysisSession(session);
            this.mObjColl.Add(LogFileLoader.KeyLogItems, session.LogEntries);
        }
        catch (OperationCanceledException exp)
        {
          this.mAbortedWithCancel = true;

          string sStatus = exp.Message;   // output: "User canceled..." ...
          Results.Add(sStatus);

          this.mLogFile = new LogFileVM();
        }
        catch (Exception Exp)
        {
          this.mInnerException = Exp;
          this.mAbortedWithErrors = true;

          this.mLogFile = new LogFileVM();

          Results.Add(Exp.ToString());
        }
        finally
        {
          this.mLogFile.IsLoading = false;  // Make sure that state engine has correct state
        }

        return Results;                     // End of async task with summary list of result strings
      },
      cancelToken).ContinueWith(ant =>
      {
        try
        {
          this.ReportBatchResultEvent(async);
        }
        catch (AggregateException aggExp)
        {
          this.mAbortedWithErrors = true;
          string errorMessage = PrintException(taskToProcess, aggExp, "LogFileLoader");
        }
        ////finally
        ////{
        ////}
      });

      if (async == false)          // Execute 'synchronously' via wait/block method
        taskToProcess.Wait();
    }

    /// <summary>
    /// Analyze AggregateException (typically returned from Task class) and return human-readable
    /// string for display in GUI.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="agg"></param>
    /// <param name="taskName"></param>
    /// <returns></returns>
    protected static string PrintException(Task task, AggregateException agg, string taskName)
    {
      string sResult = string.Empty;

      foreach (Exception ex in agg.InnerExceptions)
      {
        sResult += string.Format("{0} Caught exception '{1}'", taskName, ex.Message) + Environment.NewLine;
      }

      sResult += string.Format("{0} cancelled? {1}", taskName, task.IsCanceled) + Environment.NewLine;

      return sResult;
    }

    /// <summary>
    /// Report a result to the attached eventhalders (if any) on whether execution succeded or not.
    /// </summary>
    protected void ReportBatchResultEvent(bool bAsnc)
    {
      // non-Asnyc threads are simply blocked until they finish
      // hence completed event is not required to fire
      if (bAsnc == false)
        return;

      SendOrPostCallback callback = new SendOrPostCallback(this.ReportTaskCompletedEvent);
      this.mRequestingContext.Post(callback, null);
      this.mRequestingContext = null;
    }

    /// <summary>
    /// Save the threading context of a calling thread to enable event completion handling
    /// in original context when async task has finished (WPF, Winforms and co require this)
    /// </summary>
    /// <param name="bAsnc"></param>
    protected void SaveThreadContext(bool bAsnc)
    {
      // non-Asnyc threads are simply blocked until they finish
      // hence completed event is not required to fire
      if (bAsnc == false) return;

      if (this.mRequestingContext != null)
        throw new InvalidOperationException("This component can handle only 1 processing request at a time");

      this.mRequestingContext = (DispatcherSynchronizationContext)DispatcherSynchronizationContext.Current;
    }

    /// <summary>
    /// Report the asynchronous task as having completed
    /// </summary>
    /// <param name="e"></param>
    private void ReportTaskCompletedEvent(object e)
    {
      if (this.loadResultEvent != null)
      {
        if (this.mAbortedWithErrors == false && this.mAbortedWithCancel == false)
          this.loadResultEvent(this, new ResultEvent("Processing succeeded", false, false, this.mObjColl));
        else
          this.loadResultEvent(this, new ResultEvent("Processing was not succesfull", this.mAbortedWithErrors,
                                                      this.mAbortedWithCancel, this.mObjColl, this.mInnerException));
      }
    }

    #region Nested Classes
    /// <summary>
    /// Stores information about the result of a batch run.
    /// If an error occurs, Error is set to true and an exception may be stored in InnerException.
    /// </summary>
    public class ResultEvent : EventArgs
    {
      #region Fields
      private bool mError;
      private bool mCancel;
      private Exception mInnerException;
      private string mMessage;
      private Dictionary<string, object> mObjColl = null;
      #endregion Fields

      #region Constructors
      /// <summary>
      /// Convinience constructor to produce simple message for communicating when
      /// batch run was abborted incompletely (bCancel can be set to true or bError
      /// can be set to true).
      /// </summary>
      /// <param name="sMessage"></param>
      /// <param name="bError"></param>
      /// <param name="bCancel"></param>
      /// <param name="objColl"></param>
      /// <param name="innerException"></param>
      public ResultEvent(string sMessage,
                          bool bError,
                          bool bCancel,
                          Dictionary<string, object> objColl = null,
                          Exception innerException = null)
      {
        this.mMessage = sMessage;
        this.mError = bError;
        this.mCancel = bCancel;
        this.mInnerException = (innerException == null ? null : innerException);
        this.mObjColl = (objColl == null ? null : new Dictionary<string, object>(objColl));
      }

      /// <summary>
      /// Convinience constructor to produce simple message at the end of a batch run
      /// (Cancel not clicked and no error to be reported).
      /// </summary>
      /// <param name="message"></param>
      public ResultEvent(string message)
      {
        this.mMessage = message;
        this.mError = false;
        this.mCancel = false;
        this.mInnerException = null;
      }
      #endregion Constructors

      #region Properties
      /// <summary>
      /// Get an error message if processing task aborted with errors
      /// </summary>
      public bool Error
      {
        get { return this.mError; }
      }

      /// <summary>
      /// Get property to determine whether processing was cancelled or not.
      /// </summary>
      public bool Cancel
      {
        get { return this.mCancel; }
      }

      /// <summary>
      /// Get property to determine whether there is an innerException to
      /// document an abortion with errors.
      /// </summary>
      public Exception InnerException
      {
        get { return this.mInnerException; }
      }

      /// <summary>
      /// Get current message describing the current step being proceesed
      /// in the <see cref="ExecViewModel"/>
      /// </summary>
      public string Message
      {
        get { return this.mMessage; }
      }

      /// <summary>
      /// Dictionary of result objects from executing process in <see cref="ExecViewModel"/>
      /// </summary>
      public Dictionary<string, object> ResultObjects
      {
        get
        {
          return (this.mObjColl == null ? new Dictionary<string, object>() :
                                          new Dictionary<string, object>(this.mObjColl));
        }
      }
      #endregion Properties
    }
    #endregion Nested Classes
    #endregion Methods
  }
}

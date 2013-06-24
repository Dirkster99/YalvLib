using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using YalvLib.Model;
using YalvLib.Providers;

namespace YalvLib.ViewModel.Process
{
    /// <summary>
    /// This class implements a task based log4net loader
    /// that can run in an async task and generate an event
    /// when loading is done. The class can be used as a template
    /// for processing other tasks asynchronously.
    /// </summary>
    internal class LogFileLoader
    {
        #region fields

        public const string KeyLogItems = "LogItems";

        private bool _abortedWithCancel;
        private bool _abortedWithErrors;
        private CancellationTokenSource _cancelTokenSource;

        private ApplicationException _innerException;
        private ManageRepositoryViewModel _logFile;
        private Dictionary<string, object> _objColl;

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
        private DispatcherSynchronizationContext _requestingContext;

        #endregion fields

        #region Constructor

        public LogFileLoader()
        {
            _logFile = new ManageRepositoryViewModel();

            _abortedWithErrors = _abortedWithCancel = false;
            _innerException = null;
            _objColl = new Dictionary<string, object>();
        }

        #endregion Constructor

        public event EventHandler<ResultEvent> LoadResultEvent;

        #region Properties

        public Dictionary<string, object> ResultObjects
        {
            get
            {
                return (_objColl == null
                            ? new Dictionary<string, object>()
                            : new Dictionary<string, object>(_objColl));
            }
        }

        protected ApplicationException InnerException
        {
            get { return _innerException; }
            set { _innerException = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Cancel Asynchronous processing (if there is any right now)
        /// </summary>
        public void Cancel()
        {
            if (_cancelTokenSource != null)
                _cancelTokenSource.Cancel();
        }


        /// <summary>
        /// Process a file load operation
        /// </summary>
        /// <param name="execFunc"></param>
        /// <param name="paths"></param>
        /// <param name="providerType"></param>
        /// <param name="vm"></param>
        /// <param name="async"></param>
        internal void ExecuteAsynchronously(
            Action execFunc, bool async)
        {
            SaveThreadContext(async);

            _cancelTokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = _cancelTokenSource.Token;

            Task taskToProcess = Task.Factory.StartNew<ObservableCollection<string>>(stateObj =>
                                                           {
                                                               _abortedWithErrors = _abortedWithCancel = false;
                                                               _objColl = new Dictionary<string, object>();
                                                               ObservableCollection<string> _processResults = new ObservableCollection<string>();

                                                               // This is not run on the UI thread.


                                                               try
                                                               {
                                                                   cancelToken.ThrowIfCancellationRequested();
                                                                   execFunc();
                                                                   // processing task
                                                               }
                                                               catch (OperationCanceledException exp)
                                                               {
                                                                   _abortedWithCancel = true;
                                                                   // output: "User canceled..." ...
                                                                   _processResults.Add(exp.Message);
                                                               }
                                                               catch (Exception exp)
                                                               {
                                                                   _innerException = new ApplicationException("Error occured",exp);
                                                                   _abortedWithErrors = true;

                                                                   _processResults.Add(exp.ToString());
                                                               }

                                                               return _processResults;
                                                               // End of async task with summary list of result strings
                                                           },
                                                       cancelToken).ContinueWith(ant =>
                                                                                     {
                                                                                         try
                                                                                         {
                                                                                             ReportBatchResultEvent(async);
                                                                                         }
                                                                                         catch (AggregateException aggExp)
                                                                                         {
                                                                                             _abortedWithErrors = true;
                                                                                         }
                                                                                         ////finally
                                                                                         ////{
                                                                                         ////}
                                                                                     });

            if (async == false) // Execute 'synchronously' via wait/block method
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

            SendOrPostCallback callback = ReportTaskCompletedEvent;
            _requestingContext.Post(callback, null);
            _requestingContext = null;
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

            if (_requestingContext != null)
                throw new InvalidOperationException("This component can handle only 1 processing request at a time");

            _requestingContext = (DispatcherSynchronizationContext) SynchronizationContext.Current;
        }

        /// <summary>
        /// Report the asynchronous task as having completed
        /// </summary>
        /// <param name="e"></param>
        private void ReportTaskCompletedEvent(object e)
        {
            if (LoadResultEvent != null)
            {
                if (_abortedWithErrors == false && _abortedWithCancel == false)
                    LoadResultEvent(this, new ResultEvent("Execution succeeded", false, false, _objColl));
                else
                    LoadResultEvent(this, new ResultEvent("Execution was not successfull", _abortedWithErrors,
                                                          _abortedWithCancel, _objColl, _innerException));
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

            private readonly bool _cancel;
            private readonly bool _error;
            private readonly ApplicationException _innerException;
            private readonly string _message;
            private readonly Dictionary<string, object> _objColl;

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
                               ApplicationException innerException = null)
            {
                _message = sMessage;
                _error = bError;
                _cancel = bCancel;
                _innerException = (innerException == null ? null : innerException);
                _objColl = (objColl == null ? null : new Dictionary<string, object>(objColl));
            }

            /// <summary>
            /// Convinience constructor to produce simple message at the end of a batch run
            /// (Cancel not clicked and no error to be reported).
            /// </summary>
            /// <param name="message"></param>
            public ResultEvent(string message)
            {
                _message = message;
                _error = false;
                _cancel = false;
                _innerException = null;
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Get an error message if processing task aborted with errors
            /// </summary>
            public bool Error
            {
                get { return _error; }
            }

            /// <summary>
            /// Get property to determine whether processing was cancelled or not.
            /// </summary>
            public bool Cancel
            {
                get { return _cancel; }
            }

            /// <summary>
            /// Get property to determine whether there is an innerException to
            /// document an abortion with errors.
            /// </summary>
            public ApplicationException InnerException
            {
                get { return _innerException; }
            }

            /// <summary>
            /// Get current message describing the current step being proceesed
            /// 
            /// </summary>
            public string Message
            {
                get { return _message; }
            }

            /// <summary>
            /// Dictionary of result objects from executing process
            /// </summary>
            public Dictionary<string, object> ResultObjects
            {
                get
                {
                    return (_objColl == null
                                ? new Dictionary<string, object>()
                                : new Dictionary<string, object>(_objColl));
                }
            }

            #endregion Properties
        }

        #endregion Nested Classes

        #endregion Methods
    }
}
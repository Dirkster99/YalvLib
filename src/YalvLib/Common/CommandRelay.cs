using System;
using System.Diagnostics;
using YalvLib.Common.Interfaces;

namespace YalvLib.Common
{
    /// <summary>
    /// Class to support command binding between view and viewmodels
    /// </summary>
    public class CommandRelay : ICommandAncestor
    {
        #region fields

        /// <summary>
        /// Method to call when the command is invoked.
        /// </summary>
        protected readonly Func<object, object> ExecuteCommand;

        /// <summary>
        /// Indicates whether command can execute or not.
        /// </summary>
        protected readonly Predicate<object> CanCommandExecute;

        /// <summary>
        /// This boolean allow us to know if the state canExecute 
        /// has changed so we can rise the OnCanChangeExecute event
        /// </summary>
        private bool _lastCanExecute;

        #endregion fields

        #region constructor

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public CommandRelay(Func<object, object> execute, Predicate<object> canExecute)
        {
            ExecuteCommand = execute;
            CanCommandExecute = canExecute;
        }

        #endregion constructor

        /// <summary>
        /// Event is raised to tell subscribers that the bound command has been executed.
        /// </summary>
        public event EventHandler Executed;

        /// <summary>
        /// This event is fired when the state of whether a command can execute or not changes.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Indicate whether command can execute or not.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            bool canExec = (CanCommandExecute == null || CanCommandExecute(parameter));
            if (canExec != _lastCanExecute)
            {
                _lastCanExecute = canExec;
                OnCanExecuteChanged();
            }
            return canExec;
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            ExecuteCommand(parameter);
            OnExecuted();
        }

        /// <summary>
        /// Re-compute whether a command can be executed or not.
        /// </summary>
        public void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnExecuted()
        {
            EventHandler handler = Executed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}

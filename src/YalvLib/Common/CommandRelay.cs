namespace YalvLib.Common
{
  using System;
  using System.Diagnostics;
  using YalvLib.Common.Interfaces;

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
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="execute"></param>
    /// <param name="canExecute"></param>
    public CommandRelay(Func<object, object> execute, Predicate<object> canExecute)
    {
      this.ExecuteCommand = execute;
      this.CanCommandExecute = canExecute;
    }
    #endregion constructor

    /// <summary>
    /// This event is fired when the state of whether a command can execute or not changes.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Indicate whether command can execute or not.
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return this.CanCommandExecute == null ? true : this.CanCommandExecute(parameter);
    }

    /// <summary>
    /// Execute the command.
    /// </summary>
    /// <param name="parameter"></param>
    [DebuggerStepThrough]
    public void Execute(object parameter)
    {
      this.ExecuteCommand(parameter);
    }

    /// <summary>
    /// Re-compute whether a command can be executed or not.
    /// </summary>
    public void OnCanExecuteChanged()
    {
      if (null != this.CanExecuteChanged)
        this.CanExecuteChanged(this, EventArgs.Empty);
    }
  }
}

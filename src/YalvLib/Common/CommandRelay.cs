namespace YalvLib.Common
{
  using System;
  using System.Diagnostics;
  using YalvLib.Common.Interfaces;

  public class CommandRelay : ICommandAncestor
  {
    protected readonly Func<object, object> ExecuteCommand;
    protected readonly Predicate<object> CanCommandExecute;

    public CommandRelay(Func<object, object> execute, Predicate<object> canExecute)
    {
      this.ExecuteCommand = execute;
      this.CanCommandExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged;

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
      return this.CanCommandExecute == null ? true : this.CanCommandExecute(parameter);
    }

    [DebuggerStepThrough]
    public void Execute(object parameter)
    {
      this.ExecuteCommand(parameter);
    }

    public void OnCanExecuteChanged()
    {
      if (null != this.CanExecuteChanged)
        this.CanExecuteChanged(this, EventArgs.Empty);
    }
  }
}

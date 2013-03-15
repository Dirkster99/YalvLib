namespace YalvLib.Common.Interfaces
{
  using System.Windows.Input;

  public interface ICommandAncestor : ICommand
  {
    void OnCanExecuteChanged();
  }
}

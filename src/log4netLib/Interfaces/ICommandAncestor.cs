namespace log4netLib.Interfaces
{
    using System.Windows.Input;

    /// <summary>
    /// Implements the ICommand interface with additional methods.
    /// </summary>
    public interface ICommandAncestor : ICommand
    {
        /// <summary>
        /// Re-compute whether a command can be executed or not.
        /// </summary>
        void OnCanExecuteChanged();
    }
}

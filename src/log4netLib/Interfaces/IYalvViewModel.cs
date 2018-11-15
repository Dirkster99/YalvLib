namespace log4netLib.Interfaces
{
    /// <summary>
    /// Defines main viewModel properties of log4netLib control
    /// </summary>
    public interface IYalvViewModel
    {
        /// <summary>
        /// UpdateTextMarkers Command
        /// </summary>
        ICommandAncestor CommandUpdateTextMarkers { get; }

        /// <summary>
        /// Update calculatedDelta command
        /// </summary>
        ICommandAncestor CommandUpdateDelta { get; }
    }
}

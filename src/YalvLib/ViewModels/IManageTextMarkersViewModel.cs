namespace YalvLib.ViewModels
{
    using System;

    /// <summary>
    /// This interface allows the YalvViewModel to be able to know when a Marker
    /// has been deleted or added in order to refresh the view
    /// </summary>
    public interface IManageTextMarkersViewModel
    {
        /// <summary>
        /// EventHandler used when a marker is deleted
        /// </summary>
        event EventHandler MarkerDeleted;

        /// <summary>
        /// EventHandler used when a marker is added
        /// </summary>
        event EventHandler MarkerAdded;
    }
}
using System;

namespace YalvLib.ViewModel
{
    /// <summary>
    /// This interface allow the YalvViewModel to be able to know when a Marker
    /// Has been deleted or added in order to refresh the view
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
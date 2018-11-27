namespace YalvLib.ViewModels.Markers
{
    using System;
    using YalvLib.Model;

    /// <summary>
    /// Implements the <see cref="EventHandler"/> for the <see cref="MarkerEditEventArgs"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    internal delegate void MarkerEditEventHandler(object sender, MarkerEditEventArgs e);

    /// <summary>
    /// Implements a custom event that is fired whenever an item is changing its
    /// editing mode. This event is used to inform the listner about the change in state.
    /// </summary>
    internal class MarkerEditEventArgs : EventArgs
    {
        /// <summary>
        /// Enumerates possible state of editing for the <see cref="MarkerEditEventArgs"/>
        /// </summary>
        public enum EditEvent
        {
            /// <summary>
            /// The sender of this event has started to editing its content.
            /// </summary>
            BeginEdit = 0,

            /// <summary>
            /// The sender of this event has cancelled editing its content.
            /// </summary>
            CancelEdit = 1,

            /// <summary>
            /// The sender of this event has edit and successfully changed
            /// its content.
            /// </summary>
            CommitEdit = 2
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tm">Textmarker associated with the event</param>
        /// <param name="eventType"></param>
        public MarkerEditEventArgs(TextMarker tm, EditEvent eventType)
        {
            TextMarker = tm;
            EventType = eventType;
        }

        /// <summary>
        /// Gets the textmarker model associated with this event
        /// </summary>
        public TextMarker TextMarker { get; }

        /// <summary>
        /// Gets the type of editing state associated with this event.
        /// </summary>
        public EditEvent EventType { get; }
    }
}
using System.Windows;

namespace YalvLib.Common
{
    using System;

    /// <summary>
    /// "Implementing a Dispose method"
    /// http://msdn.microsoft.com/en-us/library/fs2xkftw.aspx
    /// </summary>
    public class DisposableObject : DependencyObject, IDisposable
    {
        #region fields
        private bool mDisposed;
        #endregion fields

        #region constructor
        /// <summary>
        /// Class constructor
        /// </summary>
        public DisposableObject()
        {
        }

        /// <summary>
        /// Class constructor with action parameter
        /// that defines the event that is raised
        /// when the object has been disposed.
        /// </summary>
        /// <param name="action"></param>
        public DisposableObject(Action action)
        {
            this.Disposed = action;
        }

        /// <summary>
        /// Class finalizer/destructor
        /// When the object is eligible for finalization,
        /// the garbage collector runs the Finalize method of the object. 
        /// </summary>
        ~DisposableObject()
        {
            this.Dispose(false);
        }
        #endregion constructor

        /// <summary>
        /// Event is raised when the object has been disposed.
        /// </summary>
        public event Action Disposed = delegate { };

        #region methods
        /// <summary>
        /// Public implementation of Dispose pattern callable by consumers.
        /// (required by IDisposable interface)
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // Use SupressFinalize in case a subclass
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);

            if (this.Disposed != null) // Raise disposed event
                this.Disposed();
        }

        /// <summary>
        /// Protected implementation of Dispose pattern -
        /// to enable inheriting classes to participate in this pattern.
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these 
            // operations, as well as in your methods that use the resource.
            if (!this.mDisposed)
            {
                if (disposing)
                {
                    this.OnDispose();
                }

                // Indicate that the instance has been disposed.
                this.mDisposed = true;
            }
        }

        /// <summary>
        /// Protected override-able implementation of Dispose pattern -
        /// to enable inheriting classes to participate in this pattern
        /// by overriding the original dispose implementation.
        /// </summary>
        protected virtual void OnDispose()
        {
        }
        #endregion methods
    }
}

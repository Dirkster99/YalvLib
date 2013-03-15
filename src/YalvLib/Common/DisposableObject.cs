namespace YalvLib.Common
{
  using System;

  /// <summary>
  /// "Implementing a Dispose method"
  /// http://msdn.microsoft.com/en-us/library/fs2xkftw.aspx
  /// </summary>
  public class DisposableObject : IDisposable
  {
    #region fields
    private bool mDisposed;
    #endregion fields

    #region constructor
    public DisposableObject()
    {
    }

    public DisposableObject(Action action)
    {
      this.Disposed = action;
    }

    ~DisposableObject()
    {
      this.Dispose(false);
    }
    #endregion constructor

    public event Action Disposed = delegate { };

    #region methods
    public void Dispose()
    {
      this.Dispose(true);

      // Use SupressFinalize in case a subclass
      // of this type implements a finalizer.
      GC.SuppressFinalize(this);

      // Raise disposed event
      if (this.Disposed != null)
        this.Disposed();
    }

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

    protected virtual void OnDispose()
    {
    }
    #endregion methods
  }
}

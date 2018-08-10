namespace YalvLib.UnitTests.ViewModel
{
    /// <summary>
    /// Implements a synchronization object to run MS Unit tests
    /// in an STA like Mode.
    /// 
    /// https://stackoverflow.com/questions/5037447/forcing-mstest-to-use-a-single-thread
    /// </summary>
    public static class IntegrationTestsSynchronization
    {
        /// <summary>
        /// Dummy object to lock on.
        /// </summary>
        public static readonly object LockObject = new object();
    }
}

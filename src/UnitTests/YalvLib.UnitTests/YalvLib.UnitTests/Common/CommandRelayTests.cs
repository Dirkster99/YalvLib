namespace YalvLib.UnitTests.Common
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using YalvLib.Common;

    [TestClass]
    public class CommandRelayTests
    {
        [TestMethod]
        public void OnCanExecuteChanged()
        {
            CommandRelay c = new CommandRelay(null, delegate
            {
                return true;
            });

            Boolean isHandlerCalled = false;
            EventHandler a = delegate (object sender, EventArgs args)
            {
                isHandlerCalled = true;
            };

            try
            {
                c.CanExecuteChanged += a;
                c.CanExecute(null);
                Assert.IsTrue(isHandlerCalled);
            }
            finally
            {
                c.CanExecuteChanged -= a;
            }
        }
    }
}

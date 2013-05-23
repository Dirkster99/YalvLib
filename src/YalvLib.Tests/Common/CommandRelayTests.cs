using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using YalvLib.Common;

namespace YalvLib.Tests.Common
{

    [TestFixture]
    public class CommandRelayTests
    {

        [Test]
        public void OnCanExecuteChanged()
        {
            CommandRelay c = new CommandRelay(null, delegate
                                                        {
                                                            return true;
                                                        });
            Boolean isHandlerCalled = false;
            EventHandler a = delegate(object sender, EventArgs args)
                                 {
                                     isHandlerCalled = true;
                                 };
            try
            {
                c.CanExecuteChanged += a;
                c.CanExecute(null);
                Assert.IsTrue(isHandlerCalled);
            } finally
            {
                c.CanExecuteChanged -= a;                                
            }
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using YalvLib.Model;

namespace YalvLib.Tests.Integration.Model
{

    [TestFixture]
    public class LogEntryFileRepositoryTests
    {

        [Test]
        public void LoadFile()
        {
            LogEntryFileRepository repository = new LogEntryFileRepository("Model/sample.xml");
            Assert.AreEqual(2, repository.LogEntries.Count);
        }

    }

}

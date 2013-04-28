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
    public class LogEntrySqliteRepositoryTests
    {

        [Test]
        public void LoadDatabase()
        {
            LogEntrySqliteRepository repository = new LogEntrySqliteRepository("Model/SampleLogs.db3");
            Assert.AreEqual(2, repository.LogEntries.Count);
            Assert.AreEqual(1, repository.LogEntries[0].Id);
            Assert.AreEqual(2, repository.LogEntries[1].Id);
        }

    }

}

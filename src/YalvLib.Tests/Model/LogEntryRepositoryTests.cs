using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using YalvLib.Model;
using YalvLib.ViewModel;

namespace YalvLib.Tests.Model
{

    [TestFixture]
    public class LogEntryRepositoryTests
    {

        [Test]
        public void AddLogEntry()
        {
            LogEntryRepository repository = new LogEntryRepository();
            LogEntry entry = new LogEntry();
            repository.AddLogEntry(entry);
            Assert.AreEqual(1, repository.LogEntries.Count());
        }

        [Test]
        public void AddLogEntry_NotOk_NullEntry()
        {
            LogEntryRepository repository = new LogEntryRepository();
            LogEntry entry = null;
            Assert.Throws<ArgumentNullException>(delegate
                                                     {
                                                         repository.AddLogEntry(entry);
                                                     });
        }

    }

}

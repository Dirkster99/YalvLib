using System;
using System.Linq;
using NUnit.Framework;
using YalvLib.Model;

namespace YalvLib.Tests.Model
{
    [TestFixture]
    public class LogEntryRepositoryTests
    {
        [Test]
        public void AddLogEntry()
        {
            var repository = new LogEntryRepository();
            var entry = new LogEntry();
            Assert.AreEqual(0, entry.Id);
            repository.AddLogEntry(entry);
            Assert.AreEqual(1, repository.LogEntries.Count());
            Assert.AreEqual(1, entry.Id);

            entry = new LogEntry();
            repository.AddLogEntry(entry);
            Assert.AreEqual(2, entry.Id);
        }

        [Test]
        public void AddLogEntry_NotOk_NullEntry()
        {
            var repository = new LogEntryRepository();
            LogEntry entry = null;
            Assert.Throws<ArgumentNullException>(delegate { repository.AddLogEntry(entry); });
        }

        [Test]
        public void EqualsTest()
        {
            var repository1 = new LogEntryRepository();
            var repository2 = new LogEntryRepository();
            var entry = new LogEntry();
            repository1.AddLogEntry(entry);
            repository2.AddLogEntry(entry);
            Assert.IsTrue(repository1.Equals(repository2));
        }
    }
}

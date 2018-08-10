namespace YalvLib.UnitTests.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Linq;
    using YalvLib.Model;

    [TestClass]
    public class LogEntryRepositoryTests
    {
        [TestMethod]
        public void AddLogEntry()
        {
            var repository = new LogEntryRepository();
            var entry = new LogEntry();
            Assert.AreEqual((uint)0, entry.Id);
            repository.AddLogEntry(entry);
            Assert.AreEqual(1, repository.LogEntries.Count());
            Assert.AreEqual((uint)1, entry.Id);

            entry = new LogEntry();
            repository.AddLogEntry(entry);
            Assert.AreEqual((uint)2, entry.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddLogEntry_NotOk_NullEntry()
        {
            var repository = new LogEntryRepository();
            LogEntry entry = null;

            repository.AddLogEntry(entry);
        }

        [TestMethod]
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

namespace YalvLib.UnitTests.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using YalvLib.Model;

    [TestClass]
    public class RepositoryMergerTests
    {
        [TestMethod]
        public void Test_2Repo_WithCrossedDatedEntries()
        {
            var entry1 = new LogEntry();
            entry1.TimeStamp = DateTime.MinValue + new TimeSpan(0, 0, 0, 1);
            var entry2 = new LogEntry();
            entry2.TimeStamp = DateTime.MinValue + new TimeSpan(0, 0, 0, 10);
            var sourceRepository1 = new LogEntryRepository();
            sourceRepository1.AddLogEntry(entry1);
            sourceRepository1.AddLogEntry(entry2);

            var entry3 = new LogEntry();
            entry3.TimeStamp = DateTime.MinValue + new TimeSpan(0, 0, 0, 5);
            var entry4 = new LogEntry();
            entry4.TimeStamp = DateTime.MinValue + new TimeSpan(0, 0, 0, 15);
            var sourceRepository2 = new LogEntryRepository();
            sourceRepository2.AddLogEntry(entry3);
            sourceRepository2.AddLogEntry(entry4);

            var merger = new RepositoryMerger();
            merger.AddSourceRepository(sourceRepository1);
            merger.AddSourceRepository(sourceRepository2);
            LogEntryRepository targetRepository = merger.Merge();

            Assert.AreEqual(entry1.TimeStamp, targetRepository.LogEntries[0].TimeStamp);
            Assert.AreEqual(entry3.TimeStamp, targetRepository.LogEntries[1].TimeStamp);
            Assert.AreEqual(entry2.TimeStamp, targetRepository.LogEntries[2].TimeStamp);
            Assert.AreEqual(entry4.TimeStamp, targetRepository.LogEntries[3].TimeStamp);
            Assert.AreEqual((uint)1, targetRepository.LogEntries[0].Id);
            Assert.AreEqual((uint)2, targetRepository.LogEntries[1].Id);
            Assert.AreEqual((uint)3, targetRepository.LogEntries[2].Id);
            Assert.AreEqual((uint)4, targetRepository.LogEntries[3].Id);
        }
    }
}

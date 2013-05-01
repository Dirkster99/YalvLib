using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using YalvLib.Model;

namespace YalvLib.Tests.Model
{

    [TestFixture]
    public class RepositoryMergerTests
    {

        [Test]
        public void Test_2Repo_WithCrossedDatedEntries()
        {
            LogEntry entry1 = new LogEntry();
            entry1.TimeStamp = DateTime.MinValue + new TimeSpan(0, 0, 0, 1);
            LogEntry entry2 = new LogEntry();
            entry2.TimeStamp = DateTime.MinValue + new TimeSpan(0, 0, 0, 10);
            LogEntryRepository sourceRepository1 = new LogEntryRepository();
            sourceRepository1.AddLogEntry(entry1);
            sourceRepository1.AddLogEntry(entry2);

            LogEntry entry3 = new LogEntry();
            entry3.TimeStamp = DateTime.MinValue + new TimeSpan(0, 0, 0, 5);
            LogEntry entry4 = new LogEntry();
            entry4.TimeStamp = DateTime.MinValue + new TimeSpan(0, 0, 0, 15);
            LogEntryRepository sourceRepository2 = new LogEntryRepository();
            sourceRepository2.AddLogEntry(entry3);
            sourceRepository2.AddLogEntry(entry4);

            RepositoryMerger merger = new RepositoryMerger();
            merger.AddSourceRepository(sourceRepository1);
            merger.AddSourceRepository(sourceRepository2);
            LogEntryRepository targetRepository = merger.Merge();

            Assert.AreEqual(entry1.TimeStamp, targetRepository.LogEntries[0].TimeStamp);
            Assert.AreEqual(entry3.TimeStamp, targetRepository.LogEntries[1].TimeStamp);
            Assert.AreEqual(entry2.TimeStamp, targetRepository.LogEntries[2].TimeStamp);
            Assert.AreEqual(entry4.TimeStamp, targetRepository.LogEntries[3].TimeStamp);
            Assert.AreEqual(1, targetRepository.LogEntries[0].Id);
            Assert.AreEqual(2, targetRepository.LogEntries[1].Id);
            Assert.AreEqual(3, targetRepository.LogEntries[2].Id);
            Assert.AreEqual(4, targetRepository.LogEntries[3].Id);
        }

    }

}

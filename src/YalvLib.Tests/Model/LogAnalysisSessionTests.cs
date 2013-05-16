using NUnit.Framework;
using YalvLib.Model;

namespace YalvLib.Tests.Model
{
    [TestFixture]
    public class LogAnalysisSessionTests
    {
        [Test]
        public void AddSourceRepository()
        {
            var session = new LogAnalysisWorkspace();
            session.AddSourceRepository(new LogEntryFileRepository());
            Assert.AreEqual(1, session.SourceRepositories.Count);
        }

        [Test]
        public void LogEntriesFrom1Repository()
        {
            var session = new LogAnalysisWorkspace();
            var repository = new LogEntryFileRepository();
            repository.AddLogEntry(new LogEntry());
            repository.AddLogEntry(new LogEntry());
            session.AddSourceRepository(repository);
            Assert.AreEqual(2, session.LogEntries.Count);
        }

        [Test]
        public void LogEntriesFrom2Repositories()
        {
            var session = new LogAnalysisWorkspace();
            for (int i = 0; i < 2; i++)
            {
                var repository = new LogEntryFileRepository();
                repository.AddLogEntry(new LogEntry());
                repository.AddLogEntry(new LogEntry());
                session.AddSourceRepository(repository);
            }
            Assert.AreEqual(4, session.LogEntries.Count);
        }
    }
}

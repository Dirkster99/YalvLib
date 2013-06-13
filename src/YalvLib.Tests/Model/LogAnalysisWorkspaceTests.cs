using NUnit.Framework;
using YalvLib.Model;

namespace YalvLib.Tests.Model
{
    [TestFixture]
    public class LogAnalysisWorkspaceTests
    {

        private LogAnalysisWorkspace _session;

        [SetUp]
        public void CreateEnvironment()
        {
            _session = new LogAnalysisWorkspace();
        }

        [Test]
        public void AddSourceRepository()
        {
            _session.AddSourceRepository(new LogEntryFileRepository());
            Assert.AreEqual(1, _session.SourceRepositories.Count);
        }

        [Test]
        public void LogEntriesFrom1Repository()
        {
            var repository = new LogEntryFileRepository();
            repository.AddLogEntry(new LogEntry());
            repository.AddLogEntry(new LogEntry());
            _session.AddSourceRepository(repository);
            Assert.AreEqual(2, _session.LogEntries.Count);
        }

        [Test]
        public void LogEntriesFrom2Repositories()
        {
            for (int i = 0; i < 2; i++)
            {
                var repository = new LogEntryFileRepository();
                repository.AddLogEntry(new LogEntry());
                repository.AddLogEntry(new LogEntry());
                _session.AddSourceRepository(repository);
            }
            Assert.AreEqual(4, _session.LogEntries.Count);
        }

        [Test]
        public void CreateNewLogAnalysisTest()
        {
            var logA = new LogAnalysis();
            _session.CurrentAnalysis = logA;
            Assert.AreEqual(_session.Analyses[0], logA);
        } 

    }
}

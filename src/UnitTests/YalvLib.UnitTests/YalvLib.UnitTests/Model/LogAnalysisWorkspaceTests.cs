namespace YalvLib.UnitTests.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using YalvLib.Model;

    [TestClass]
    public class LogAnalysisWorkspaceTests
    {

        private LogAnalysisWorkspace _session;

        [TestInitialize]
        public void CreateEnvironment()
        {
            _session = new LogAnalysisWorkspace();
        }

        [TestMethod]
        public void AddSourceRepository()
        {
            _session.AddSourceRepository(new LogEntryFileRepository());
            Assert.AreEqual(1, _session.SourceRepositories.Count);
        }

        [TestMethod]
        public void LogEntriesFrom1Repository()
        {
            var repository = new LogEntryFileRepository();
            repository.AddLogEntry(new LogEntry());
            repository.AddLogEntry(new LogEntry());
            _session.AddSourceRepository(repository);
            Assert.AreEqual(2, _session.LogEntries.Count);
        }

        [TestMethod]
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

        [TestMethod]
        public void CreateNewLogAnalysisTest()
        {
            var logA = new LogAnalysis();
            _session.CurrentAnalysis = logA;
            Assert.AreEqual(_session.Analyses[0], logA);
        } 

    }
}

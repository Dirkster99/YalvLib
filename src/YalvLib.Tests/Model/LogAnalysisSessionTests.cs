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
    public class LogAnalysisSessionTests
    {

        [Test]
        public void AddSourceRepository()
        {
            LogAnalysisSession session = new LogAnalysisSession();
            session.AddSourceRepository(new LogEntryFileRepository("fake"));
            Assert.AreEqual(1, session.SourceRepositories.Count);
        }

        [Test]
        public void LogEntries_From1Repository()
        {
            LogAnalysisSession session = new LogAnalysisSession();
            LogEntryFileRepository repository = new LogEntryFileRepository("fake");
            repository.AddLogEntry(new LogEntry());
            repository.AddLogEntry(new LogEntry());
            session.AddSourceRepository(repository);
            Assert.AreEqual(2, session.LogEntries.Count);            
        }

        [Test]
        public void LogEntries_From2Repositories()
        {
            LogAnalysisSession session = new LogAnalysisSession();
            for (int i = 0; i < 2; i++)
            {
                LogEntryFileRepository repository = new LogEntryFileRepository("fake");
                repository.AddLogEntry(new LogEntry());
                repository.AddLogEntry(new LogEntry());
                session.AddSourceRepository(repository);
            }
            Assert.AreEqual(4, session.LogEntries.Count);
        }
    }

}

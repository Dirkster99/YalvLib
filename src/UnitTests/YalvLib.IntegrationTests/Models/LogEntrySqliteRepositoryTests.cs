namespace YalvLib.IntegrationTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using YalvLib.Model;

    [TestClass]
    public class LogEntrySqliteRepositoryTests
    {
        [TestMethod]
        public void LoadDatabase()
        {
            LogEntrySqliteRepository repository = new LogEntrySqliteRepository("Models/SampleLogs.db3");
            Assert.AreEqual(2, repository.LogEntries.Count);
            Assert.AreEqual((uint)1, repository.LogEntries[0].Id);
            Assert.AreEqual((uint)2, repository.LogEntries[1].Id);
        }
    }
}

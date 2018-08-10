namespace YalvLib.IntegrationTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using YalvLib.Model;

    [TestClass]
    public class LogEntryFileRepositoryTests
    {
        [TestMethod]
        public void LoadFile()
        {
            LogEntryFileRepository repository = new LogEntryFileRepository("Models/sample.xml");
            Assert.AreEqual(2, repository.LogEntries.Count);
            // [FT] Following is working but should be placed in a nice unit test.
            //LogAnalysisWorkspace session = new LogAnalysisWorkspace();
            //session.AddSourceRepository(repository);
            //new ExportSession().Export(session);
        }

        [TestMethod]
        public void LoadFileWithSpecialCharacters()
        {
            LogEntryFileRepository repository = null;

            try
            {
                repository = new LogEntryFileRepository("Models/sample_encoding.xml");
            }
            catch
            {
                // repository construction should work without exception
                Assert.Fail();
            }

            Assert.AreEqual(@"tongbong-PC\Gwenaël", repository.LogEntries[0].UserName);
            // Tests on the dataGrid display are still to be written to ensure the "good looking" of the app.
        }

        [TestMethod]
        public void EqualsTest()
        {
            LogEntryFileRepository repo = new LogEntryFileRepository("Models/sample.xml");
            LogEntryFileRepository repo2 = new LogEntryFileRepository("Models/sample.xml");
            Assert.IsTrue(repo.Equals(repo2));
        }
    }
}

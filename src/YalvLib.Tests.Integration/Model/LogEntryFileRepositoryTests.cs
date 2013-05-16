using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using YalvLib.Infrastructure.Sqlite;
using YalvLib.Model;

namespace YalvLib.Tests.Integration.Model
{

    [TestFixture]
    public class LogEntryFileRepositoryTests
    {

        [Test]
        public void LoadFile()
        {
            LogEntryFileRepository repository = new LogEntryFileRepository("Model/sample.xml");
            Assert.AreEqual(2, repository.LogEntries.Count);
            // [FT] Following is working but should be placed in a nice unit test.
            //LogAnalysisWorkspace session = new LogAnalysisWorkspace();
            //session.AddSourceRepository(repository);
            //new ExportSession().Export(session);
        }

        [Test]
        public void LoadFileWithSpecialCharacters()
        {
            LogEntryFileRepository repository = null;
            Assert.DoesNotThrow(delegate
            {
               repository = new LogEntryFileRepository("Model/sample_encoding.xml");
            });
            Assert.AreEqual(@"tongbong-PC\Gwenaël", repository.LogEntries[0].UserName);
            // Tests on the dataGrid display are still to be written to ensure the "good looking" of the app.
        }

    }

}

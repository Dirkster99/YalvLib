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
            //LogAnalysisSession session = new LogAnalysisSession();
            //session.AddSourceRepository(repository);
            //new ExportSession().Export(session);
        }

    }

}

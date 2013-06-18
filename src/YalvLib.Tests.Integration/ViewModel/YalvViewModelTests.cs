using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using YalvLib.Model;
using YalvLib.Providers;
using YalvLib.ViewModel;

namespace YalvLib.Tests.Integration.ViewModel
{
    [TestFixture]
    public class YalvViewModelTests
    {

        [Test]
        public void GetListRepositories()
        {
            YalvRegistry.Instance.SetActualLogAnalysisWorkspace(new LogAnalysisWorkspace());
            YalvViewModel yalvVm = new YalvViewModel();
            List<string> files = new List<string>(){"Model/sample.xml", "Model/sample_encoding.xml"};
            yalvVm.ManageRepositoriesViewModel.LoadFiles(files, EntriesProviderType.Xml);
            Assert.AreEqual(YalvRegistry.Instance.ActualWorkspace.SourceRepositories.Count, 2);
            Assert.AreEqual(YalvRegistry.Instance.ActualWorkspace.LogEntries.Count, 3);
            yalvVm.LoadFiles(files);
            Assert.AreEqual(YalvRegistry.Instance.ActualWorkspace.SourceRepositories.Count, 2);
            Assert.AreEqual(YalvRegistry.Instance.ActualWorkspace.LogEntries.Count, 3);

        }
    }
}

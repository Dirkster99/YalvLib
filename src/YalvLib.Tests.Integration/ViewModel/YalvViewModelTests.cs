using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using YalvLib.ViewModel;

namespace YalvLib.Tests.Integration.ViewModel
{
    [TestFixture]
    public class YalvViewModelTests
    {

        [Test]
        public void GetListRepositories()
        {
            YalvViewModel yalvVm = new YalvViewModel();
            List<string> files = new List<string>(){"Model/sample.xml", "Model/sample_encoding.xml"};
            yalvVm.LoadFiles(files);
            Assert.AreEqual(yalvVm.ManageRepositoriesViewModel.Repositories.Count, 2);
            Assert.AreEqual(yalvVm.LogEntryRows.LogEntryRowViewModels.Count, 3);
            yalvVm.LoadFiles(files);
            Assert.AreEqual(yalvVm.ManageRepositoriesViewModel.Repositories.Count, 2);
            Assert.AreEqual(yalvVm.LogEntryRows.LogEntryRowViewModels.Count, 3);

        }
    }
}

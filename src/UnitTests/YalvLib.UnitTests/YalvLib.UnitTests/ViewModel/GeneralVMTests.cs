using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YalvLib.Model;
using YalvLib.ViewModel;

namespace YalvLib.Tests.ViewModel
{

    [TestClass]
    public class GeneralVMTests
    {

        [TestMethod]
        public void foo()
        {
            //LogEntry e1 = new LogEntry();
            //LogEntry e2 = new LogEntry();
            //TextMarker t1 = new TextMarker(new List<LogEntry> { e1, e2 }, "Toto", "Hello");
            //TextMarker t2 = new TextMarker(new List<LogEntry> { e1 }, "Toto", "Hello");
            //YalvViewModel yalvVM = YalvViewModelFactory.CreateVM(
            //                                                new List<LogEntry> { e1, e2 },
            //                                                new List<TextMarker> { t1, t2 });
            //Assert.AreEqual(2, yalvVM.LogEntryRows.LogEntryRowViewModels.Count);
            //Assert.AreEqual(2, yalvVM.ManageTextMarkersViewModel.TextMarkerViewModels);

            //Assert.AreEqual(2, yalvVM.LogEntryRows.LogEntryRowViewModels[0].TextMarkerQuantity);
            //Assert.AreEqual(1, yalvVM.LogEntryRows.LogEntryRowViewModels[1].TextMarkerQuantity);

            // delete t1
            //yalvVM.ManageTextMarkersViewModel.TextMarkerViewModels[0].CommandCancelTextMarker.CanExecute(null);

            //Assert.AreEqual(1, yalvVM.LogEntryRows.LogEntryRowViewModels[0].TextMarkerQuantity);
            //Assert.AreEqual(0, yalvVM.LogEntryRows.LogEntryRowViewModels[1].TextMarkerQuantity);
        }

    }

}

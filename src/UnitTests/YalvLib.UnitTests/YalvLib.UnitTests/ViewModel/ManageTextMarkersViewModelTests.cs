namespace YalvLib.Tests.ViewModel
{
    using log4netLib.Interfaces;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using YalvLib.Common;
    using YalvLib.Model;
    using YalvLib.ViewModels;

    [TestClass]
    public class ManageTextMarkersViewModelTests
    {
        private ManageTextMarkersViewModel _manageTextMarkers;
        private LogEntry _entry;

        [TestInitialize]
        public void InitEnvironment()
        {
            YalvRegistry.Instance.SetActualLogAnalysisWorkspace(new LogAnalysisWorkspace()); 
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis = new LogAnalysis();
            _manageTextMarkers = new ManageTextMarkersViewModel(YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis);
            _entry = new LogEntry();     
        }

        [TestMethod]
        public void GetTextMarkersViewModelsTest()
        {
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.AddTextMarker(new List<LogEntry>() { _entry }, "plop", "Coincoin");
            List<TextMarker> textMarkers = YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.GetTextMarkersForEntry(_entry);
            _manageTextMarkers.GenerateViewModels(textMarkers);
            Assert.AreEqual(_manageTextMarkers.TextMarkerViewModels.Count, textMarkers.Count);
            Assert.AreEqual(_manageTextMarkers.TextMarkerViewModels[0].Marker.Author, "plop");
        }

        [TestMethod]
        public void NotificationsMarkersUpdateTest()
        {
            List<TextMarker> textMarkers = YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.GetTextMarkersForEntry(_entry);

            PropertyChangedEventHandler delegateViewModelsTextMarker = (senderTextMarkerVM, e) => Assert.AreEqual("TextMarkerToAdd", e.PropertyName);
            try
            {
                _manageTextMarkers.PropertyChanged += delegateViewModelsTextMarker;
                _manageTextMarkers.GenerateViewModels(textMarkers);
            }finally
            {
                _manageTextMarkers.PropertyChanged -= delegateViewModelsTextMarker;
            }
        }
    }
}

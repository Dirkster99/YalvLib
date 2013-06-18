using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using YalvLib.Common;
using YalvLib.Model;
using YalvLib.ViewModel;

namespace YalvLib.Tests.ViewModel
{
    [TestFixture]
    public class ManageTextMarkersViewModelTests
    {
        private ManageTextMarkersViewModel _manageTextMarkers;
        private LogEntry _entry;

        [SetUp]
        public void InitEnvironment()
        {
            _manageTextMarkers = new ManageTextMarkersViewModel();
            _entry = new LogEntry();
            YalvRegistry.Instance.SetActualLogAnalysisWorkspace(new LogAnalysisWorkspace());
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis = new LogAnalysis();
            
        }

        [Test]
        public void GetTextMarkersViewModelsTest()
        {
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.AddTextMarker(new List<LogEntry>() { _entry }, "plop", "Coincoin");
            List<TextMarker> textMarkers = YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.GetTextMarkersForEntry(_entry);
            _manageTextMarkers.GenerateViewModels(textMarkers);
            Assert.AreEqual(_manageTextMarkers.TextMarkerViewModels.Count, textMarkers.Count);
            Assert.AreEqual(_manageTextMarkers.TextMarkerViewModels[0].Marker.Author, "plop");
        }

        [Test]
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

        [Test]
        public void NotificationMarkerDelete()
        {
            YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.AddTextMarker(new List<LogEntry>() {_entry}, "Hallo",
                                                                         "c'est cotelette que vous comprenez pas?");
            List<TextMarker> textMarkers = YalvRegistry.Instance.ActualWorkspace.CurrentAnalysis.GetTextMarkersForEntry(_entry);
            _manageTextMarkers.GenerateViewModels(textMarkers);
            _manageTextMarkers.SelectedEntries = new List<LogEntryRowViewModel>(){new LogEntryRowViewModel(_entry)};

            EventHandler e = (sender, args) => Assert.IsInstanceOf<CommandRelay>(sender);

            Assert.AreEqual(1, _manageTextMarkers.TextMarkerViewModels.Count);

            _manageTextMarkers.TextMarkerViewModels[0].CommandCancelTextMarker.Executed += e;
            _manageTextMarkers.TextMarkerViewModels[0].CommandCancelTextMarker.Execute(null);

            Assert.AreEqual(0, _manageTextMarkers.TextMarkerViewModels.Count);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using YalvLib.Model;
using YalvLib.ViewModel;

namespace YalvLib.Tests.ViewModel
{
    [TestFixture]
    public class DisplayTextMarkersViewModelTests
    {
        private DisplayTextMarkersViewModel _displayTextMarkers;
        private LogEntry _entry;

        [SetUp]
        public void InitEnvironment()
        {
            _displayTextMarkers = new DisplayTextMarkersViewModel();
            _entry = new LogEntry();
            YalvRegistry.Instance.SetActualLogAnalysisSession(new LogAnalysisWorkspace());
            
        }

        [Test]
        public void GetTextMarkersViewModelsTest()
        {
            YalvRegistry.Instance.ActualWorkspace.Analysis.AddTextMarker(new List<LogEntry>() { _entry }, "plop", "Coincoin");
            List<TextMarker> textMarkers = YalvRegistry.Instance.ActualWorkspace.Analysis.GetTextMarkersForEntry(_entry);
            _displayTextMarkers.GenerateViewModels(textMarkers);
            Assert.AreEqual(_displayTextMarkers.TextMarkerViewModels.Count, textMarkers.Count);
            Assert.AreEqual(_displayTextMarkers.TextMarkerViewModels[0].Marker.Author, "plop");
        }

        [Test]
        public void NotificationsMarkersUpdateTest()
        {
            List<TextMarker> textMarkers = YalvRegistry.Instance.ActualWorkspace.Analysis.GetTextMarkersForEntry(_entry);

            PropertyChangedEventHandler delegateViewModelsTextMarker = (senderTextMarkerVM, e) => Assert.AreEqual("TextMarkerViewModels", e.PropertyName);
            try
            {
                _displayTextMarkers.PropertyChanged += delegateViewModelsTextMarker;
                _displayTextMarkers.GenerateViewModels(textMarkers);
            }finally
            {
                _displayTextMarkers.PropertyChanged -= delegateViewModelsTextMarker;
            }
        }
    }
}

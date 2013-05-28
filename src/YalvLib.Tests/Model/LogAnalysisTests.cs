using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using YalvLib.Model;

namespace YalvLib.Tests.Model
{
    [TestFixture]
    public class LogAnalysisTests
    {

        private LogAnalysis _analysis;
        private LogEntry _entry1;
        private LogEntry _entry2;

        [SetUp]
        public void InitEnvironment()
        {
            _analysis = new LogAnalysis();
            _entry1 = new LogEntry();
            _entry2 = new LogEntry();
        }

        [Test]
        public void CreateInstance()
        {
            Assert.AreEqual(3, _analysis.Markers.Count);
            Assert.IsTrue(_analysis.ColorMarkers.Any(x => x.HighlightColor.Equals(Color.Chocolate)));
            Assert.IsTrue(_analysis.ColorMarkers.Any(x => x.HighlightColor.Equals(Color.BlueViolet)));
            Assert.IsTrue(_analysis.ColorMarkers.Any(x => x.HighlightColor.Equals(Color.CadetBlue)));
        }

        [Test]
        public void IsMultiMarkerTest()
        {
            TextMarker marker = _analysis.AddTextMarker(new List<LogEntry> {_entry1, _entry2}, "ME", "My message");
            Assert.IsTrue(_analysis.IsMultiMarker(marker));
            marker.LogEntries.Remove(_entry1);
            marker.LogEntries.Remove(_entry2);
            Assert.IsFalse(_analysis.IsMultiMarker(marker));
        }

        [Test]
        public void AddTextMarkerTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> {_entry1}, "ME", "My message");
            Assert.AreEqual(1, _analysis.TextMarkers.Count);
        }

        [Test]
        public void RemoveTextMarkerFromEntryTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> {_entry1}, "ME", "My message");
            _analysis.RemoveTextMarker(_entry1);
            Assert.AreEqual(0, _analysis.TextMarkers.Count);
        }
        
        [Test]
        public void DoNotDeleteTextMarkerTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> {_entry1, _entry2}, "ME", "My message");
            _analysis.RemoveTextMarker(_entry1);
            Assert.AreEqual(1, _analysis.TextMarkers.Count);
        }


        [Test]
        public void DeleteTextMarkerFromMarkerTest()
        {
            TextMarker marker = _analysis.AddTextMarker(new List<LogEntry> {_entry1, _entry2}, "ME", "My message");
            _analysis.DeleteTextMarker(marker);
            Assert.AreEqual(0, _analysis.TextMarkers.Count);
        }

        [Test]
        public void GetTextMarkersForEntryTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> { _entry1 }, "ME", "My message");
            _analysis.AddTextMarker(new List<LogEntry> { _entry1, _entry2 }, "ME2", "My message2");
            List<TextMarker> tMarkers = _analysis.GetTextMarkersForEntry(_entry1);
            Assert.AreEqual(tMarkers.Count, 2);       
        }

        [Test]
        public void GetTextMarkersForEntriesTest()
        {
            LogEntry entry3 = new LogEntry();
            _analysis.AddTextMarker(new List<LogEntry> { _entry1 }, "ME", "My message");
            _analysis.AddTextMarker(new List<LogEntry> { _entry1, _entry2, entry3 }, "ME2", "My message2");
            _analysis.AddTextMarker(new List<LogEntry> { _entry1, entry3 }, "ME2", "My message2");
            List<TextMarker> tMarkers = _analysis.GetTextMarkersForEntries(new List<LogEntry> { _entry1, _entry2 });
            Assert.AreEqual(3, tMarkers.Count);
            tMarkers = _analysis.GetTextMarkersForEntries(new List<LogEntry> { entry3 });
            Assert.AreEqual(2, tMarkers.Count);
        }

        [Test]
        public void ExistMarkerForLogEntryTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> { _entry1 }, "ME", "My message");
            _analysis.AddTextMarker(new List<LogEntry> { _entry1, _entry2 }, "ME2", "My message2");
            LogEntry entry3 = new LogEntry();
            Assert.IsTrue(_analysis.ExistTextMarkerForLogEntry(_entry1));
            Assert.IsFalse(_analysis.ExistTextMarkerForLogEntry(entry3));
        }

        [Test]
        public void ExistMarkerForLogEntriesTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> { _entry1 }, "ME", "My message");
            _analysis.AddTextMarker(new List<LogEntry> { _entry1, _entry2 }, "ME2", "My message2");
            LogEntry entry3 = new LogEntry();
            LogEntry entry4 = new LogEntry();
            Assert.IsTrue(_analysis.ExistTextMarkerForLogEntries(new List<LogEntry>(){_entry1, _entry2}));
            Assert.IsFalse(_analysis.ExistTextMarkerForLogEntries(new List<LogEntry>(){entry3, entry4}));
        }

        [Test]
        public void SetColorMarkersForEntries()
        {
            _analysis.SetColorMarker(new List<LogEntry> {_entry1}, Color.BlueViolet);
            Assert.IsTrue(_analysis.ColorMarkers[0].LogEntries.Contains(_entry1));
            Assert.IsFalse(_analysis.ColorMarkers[0].LogEntries.Contains(_entry2));
        }

        [Test]
        public void GetColorMarkerFromColor()
        {
            Assert.AreEqual(_analysis.GetColorMarker(Color.BlueViolet), _analysis.ColorMarkers[0]);
        }


    }
}

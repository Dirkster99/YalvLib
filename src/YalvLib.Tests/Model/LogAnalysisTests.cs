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
        public void IsMultiMarker()
        {
            TextMarker marker = _analysis.AddTextMarker(new List<LogEntry> {_entry1, _entry2}, "ME", "My message");
            Assert.IsTrue(_analysis.IsMultiMarker(marker));
            marker.LogEntries.Remove(_entry1);
            marker.LogEntries.Remove(_entry2);
            Assert.IsFalse(_analysis.IsMultiMarker(marker));
        }

        [Test]
        public void AddTextMarker()
        {
            _analysis.AddTextMarker(new List<LogEntry> {_entry1}, "ME", "My message");
            Assert.AreEqual(1, _analysis.TextMarkers.Count);
        }

        [Test]
        public void RemoveTextMarkerFromEntry()
        {
            _analysis.AddTextMarker(new List<LogEntry> {_entry1}, "ME", "My message");
            _analysis.RemoveTextMarker(_entry1);
            Assert.AreEqual(0, _analysis.TextMarkers.Count);
        }
        
        [Test]
        public void DoNotDeleteTextMarker()
        {
            _analysis.AddTextMarker(new List<LogEntry> {_entry1, _entry2}, "ME", "My message");
            _analysis.RemoveTextMarker(_entry1);
            Assert.AreEqual(1, _analysis.TextMarkers.Count);
        }


        [Test]
        public void DeleteTextMarkerFromMarker()
        {
            TextMarker marker = _analysis.AddTextMarker(new List<LogEntry> {_entry1, _entry2}, "ME", "My message");
            _analysis.DeleteTextMarker(marker);
            Assert.AreEqual(0, _analysis.TextMarkers.Count);
        }

        [Test]
        public void GetTextMarkersForEntry()
        {
            _analysis.AddTextMarker(new List<LogEntry> { _entry1 }, "ME", "My message");
            _analysis.AddTextMarker(new List<LogEntry> { _entry1, _entry2 }, "ME2", "My message2");
            List<TextMarker> tMarkers = _analysis.GetTextMarkersForEntry(_entry1);
            Assert.AreEqual(tMarkers.Count, 2);
        }
    }
}

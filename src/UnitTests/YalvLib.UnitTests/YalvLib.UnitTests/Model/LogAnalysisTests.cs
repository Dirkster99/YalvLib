namespace YalvLib.UnitTests.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using YalvLib.Model;

    [TestClass]
    public class LogAnalysisTests
    {
        private LogAnalysis _analysis;
        private LogEntry _entry1;
        private LogEntry _entry2;

        [TestInitialize]
        public void InitEnvironment()
        {
            _analysis = new LogAnalysis();
            _entry1 = new LogEntry();
            _entry2 = new LogEntry();
        }

        [TestMethod]
        public void IsMultiMarkerTest()
        {
            TextMarker marker = _analysis.AddTextMarker(new List<LogEntry> {_entry1, _entry2}, "ME", "My message");
            Assert.IsTrue(_analysis.IsMultiMarker(marker));
            marker.LogEntries.Remove(_entry1);
            marker.LogEntries.Remove(_entry2);
            Assert.IsFalse(_analysis.IsMultiMarker(marker));
        }

        [TestMethod]
        public void AddTextMarkerTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> {_entry1}, "ME", "My message");
            Assert.AreEqual(1, _analysis.TextMarkers.Count);
        }

        [TestMethod]
        public void RemoveTextMarkerFromEntryTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> {_entry1}, "ME", "My message");
            _analysis.RemoveTextMarker(_entry1);
            Assert.AreEqual(0, _analysis.TextMarkers.Count);
        }

        [TestMethod]
        public void DoNotDeleteTextMarkerTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> {_entry1, _entry2}, "ME", "My message");
            _analysis.RemoveTextMarker(_entry1);
            Assert.AreEqual(1, _analysis.TextMarkers.Count);
        }


        [TestMethod]
        public void DeleteTextMarkerFromMarkerTest()
        {
            TextMarker marker = _analysis.AddTextMarker(new List<LogEntry> {_entry1, _entry2}, "ME", "My message");
            _analysis.DeleteTextMarker(marker);
            Assert.AreEqual(0, _analysis.TextMarkers.Count);
        }

        [TestMethod]
        public void GetTextMarkersForEntryTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> { _entry1 }, "ME", "My message");
            _analysis.AddTextMarker(new List<LogEntry> { _entry1, _entry2 }, "ME2", "My message2");
            List<TextMarker> tMarkers = _analysis.GetTextMarkersForEntry(_entry1);
            Assert.AreEqual(tMarkers.Count, 2);       
        }

        [TestMethod]
        public void GetTextMarkersForEntriesTest()
        {
            LogEntry entry3 = new LogEntry();
            _analysis.AddTextMarker(new List<LogEntry> { _entry1 }, "ME", "My message");
            _analysis.AddTextMarker(new List<LogEntry> { _entry1, _entry2, entry3 }, "ME2", "My message2");
            _analysis.AddTextMarker(new List<LogEntry> { _entry1, entry3 }, "ME2", "My message2");
            List<TextMarker> tMarkers = _analysis.GetTextMarkersForEntries(new List<LogEntry> { _entry1, _entry2 });
            Assert.AreEqual(3, tMarkers.Count);
        }

        [TestMethod]
        public void ExistMarkerForLogEntryTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> { _entry1 }, "ME", "My message");
            _analysis.AddTextMarker(new List<LogEntry> { _entry1, _entry2 }, "ME2", "My message2");
            LogEntry entry3 = new LogEntry(){App = "lol"};
            Assert.IsTrue(_analysis.ExistTextMarkerForLogEntry(_entry1));
            Assert.IsFalse(_analysis.ExistTextMarkerForLogEntry(entry3));
        }

        [TestMethod]
        public void ExistMarkerForLogEntriesTest()
        {
            _analysis.AddTextMarker(new List<LogEntry> { _entry1 }, "ME", "My message");
            _analysis.AddTextMarker(new List<LogEntry> { _entry1, _entry2 }, "ME2", "My message2");
            LogEntry entry3 = new LogEntry();
            LogEntry entry4 = new LogEntry();
            Assert.IsTrue(_analysis.ExistTextMarkerForLogEntries(new List<LogEntry>(){_entry1, _entry2}));
        }
    }
}

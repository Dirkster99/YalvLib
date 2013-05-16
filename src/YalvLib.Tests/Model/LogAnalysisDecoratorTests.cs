using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using YalvLib.Model;

namespace YalvLib.Tests.Model
{
    [TestFixture]
    public class LogAnalysisDecoratorTests
    {
        [Test]
        public void CreateInstance()
        {
            var analysis = new LogAnalysisDecorator();
            Assert.AreEqual(3, analysis.Markers.Count);
            Assert.IsTrue(analysis.ColorMarkers.Any(x => x.HighlightColor.Equals(Color.Chocolate)));
            Assert.IsTrue(analysis.ColorMarkers.Any(x => x.HighlightColor.Equals(Color.BlueViolet)));
            Assert.IsTrue(analysis.ColorMarkers.Any(x => x.HighlightColor.Equals(Color.CadetBlue)));
        }

        [Test]
        public void IsMultiMarker()
        {
            var analysis = new LogAnalysisDecorator();
            var entry1 = new LogEntry();
            var entry2 = new LogEntry();
            TextMarker marker = analysis.AddTextMarker(new List<LogEntry> {entry1, entry2}, "ME", "My message");
            Assert.IsTrue(analysis.IsMultiMarker(marker));
            marker.RemoveEntry(entry1);
            marker.RemoveEntry(entry2);
            Assert.IsFalse(analysis.IsMultiMarker(marker));
        }

        [Test]
        public void AddTextMarker()
        {
            var analysis = new LogAnalysisDecorator();
            var entry = new LogEntry();
            analysis.AddTextMarker(new List<LogEntry> {entry}, "ME", "My message");
            Assert.AreEqual(1, analysis.TextMarkers.Count);
        }

        [Test]
        public void RemoveTextMarkerFromEntry()
        {
            var analysis = new LogAnalysisDecorator();
            var entry1 = new LogEntry();
            analysis.AddTextMarker(new List<LogEntry> {entry1}, "ME", "My message");
            analysis.RemoveTextMarker(entry1);
            Assert.AreEqual(0, analysis.TextMarkers.Count);
        }

        [Test]
        public void DoNotDeleteTextMarker()
        {
            var analysis = new LogAnalysisDecorator();
            var entry1 = new LogEntry();
            var entry2 = new LogEntry();
            analysis.AddTextMarker(new List<LogEntry> {entry1, entry2}, "ME", "My message");
            analysis.RemoveTextMarker(entry1);
            Assert.AreEqual(1, analysis.TextMarkers.Count);
        }


        [Test]
        public void DeleteTextMarkerFromMarker()
        {
            var analysis = new LogAnalysisDecorator();
            var entry1 = new LogEntry();
            var entry2 = new LogEntry();
            TextMarker marker = analysis.AddTextMarker(new List<LogEntry> {entry1, entry2}, "ME", "My message");
            analysis.DeleteTextMarker(marker);
            Assert.AreEqual(0, analysis.TextMarkers.Count);
        }
    }
}

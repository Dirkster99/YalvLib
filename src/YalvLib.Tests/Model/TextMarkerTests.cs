using System;
using System.Collections.Generic;
using NUnit.Framework;
using YalvLib.Model;

namespace YalvLib.Tests.Model
{
    [TestFixture]
    public class TextMarkerTests
    {
        private LogEntry _logEntry;
        private string _author;
        private string _message;

        [SetUp]
        private void CreateEntry()
        {
            _logEntry = new LogEntry();
            _logEntry.App = "App";
            _logEntry.Class = "Class";
            _logEntry.File = "File";
            _logEntry.HostName = "Host";
            _logEntry.LevelIndex = LevelIndex.ERROR;
            _logEntry.Line = 71;
            _logEntry.Logger = "Logger";
            _logEntry.MachineName = "Machine";
            _logEntry.Message = "Message";
            _logEntry.Method = "Method";
            _logEntry.Thread = "Thread";
            _logEntry.Throwable = "Throw";
            _logEntry.TimeStamp = DateTime.MaxValue;
            _logEntry.UserName = "User";

            _author = "Gwen";
            _message = "Hoo ben gaspard dis pas tout quand meme";
        }

        [Test]
        public void CreateTextMarkerTest()
        {
            var list = new List<LogEntry> { _logEntry };

            var tm = new TextMarker(list, _author, _message);
            Assert.AreEqual(tm.Author, _author);
            Assert.AreEqual(tm.Message, _message);
            Assert.AreEqual(tm.LogEntries, list);
            Assert.AreEqual(tm.LogEntryCount(), list.Count);
            Assert.AreEqual(tm.DateCreation, tm.DateLastModification);
            Assert.AreEqual(tm.LogEntries, list);
        }

        [Test]
        public void EditTextMarkerTest()
        {
            var list = new List<LogEntry> { _logEntry };

            var tm = new TextMarker(list, _author, _message)
                         {Author = "Toto", Message = "On va sur la planete des doudounes quoi!"};

            var dt = new DateTime();
            tm.DateLastModification = dt;

            Assert.AreEqual(tm.Author, "Toto");
            Assert.AreEqual(tm.Message, "On va sur la planete des doudounes quoi!");
            Assert.AreEqual(tm.DateLastModification, dt);
        }
    }
}

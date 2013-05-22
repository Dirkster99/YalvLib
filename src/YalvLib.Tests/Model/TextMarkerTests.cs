using System;
using System.Collections.Generic;
using NUnit.Framework;
using YalvLib.Model;

namespace YalvLib.Tests.Model
{
    [TestFixture]
    public class TextMarkerTests
    {
        private LogEntry CreateEntry()
        {
            var entry = new LogEntry();
            entry.App = "App";
            entry.Class = "Class";
            entry.File = "File";
            entry.HostName = "Host";
            entry.LevelIndex = LevelIndex.ERROR;
            entry.Line = 71;
            entry.Logger = "Logger";
            entry.MachineName = "Machine";
            entry.Message = "Message";
            entry.Method = "Method";
            entry.Thread = "Thread";
            entry.Throwable = "Throw";
            entry.TimeStamp = DateTime.MaxValue;
            entry.UserName = "User";
            return entry;
        }

        [Test]
        public void CreateTextMarkerTest()
        {
            var list = new List<LogEntry> {CreateEntry()};

            string author = "Gwen";
            string message = "Hoo ben gaspard dis pas tout quand meme";

            var tm = new TextMarker(list, author, message);
            Assert.AreEqual(tm.Author, author);
            Assert.AreEqual(tm.Message, message);
            Assert.AreEqual(tm.LogEntries, list);
            Assert.AreEqual(tm.LogEntryCount(), list.Count);
            Assert.AreEqual(tm.DateCreation, tm.DateLastModification);
            Assert.AreEqual(tm.LogEntries, list);
        }

        [Test]
        public void EditTextMarkerTest()
        {
            var list = new List<LogEntry> {CreateEntry()};

            string author = "Gwen";
            string message = "Hoo ben gaspard dis pas tout quand meme";

            var tm = new TextMarker(list, author, message)
                         {Author = "Toto", Message = "On va sur la planete des doudounes quoi!"};

            var dt = new DateTime();
            tm.DateLastModification = dt;

            Assert.AreEqual(tm.Author, "Toto");
            Assert.AreEqual(tm.Message, "On va sur la planete des doudounes quoi!");
            Assert.AreEqual(tm.DateLastModification, dt);
        }
    }
}

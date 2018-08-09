namespace YalvLib.UnitTests.Model
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using YalvLib.Model;

    [TestClass]
    public class TextMarkerTests
    {
        private LogEntry _logEntry;
        private string _author;
        private string _message;

        [TestInitialize]
        public void CreateEntry()
        {
            _logEntry = new LogEntry
                            {
                                App = "App",
                                Class = "Class",
                                File = "File",
                                HostName = "Host",
                                LevelIndex = LevelIndex.ERROR,
                                Line = 71,
                                Logger = "Logger",
                                MachineName = "Machine",
                                Message = "Message",
                                Method = "Method",
                                Thread = "Thread",
                                Throwable = "Throw",
                                TimeStamp = DateTime.MaxValue,
                                UserName = "User"
                            };

            _author = "Gwen";
            _message = "Hoo ben gaspard dis pas tout quand meme";
        }

        [TestMethod]
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

        [TestMethod]
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

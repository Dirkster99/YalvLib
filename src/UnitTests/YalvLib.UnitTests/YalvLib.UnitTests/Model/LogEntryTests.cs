﻿namespace YalvLib.UnitTests.Model
{
    using log4netLib.Enums;
    using YalvLib.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class LogEntryTests
    {
        [TestMethod]
        public void CopyConstructor()
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
            var copy = new LogEntry(entry);
            Assert.AreEqual(entry.App, copy.App);
            Assert.AreEqual(entry.Class, copy.Class);
            Assert.AreEqual(entry.File, copy.File);
            Assert.AreEqual(entry.HostName, copy.HostName);
            Assert.AreEqual(entry.LevelIndex, copy.LevelIndex);
            Assert.AreEqual(entry.Line, copy.Line);
            Assert.AreEqual(entry.Logger, copy.Logger);
            Assert.AreEqual(entry.MachineName, copy.MachineName);
            Assert.AreEqual(entry.Message, copy.Message);
            Assert.AreEqual(entry.Method, copy.Method);
            Assert.AreEqual(entry.Thread, copy.Thread);
            Assert.AreEqual(entry.Throwable, copy.Throwable);
            Assert.AreEqual(entry.TimeStamp, copy.TimeStamp);
            Assert.AreEqual(entry.UserName, copy.UserName);
        }
    }
}

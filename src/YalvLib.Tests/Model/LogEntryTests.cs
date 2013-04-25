using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using YalvLib.Model;

namespace YalvLib.Tests.Model
{

    [TestFixture]
    public class LogEntryTests
    {

        [Test]
        public void LevelIndex_Error()
        {
            LogEntry entry = new LogEntry();
            entry.Level = "Error";
            Assert.AreEqual(LevelIndex.ERROR, entry.LevelIndex);
            entry.Level = "ERRor";
            Assert.AreEqual(LevelIndex.ERROR, entry.LevelIndex);
        }

        [Test]
        public void LevelIndex_Debug()
        {
            LogEntry entry = new LogEntry();
            entry.Level = "Debug";
            Assert.AreEqual(LevelIndex.DEBUG, entry.LevelIndex);
            entry.Level = "DEBug";
            Assert.AreEqual(LevelIndex.DEBUG, entry.LevelIndex);
        }

        [Test]
        public void LevelIndex_Fatal()
        {
            LogEntry entry = new LogEntry();
            entry.Level = "Fatal";
            Assert.AreEqual(LevelIndex.FATAL, entry.LevelIndex);
            entry.Level = "FATal";
            Assert.AreEqual(LevelIndex.FATAL, entry.LevelIndex);
        }

        [Test]
        public void LevelIndex_Info()
        {
            LogEntry entry = new LogEntry();
            entry.Level = "Info";
            Assert.AreEqual(LevelIndex.INFO, entry.LevelIndex);
            entry.Level = "INFo";
            Assert.AreEqual(LevelIndex.INFO, entry.LevelIndex);
        }

        [Test]
        public void LevelIndex_Warn()
        {
            LogEntry entry = new LogEntry();
            entry.Level = "Warn";
            Assert.AreEqual(LevelIndex.WARN, entry.LevelIndex);
            entry.Level = "WArn";
            Assert.AreEqual(LevelIndex.WARN, entry.LevelIndex);
        }

        [Test]
        public void LevelIndex_None()
        {
            LogEntry entry = new LogEntry();
            entry.Level = "CannotBeParsed";
            Assert.AreEqual(LevelIndex.NONE, entry.LevelIndex);
        } 

    }

}

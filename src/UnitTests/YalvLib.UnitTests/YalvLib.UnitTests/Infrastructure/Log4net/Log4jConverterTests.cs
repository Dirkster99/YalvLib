﻿namespace YalvLib.UnitTests.Infrastructure.Log4Net
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Linq;
    using YalvLib.Infrastructure.Log4Net;
    using System.Globalization;
    using System.Threading;
    using log4netLib.Enums;
    using YalvLib.Model;

    [TestClass]
    public class Log4jConverterTests
    {
        [TestMethod]
        public void LogEntry2Event()
        {
            // Make sure cutlure of test is fixed to ensure correct DateTime String
            // conversion and interpretation of expected value below
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            LogEntry entry = TestDataProvider.CreateLogEntry();
            Event e = Log4jConverter.Convert(entry);
            Assert.AreEqual("ERROR", e.Level);
            Assert.AreEqual("This is an error message!", e.Message);
            Assert.AreEqual("YALV.Samples.vshost.exe", e.Properties.First(x => x.Name.Equals(Log4jConverter.AppKey)).Value);
            Assert.AreEqual("YALV.Samples.MainWindow", e.LocationInfo.Class);
            Assert.AreEqual(@"c:\Workspace\YalvLib\src\YALV.Samples\MainWindow.xaml.cs", e.LocationInfo.File);
            Assert.AreEqual("tongbong-PC", e.Properties.First(x => x.Name.Equals(Log4jConverter.HostKey)).Value);
            Assert.AreEqual("76", e.LocationInfo.Line);
            Assert.AreEqual("YALV.Samples.LogService", e.Logger);
            Assert.AreEqual("tongbong-PC", e.Properties.First(x => x.Name.Equals(Log4jConverter.MachineKey)).Value);
            Assert.AreEqual("method4", e.LocationInfo.Method);
            Assert.AreEqual("10", e.Thread);
            Assert.AreEqual("System.Exception: Warning Exception!", e.Throwable);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 1).TotalMilliseconds.ToString(), e.Timestamp);
            Assert.AreEqual("tongbong-PC\tongbong", e.Properties.First(x => x.Name.Equals(Log4jConverter.UserKey)).Value);
        }

        [TestMethod]
        public void Event2LogEntry()
        {
            Event e = TestDataProvider.CreateLog4jEvent("ERROR");
            LogEntry logEntry = Log4jConverter.Convert(e);

            Assert.AreEqual(LevelIndex.ERROR, logEntry.LevelIndex);
            Assert.AreEqual("This is an error message!", logEntry.Message);
            Assert.AreEqual("YALV.Samples.vshost.exe", logEntry.App);
            Assert.AreEqual("YALV.Samples.MainWindow", logEntry.Class);
            Assert.AreEqual(@"c:\Workspace\YalvLib\src\YALV.Samples\MainWindow.xaml.cs", logEntry.File);
            Assert.AreEqual("tongbong-PC", logEntry.HostName);
            Assert.AreEqual((uint)76, logEntry.Line);
            Assert.AreEqual("YALV.Samples.LogService", logEntry.Logger);
            Assert.AreEqual("tongbong-PC", logEntry.MachineName);
            Assert.AreEqual("method4", logEntry.Method);
            Assert.AreEqual("10", logEntry.Thread);
            Assert.AreEqual("System.Exception: Warning Exception!", logEntry.Throwable);

            var timeZone = TimeZoneInfo.Local;  // Make sure this works for all time zones around the globe
            var timeZoneSpan = timeZone.BaseUtcOffset;
            var testTime = new DateTime(1970, 1, 2, 1, 1, 1);
            testTime = testTime.Add(timeZoneSpan);
            Assert.AreEqual(testTime, logEntry.TimeStamp);

            Assert.AreEqual("tongbong-PC\tongbong", logEntry.UserName);
        }

        [TestMethod]
        public void LevelIndex_Error()
        {
            Event e = TestDataProvider.CreateLog4jEvent("Error");
            LogEntry logEntry = Log4jConverter.Convert(e);
            Assert.AreEqual(LevelIndex.ERROR, logEntry.LevelIndex);

            e = TestDataProvider.CreateLog4jEvent("ERror");
            logEntry = Log4jConverter.Convert(e);
            Assert.AreEqual(LevelIndex.ERROR, logEntry.LevelIndex);
        }

        [TestMethod]
        public void LevelIndex_Debug()
        {
            Event e = TestDataProvider.CreateLog4jEvent("Debug");
            LogEntry logEntry = Log4jConverter.Convert(e);
            Assert.AreEqual(LevelIndex.DEBUG, logEntry.LevelIndex);

            e = TestDataProvider.CreateLog4jEvent("DEbug");
            logEntry = Log4jConverter.Convert(e);
            Assert.AreEqual(LevelIndex.DEBUG, logEntry.LevelIndex);
        }

        [TestMethod]
        public void LevelIndex_Fatal()
        {
            Event e = TestDataProvider.CreateLog4jEvent("Fatal");
            LogEntry logEntry = Log4jConverter.Convert(e);
            Assert.AreEqual(LevelIndex.FATAL, logEntry.LevelIndex);

            e = TestDataProvider.CreateLog4jEvent("FAtal");
            logEntry = Log4jConverter.Convert(e);
            Assert.AreEqual(LevelIndex.FATAL, logEntry.LevelIndex);
        }

        [TestMethod]
        public void LevelIndex_Info()
        {
            Event e = TestDataProvider.CreateLog4jEvent("Info");
            LogEntry logEntry = Log4jConverter.Convert(e);
            Assert.AreEqual(LevelIndex.INFO, logEntry.LevelIndex);

            e = TestDataProvider.CreateLog4jEvent("INfo");
            logEntry = Log4jConverter.Convert(e);
            Assert.AreEqual(LevelIndex.INFO, logEntry.LevelIndex);
        }

        [TestMethod]
        public void LevelIndex_Warn()
        {
            Event e = TestDataProvider.CreateLog4jEvent("Warn");
            LogEntry logEntry = Log4jConverter.Convert(e);
            Assert.AreEqual(LevelIndex.WARN, logEntry.LevelIndex);

            e = TestDataProvider.CreateLog4jEvent("WArn");
            logEntry = Log4jConverter.Convert(e);
            Assert.AreEqual(LevelIndex.WARN, logEntry.LevelIndex);
        }

        [TestMethod]
        public void LevelIndex_None()
        {
            Event e = TestDataProvider.CreateLog4jEvent("cannot be parsed");
            LogEntry logEntry = Log4jConverter.Convert(e);
            Assert.AreEqual(LevelIndex.NONE, logEntry.LevelIndex);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using YalvLib.Infrastructure.Log4Net;
using YalvLib.Model;

namespace YalvLib.Tests.Infrastructure.Log4net
{

    [TestFixture]
    public class Event2LogEntryTests
    {

        [Test]
        public void Test1()
        {
            Event e = TestDataProvider.CreateLog4jEvent("ERROR");
            LogEntry logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.ERROR, logEntry.LevelIndex);
            Assert.AreEqual("This is an error message!", logEntry.Message);
            Assert.AreEqual("YALV.Samples.vshost.exe", logEntry.App);
            Assert.AreEqual("YALV.Samples.MainWindow", logEntry.Class);
            Assert.AreEqual(@"c:\Workspace\YalvLib\src\YALV.Samples\MainWindow.xaml.cs", logEntry.File);
            Assert.AreEqual("tongbong-PC", logEntry.HostName);
            Assert.AreEqual(76, logEntry.Line);
            Assert.AreEqual("YALV.Samples.LogService", logEntry.Logger);
            Assert.AreEqual("tongbong-PC", logEntry.MachineName);
            Assert.AreEqual("method4", logEntry.Method);
            Assert.AreEqual("10", logEntry.Thread);
            Assert.AreEqual("System.Exception: Warning Exception!", logEntry.Throwable);
            Assert.AreEqual((DateTime.MinValue + new TimeSpan(1, 1, 1, 1)).ToLocalTime(), logEntry.TimeStamp);
            Assert.AreEqual("tongbong-PC\tongbong", logEntry.UserName);
        }

        [Test]
        public void LevelIndex_Error()
        {
            Event e = TestDataProvider.CreateLog4jEvent("Error");
            LogEntry logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.ERROR, logEntry.LevelIndex);

            e = TestDataProvider.CreateLog4jEvent("ERror");
            logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.ERROR, logEntry.LevelIndex);
        }

        [Test]
        public void LevelIndex_Debug()
        {
            Event e = TestDataProvider.CreateLog4jEvent("Debug");
            LogEntry logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.DEBUG, logEntry.LevelIndex);

            e = TestDataProvider.CreateLog4jEvent("DEbug");
            logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.DEBUG, logEntry.LevelIndex);
        }

        [Test]
        public void LevelIndex_Fatal()
        {
            Event e = TestDataProvider.CreateLog4jEvent("Fatal");
            LogEntry logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.FATAL, logEntry.LevelIndex);

            e = TestDataProvider.CreateLog4jEvent("FAtal");
            logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.FATAL, logEntry.LevelIndex);
        }

        [Test]
        public void LevelIndex_Info()
        {
            Event e = TestDataProvider.CreateLog4jEvent("Info");
            LogEntry logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.INFO, logEntry.LevelIndex);

            e = TestDataProvider.CreateLog4jEvent("INfo");
            logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.INFO, logEntry.LevelIndex);
        }

        [Test]
        public void LevelIndex_Warn()
        {
            Event e = TestDataProvider.CreateLog4jEvent("Warn");
            LogEntry logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.WARN, logEntry.LevelIndex);

            e = TestDataProvider.CreateLog4jEvent("WArn");
            logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.WARN, logEntry.LevelIndex);
        }

        [Test]
        public void LevelIndex_None()
        {
            Event e = TestDataProvider.CreateLog4jEvent("cannot be parsed");
            LogEntry logEntry = Event2LogEntry.Convert(e);
            Assert.AreEqual(LevelIndex.NONE, logEntry.LevelIndex);
        } 

    }

}

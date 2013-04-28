using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YalvLib.Infrastructure.Log4Net;
using YalvLib.Model;

namespace YalvLib.Tests.Infrastructure.Log4net
{

    public class TestDataProvider
    {

        public static Event CreateLog4jEvent(String level)
        {
            Event e = new Event();
            e.Level = level;
            e.Logger = "YALV.Samples.LogService";
            e.Message = "This is an error message!";
            e.Thread = "10";
            e.Throwable = "System.Exception: Warning Exception!";
            e.LocationInfo = new LocationInfo();
            e.LocationInfo.Class = "YALV.Samples.MainWindow";
            e.LocationInfo.File = @"c:\Workspace\YalvLib\src\YALV.Samples\MainWindow.xaml.cs";
            e.LocationInfo.Line = "76";
            e.LocationInfo.Method = "method4";
            e.Timestamp = "90061000";                           
            e.Properties.Add(new Data()
            {
                Name = "SampleDate",
                Value = "20130420"
            });
            e.Properties.Add(new Data()
            {
                Name = "log4japp",
                Value = "YALV.Samples.vshost.exe"
            });
            e.Properties.Add(new Data()
            {
                Name = "log4net:UserName",
                Value = "tongbong-PC\tongbong"
            });
            e.Properties.Add(new Data()
            {
                Name = "log4jmachinename",
                Value = "tongbong-PC"
            });
            e.Properties.Add(new Data()
            {
                Name = "log4net:HostName",
                Value = "tongbong-PC"
            });
            return e;
        }

        public static LogEntry CreateLogEntry()
        {
            LogEntry entry = new LogEntry();
            entry.LevelIndex = LevelIndex.ERROR;
            entry.Logger = "YALV.Samples.LogService";
            entry.Message = "This is an error message!";
            entry.Thread = "10";
            entry.Throwable = "System.Exception: Warning Exception!";
            entry.Class = "YALV.Samples.MainWindow";
            entry.File = @"c:\Workspace\YalvLib\src\YALV.Samples\MainWindow.xaml.cs";
            entry.Line = 76;
            entry.Method = "method4";
            entry.TimeStamp = DateTime.MinValue + new TimeSpan(0, 0, 0, 1);
            entry.App = "YALV.Samples.vshost.exe";
            entry.UserName = "tongbong-PC\tongbong";
            entry.MachineName = "tongbong-PC";
            entry.HostName = "tongbong-PC";
            return entry;
        }
    }

}

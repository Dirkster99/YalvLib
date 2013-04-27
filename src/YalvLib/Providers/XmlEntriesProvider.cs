using System.Xml.Linq;
using System.Xml.Serialization;
using YalvLib.Infrastructure;
using YalvLib.Infrastructure.Log4Net;
using YalvLib.Model;

namespace YalvLib.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using YalvLib.Common;
    using YalvLib.Domain;
    using YalvLib.ViewModel;

    internal class XmlEntriesProvider : AbstractEntriesProviderBase
    {

        private const string log4jNs = "http://jakarta.apache.org/log4j";

        public static Event Deserialize(XmlReader reader)
        {
            return ((Event)(GetXmlSerializer().Deserialize(reader)));
        }

        private static XmlSerializer _xmlSerializerSingleton;
        private static XmlSerializer GetXmlSerializer()
        {
            if (_xmlSerializerSingleton == null)
            {
                var rootAttribute = CreateRootAttribute();
                _xmlSerializerSingleton = new XmlSerializer(typeof(Event), rootAttribute);                            
            }
            return _xmlSerializerSingleton;
        }

        private static XmlRootAttribute CreateRootAttribute()
        {
            var rootAttribute = new XmlRootAttribute();
            rootAttribute.ElementName = "event";
            rootAttribute.Namespace = log4jNs;
            rootAttribute.IsNullable = true;
            return rootAttribute;
        }

        //private LogEntry ParseLogEntry(XElement eventEl)
        //{
        //    LogEntry entry = new LogEntry();

        //    entry.Logger = (string)eventEl.Attribute("logger");
        //    DateTime actualDateTime = _originalDateTime.AddMilliseconds((Double)eventEl.Attribute("timestamp")).ToLocalTime();
        //    entry.TimeStamp = actualDateTime;
        //    entry.Delta = GlobalHelper.GetTimeDelta(_previousDateTime, entry.TimeStamp);
        //    _previousDateTime = actualDateTime;
        //    entry.Level = (String)eventEl.Attribute("level");
        //    entry.Thread = (String)eventEl.Attribute("thread");
        //    entry.Throwable = (String)eventEl.Element("throwable");
        //    entry.Message = (String)eventEl.Element("message");

        //    // parsing location info
        //    XElement locationInfoElement = eventEl.Element("locationInfo");
        //    entry.Class = (String)locationInfoElement.Attribute("class");
        //    entry.Method = (String)locationInfoElement.Attribute("method");
        //    entry.File = (String)locationInfoElement.Attribute("file");
        //    entry.Line = (String)locationInfoElement.Attribute("line");

        //    // parsing properties
        //    XElement propertiesELement = eventEl.Element("properties");
        //    foreach (var property in propertiesELement.Elements())
        //    {
        //        entry.AddProperty((String)property.Attribute("key"),
        //                          (String)property.Attribute("value"));
        //    }
        //    return entry;
        //}


        public override IEnumerable<LogEntry> GetEntries(string dataSource, FilterParams filter)
        {
            List<LogEntry> entries = new List<LogEntry>();

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Fragment
            };
            NameTable nt = new NameTable();
            XmlNamespaceManager mgr = new XmlNamespaceManager(nt);
            mgr.AddNamespace("log4j", log4jNs);

            XmlParserContext pc = new XmlParserContext(nt, mgr, "", XmlSpace.Default);

            using (XmlReader xr = XmlReader.Create(dataSource, settings, pc))
            {
                UInt32 logEntryId = 1;
                DateTime prevTimeStamp = DateTime.MinValue;
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element && xr.LocalName == "event")
                    {
                        Event log4jEvent = Deserialize(xr);
                        LogEntry entry = Event2LogEntry.Convert(log4jEvent);
                        entry.Id = logEntryId;
                        entry.Delta = GlobalHelper.GetTimeDelta(prevTimeStamp, entry.TimeStamp);
                        prevTimeStamp = entry.TimeStamp;
                        entries.Add(entry);
                    }
                }
            }
            return entries;
            //var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
            //var nt = new NameTable();
            //var mgr = new XmlNamespaceManager(nt);
            //mgr.AddNamespace("log4j", GlobalHelper.LAYOUT_LOG4J);
            //var pc = new XmlParserContext(nt, mgr, string.Empty, XmlSpace.Default);
            //var date = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            //using (var stream = new FileStream(dataSource, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            //{
            //  using (var reader = new StreamReader(stream, System.Text.Encoding.Default, true))
            //  {
            //    using (var xmlTextReader = XmlReader.Create(reader, settings, pc))
            //    {
            //      var entryId = 1;
            //      DateTime? prevTimeStamp = null;
            //      while (xmlTextReader.Read())
            //      {
            //        if ((xmlTextReader.NodeType != XmlNodeType.Element) || (xmlTextReader.Name != "log4j:event"))
            //          continue;

            //        var entry = new LogEntry { Id = entryId, Path = dataSource };

            //        entry.Logger = xmlTextReader.GetAttribute("logger");

            //        entry.TimeStamp = date.AddMilliseconds(Convert.ToDouble(xmlTextReader.GetAttribute("timestamp"))).ToLocalTime();
            //        entry.Delta = prevTimeStamp.HasValue ? GlobalHelper.GetTimeDelta(prevTimeStamp.Value, entry.TimeStamp) : "-";
            //        prevTimeStamp = entry.TimeStamp;

            //        entry.Level = xmlTextReader.GetAttribute("level");
            //        entry.Thread = xmlTextReader.GetAttribute("thread");

            //        while (xmlTextReader.Read())
            //        {
            //          var breakLoop = false;
            //          switch (xmlTextReader.Name)
            //          {
            //            case "log4j:event":
            //              breakLoop = true;
            //              break;
            //            default:
            //              switch (xmlTextReader.Name)
            //              {
            //                case ("log4j:message"):
            //                  entry.Message = xmlTextReader.ReadString();
            //                  break;
            //                case ("log4j:data"):
            //                  switch (xmlTextReader.GetAttribute("name"))
            //                  {
            //                    case ("log4net:UserName"):
            //                      entry.UserName = xmlTextReader.GetAttribute("value");
            //                      break;
            //                    case ("log4japp"):
            //                      entry.App = xmlTextReader.GetAttribute("value");
            //                      break;
            //                    case ("log4jmachinename"):
            //                      entry.MachineName = xmlTextReader.GetAttribute("value");
            //                      break;
            //                    case ("log4net:HostName"):
            //                      entry.HostName = xmlTextReader.GetAttribute("value");
            //                      break;
            //                  }

            //                  break;
            //                case ("log4j:throwable"):
            //                  entry.Throwable = xmlTextReader.ReadString();
            //                  break;

            //                case ("log4j:locationInfo"):
            //                  entry.Class = xmlTextReader.GetAttribute("class");
            //                  entry.Method = xmlTextReader.GetAttribute("method");
            //                  entry.File = xmlTextReader.GetAttribute("file");
            //                  entry.Line = xmlTextReader.GetAttribute("line");
            //                  break;
            //              }

            //              break;
            //          }

            //          if (breakLoop)
            //            break;
            //        }

            //        if (filterByParameters(entry, filter))
            //        {
            //          yield return entry;
            //          entryId++;
            //        }
            //      }
            //    }
            //  }
            //}
        }

        //private static bool filterByParameters(LogEntry entry, FilterParams parameters)
        //{
        //  if (entry == null)
        //    throw new ArgumentNullException("entry");

        //  if (parameters == null)
        //    throw new ArgumentNullException("parameters");

        //  bool accept = false;
        //  switch (parameters.Level)
        //  {
        //    case 1:
        //      if (string.Equals(entry.Level, "ERROR",
        //          StringComparison.InvariantCultureIgnoreCase))
        //        accept = true;
        //      break;

        //    case 2:
        //      if (string.Equals(entry.Level, "INFO",
        //          StringComparison.InvariantCultureIgnoreCase))
        //        accept = true;
        //      break;

        //    case 3:
        //      if (string.Equals(entry.Level, "DEBUG",
        //          StringComparison.InvariantCultureIgnoreCase))
        //        accept = true;
        //      break;

        //    case 4:
        //      if (string.Equals(entry.Level, "WARN",
        //          StringComparison.InvariantCultureIgnoreCase))
        //        accept = true;
        //      break;

        //    case 5:
        //      if (string.Equals(entry.Level, "FATAL",
        //          StringComparison.InvariantCultureIgnoreCase))
        //        accept = true;
        //      break;

        //    default:
        //      accept = true;
        //      break;
        //  }

        //  if (parameters.Date.HasValue)
        //    if (entry.TimeStamp < parameters.Date)
        //      accept = false;

        //  if (!string.IsNullOrEmpty(parameters.Thread))
        //    if (!string.Equals(entry.Thread, parameters.Thread, StringComparison.InvariantCultureIgnoreCase))
        //      accept = false;

        //  if (!string.IsNullOrEmpty(parameters.Message))
        //    if (!entry.Message.ToUpper().Contains(parameters.Message.ToUpper()))
        //      accept = false;

        //  if (!string.IsNullOrEmpty(parameters.Logger))
        //    if (!entry.Logger.ToUpper().Contains(parameters.Logger.ToUpper()))
        //      accept = false;

        //  return accept;
        //}
    }
}
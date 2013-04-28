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

        private const string Log4jNs = "http://jakarta.apache.org/log4j";

        public override IEnumerable<LogEntry> GetEntries(string dataSource, FilterParams filter)
        {
            List<LogEntry> entries = new List<LogEntry>();

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Fragment
            };
            NameTable nt = new NameTable();
            XmlNamespaceManager mgr = new XmlNamespaceManager(nt);
            mgr.AddNamespace("log4j", Log4jNs);

            XmlParserContext pc = new XmlParserContext(nt, mgr, "", XmlSpace.Default);

            using (XmlReader xr = XmlReader.Create(dataSource, settings, pc))
            {
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element && xr.LocalName == "event")
                    {
                        Event log4jEvent = Deserialize(xr);
                        LogEntry entry = Event2LogEntry.Convert(log4jEvent);
                        entries.Add(entry);
                        // [FT] We may need that in the future.
                        //        if (filterByParameters(entry, filter))
                        //        {
                        //          yield return entry;
                        //          entryId++;
                        //        }
                    }
                }
            }
            return entries;
        }

        private static Event Deserialize(XmlReader reader)
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
            rootAttribute.Namespace = Log4jNs;
            rootAttribute.IsNullable = true;
            return rootAttribute;
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
﻿namespace YalvLib.Providers
{
    using System.Text;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using YalvLib.Exceptions;
    using YalvLib.Infrastructure.Log4Net;
    using YalvLib.Model;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using YalvLib.Domain;

    /// <summary>
    /// Data provider to read log data items from log4net Xml standard format.
    /// </summary>
    internal class XmlEntriesProvider : AbstractEntriesProviderBase
    {
        #region fields
        private const string Log4jNs = "http://jakarta.apache.org/log4j";
        private readonly List<string> _xmlParserMess = new List<string>();
        #endregion fields

        #region methods
        /// <summary>
        /// Get collection of logitems representing a log file from Xml data source.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override IEnumerable<LogEntry> GetEntries(
            string dataSource,
            FilterParams filter)
        {
            List<LogEntry> entries = new List<LogEntry>();
            LogEntry entry;

            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Fragment
            };
            settings.ValidationEventHandler += settings_ValidationEventHandler;
            NameTable nt = new NameTable();
            XmlNamespaceManager mgr = new XmlNamespaceManager(nt);
            mgr.AddNamespace("log4j", Log4jNs);

            XmlParserContext pc = new XmlParserContext(nt, mgr, "", XmlSpace.Default);


            Encoding fileEncoding = Encoding.Default;

            using (FileStream fs = new FileStream(dataSource, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader reader = DetectEncoding.OpenStream(fs, Encoding.UTF8))
                {
                    // assign encoding after ReadToEnd() so that the StreamReader can autodetect the encoding
                    fileEncoding = reader.CurrentEncoding;
                }
            }

            // Specifying encoding for the xml files
            // Still having some display issues on the dataGrid, special characters are not well displayed on it.
            pc.Encoding = fileEncoding;

            using (XmlReader xr = XmlReader.Create(dataSource, settings, pc))
            {
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element && xr.LocalName == "event")
                    {
                        Event log4jEvent = Deserialize(xr);
                        try
                        {
                            entry = Event2LogEntry.Convert(log4jEvent);
                        }
                        catch (Exception ex)
                        {
                            IXmlLineInfo xmlInfo = (IXmlLineInfo)xr;
                            throw new Exception(string.Format("Error parsing file {0}, on line number {1}, on position {2}", dataSource, xmlInfo.LineNumber, xmlInfo.LinePosition), ex);
                        }

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
            settings.ValidationEventHandler -= settings_ValidationEventHandler;
            if (entries.Count == 0)
            {
                throw new NotValidValueException("No valid entries were found. Please check your data file.");
            }
            return entries;
        }

        /// <summary>
        /// Implements a simple way for returning meaningful messages to the user.
        /// warnings or errors.
        /// </summary>
        /// <returns></returns>
        public override List<string> GetMessages()
        {
            return _xmlParserMess;
        }

        private void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Warning:
                    _xmlParserMess.Add(string.Format("Warning : line {0}, Position {1} \n {2}",
                                        e.Exception.LineNumber, e.Exception.LinePosition, e.Exception.Message));
                    break;

                case XmlSeverityType.Error:
                    _xmlParserMess.Add(string.Format("Error : line {0}, Position {1} \n {2}",
                                        e.Exception.LineNumber, e.Exception.LinePosition, e.Exception.Message));
                    break;

                default:
                    _xmlParserMess.Add(string.Format("Unhandled severity type : {0} {1}",
                                       e.Severity, e.Exception.Message));
                    break;
            }
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
            var rootAttribute = new XmlRootAttribute { ElementName = "event", Namespace = Log4jNs, IsNullable = true };
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
        #endregion methods
    }
}
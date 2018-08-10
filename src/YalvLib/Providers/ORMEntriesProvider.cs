namespace YalvLib.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using YalvLib.Infrastructure;
    using YalvLib.Model;
    using YalvLib.Domain;

    /// <summary>
    /// Base class for file based log file providers. 
    /// </summary>
    public abstract class ORMEntriesProvider : AbstractEntriesProviderBase
    {
        /// <summary>
        /// Minimum date available for this class and its inherating classes
        /// </summary>
        protected static readonly DateTime MinDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// Get log file entries from a datasource with an applied filter in <paramref name="filter"/>.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override IEnumerable<LogEntry> GetEntries(string dataSource, FilterParams filter)
        {
            IEnumerable<LogEntry> enumerable = this.InternalGetEntries(dataSource, filter);

            // avoid file locks
            return enumerable.ToArray();
        }

        /// <summary>
        /// Create a database connection based on a string connection descriptor.
        /// </summary>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        protected abstract IDbConnection CreateConnection(string dataSource);

        private static void AddLevelClause(IDbCommand command, string level)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (string.IsNullOrEmpty(level))
                throw new ArgumentNullException("level");

            command.CommandText += @" and level = @level";

            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@level";
            parameter.Value = level;
            command.Parameters.Add(parameter);
        }

        private static void AddLoggerClause(IDbCommand command, string logger)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (string.IsNullOrEmpty(logger))
                return;

            command.CommandText += @" and logger like @logger";

            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@logger";
            parameter.Value = string.Format("%{0}%", logger);
            command.Parameters.Add(parameter);
        }

        private static void AddThreadClause(IDbCommand command, string thread)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (string.IsNullOrEmpty(thread))
                return;

            command.CommandText += @" and thread like @thread";

            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@thread";
            parameter.Value = string.Format("%{0}%", thread);
            command.Parameters.Add(parameter);
        }

        private static void AddMessageClause(IDbCommand command, string message)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (string.IsNullOrEmpty(message))
                return;

            command.CommandText += @" and message like @message";

            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@message";
            parameter.Value = string.Format("%{0}%", message);
            command.Parameters.Add(parameter);
        }

        private static void AddOrderByClause(IDbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            command.CommandText += @" order by date ";
        }

        private static string GetValue(string item, string key)
        {
            return string.IsNullOrEmpty(item) ? string.Empty : item.Remove(0, key.Length);
        }

        private static string Find(IEnumerable<string> items, string key)
        {
            return items.Where(i => i.StartsWith(key)).SingleOrDefault();
        }

        private IEnumerable<LogEntry> InternalGetEntries(string dataSource, FilterParams filter)
        {
            using (IDbConnection connection = this.CreateConnection(dataSource))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText =
                            @"select caller, date, level, logger, thread, message, exception from log where date >= @date";

                        IDbDataParameter parameter = command.CreateParameter();
                        parameter.ParameterName = "@date";
                        parameter.Value = filter.Date.HasValue ? filter.Date.Value : MinDateTime;
                        command.Parameters.Add(parameter);

                        switch (filter.Level)
                        {
                            case 1:
                                AddLevelClause(command, "ERROR");
                                break;

                            case 2:
                                AddLevelClause(command, "INFO");
                                break;

                            case 3:
                                AddLevelClause(command, "DEBUG");
                                break;

                            case 4:
                                AddLevelClause(command, "WARN");
                                break;

                            case 5:
                                AddLevelClause(command, "FATAL");
                                break;

                            default:
                                break;
                        }

                        AddLoggerClause(command, filter.Logger);
                        AddThreadClause(command, filter.Thread);
                        AddMessageClause(command, filter.Message);

                        AddOrderByClause(command);

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string caller = "";
                                try
                                {
                                    reader.GetString(0);
                                }
                                catch
                                {
                                    // [FT] catching exception because when using sqlite, caller is empty text
                                    // and GetString() method raises an exception.
                                }

                                string[] split = caller.Split(',');

                                const string MachineKey = "{log4jmachinename=";
                                string item0 = Find(split, MachineKey);
                                string machineName = GetValue(item0, MachineKey);

                                const string HostKey = " log4net:HostName=";
                                string item1 = Find(split, HostKey);
                                string hostName = GetValue(item1, HostKey);

                                const string UserKey = " log4net:UserName=";
                                string item2 = Find(split, UserKey);
                                string userName = GetValue(item2, UserKey);

                                const string AppKey = " log4japp=";
                                string item3 = Find(split, AppKey);
                                string app = GetValue(item3, AppKey);

                                DateTime timeStamp = reader.GetDateTime(1);
                                string level = reader.GetString(2);
                                string logger = reader.GetString(3);
                                string thread = reader.GetString(4);
                                string message = reader.GetString(5);
                                string exception = reader.GetString(6);

                                LogEntry entry = new LogEntry
                                {
                                    TimeStamp = timeStamp,
                                    LevelIndex = LevelConverter.From(level),
                                    Thread = thread,
                                    Logger = logger,
                                    Message = message,
                                    Throwable = exception,
                                    MachineName = machineName,
                                    HostName = hostName,
                                    UserName = userName,
                                    App = app,
                                };

                                // TODO: altri filtri
                                yield return entry;
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
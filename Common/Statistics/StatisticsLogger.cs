#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System.IO;
using System.Text;
using System.Xml;

namespace ClearCanvas.Common.Statistics
{
    /// <summary>
    /// Provides statistics logging mechanism.
    /// </summary>
    public class StatisticsLogger
    {
        private static readonly XmlDocument _doc = new XmlDocument();
        private static readonly object[] _extensions;
        private static readonly StatisticsLoggerExtensionPoint _xp = new StatisticsLoggerExtensionPoint();

        static StatisticsLogger()
        {
            try
            {
                _extensions = _xp.CreateExtensions();
            }
            catch (PluginException)
            {
                _extensions = new object[0];
            }
        }

        private static string GetLogString(StatisticsSet statistics, bool recursive)
        {
            XmlElement el = statistics.GetXmlElement(_doc, recursive);

            using (var sw = new StringWriter())
            {
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineOnAttributes = false,
                    OmitXmlDeclaration = true,
                    Encoding = Encoding.UTF8
                };

                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    el.WriteTo(writer);
                    writer.Flush();

                    return sw.ToString();
                }
            }
        }

        /// <summary>
        /// Logs a statistics.
        /// </summary>
        /// <param name="logName">The name of the target log.</param>
        /// <param name="level">The log level used for logging the statistics</param>
        /// <param name="recursive">Bool telling if the log should be recursive, or just display averages.</param>
        /// <param name="statistics">The statistics to be logged</param>
        public static void Log(string logName, LogLevel level, bool recursive, StatisticsSet statistics)
        {
            Platform.Log(logName, level, GetLogString(statistics, recursive));

            foreach (IStatisticsLoggerListener extension in _extensions)
                extension.OnStatisticsLogged(statistics);
        }

        /// <summary>
        /// Logs a statistics.
        /// </summary>
        /// <param name="level">The log level used for logging the statistics</param>
        /// <param name="recursive">Bool telling if the log should be recursive, or just display averages.</param>
        /// <param name="statistics">The statistics to be logged</param>
        public static void Log(LogLevel level, bool recursive, StatisticsSet statistics)
        {
            Platform.Log(level, GetLogString(statistics, recursive));

            foreach (IStatisticsLoggerListener extension in _extensions)
                extension.OnStatisticsLogged(statistics);
        }

        /// <summary>
        /// Logs a statistics.
        /// </summary>
        /// <param name="level">The log level used for logging the statistics</param>
        /// <param name="statistics">The statistics to be logged</param>
        public static void Log(LogLevel level, StatisticsSet statistics)
        {
            Log(level, true, statistics);
        }

        /// <summary>
        /// Logs a statistics.
        /// </summary>
        /// <param name="logName">The name of the target log.</param>
        /// <param name="level">The log level used for logging the statistics</param>
        /// <param name="statistics">The statistics to be logged</param>
        public static void Log(string logName, LogLevel level, StatisticsSet statistics)
        {
            Log(logName, level, true, statistics);
        }
    }
}
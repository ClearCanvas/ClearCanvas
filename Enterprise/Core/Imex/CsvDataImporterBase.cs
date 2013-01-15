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

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Abstract base class for classes that implement <see cref="ICsvDataImporter"/>.
    /// </summary>
    public abstract class CsvDataImporterBase : ICsvDataImporter, IApplicationRoot
    {
        private const int DEFAULT_BATCH_SIZE = 20;

        #region ICsvDataImporter Members

        /// <summary>
        /// Imports the specified list of rows, where each row is a string of comma separated values (CSV).
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="context"></param>
        public virtual void Import(List<string> rows, IUpdateContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            if (args.Length == 0)
            {
                Platform.Log(LogLevel.Error, "Name of data file to import must be supplied as first argument.");
                return;
            }

            try
            {
                using (StreamReader reader = File.OpenText(args[0]))
                {
                    List<string> lines = null;
                    while ((lines = ReadLines(reader, DEFAULT_BATCH_SIZE)).Count > 0)
                    {
                        using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
                        {
                            ((IUpdateContext)PersistenceScope.CurrentContext).ChangeSetRecorder.OperationName = this.GetType().FullName;
                            Import(lines, (IUpdateContext)PersistenceScope.CurrentContext);
                            scope.Complete();
                        }
                    }
                }

            }
            catch(EntityValidationException e)
            {
                Log(LogLevel.Error, e.Message);
            }
            catch (Exception e)
            {
                Log(LogLevel.Error, e.ToString());
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Parses the specified line as a list of comma-separated values.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        protected string[] ParseCsv(string line)
        {
            string[] fields = StringUtilities.SplitQuoted(line, ",");

            // replace empty strings with nulls
            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].Length == 0)
                    fields[i] = null;
            }
            return fields;
        }

        /// <summary>
        /// Parses the specified line as a list of comma-separated values, throwing an exception
        /// if the number of fields is less than expected.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="expectedFieldCount"></param>
        /// <returns></returns>
        protected string[] ParseCsv(string line, int expectedFieldCount)
        {
            string[] fields = ParseCsv(line);
            if (fields.Length < expectedFieldCount)
                throw new ImportException(string.Format("Row must have {0} fields", expectedFieldCount));
            return fields;
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        protected void Log(LogLevel level, string message)
        {
            Platform.Log(level, message);
        }

        /// <summary>
        /// Tries to parse the specified string as an enum value.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected T TryParseEnum<T>(string value, T defaultValue)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        private List<string> ReadLines(StreamReader reader, int numLines)
        {
            List<string> lines = new List<string>();
            string line = null;
            while (lines.Count < numLines && (line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                    lines.Add(line);
            }
            return lines;
        }

        #endregion
    }
}

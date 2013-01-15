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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Upgrade;
using ClearCanvas.ImageServer.Enterprise.SqlServer;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model.SqlServer.UpgradeScripts
{
    public class BaseUpgradeScript : IPersistentStoreUpgradeScript
    {
        private readonly Version _upgradeFromVersion;
        private readonly Version _upgradeToVersion;
        private readonly string _scriptName;
        private readonly bool _upgradeStoredProcs = false;
        public BaseUpgradeScript(Version upgradeFromVersion, Version upgradeToVersion, string scriptName)
        {
            if (upgradeToVersion == null)
            {
                _upgradeToVersion = Assembly.GetExecutingAssembly().GetName().Version;
                _upgradeStoredProcs = true;
            }
            else
                _upgradeToVersion = upgradeToVersion;
            _upgradeFromVersion = upgradeFromVersion;
            _scriptName = scriptName;
        }
        public string GetScript()
        {
            string sql;

            using (Stream stream = GetType().Assembly.GetManifestResourceStream(GetType(), _scriptName))
            {
                if (stream == null)
                    throw new ApplicationException("Unable to load script resource (is the script an embedded resource?): " + _scriptName);

                StreamReader reader = new StreamReader(stream);
                sql = reader.ReadToEnd();
                stream.Close();
            }
            return sql;
        }

        public Version SourceVersion
        {
            get {return _upgradeFromVersion;}
        }

        public Version DestinationVersion
        {
            get { return _upgradeToVersion; }
        }

        public void Execute()
        {
            // Wrap the upgrade in a single commit.
            using (
                IUpdateContext updateContext =
                    PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                UpdateContext context = updateContext as UpdateContext;
                if (context == null)
                {
                    Console.WriteLine("Unexpected error opening connection to the database.");
                    throw new ApplicationException("Error opening connection to the database.");
                }

                ExecuteSql(context, GetScript());

                DatabaseVersionUpdateColumns columns = new DatabaseVersionUpdateColumns();
                DatabaseVersionSelectCriteria criteria = new DatabaseVersionSelectCriteria();

                columns.Revision = DestinationVersion.Revision.ToString();
                columns.Build = DestinationVersion.Build.ToString();
                columns.Minor = DestinationVersion.Minor.ToString();
                columns.Major = DestinationVersion.Major.ToString();

                IDatabaseVersionEntityBroker broker = context.GetBroker<IDatabaseVersionEntityBroker>();
                broker.Update(criteria, columns);

                updateContext.Commit();
            }

            if (_upgradeStoredProcs)
            {
                RunSqlScriptApplication app = new RunSqlScriptApplication();
                app.RunApplication(new string[] {"-storedprocedures"});
            }
            return;
        }

        private static void ExecuteSql(UpdateContext context, string rawSqlText)
        {
            Regex regex = new Regex("^\\s*GO\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string[] lines = regex.Split(rawSqlText);

            using (SqlCommand cmd = context.Connection.CreateCommand())
            {
                cmd.Connection = context.Connection;
                cmd.Transaction = context.Transaction;
                cmd.CommandTimeout = context.CommandTimeout;

                foreach (string line in lines)
                {
                    if (line.Length > 0)
                    {
                        try
                        {
                            cmd.CommandText = line;
                            cmd.CommandType = CommandType.Text;

                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Unexpected error with line in upgrade script: {0}", line);
                            Console.WriteLine("Error: {0}", e.Message);
                            throw;
                        }
                    }
                }
            }
        }
    }
}
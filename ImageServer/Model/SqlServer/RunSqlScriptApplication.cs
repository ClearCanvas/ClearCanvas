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
using System.Text.RegularExpressions;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise.SqlServer;

namespace ClearCanvas.ImageServer.Model.SqlServer
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class RunSqlScriptApplication : IApplicationRoot
    {
        #region Public Methods
        public void RunApplication(string[] args)
        {
            CommandLine cmdLine = new CommandLine();
            try
            {
                cmdLine.Parse(args);

                if (cmdLine.Switches.ContainsKey("storedprocedures") && cmdLine.Switches["storedprocedures"])
                {
                    Console.WriteLine("Upgrading the stored procedures.");
                    if (!RunEmbeddedScript("ClearCanvas.ImageServer.Model.SqlServer.Scripts.ImageServerStoredProcedures.sql"))
                        Environment.ExitCode = -1;
                    else
                        Environment.ExitCode = 0;
                }

                if (cmdLine.Switches.ContainsKey("defaultdata") && cmdLine.Switches["defaultdata"])
                {
                    Console.WriteLine("Upgrading the stored procedures.");
                    if (!RunEmbeddedScript("ClearCanvas.ImageServer.Model.SqlServer.Scripts.ImageServerDefaultData.sql"))
                        Environment.ExitCode = -1;
                    else
                        Environment.ExitCode = 0;
                }

                foreach (string script in cmdLine.Positional)
                {
                    if (!RunScript(script))
                    {
                        Console.WriteLine("Upgrading to execute script: {0}", script);
                        Environment.ExitCode = -1;
                        return;
                    }
                }				
            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                cmdLine.PrintUsage(Console.Out);
                Environment.ExitCode = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception when executing script: {0}", e.Message);
                Environment.ExitCode = -1;
            }
        }
        #endregion

        static bool RunScript(string scriptName)
        {
            using (
                IUpdateContext updateContext =
                    PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                UpdateContext context = updateContext as UpdateContext;
                if (context == null)
                {
                    Console.WriteLine("Unexpected error opening connection to the database.");
                    return false;
                }

                string sql;

                using (Stream stream = File.OpenRead(scriptName))
                {
                    StreamReader reader = new StreamReader(stream);
                    sql = reader.ReadToEnd();
                    stream.Close();
                }

                ExecuteSql(context, sql);

                updateContext.Commit();
            }
            return true;
        }

        bool RunEmbeddedScript(string embeddedScriptName)
        {
            using (
                IUpdateContext updateContext =
                    PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                UpdateContext context = updateContext as UpdateContext;
                if (context == null)
                {
                    Console.WriteLine("Unexpected error opening connection to the database.");
                    return false;
                }


                string sql;

                using (Stream stream = GetType().Assembly.GetManifestResourceStream(embeddedScriptName))
                {
                    if (stream == null)
                    {
                        Console.WriteLine("Unable to load embedded script: {0}", embeddedScriptName);
                        return false;
                    }
                    StreamReader reader = new StreamReader(stream);
                    sql = reader.ReadToEnd();
                    stream.Close();
                }

                ExecuteSql(context, sql);

                updateContext.Commit();
            }
            return true;
        }

        private static void ExecuteSql(UpdateContext context, string rawSqlText)
        {
            Regex regex = new Regex("^\\s*GO\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string[] lines = regex.Split(rawSqlText);

            using (SqlCommand cmd = context.Connection.CreateCommand())
            {
                cmd.Connection = context.Connection;
                cmd.Transaction = context.Transaction;

                foreach (string line in lines)
                {
                    if (line.Length > 0)
                    {
                        cmd.CommandText = line;
                        cmd.CommandType = CommandType.Text;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
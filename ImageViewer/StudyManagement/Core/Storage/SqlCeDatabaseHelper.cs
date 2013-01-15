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

#region Additional permission to link with SQL Server Compact Edition

// Additional permission under GNU GPL version 3 section 7
// 
// If you modify this Program, or any covered work, by linking or combining it
// with SQL Server Compact Edition (or a modified version of that library),
// containing parts covered by the terms of the SQL Server Compact Edition
// EULA, the licensors of this Program grant you additional permission to
// convey the resulting work.

#endregion

using System;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage
{
	public static class SqlCeDatabaseHelper<TContext>
	{
        public static void CreateDatabase(string fileName)
        {
			var filePath = GetDatabaseFilePath(fileName);

            // ensure the parent directory exists before trying to create database
            Directory.CreateDirectory(GetDatabaseDirectory());

            //NOTE: Since we're using CE 4.0, the LINQ CreateDatabase function won't work because it creates a 3.5 database.
            var resourceResolver = new ResourceResolver(typeof (TContext).Assembly);
            using (Stream resourceStream = resourceResolver.OpenResource(fileName))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var buffer = new byte[1024];
                    int bytesRead = resourceStream.Read(buffer, 0, buffer.Length);
                    // write the required bytes
                    while (bytesRead > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        bytesRead = resourceStream.Read(buffer, 0, buffer.Length);
                    }

                    fileStream.Close();
                }

                resourceStream.Close();
            }
        }

	    public static IDbConnection CreateConnection(string fileName, int timeoutMilliseconds = 2000)
	    {
	        int retryCount = 0;
	        int startTickCount = Environment.TickCount;
            while (true)
	        {
	            try
	            {
                    //While the database is being created (via CreateDatabase), other threads can be trying to either
                    //create the database, or create a connection to it before it's finished being written out.
                    //Rather than using an ExclusiveLock around CreateConnection all the time, we just put in a retry
                    //mechanism for the odd case where this collision actually occurs. Much less expensive, given
                    //how infrequently the database files are created.
                    //Note that an ExclusiveLock just around CreateDatabase doesn't work because the SqlCeConnection
                    //can still fail to open because it can't access the file while it's being created.
	                var connection = CreateConnectionInternal(fileName);
                    if (retryCount > 0)
                        Platform.Log(LogLevel.Info, "Successfully opened database connection to '{0}' after {1} retries.", fileName, retryCount);

	                return connection;
	            }
	            catch (Exception e)
	            {
                    //No timeout.
	                if (timeoutMilliseconds <= 0)
	                    throw;

                    var elapsed = Environment.TickCount - startTickCount;
	                var remaining = timeoutMilliseconds - elapsed;

	                //retry if there's some time left before the timeout, or there hasn't been at least one retry.
	                if (remaining <= 0 && retryCount != 0)
	                    throw;

                    ++retryCount;
	                var waitMilliseconds = Math.Min(50, remaining);
	                Platform.Log(LogLevel.Warn, e,
	                                "Failed to create database connection for '{0}'; waiting {1}ms before trying again.",
	                                fileName, waitMilliseconds);
	                Thread.Sleep(waitMilliseconds);
	            }
	        }
	    }

	    public static string GetDatabaseFilePath(string fileName)
		{
			return Path.Combine(GetDatabaseDirectory(), fileName);
		}

        public static string GetDatabaseDirectory()
        {
            return Platform.ApplicationDataDirectory;
        }

        private static IDbConnection CreateConnectionInternal(string fileName)
        {
            string filePath = GetDatabaseFilePath(fileName);
            if (!File.Exists(filePath))
                CreateDatabase(fileName);

            // TODO (CR Jun 2012): Why are we limiting the database size?
            var connectString = string.Format("Data Source = {0}; Default Lock Timeout = 10000;Max Database Size=2048;", filePath);

            // now we can create a long-lived connection
            var connection = new SqlCeConnection(connectString);
            connection.Open();
            return connection;
        }
    }
}

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
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    /// <summary>
    /// SQL Server implementation of <see cref="IPersistentStore"/>.
    /// </summary>
    [ExtensionOf(typeof(PersistentStoreExtensionPoint))]
    public class PersistentStore : IPersistentStore
    {
	    /// <summary>
	    /// Reset event to signal when stopping the service thread.
	    /// </summary>
		private CancellationTokenSource _cancelToken;
	    private static volatile bool _shutdownRequested;
        private static readonly object SyncRoot = new object();
        private static int _connectionCounter;
        private String _connectionString;
        private ITransactionNotifier _transactionNotifier;
        private int _maxPoolSize;

        #region IPersistentStore Members

		/// <summary>
		/// Boolean that can be set on the persistent store to signal that the process is shutting down.
		/// </summary>
		public bool ShutdownRequested { get { return _shutdownRequested; } set
		{
			_shutdownRequested = value;
			if (_shutdownRequested)
				_cancelToken.Cancel();
		} }

        public Version Version
        {
            get
            {
                using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                {
                    var broker = read.GetBroker<IPersistentStoreVersionEntityBroker>();
                    var criteria = new PersistentStoreVersionSelectCriteria();
                    criteria.Major.SortDesc(0);
                    criteria.Minor.SortDesc(1);
                    criteria.Build.SortDesc(2);
                    criteria.Revision.SortDesc(3);

                    IList<PersistentStoreVersion> versions = broker.Find(criteria);
                    if (versions.Count == 0)
                        return null;

                    PersistentStoreVersion ver = CollectionUtils.FirstElement(versions);

                    return new Version(
                        int.Parse(ver.Major),
                        int.Parse(ver.Minor),
                        int.Parse(ver.Build),
                        int.Parse(ver.Revision));
                }
            }
        }

        public void Initialize()
        {
	        _cancelToken = new CancellationTokenSource();

            // Retrieve the partial connection string named databaseConnection
            // from the application's app.config or web.config file.
            ConnectionStringSettings settings =
                ConfigurationManager.ConnectionStrings["ImageServerConnectString"];
			
            if (null != settings)
            {
                // Retrieve the partial connection string.
                _connectionString = settings.ConnectionString;
                var sb = new SqlConnectionStringBuilder(_connectionString);
                _maxPoolSize = sb.MaxPoolSize;
            }
#if UNIT_TESTS
            else 
            {
                _connectionString = "Data Source=127.0.0.1;Integrated Security=True;Persist Security Info=True;Initial Catalog=ImageServer";
                var sb = new SqlConnectionStringBuilder(_connectionString);
                _maxPoolSize = sb.MaxPoolSize;
            }
#endif
        }

        public void SetTransactionNotifier(ITransactionNotifier transactionNotifier)
        {
            _transactionNotifier = transactionNotifier;
        }

        private SqlConnection OpenConnection()
        {
            // Needed for retries;
            Random rand = null;

            for (int i = 1;; i++)
            {
                try
                {
	                var connection = new SqlConnection(_connectionString);
	                var task = connection.OpenAsync();
	                task.Wait(_cancelToken.Token);

	                connection.Disposed += Connection_Disposed;

	                lock (SyncRoot)
	                {
		                _connectionCounter++;
		                if (SqlServerSettings.Default.ConnectionPoolUsageWarningLevel <= 0)
		                {
			                Platform.Log(LogLevel.Warn, "# Max SqlConnection Pool Size={0}, current Db Connections={1}",
			                             _maxPoolSize, _connectionCounter);
		                }
		                else if (_connectionCounter > _maxPoolSize/SqlServerSettings.Default.ConnectionPoolUsageWarningLevel)
		                {
			                if (_connectionCounter%3 == 0)
			                {
				                Platform.Log(LogLevel.Warn, "# Max SqlConnection Pool Size={0}, current Db Connections={1}",
				                             _maxPoolSize, _connectionCounter);
			                }
		                }
	                }
	                return connection;
                }
                catch (SqlException e)
                {
	                // The connection failed.  Check the Sql error class 0x14 is for connection failure, let the 
	                // other error types through.
	                if ((i >= 10) || e.Class != 0x14 || _cancelToken.IsCancellationRequested)
		                throw;

	                if (rand == null) rand = new Random();

	                // Sleep a random amount between 5 and 10 seconds
	                int sleepTime = rand.Next(5*1000, 10*1000);
	                Platform.Log(LogLevel.Warn,
	                             "Failure connecting to the database, sleeping {0} milliseconds and retrying", sleepTime);

	                if (_cancelToken.IsCancellationRequested)
		                throw;
                }
                catch (AggregateException e)
                {
	                var x = e.InnerException as SqlException;
					if (x != null)
					{
						// The connection failed.  Check the Sql error class 0x14 is for connection failure, let the 
						// other error types through.
						if ((i >= 10) || x.Class != 0x14 || _cancelToken.IsCancellationRequested)
							throw;

						if (rand == null) rand = new Random();

						// Sleep a random amount between 5 and 10 seconds
						int sleepTime = rand.Next(5 * 1000, 10 * 1000);
						Platform.Log(LogLevel.Warn,
									 "Failure connecting to the database, sleeping {0} milliseconds and retrying", sleepTime);

						if (_cancelToken.IsCancellationRequested)
							throw;
					}
					else
						throw;
                }
            }
        }

        static void Connection_Disposed(object sender, EventArgs e)
        {
            lock (SyncRoot)
            {
                _connectionCounter --;
            }
        }

        public IReadContext OpenReadContext()
        {
            try
            {
                SqlConnection connection = OpenConnection();
                
                return new ReadContext(connection, _transactionNotifier);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Exception when opening database connection for reading");

                throw new PersistenceException("Unexpected exception opening database connection for reading", e);
            }
        }

        public IUpdateContext OpenUpdateContext(UpdateContextSyncMode mode)
        {
            try
            {
                SqlConnection connection = OpenConnection();

                return new UpdateContext(connection, _transactionNotifier, mode);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Exception when opening database connection for update");

                throw new PersistenceException("Unexpected exception opening database connection for update", e);
            }
        }


        #endregion
    }
}
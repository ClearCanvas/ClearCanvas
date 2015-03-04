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
using System.Data;
using System.Data.SqlClient;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    /// <summary>
    /// Baseline implementation of <see cref="IUpdateContext"/> for use with ADO.NET and SQL server.
    /// </summary>
    /// <remarks>
    /// This mechanism uses transaction wrappers in ADO.NET.  The transaction is started when the update
    /// context is created.
    /// </remarks>
    public class UpdateContext : PersistenceContext,IUpdateContext
    {
        #region Private Members
        private SqlTransaction _transaction;
        private UpdateContextSyncMode _mode;
        #endregion

        #region Constructors
        internal UpdateContext(SqlConnection connection, ITransactionNotifier transactionNotifier, UpdateContextSyncMode mode)
            : base (connection, transactionNotifier)
        {
            _transaction = connection.BeginTransaction(mode == UpdateContextSyncMode.Flush ? IsolationLevel.ReadCommitted : IsolationLevel.Serializable);
            _mode = mode;
        }
        #endregion

        #region Public Members
        public SqlTransaction Transaction
        {
            get { return _transaction; }
        }
        #endregion

        #region IUpdateContext Members

        void IUpdateContext.Commit()
        {
            if (_transaction != null && _transaction.Connection != null)
            {
				EventsHelper.Fire(PreCommit, this, EventArgs.Empty);

                try
                {
                    _transaction.Commit();
                }
                catch (SqlException e)
                {
                    // Discovered during 1.5 testing that when an timeout exception happens on
                    // a Commit(), the transaction is still committed in the background.  
                    // Added in a catch of the timeout exception only (by looking at SqlException.Number == -2 
                    // to identify a timeout exception), and also logged a warning message.
                    // 
                    // Found -2 magic number for timeout here:  
                    // http://blog.colinmackay.net/archive/2007/06/23/65.aspx
                    // and here:
                    // http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/2e30b3b1-2481-4a8d-b458-4dd4ec799a3f
                    if (e.Number != -2)
                        throw;

                    Platform.Log(LogLevel.Warn,e,"Timeout encountered on Commit of transaction.  Assuming transaction has completed.");
                }
                _transaction.Dispose();
                _transaction = null;
				
 				EventsHelper.Fire(PostCommit, this, EventArgs.Empty);
           }
            else
            {
                string errorMessage = "Attempting to commit transaction that is invalid. ";
                errorMessage += "Stack Trace: " + Environment.StackTrace;   

                Platform.Log(LogLevel.Error, errorMessage);
            }
        }

		/// <summary>
		/// Gets the set of entities that are affected by this update context, along with the type of change for each entity.
		/// </summary>
		/// <remarks>Not supported by this implementation.</remarks>
		IDictionary<object, EntityChangeType> IUpdateContext.GetAffectedEntities()
		{
			throw new NotSupportedException();
		}
		
    	public event EventHandler PreCommit;
		
    	public event EventHandler PostCommit;

    	#endregion

        #region IDisposable Members

        /// <summary>
        /// Commits the transaction (does not flush anything to the database)
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_transaction != null)
                {
                    try
                    {
                        if (_transaction.Connection != null)
                            _transaction.Rollback();
                        _transaction = null;
                    }
                    catch (SqlException e)
                    {
                        Platform.Log(LogLevel.Error, e);
                        _transaction = null;
                    }
                }
                // end the transaction
            }

            // important to call base class to close the session, etc.
            base.Dispose(disposing);
        }

        #endregion

    }
}
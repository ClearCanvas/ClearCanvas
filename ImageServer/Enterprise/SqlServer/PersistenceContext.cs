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
using System.Data.SqlClient;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    /// <summary>
    /// Defines the extension point for all NHibernate broker classes.
    /// </summary>
    [ExtensionPoint]
    public class BrokerExtensionPoint : ExtensionPoint<IPersistenceBroker>
    {
    }

    public abstract class PersistenceContext : IPersistenceContext
    {
        #region Constructors
        protected PersistenceContext(SqlConnection connection, ITransactionNotifier transactionNotifier)
        {
            _connection = connection;
            _transactionNotifier = transactionNotifier;
        }
        #endregion

        #region Private Members
        private SqlConnection _connection;
        private ITransactionNotifier _transactionNotifier;
        private IEntityChangeSetRecorder _changeSetRecorder = new ChangeSetRecorder();
        #endregion

        #region Properties
        public SqlConnection Connection
        {
            get { return _connection; }
        }

        public int CommandTimeout
        {
            get { return SqlServerSettings.Default.CommandTimeout; }
        }
        #endregion

        #region IPersistenceContext Members

        public TBrokerInterface GetBroker<TBrokerInterface>() where TBrokerInterface : IPersistenceBroker
        {
            return (TBrokerInterface)GetBroker(typeof(TBrokerInterface));
        }

        public object GetBroker(Type brokerInterface)
        {
            BrokerExtensionPoint xp = new BrokerExtensionPoint();
            IPersistenceBroker broker = (IPersistenceBroker)xp.CreateExtension(new TypeExtensionFilter(brokerInterface));
            broker.SetContext(this);
            return broker;
        }

        public void Lock(object domainObject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

		public void Lock(object domainObject, DirtyState state)
        {
            throw new Exception("The method or operation is not implemented.");
        }
		
		public object Load(EntityRef entityRef, EntityLoadFlags flags)
        {
            throw new Exception("The method or operation is not implemented.");
        }

    	public TEntity Load<TEntity>(EntityRef entityRef)// where TEntity : Entity
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public TEntity Load<TEntity>(EntityRef entityRef, EntityLoadFlags flags)// where TEntity : Entity
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void SynchState()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IEntityChangeSetRecorder ChangeSetRecorder
        {
            get
            {
                return _changeSetRecorder;
            }
            set
            {
                _changeSetRecorder = value;
            }
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        /// <summary>
        /// Commits the transaction (does not flush anything to the database)
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
            }
        }
    }
}
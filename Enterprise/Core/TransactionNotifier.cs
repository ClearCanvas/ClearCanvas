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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// A simple implementation of ITransactionMonitor.  In reality, ITransactionMonitor should be implemented
    /// by a remote service running on the server, but for the time being there is a simple stub that
    /// exists as part of the client process.  The stub will transmit change set notifications within the client
    /// process only - hence multiple clients will not be aware of each others' changes.
    /// </summary>
    public class TransactionNotifier : ITransactionNotifier
    {
        private Dictionary<Type, EventHandler<EntityChangeEventArgs>> _eventMap;

        internal TransactionNotifier()
        {
            _eventMap = new Dictionary<Type, EventHandler<EntityChangeEventArgs>>();
        }


        #region ITransactionNotifier Members

        public void Subscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler)
        {
            if (_eventMap.ContainsKey(entityType))
            {
                _eventMap[entityType] += eventHandler;
            }
            else
            {
                _eventMap.Add(entityType, eventHandler);
            }
        }

        public void Unsubscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler)
        {
            if (_eventMap.ContainsKey(entityType))
            {
                _eventMap[entityType] -= eventHandler;
            }
        }

        public void Queue(ICollection<EntityChange> changeSet)
        {
            // in a real implementation, the changeSet would be forwarded to some server
            // but for now we just post the changes as local events within this process
            foreach (EntityChange change in changeSet)
            {
                Notify(change);
            }
        }

        #endregion

        private void Notify(EntityChange change)
        {
            foreach (Type eventKeyClass in _eventMap.Keys)
            {
/*                // check if the eventKeyClass is the entity class, or a superclass of it
                if (eventKeyClass.IsAssignableFrom(change.EntityRef))
                {
                    // create an entity ref based on the eventKeyClass, regardless of the actual entity class
                    //EntityRefBase entityRef = EntityRefFactory.CreateReference(eventKeyClass, change.EntityOID, change.Version);
                    //EventsHelper.Fire(_eventMap[change.EntityClass], null, new EntityChangeEventArgs(entityRef, change.ChangeType));
                }
 * */
            }
        }
    }
}

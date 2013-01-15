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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Specifies the inteface to the transaction monitor.  This is very preliminary and will likely
    /// change.
    /// </summary>
    public interface ITransactionNotifier
    {
        /// <summary>
        /// Allows a client to subscribe for change notifications for a given type of entity.
        /// It is extremely important that clients explicitly unsubscribe in order that resources
        /// are properly released.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="eventHandler"></param>
        void Subscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler);

        /// <summary>
        /// Allows a client to unsubscribe from change notifications for a given type of entity.
        /// It is extremely important that clients explicitly unsubscribe in order that resources
        /// are properly released.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="eventHandler"></param>
        void Unsubscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler);

        /// <summary>
        /// Allows a client to queue a set of changes for notification to other clients.
        /// </summary>
        /// <param name="changeSet"></param>
        void Queue(ICollection<EntityChange> changeSet);
    }
}

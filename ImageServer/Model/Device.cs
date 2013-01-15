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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model
{
    public partial class Device
    {
        #region Private Members
        private ServerPartition _serverPartition;
        #endregion

        #region Public Properties
        public ServerPartition ServerPartition
        {
            get
            {
                if (_serverPartition == null)
                    _serverPartition = ServerPartition.Load(ServerPartitionKey);
                return _serverPartition;
            }
        }

        #endregion

        /// <summary>
        /// Gets a list of Web Study Move or AutoRoute WorkQueue entries 
        /// that are in progress for this device.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<WorkQueue> GetAllCurrentMoveEntries(IPersistenceContext context)
        {
            IQueryCurrentStudyMove broker = context.GetBroker<IQueryCurrentStudyMove>();
            QueryCurrentStudyMoveParameters criteria = new QueryCurrentStudyMoveParameters();
            criteria.DeviceKey = Key;
            return new List<WorkQueue>(broker.Find(criteria));
        }
    }
}

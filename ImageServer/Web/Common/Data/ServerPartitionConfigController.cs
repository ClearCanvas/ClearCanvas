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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class ServerPartitionConfigController
    {
        #region Private members

        /// <summary>
        /// The adapter class to set/retrieve server partitions from server partition table
        /// </summary>
        private readonly ServerPartitionDataAdapter _serverAdapter = new ServerPartitionDataAdapter();

        #endregion

        #region public methods

        /// <summary>
        /// Add a partition in the database.
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="groupsWithDataAccess"></param>
        public bool AddPartition(ServerPartition partition, List<string> groupsWithDataAccess)
        {
            Platform.Log(LogLevel.Info, "Adding new server partition : AETitle = {0}", partition.AeTitle);

            bool result = _serverAdapter.AddServerPartition(partition, groupsWithDataAccess);

            if (result)
                Platform.Log(LogLevel.Info, "Server Partition added : AETitle = {0}", partition.AeTitle);
            else
                Platform.Log(LogLevel.Info, "Failed to add Server Partition: AETitle = {0}", partition.AeTitle);

            return result;
        }

        /// <summary>
        /// Update the partition whose GUID and new information are specified in <paramref name="partition"/>.
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="groupsWithDataAccess"></param>
        /// <returns></returns>
        public bool UpdatePartition(ServerPartition partition, List<string> groupsWithDataAccess)
        {
            Platform.Log(LogLevel.Info, "Updating server partition: AETitle = {0}", partition.AeTitle);

            bool result = _serverAdapter.Update(partition, groupsWithDataAccess);

            if (result)
                Platform.Log(LogLevel.Info, "Server Partition updated : AETitle = {0}", partition.AeTitle);
            else
                Platform.Log(LogLevel.Info, "Failed to update Server Partition: AETitle = {0}", partition.AeTitle);

            return result;
        }

        

        /// <summary>
        /// Retrieves a list of <seealso cref="ServerPartition"/> matching the specified criteria.
        /// </summary>
        /// <returns>A list of partitions</returns>
        public IList<ServerPartition> GetPartitions(ServerPartitionSelectCriteria criteria)
        {
            return _serverAdapter.GetServerPartitions(criteria);
        }

        /// <summary>
        /// Retrieves a list of <seealso cref="ServerPartition"/> matching the specified criteria.
        /// </summary>
        /// <returns>A list of partitions</returns>
        public ServerPartition GetPartition(ServerEntityKey key)
        {
            return _serverAdapter.GetServerPartition(key);
        }

        /// <summary>
        /// Retrieves all server paritions.
        /// </summary>
        /// <returns></returns>
        public IList<ServerPartition> GetAllPartitions()
        {
        	var searchCriteria = new ServerPartitionSelectCriteria();
        	searchCriteria.AeTitle.SortAsc(0);
			return GetPartitions(searchCriteria);
        }

        /// <summary>
        /// Checks if a specified partition can be deleted
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool CanDelete(ServerPartition partition)
        {
            return partition.StudyCount <= 0 && !partition.ServerPartitionTypeEnum.Equals(ServerPartitionTypeEnum.VFS);
        }

		/// <summary>
		/// Checks if a specified partition can be edited
		/// </summary>
		/// <param name="partition"></param>
		/// <returns></returns>
		public bool CanEdit(ServerPartition partition)
		{
			return !partition.ServerPartitionTypeEnum.Equals(ServerPartitionTypeEnum.VFS);
		}

        /// <summary>
        /// Delete the specified partition
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool Delete(ServerPartition partition)
        {
            Platform.Log(LogLevel.Info, "Deleting server partition: AETitle = {0}", partition.AeTitle);

            try
            {
                using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    var broker = ctx.GetBroker<IDeleteServerPartition>();
                    var parms = new ServerPartitionDeleteParameters
	                    {
		                    ServerPartitionKey = partition.Key
	                    };
	                if (!broker.Execute(parms))
                        throw new Exception("Unable to delete server partition from database");
                    ctx.Commit();
                }

                Platform.Log(LogLevel.Info, "Server Partition deleted : AETitle = {0}", partition.AeTitle);
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Info, e, "Failed to delete Server Partition: AETitle = {0}", partition.AeTitle);
                return false;
            }


        }

        #endregion // public methods

    }
}

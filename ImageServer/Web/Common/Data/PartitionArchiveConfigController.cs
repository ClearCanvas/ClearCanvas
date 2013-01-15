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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class PartitionArchiveConfigController
    {
        #region Private members

        /// <summary>
        /// The adapter class to set/retrieve archive partitions from archive partition table
        /// </summary>
        private readonly PartitionArchiveDataAdapter _archiveAdapter = new PartitionArchiveDataAdapter();

        #endregion

        #region public methods

        /// <summary>
        /// Add a partition in the database.
        /// </summary>
        /// <param name="partition"></param>
        public bool AddPartition(PartitionArchive partition)
        {
            Platform.Log(LogLevel.Info, "Adding new partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);

            bool result = _archiveAdapter.AddPartitionArchive(partition);

            if (result)
                Platform.Log(LogLevel.Info, "Added new partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);
            else
                Platform.Log(LogLevel.Info, "Failed to add new partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);

            return result;
        }

        /// <summary>
        /// Update the partition whose GUID and new information are specified in <paramref name="partition"/>.
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool UpdatePartition(PartitionArchive partition)
        {
            Platform.Log(LogLevel.Info, "Updating partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);

            bool result = _archiveAdapter.Update(partition);

            if (result)
                Platform.Log(LogLevel.Info, "Updated partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);
            else
                Platform.Log(LogLevel.Info, "Failed to update partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);

            return result;
        }

        

        /// <summary>
        /// Retrieves a list of <seealso cref="PartitionArchive"/> matching the specified criteria.
        /// </summary>
        /// <returns>A list of partitions</returns>
        public IList<PartitionArchive> GetPartitions(PartitionArchiveSelectCriteria criteria)
        {
            return _archiveAdapter.GetPartitionArchives(criteria);
        }

        /// <summary>
        /// Retrieves all archive paritions.
        /// </summary>
        /// <returns></returns>
        public IList<PartitionArchive> GetAllPartitions()
        {
        	PartitionArchiveSelectCriteria searchCriteria = new PartitionArchiveSelectCriteria();
			return GetPartitions(searchCriteria);
        }

        /// <summary>
        /// Delete the specified partition
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool Delete(PartitionArchive partition)
        {
            return _archiveAdapter.Delete(partition.GetKey());
        }

        /// <summary>
        /// Determine if the specified partition can be deleted. If studies are scheduled
        /// to be archived on that partition or studies are already archived on that partition,
        /// then the partition may not be deleted.
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool CanDelete(PartitionArchive partition)
        {          
            ArchiveQueueAdaptor archiveQueueAdaptor = new ArchiveQueueAdaptor();
            ArchiveQueueSelectCriteria selectCriteria = new ArchiveQueueSelectCriteria();
            selectCriteria.PartitionArchiveKey.EqualTo(partition.GetKey());

            ArchiveStudyStorageAdaptor archiveStudyStorageAdaptor = new ArchiveStudyStorageAdaptor();
            ArchiveStudyStorageSelectCriteria criteria = new ArchiveStudyStorageSelectCriteria();
            criteria.PartitionArchiveKey.EqualTo(partition.GetKey());

            int queueItems = archiveQueueAdaptor.GetCount(selectCriteria);
			int storageItems = 0;
			// only check if we need to.
			if (queueItems == 0) storageItems = archiveStudyStorageAdaptor.GetCount(criteria);

			return !((queueItems > 0) || (storageItems > 0));
        }

        #endregion // public methods

    }
}

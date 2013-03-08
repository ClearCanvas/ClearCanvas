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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Common.Caching;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Provides access to the PartitionSopClass configuration. This class uses caching for performance purpose.
    /// </summary>
    public class PartitionSopClassConfiguration
    {
        const string _cacheId = "ParitionSopClassConfiguration";

        /// <summary>
        /// Returns the <see cref="PartitionSopClass"/> for the specified sop class and partition or NULL if the sop class UID is not set.
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="sopClassUid"></param>
        /// <returns></returns>
        public PartitionSopClass GetPartitionSopClass(ServerPartition partition, string sopClassUid)
        {
            Platform.CheckForNullReference(partition, "partition");
            Platform.CheckForEmptyString(sopClassUid, "sopClassUid");

            var list = LoadPartitionSopClasses(partition);
            return list.SingleOrDefault(entry => entry.SopClassUid == sopClassUid);
        }

        /// <summary>
        /// Returns list of all <see cref="PartitionSopClass"/> for the specified partition.
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public IEnumerable<PartitionSopClass> GetAllPartitionSopClasses(ServerPartition partition)
        {
            return LoadPartitionSopClasses(partition);
        }

        #region Private Methods

        private IEnumerable<PartitionSopClass> LoadPartitionSopClasses(ServerPartition partition)
        {
            IList<PartitionSopClass> list = null;

            if (Cache.IsSupported())
            {
                var cacheKey = "PartitionSopClasses-" + partition.Key;
                using (var cacheClient = Cache.CreateClient(_cacheId))
                {
                    list = cacheClient.Get(cacheKey, new CacheGetOptions("default")) as IList<PartitionSopClass>;
                    if (list == null)
                    {
                        Platform.Log(LogLevel.Debug, "Loading PartitionSopClass from database");
                        list = LoadPartitionSopClassesFromDatabase(partition);
                        cacheClient.Put(cacheKey, list, new CachePutOptions("default", TimeSpan.FromSeconds(30), false /*no sliding. Reload 60 seconds*/));
                    }
                }
            }
            else
            {
                list = LoadPartitionSopClassesFromDatabase(partition);
            }

            return list;
        }

        private IList<PartitionSopClass> LoadPartitionSopClassesFromDatabase(ServerPartition partition)
        {
            using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                // Set the input parameters for query
                var inputParms = new PartitionSopClassQueryParameters {ServerPartitionKey = partition.GetKey()};

                var broker = context.GetBroker<IQueryServerPartitionSopClasses>();
                return broker.Find(inputParms);
            }
        }

        #endregion
    }
}
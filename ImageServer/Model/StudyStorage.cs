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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Model
{
    public partial class StudyStorage
    {
    	#region Private Fields
        private Study _study;
        private ServerPartition _partition;
		#endregion
		
		#region Public Properties
        

        public ServerPartition ServerPartition
        {
            get
            {
                if (_partition==null)
                {
                    _partition = ServerPartition.Load(ServerPartitionKey);
                }
                return _partition;
            }
        }

        #endregion
		
		public Study LoadStudy(IPersistenceContext context)
        {
            return Study.Find(context, StudyInstanceUid, ServerPartition);
        }

        public Study GetStudy()
        {
            if (_study == null)
            {
                lock (SyncRoot)
                {
                    using (var context = new ServerExecutionContext())
                    {
                        _study = LoadStudy(context.ReadContext);
                    }
                }
            }

            return _study;
        }
		
        public void Archive(IUpdateContext context)
        {
            var insertArchiveQueueBroker = context.GetBroker<IInsertArchiveQueue>();
            var parms = new InsertArchiveQueueParameters
                            {
                                ServerPartitionKey = ServerPartitionKey, 
                                StudyStorageKey = Key
                            };
            if (!insertArchiveQueueBroker.Execute(parms))
            {
                throw new ApplicationException("Unable to schedule study archive");
            }
        }

		/// <summary>
		/// Insert a request to restore the specified <seealso cref="StudyStorage"/>
		/// </summary>
		/// <returns>Reference to the <see cref="RestoreQueue"/> that was inserted.</returns>
		public RestoreQueue InsertRestoreRequest()
		{
			// TODO:
			// Check the stored procedure to see if it will insert another request if one already exists

			using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				var broker = updateContext.GetBroker<IInsertRestoreQueue>();
				var parms = new InsertRestoreQueueParameters { StudyStorageKey = Key };

				RestoreQueue queue = broker.FindOne(parms);

				if (queue == null)
				{
					Platform.Log(LogLevel.Error, "Unable to request restore for study {0}", StudyInstanceUid);
					return null;
				}

				updateContext.Commit();
				Platform.Log(LogLevel.Info, "Restore requested for study {0}", StudyInstanceUid);
				return queue;
			}
		}

		public static StudyStorage Load(IPersistenceContext read, ServerEntityKey partitionKey, string studyInstanceUid)
		{
	        var broker = read.GetBroker<IStudyStorageEntityBroker>();
			var criteria = new StudyStorageSelectCriteria();
			criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
			criteria.ServerPartitionKey.EqualTo(partitionKey);
            StudyStorage theObject = broker.FindOne(criteria);
            return theObject;        
		}

		public static StudyStorage Load(ServerEntityKey partitionKey, string studyInstanceUid)
		{
            using (var context = new ServerExecutionContext())
            {
				return Load(context.ReadContext, partitionKey, studyInstanceUid);
			}
		}
    }
}

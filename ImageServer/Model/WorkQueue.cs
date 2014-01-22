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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.WorkQueue;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public partial class WorkQueue
    {
        #region Private Members
        protected StudyStorage _studyStorage;
        private Study _study;
        #endregion

        private void LoadRelatedEntities()
        {
            if (_study==null || _studyStorage==null)
            {
                using (var context = new ServerExecutionContext())
                {
                    lock (SyncRoot)
                    {
                        if (_study == null)
                            _study = LoadStudy(context.ReadContext);

                        if (_studyStorage == null)
                            _studyStorage = LoadStudyStorage(context.ReadContext);
                    }
                }    
            }
        }


        /// <summary>
        /// Delete the Work Queue record from the system.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Delete(IPersistenceContext context)
        {
            var workQueueUidBroker = context.GetBroker<IWorkQueueUidEntityBroker>();
            var criteria = new WorkQueueUidSelectCriteria();
            criteria.WorkQueueKey.EqualTo(GetKey());
            workQueueUidBroker.Delete(criteria);

            var workQueueBroker = context.GetBroker<IWorkQueueEntityBroker>();
            return workQueueBroker.Delete(GetKey());
        }

        /// <summary>
        /// Loads the related <see cref="Study"/> entity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Study LoadStudy(IPersistenceContext context)
        {
            if (_study == null)
            {
                lock (SyncRoot)
                {
                    _study = Study.Find(context, StudyStorageKey);
                }
            }
            return _study;
        }

        /// <summary>
        /// Loads the related <see cref="StudyStorage"/> entity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private StudyStorage LoadStudyStorage(IPersistenceContext context)
        {
            if (_studyStorage==null)
            {
                lock (SyncRoot)
                {
                    _studyStorage = StudyStorage.Load(context, StudyStorageKey); 
                }
            }
            return _studyStorage;
        }

        public IList<StudyStorageLocation> LoadStudyLocations(IPersistenceContext context)
        {
            StudyStorage storage = LoadStudyStorage(context);
            return StudyStorageLocation.FindStorageLocations(context, storage);
        }

        public StudyStorage StudyStorage
        {
            get
            {
                LoadRelatedEntities();
                return _studyStorage;
            }
        }

        public Study Study
        {
            get
            {
                LoadRelatedEntities();
                return _study;
            }
            set { _study = value; }
        }

        public WorkQueueData SerializeData
        {
            get
            {
                if (Data == null)
                    return null;

                try
                {
                    return ImageServerSerializer.DeserializeWorkQueueData(Data);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set { Data = ImageServerSerializer.SerializeWorkQueueDataToXmlDocument(value); }
        }
    }
}

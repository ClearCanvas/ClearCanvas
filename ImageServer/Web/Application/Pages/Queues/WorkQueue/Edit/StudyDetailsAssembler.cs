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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// Assembles an instance of  <see cref="StudyDetails"/> based on a <see cref="Study"/> object.
    /// </summary>
    public class StudyDetailsAssembler
    {
        /// <summary>
        /// Creates an instance of <see cref="StudyDetails"/> base on a <see cref="Study"/> object.
        /// </summary>
        /// <param name="study"></param>
        /// <returns></returns>
        public StudyDetails CreateStudyDetail(Study study)
        {
            var details = new StudyDetails();
            details.StudyInstanceUID = study.StudyInstanceUid;
            details.PatientName = study.PatientsName;
            details.AccessionNumber = study.AccessionNumber;
            details.PatientID = study.PatientId;
            details.StudyDescription = study.StudyDescription;
            details.StudyDate = study.StudyDate;
            details.StudyTime = study.StudyTime;

            var controller = new StudyController();
            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                details.Modalities = controller.GetModalitiesInStudy(ctx, study);
            }

            if (study.StudyInstanceUid != null)
            {
            	StudyStorage storages = StudyStorage.Load(study.StudyStorageKey);
                if (storages != null)
                {
                    details.WriteLock = storages.WriteLock;
                	details.ReadLock = storages.ReadLock;
                    details.Status = storages.StudyStatusEnum.ToString();
                }
            }
			
            return details;
        }
    }
}
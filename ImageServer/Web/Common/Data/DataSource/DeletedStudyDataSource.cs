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
using System.Collections;
using System.Collections.Generic;
using System.Web;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Query;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Services.WorkQueue.DeleteStudy.Extensions;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

namespace ClearCanvas.ImageServer.Web.Common.Data.DataSource
{
	public class DeletedStudyDataSource 
	{
		private IList<DeletedStudyInfo> _studies;

		public string AccessionNumber { get; set; }

		public DeletedStudyInfo  Find(object key)
		{
			return CollectionUtils.SelectFirst(_studies,
			                                   info => info.RowKey.Equals(key));
		}

		public string PatientId { get; set; }

		public DateTime? StudyDate { get; set; }

		public string DeletedBy { get; set; }

		public string PatientsName { get; set; }

		public string StudyDescription { get; set; }

		private StudyDeleteRecordSelectCriteria GetSelectCriteria()
		{
			var criteria = new StudyDeleteRecordSelectCriteria();

            QueryHelper.SetGuiStringCondition(criteria.AccessionNumber, AccessionNumber);
            QueryHelper.SetGuiStringCondition(criteria.PatientId, PatientId);
            QueryHelper.SetGuiStringCondition(criteria.PatientsName, PatientsName);
            QueryHelper.SetGuiStringCondition(criteria.StudyDescription, StudyDescription);
            
			if (StudyDate != null)
				criteria.StudyDate.Like("%" + DateParser.ToDicomString(StudyDate.Value) + "%");

			return criteria;
		}

		public IEnumerable Select(int startRowIndex, int maxRows)
		{

			IStudyDeleteRecordEntityBroker broker = HttpContext.Current.GetSharedPersistentContext().GetBroker<IStudyDeleteRecordEntityBroker>();
			StudyDeleteRecordSelectCriteria criteria = GetSelectCriteria();
			criteria.Timestamp.SortDesc(0);
			IList<StudyDeleteRecord> list = broker.Find(criteria, startRowIndex, maxRows);

			_studies = CollectionUtils.Map(
				list, (StudyDeleteRecord record) => DeletedStudyInfoAssembler.CreateDeletedStudyInfo(record));

			// Additional filter: DeletedBy
            if (String.IsNullOrEmpty(DeletedBy)==false)
            {
                _studies = CollectionUtils.Select(_studies, delegate(DeletedStudyInfo record)
                                       {
                                           if (String.IsNullOrEmpty(record.UserId) || String.IsNullOrEmpty(record.UserName))
                                               return false;

                                           // either the id or user matches
                                           return record.UserId.ToUpper().IndexOf(DeletedBy.ToUpper()) >= 0 ||
                                                  record.UserName.ToUpper().IndexOf(DeletedBy.ToUpper()) >= 0;
                                       });
            }

			return _studies;
		
		}

		public int SelectCount()
		{
			StudyDeleteRecordSelectCriteria criteria = GetSelectCriteria();

            IStudyDeleteRecordEntityBroker broker =HttpContext.Current.GetSharedPersistentContext().GetBroker<IStudyDeleteRecordEntityBroker>();
		    return broker.Count(criteria);
		}
	}

	internal static class DeletedStudyInfoAssembler
	{
		public static DeletedStudyInfo CreateDeletedStudyInfo(StudyDeleteRecord record)
		{
			Filesystem fs = Filesystem.Load(record.FilesystemKey);

		    StudyDeleteExtendedInfo extendedInfo = XmlUtils.Deserialize<StudyDeleteExtendedInfo>(record.ExtendedInfo);
			DeletedStudyInfo info = new DeletedStudyInfo
			                        	{
			                        		DeleteStudyRecord = record.GetKey(),
			                        		RowKey = record.GetKey().Key,
			                        		StudyInstanceUid = record.StudyInstanceUid,
			                        		PatientsName = record.PatientsName,
			                        		AccessionNumber = record.AccessionNumber,
			                        		PatientId = record.PatientId,
			                        		StudyDate = record.StudyDate,
			                        		PartitionAE = record.ServerPartitionAE,
			                        		StudyDescription = record.StudyDescription,
			                        		BackupFolderPath = fs.GetAbsolutePath(record.BackupPath),
			                        		ReasonForDeletion = record.Reason,
			                        		DeleteTime = record.Timestamp,
			                        		UserName = extendedInfo.UserName,
			                        		UserId = extendedInfo.UserId
			                        	};
			if (record.ArchiveInfo!=null)
				info.Archives = XmlUtils.Deserialize<DeletedStudyArchiveInfoCollection>(record.ArchiveInfo);

            
			return info;
		}
	}
}
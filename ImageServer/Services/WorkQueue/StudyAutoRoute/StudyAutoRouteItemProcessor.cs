#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.AutoRoute;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyAutoRoute
{
	[StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.None)]
	public class StudyAutoRouteItemProcessor : AutoRouteItemProcessor
	{
		/// <summary>
		/// Gets the list of instances to be sent from the study xml
		/// </summary>
		/// <returns></returns>
		protected override IEnumerable<StorageInstance> GetStorageInstanceList()
		{
			Platform.CheckForNullReference(StorageLocation, "StorageLocation");

			var list = new List<StorageInstance>();

			string studyPath = StorageLocation.GetStudyPath();
			StudyXml studyXml = LoadStudyXml(StorageLocation);
			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					string seriesPath = Path.Combine(studyPath, seriesXml.SeriesInstanceUid);
					string instancePath = Path.Combine(seriesPath, instanceXml.SopInstanceUid + ServerPlatform.DicomFileExtension);
					var instance = new StorageInstance(instancePath)
						{
							SopClass = instanceXml.SopClass,
							TransferSyntax = instanceXml.TransferSyntax,
							SopInstanceUid = instanceXml.SopInstanceUid,
							StudyInstanceUid = studyXml.StudyInstanceUid,
							SeriesInstanceUid = seriesXml.SeriesInstanceUid,
							PatientId = studyXml.PatientId,
							PatientsName = studyXml.PatientsName
						};

					list.Add(instance);
				}
			}

			return list;
		}

		protected override void OnComplete()
		{
			PostProcessing(WorkQueueItem,
			               WorkQueueProcessorStatus.Complete,
			               WorkQueueProcessorDatabaseUpdate.None);
		}

		protected override bool CanStart()
		{
			IList<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem,
			                                                                new[]
				                                                                {
					                                                                WorkQueueTypeEnum.StudyProcess,
					                                                                WorkQueueTypeEnum.ReconcileStudy
				                                                                },
			                                                                new[]
				                                                                {
					                                                                WorkQueueStatusEnum.Idle,
					                                                                WorkQueueStatusEnum.InProgress,
					                                                                WorkQueueStatusEnum.Pending
				                                                                });

			if (relatedItems != null && relatedItems.Count > 0)
			{
				// can't do it now. Reschedule it for future
				List<Model.WorkQueue> list = CollectionUtils.Sort(relatedItems,
				                                                  (item1, item2) =>
				                                                  item1.ScheduledTime.CompareTo(item2.ScheduledTime));

				DateTime newScheduledTime = list[0].ScheduledTime.AddSeconds(WorkQueueProperties.PostponeDelaySeconds);
				if (newScheduledTime < Platform.Time.AddMinutes(1))
					newScheduledTime = Platform.Time.AddMinutes(1);

				PostponeItem(newScheduledTime, newScheduledTime.AddDays(1), "Study is being processed or reconciled.");
				Platform.Log(LogLevel.Info, "{0} postponed to {1}. Study UID={2}", WorkQueueItem.WorkQueueTypeEnum, newScheduledTime,
				             StorageLocation.StudyInstanceUid);
				return false;
			}

			return base.CanStart();
		}
	}
}

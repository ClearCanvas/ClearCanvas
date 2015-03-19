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
using System.Web;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue
{
    public static class ReconcileDetailsAssembler
    {
        public static ReconcileDetails CreateReconcileDetails(StudyIntegrityQueueSummary item)
        {
            ReconcileDetails details = item.TheStudyIntegrityQueueItem.StudyIntegrityReasonEnum.Equals(
                                           StudyIntegrityReasonEnum.InconsistentData)
                                           ? new ReconcileDetails(item.TheStudyIntegrityQueueItem)
                                           : new DuplicateEntryDetails(item.TheStudyIntegrityQueueItem);

            Study study = item.StudySummary.TheStudy;
            details.StudyInstanceUid = study.StudyInstanceUid;

            //Set the demographic details of the Existing Patient
            details.ExistingStudy = new ReconcileDetails.StudyInfo();
            details.ExistingStudy.StudyInstanceUid = item.StudySummary.StudyInstanceUid;
            details.ExistingStudy.AccessionNumber = item.StudySummary.AccessionNumber;
            details.ExistingStudy.StudyDate = item.StudySummary.StudyDate;
            details.ExistingStudy.Patient.PatientID = item.StudySummary.PatientId;
            details.ExistingStudy.Patient.Name = item.StudySummary.PatientsName;
            details.ExistingStudy.Patient.Sex = study.PatientsSex;
            details.ExistingStudy.Patient.IssuerOfPatientID = study.IssuerOfPatientId;
            details.ExistingStudy.Patient.BirthDate = study.PatientsBirthDate;
            details.ExistingStudy.Series = CollectionUtils.Map(
                study.Series.Values,
                delegate(Series theSeries)
                    {
                        var seriesDetails = new ReconcileDetails.SeriesDetails
                                                {
                                                    Description = theSeries.SeriesDescription,
                                                    SeriesInstanceUid = theSeries.SeriesInstanceUid,
                                                    Modality = theSeries.Modality,
                                                    NumberOfInstances = theSeries.NumberOfSeriesRelatedInstances,
                                                    SeriesNumber = theSeries.SeriesNumber
                                                };
                        return seriesDetails;
                    });


            details.ConflictingImageSet = item.QueueData.Details;


            details.ConflictingStudyInfo = new ReconcileDetails.StudyInfo();

            if (item.QueueData.Details != null)
            {
                // extract the conflicting study info from Details
                details.ConflictingStudyInfo.AccessionNumber = item.QueueData.Details.StudyInfo.AccessionNumber;
                details.ConflictingStudyInfo.StudyDate = item.QueueData.Details.StudyInfo.StudyDate;
                details.ConflictingStudyInfo.StudyInstanceUid = item.QueueData.Details.StudyInfo.StudyInstanceUid;
                details.ConflictingStudyInfo.StudyDate = item.QueueData.Details.StudyInfo.StudyDate;

                details.ConflictingStudyInfo.Patient = new ReconcileDetails.PatientInfo
                                                           {
                                                               BirthDate =
                                                                   item.QueueData.Details.StudyInfo.PatientInfo.
                                                                   PatientsBirthdate,
                                                               IssuerOfPatientID =
                                                                   item.QueueData.Details.StudyInfo.PatientInfo.
                                                                   IssuerOfPatientId,
                                                               Name = item.QueueData.Details.StudyInfo.PatientInfo.Name,
                                                               PatientID =
                                                                   item.QueueData.Details.StudyInfo.PatientInfo.
                                                                   PatientId,
                                                               Sex = item.QueueData.Details.StudyInfo.PatientInfo.Sex
                                                           };

                details.ConflictingStudyInfo.Series =
                    CollectionUtils.Map(
                        item.QueueData.Details.StudyInfo.Series,
                        delegate(SeriesInformation input)
                            {
                                var seriesDetails = new ReconcileDetails.SeriesDetails
                                                        {
                                                            Description = input.SeriesDescription,
                                                            Modality = input.Modality,
                                                            SeriesInstanceUid = input.SeriesInstanceUid,
                                                            NumberOfInstances = input.NumberOfInstances
                                                        };
                                return seriesDetails;
                            });
            }
            else
            {
                // Extract the conflicting study info from StudyData
                // Note: Not all fields are available.
                ImageSetDescriptor desc =
                    ImageSetDescriptor.Parse(item.TheStudyIntegrityQueueItem.StudyData.DocumentElement);
                string value;

                if (desc.TryGetValue(DicomTags.AccessionNumber, out value))
                    details.ConflictingStudyInfo.AccessionNumber = value;

                if (desc.TryGetValue(DicomTags.StudyDate, out value))
                    details.ConflictingStudyInfo.StudyDate = value;

                if (desc.TryGetValue(DicomTags.StudyInstanceUid, out value))
                    details.ConflictingStudyInfo.StudyInstanceUid = value;

                details.ConflictingStudyInfo.Patient = new ReconcileDetails.PatientInfo();

                if (desc.TryGetValue(DicomTags.PatientsBirthDate, out value))
                    details.ConflictingStudyInfo.Patient.BirthDate = value;

                if (desc.TryGetValue(DicomTags.IssuerOfPatientId, out value))
                    details.ConflictingStudyInfo.Patient.IssuerOfPatientID = value;

                if (desc.TryGetValue(DicomTags.PatientsName, out value))
                    details.ConflictingStudyInfo.Patient.Name = value;

                if (desc.TryGetValue(DicomTags.PatientId, out value))
                    details.ConflictingStudyInfo.Patient.PatientID = value;

                if (desc.TryGetValue(DicomTags.PatientsSex, out value))
                    details.ConflictingStudyInfo.Patient.Sex = value;


                var series = new List<ReconcileDetails.SeriesDetails>();
                details.ConflictingStudyInfo.Series = series;

                var uidBroker =
					HttpContext.Current.GetSharedPersistentContext().GetBroker<IStudyIntegrityQueueUidEntityBroker>();
                var criteria = new StudyIntegrityQueueUidSelectCriteria();
                criteria.StudyIntegrityQueueKey.EqualTo(item.TheStudyIntegrityQueueItem.GetKey());

                IList<StudyIntegrityQueueUid> uids = uidBroker.Find(criteria);

                Dictionary<string, List<StudyIntegrityQueueUid>> seriesGroups = CollectionUtils.GroupBy(uids,
                                                                                                        uid =>
                                                                                                        uid.
                                                                                                            SeriesInstanceUid);

                foreach (string seriesUid in seriesGroups.Keys)
                {
                    var seriesDetails = new ReconcileDetails.SeriesDetails
                                            {
                                                SeriesInstanceUid = seriesUid,
                                                Description = seriesGroups[seriesUid][0].SeriesDescription,
                                                NumberOfInstances = seriesGroups[seriesUid].Count
                                            };
                    //seriesDetails.Modality = "N/A";
                    series.Add(seriesDetails);
                }
            }


            return details;
        }
    }
}
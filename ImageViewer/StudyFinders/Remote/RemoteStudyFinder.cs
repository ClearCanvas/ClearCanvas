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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.StudyFinders.Remote
{
    [ExtensionOf(typeof(ServiceNodeServiceProviderExtensionPoint))]
    internal class StudyFinderServiceProvider : ServiceNodeServiceProvider
    {
        private bool IsRemoteServiceNode
        {
            get
            {
                var dicomServiceNode = Context.ServiceNode as IDicomServiceNode;
                return dicomServiceNode != null && !dicomServiceNode.IsLocal;
            }
        }

        public override bool IsSupported(Type type)
        {
            return type == typeof(IStudyFinder) && IsRemoteServiceNode;
        }

        public override object GetService(Type type)
        {
            return IsSupported(type) ? new RemoteStudyFinder() : null;
        }
    }

    [ExtensionOf(typeof(StudyFinderExtensionPoint))]
    public class RemoteStudyFinder : StudyFinder
	{
		public RemoteStudyFinder()
			: base("DICOM_REMOTE")
		{
		}

        public override StudyItemList Query(QueryParameters queryParams, IApplicationEntity targetServer)
        {
            Platform.CheckForNullReference(queryParams, "queryParams");
            Platform.CheckForNullReference(targetServer, "targetServer");

            var selectedServer = targetServer.ToServiceNode();

            //.NET strings are unicode, therefore, so is all the query data.
            const string utf8 = "ISO_IR 192";
            var requestCollection = new DicomAttributeCollection { SpecificCharacterSet = utf8 };
            requestCollection[DicomTags.SpecificCharacterSet].SetStringValue(utf8);

			requestCollection[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
			requestCollection[DicomTags.StudyInstanceUid].SetStringValue("");

			requestCollection[DicomTags.PatientId].SetStringValue(queryParams["PatientId"]);
			requestCollection[DicomTags.AccessionNumber].SetStringValue(queryParams["AccessionNumber"]);
			requestCollection[DicomTags.PatientsName].SetStringValue(queryParams["PatientsName"]);
			requestCollection[DicomTags.ReferringPhysiciansName].SetStringValue(queryParams["ReferringPhysiciansName"]);
			requestCollection[DicomTags.StudyDate].SetStringValue(queryParams["StudyDate"]);
			requestCollection[DicomTags.StudyTime].SetStringValue("");
			requestCollection[DicomTags.StudyDescription].SetStringValue(queryParams["StudyDescription"]);
			requestCollection[DicomTags.PatientsBirthDate].SetStringValue("");
			requestCollection[DicomTags.ModalitiesInStudy].SetStringValue(queryParams["ModalitiesInStudy"]);
			requestCollection[DicomTags.NumberOfStudyRelatedInstances].SetStringValue("");
			requestCollection[DicomTags.InstanceAvailability].SetEmptyValue(); // must not be included in request

			requestCollection[DicomTags.PatientSpeciesDescription].SetStringValue(GetString(queryParams, "PatientSpeciesDescription"));
			var codeValue = GetString(queryParams, "PatientSpeciesCodeSequenceCodeValue");
			var codeMeaning = GetString(queryParams, "PatientSpeciesCodeSequenceCodeMeaning");
			if (codeValue != null || codeMeaning != null)
			{
				var codeSequenceMacro = new CodeSequenceMacro
				{
					CodingSchemeDesignator = "",
					CodeValue = codeValue,
					CodeMeaning = codeMeaning
				};
				requestCollection[DicomTags.PatientSpeciesCodeSequence].AddSequenceItem(codeSequenceMacro.DicomSequenceItem);
			}

			requestCollection[DicomTags.PatientBreedDescription].SetStringValue(GetString(queryParams, "PatientBreedDescription"));
			codeValue = GetString(queryParams, "PatientBreedCodeSequenceCodeValue");
			codeMeaning = GetString(queryParams, "PatientBreedCodeSequenceCodeMeaning");
			if (codeValue != null || codeMeaning != null)
			{
				var codeSequenceMacro = new CodeSequenceMacro
				{
					CodingSchemeDesignator = "",
					CodeValue = codeValue,
					CodeMeaning = codeMeaning
				};
				requestCollection[DicomTags.PatientBreedCodeSequence].AddSequenceItem(codeSequenceMacro.DicomSequenceItem);
			}

			requestCollection[DicomTags.ResponsiblePerson].SetStringValue(GetString(queryParams, "ResponsiblePerson"));
			requestCollection[DicomTags.ResponsiblePersonRole].SetStringValue("");
			requestCollection[DicomTags.ResponsibleOrganization].SetStringValue(GetString(queryParams,"ResponsibleOrganization"));

			IList<DicomAttributeCollection> results = Query(selectedServer, requestCollection);
			
			StudyItemList studyItemList = new StudyItemList();
			foreach (DicomAttributeCollection result in results)
			{
				StudyItem item = new StudyItem(result[DicomTags.StudyInstanceUid].GetString(0, ""), selectedServer);

				//TODO: add DicomField attributes to the StudyItem class (implement typeconverter for PersonName class).
				item.PatientsBirthDate = result[DicomTags.PatientsBirthDate].GetString(0, "");
				item.AccessionNumber = result[DicomTags.AccessionNumber].GetString(0, "");
				item.StudyDescription = result[DicomTags.StudyDescription].GetString(0, "");
				item.StudyDate = result[DicomTags.StudyDate].GetString(0, "");
				item.StudyTime = result[DicomTags.StudyTime].GetString(0, "");
				item.PatientId = result[DicomTags.PatientId].GetString(0, "");
				item.PatientsName = new PersonName(result[DicomTags.PatientsName].GetString(0, ""));
				item.ReferringPhysiciansName = new PersonName(result[DicomTags.ReferringPhysiciansName].GetString(0, ""));
				item.ModalitiesInStudy = DicomStringHelper.GetStringArray(result[DicomTags.ModalitiesInStudy].ToString());
				DicomAttribute attribute = result[DicomTags.NumberOfStudyRelatedInstances];
				if (!attribute.IsEmpty && !attribute.IsNull)
					item.NumberOfStudyRelatedInstances = attribute.GetInt32(0, 0);

				item.SpecificCharacterSet = result.SpecificCharacterSet;
				item.InstanceAvailability = result[DicomTags.InstanceAvailability].GetString(0, "");
				if (String.IsNullOrEmpty(item.InstanceAvailability))
					item.InstanceAvailability = "ONLINE";

				item.PatientSpeciesDescription = result[DicomTags.PatientSpeciesDescription].GetString(0, "");
				var patientSpeciesCodeSequence = result[DicomTags.PatientSpeciesCodeSequence];
				if (!patientSpeciesCodeSequence.IsNull && patientSpeciesCodeSequence.Count > 0)
				{
					var codeSequenceMacro = new CodeSequenceMacro(((DicomSequenceItem[])result[DicomTags.PatientSpeciesCodeSequence].Values)[0]);
					item.PatientSpeciesCodeSequenceCodingSchemeDesignator = codeSequenceMacro.CodingSchemeDesignator;
					item.PatientSpeciesCodeSequenceCodeValue = codeSequenceMacro.CodeValue;
					item.PatientSpeciesCodeSequenceCodeMeaning = codeSequenceMacro.CodeMeaning;
				}

				item.PatientBreedDescription = result[DicomTags.PatientBreedDescription].GetString(0, "");
				var patientBreedCodeSequence = result[DicomTags.PatientBreedCodeSequence];
				if (!patientBreedCodeSequence.IsNull && patientBreedCodeSequence.Count > 0)
				{
					var codeSequenceMacro = new CodeSequenceMacro(((DicomSequenceItem[])result[DicomTags.PatientBreedCodeSequence].Values)[0]);
					item.PatientBreedCodeSequenceCodingSchemeDesignator = codeSequenceMacro.CodingSchemeDesignator;
					item.PatientBreedCodeSequenceCodeValue = codeSequenceMacro.CodeValue;
					item.PatientBreedCodeSequenceCodeMeaning = codeSequenceMacro.CodeMeaning;
				}

				item.ResponsiblePerson = new PersonName(result[DicomTags.ResponsiblePerson].GetString(0, ""));
				item.ResponsiblePersonRole = result[DicomTags.ResponsiblePersonRole].GetString(0, "");
				item.ResponsibleOrganization = result[DicomTags.ResponsibleOrganization].GetString(0, "");
				studyItemList.Add(item);
			}

        	AuditHelper.LogQueryIssued(selectedServer.AETitle, selectedServer.ScpParameters.HostName, EventSource.CurrentUser,
        	                           EventResult.Success, SopClass.StudyRootQueryRetrieveInformationModelFindUid,
        	                           requestCollection);

			return studyItemList;
        }

        protected static IList<DicomAttributeCollection> Query(IApplicationEntity server, DicomAttributeCollection requestCollection)
		{
			//special case code for ModalitiesInStudy.  An IStudyFinder must accept a multi-valued
			//string for ModalitiesInStudy (e.g. "MR\\CT") and process it appropriately for the 
			//datasource that is being queried.  In this case (Dicom) does not allow multiple
			//query keys, so we have to execute one query per modality specified in the 
			//ModalitiesInStudy query item.

			List<string> modalityFilters = new List<string>();
			if (requestCollection.Contains(DicomTags.ModalitiesInStudy))
			{
				string modalityFilterString = requestCollection[DicomTags.ModalitiesInStudy].ToString();
				if (!String.IsNullOrEmpty(modalityFilterString))
					modalityFilters.AddRange(modalityFilterString.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries));

				if (modalityFilters.Count == 0)
					modalityFilters.Add(""); //make sure there is at least one filter, the default.
			}

			SortedList<string, DicomAttributeCollection> resultsByStudy = new SortedList<string, DicomAttributeCollection>();

			string combinedFilter = requestCollection[DicomTags.ModalitiesInStudy];

			try 
			{
				foreach (string modalityFilter in modalityFilters) 
				{
					using (StudyRootFindScu scu = new StudyRootFindScu())
					{
						requestCollection[DicomTags.ModalitiesInStudy].SetStringValue(modalityFilter);

                        IList<DicomAttributeCollection> results = scu.Find(
                            DicomServer.AETitle, server.AETitle, server.ScpParameters.HostName, server.ScpParameters.Port, requestCollection);

						scu.Join(new TimeSpan(0, 0, 0, 0, 1000));

						if(scu.Status == ScuOperationStatus.Canceled)
						{
							String message = String.Format(SR.MessageFormatRemoteServerCancelledFind, 
								scu.FailureDescription ?? "no failure description provided");
							throw new DicomException(message);
						}
						if (scu.Status == ScuOperationStatus.ConnectFailed)
						{
							String message = String.Format(SR.MessageFormatConnectionFailed,
								scu.FailureDescription ?? "no failure description provided");
							throw new DicomException(message);
						}
						else if (scu.Status == ScuOperationStatus.AssociationRejected)
						{
							String message = String.Format(SR.MessageFormatAssociationRejected,
								scu.FailureDescription ?? "no failure description provided");
							throw new DicomException(message);
						}
						if (scu.Status == ScuOperationStatus.Failed)
						{
							String message = String.Format(SR.MessageFormatQueryOperationFailed,
								scu.FailureDescription ?? "no failure description provided");
							throw new DicomException(message);
						}
						if (scu.Status == ScuOperationStatus.TimeoutExpired)
						{
							String message = String.Format(SR.MessageFormatConnectTimeoutExpired,
								scu.FailureDescription ?? "no failure description provided");
							throw new DicomException(message);
						}
						if (scu.Status == ScuOperationStatus.NetworkError)
						{
							throw new DicomException(SR.MessageUnexpectedNetworkError);
						}
						if (scu.Status == ScuOperationStatus.UnexpectedMessage)
						{
							throw new DicomException(SR.MessageUnexpectedMessage);
						}

						//if this function returns true, it means that studies came back whose 
						//modalities did not match the filter, meaning that filtering on
						//ModalitiesInStudy is not supported by that server.
						if (FilterResultsByModality(results, resultsByStudy, modalityFilter))
							break;
					}
				}

				return new List<DicomAttributeCollection>(resultsByStudy.Values);
			}
			finally
			{
				//for consistencies sake, put the original filter back.
				requestCollection[DicomTags.ModalitiesInStudy].SetStringValue(combinedFilter);
			}
		}

		protected static bool FilterResultsByModality(IList<DicomAttributeCollection> results, IDictionary<string, DicomAttributeCollection> resultsByStudy, string modalityFilter)
		{
			//if this particular filter is a wildcard filter, we won't try to be smart about running extra queries.
			bool isWildCardQuery = (modalityFilter.IndexOfAny(new char[] { '?', '*' }) >= 0);

			//if the filter is "", then everything is a match.
			bool everythingMatches = String.IsNullOrEmpty(modalityFilter);

			foreach (DicomAttributeCollection result in results)
			{
				string studyInstanceUid = result[DicomTags.StudyInstanceUid].ToString();
				if (resultsByStudy.ContainsKey(studyInstanceUid))
					continue;

				bool matchesFilter = true;

				if (!everythingMatches)
				{
					//the server does not support this optional tag at all.
					if (!result.Contains(DicomTags.ModalitiesInStudy))
					{
						everythingMatches = true;
					}
					else if (!isWildCardQuery)
					{
						string returnedModalities = result[DicomTags.ModalitiesInStudy].ToString();
						string[] returnedModalitiesArray = returnedModalities.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

						if (returnedModalitiesArray == null || returnedModalitiesArray.Length == 0)
						{
							matchesFilter = false;
						}
						else
						{
							matchesFilter = false;
							foreach (string returnedModality in returnedModalitiesArray)
							{
								if (returnedModality == modalityFilter)
								{
									matchesFilter = true;
									break;
								}
							}

							// if we get back any studies that do not contain the modality specified in the filter,
							// then that means the server does not support queries on ModalitiesInStudy, so we may
							// as well stop querying because we already have all the results.
							if (!matchesFilter)
								everythingMatches = true;
						}
					}
					else
					{
						//!!We don't actually use wildcard queries for modality, so this is not critical right now.  When C-FIND is written
						//!!a method for post-filtering with wildcards will need to be determined.  At which point this can be completed as well.
					}
				}

				if (matchesFilter)
					resultsByStudy[studyInstanceUid] = result;
			}

			return everythingMatches;
		}

		private static string GetString(QueryParameters queryParams, string key)
		{
			string sResult;
			if (queryParams.TryGetValue(key, out sResult))
				return sResult;
			return "";
		}
	}
}

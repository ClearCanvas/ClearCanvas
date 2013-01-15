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
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyFinders.Local
{
    [ExtensionOf(typeof(ServiceNodeServiceProviderExtensionPoint))]
    internal class StudyFinderServiceProvider : ServiceNodeServiceProvider
    {
        private bool IsLocalServiceNode
        {
            get
            {
                var dicomServiceNode = Context.ServiceNode as IDicomServiceNode;
                return dicomServiceNode != null && dicomServiceNode.IsLocal && StudyStore.IsSupported;
            }
        }

        public override bool IsSupported(Type type)
        {
            return type == typeof(IStudyFinder) && IsLocalServiceNode;
        }

        public override object GetService(Type type)
        {
            return IsSupported(type) ? new LocalStoreStudyFinder() : null;
        }
    }

    //TODO (Marmot):Move once IStudyFinder gets moved to Common.

    [ExtensionOf(typeof(StudyFinderExtensionPoint))]
    public class LocalStoreStudyFinder : StudyFinder
    {
        public LocalStoreStudyFinder()
			: base("DICOM_LOCAL")
        {
        }

        public override StudyItemList Query(QueryParameters queryParams, IApplicationEntity server)
        {
			Platform.CheckForNullReference(queryParams, "queryParams");

            //.NET strings are unicode, therefore, so is all the query data.
            const string utf8 = "ISO_IR 192";
            var collection = new DicomAttributeCollection {SpecificCharacterSet = utf8};
            collection[DicomTags.SpecificCharacterSet].SetStringValue(utf8);

            collection[DicomTags.QueryRetrieveLevel].SetStringValue("STUDY");
            collection[DicomTags.PatientId].SetStringValue(queryParams["PatientId"]);
            collection[DicomTags.AccessionNumber].SetStringValue(queryParams["AccessionNumber"]);
            collection[DicomTags.PatientsName].SetStringValue(queryParams["PatientsName"]);
			collection[DicomTags.ReferringPhysiciansName].SetStringValue(queryParams["ReferringPhysiciansName"]);
			collection[DicomTags.StudyDate].SetStringValue(queryParams["StudyDate"]);
			collection[DicomTags.StudyTime].SetStringValue("");
			collection[DicomTags.StudyDescription].SetStringValue(queryParams["StudyDescription"]);
        	collection[DicomTags.PatientsBirthDate].SetStringValue("");
            collection[DicomTags.ModalitiesInStudy].SetStringValue(queryParams["ModalitiesInStudy"]);
			collection[DicomTags.StudyInstanceUid].SetStringValue(queryParams["StudyInstanceUid"]);
			collection[DicomTags.NumberOfStudyRelatedInstances].SetStringValue("");
			collection[DicomTags.InstanceAvailability].SetEmptyValue(); // must not be included in request

			collection[DicomTags.PatientSpeciesDescription].SetStringValue(GetString(queryParams, "PatientSpeciesDescription"));
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
				collection[DicomTags.PatientSpeciesCodeSequence].AddSequenceItem(codeSequenceMacro.DicomSequenceItem);
            }

			collection[DicomTags.PatientBreedDescription].SetStringValue(GetString(queryParams, "PatientBreedDescription"));
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
				collection[DicomTags.PatientBreedCodeSequence].AddSequenceItem(codeSequenceMacro.DicomSequenceItem);
            }

			collection[DicomTags.ResponsiblePerson].SetStringValue(GetString(queryParams, "ResponsiblePerson"));
			collection[DicomTags.ResponsiblePersonRole].SetStringValue("");
			collection[DicomTags.ResponsibleOrganization].SetStringValue(GetString(queryParams, "ResponsibleOrganization"));

            var localServer = ServerDirectory.GetLocalServer();
            var studyItemList = new StudyItemList();
			using (var context = new DataAccessContext())
			{
				foreach (DicomAttributeCollection result in context.GetStudyStoreQuery().Query(collection))
				{
                    var item = new StudyItem(result[DicomTags.StudyInstanceUid].ToString(), localServer);
					item.SpecificCharacterSet = result.SpecificCharacterSet;
					item.PatientId = result[DicomTags.PatientId].ToString();
					item.PatientsName = new PersonName(result[DicomTags.PatientsName].ToString());
					item.ReferringPhysiciansName = new PersonName(result[DicomTags.ReferringPhysiciansName].GetString(0, ""));
					item.PatientsBirthDate = result[DicomTags.PatientsBirthDate].ToString();
					item.StudyDate = result[DicomTags.StudyDate].ToString();
					item.StudyTime = result[DicomTags.StudyTime].ToString();
					item.StudyDescription = result[DicomTags.StudyDescription].ToString();
					item.ModalitiesInStudy = DicomStringHelper.GetStringArray(result[DicomTags.ModalitiesInStudy].ToString());
					item.AccessionNumber = result[DicomTags.AccessionNumber].ToString();
					item.NumberOfStudyRelatedInstances = result[DicomTags.NumberOfStudyRelatedInstances].GetInt32(0, 0);
					item.InstanceAvailability = result[DicomTags.InstanceAvailability].GetString(0, "");
					if (String.IsNullOrEmpty(item.InstanceAvailability))
						item.InstanceAvailability = "ONLINE";

					item.PatientSpeciesDescription = result[DicomTags.PatientSpeciesDescription].GetString(0, "");
					var patientSpeciesCodeSequence = result[DicomTags.PatientSpeciesCodeSequence];
					if (!patientSpeciesCodeSequence.IsNull && patientSpeciesCodeSequence.Count > 0)
					{
						var codeSequenceMacro = new CodeSequenceMacro(((DicomSequenceItem[]) result[DicomTags.PatientSpeciesCodeSequence].Values)[0]);
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
			}

			AuditHelper.LogQueryIssued(null, null, EventSource.CurrentUser, EventResult.Success,
									   SopClass.StudyRootQueryRetrieveInformationModelFindUid, collection);

			return studyItemList;
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

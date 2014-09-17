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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Annotations.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class PatientAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public PatientAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.Patient", new AnnotationResourceResolver(typeof(PatientAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.EthnicGroup",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.EthnicGroup),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.PatientComments",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.PatientComments),
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.PatientId",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.PatientId; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.OtherPatientIds",
						resolver,
						delegate(Frame frame)
							{
								var patientIds = new List<string>();
								DicomAttribute attribute;
								if (frame.ParentImageSop.TryGetAttribute(DicomTags.OtherPatientIds, out attribute))
									patientIds.AddRange(DicomStringHelper.GetStringArray(attribute));

								if (frame.ParentImageSop.TryGetAttribute(DicomTags.OtherPatientIdsSequence, out attribute) && !attribute.IsEmpty && !attribute.IsNull)
								{
									var sqAttr = (DicomAttributeSQ)attribute;
									for (var i = 0; i < sqAttr.Count; i++)
									{
										var attr = sqAttr[i][DicomTags.PatientId];
										if (attr != null && !string.IsNullOrEmpty(attr))
											patientIds.Add(attr);
									}
								}

								return DicomStringHelper.GetDicomStringArray(patientIds.Distinct());
							},
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.PatientsBirthDate",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.PatientsBirthDate; },
						DicomDataFormatHelper.DateFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.PatientsBirthTime",
						resolver,
						FrameDataRetrieverFactory.GetStringRetriever(DicomTags.PatientsBirthTime),
						DicomDataFormatHelper.TimeFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<PersonName>
					(
						"Dicom.Patient.PatientsName",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.PatientsName; },
						DicomDataFormatHelper.PersonNameFormatter
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.PatientsSex",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.PatientsSex; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<PersonName>
					(
						"Dicom.Patient.ResponsiblePerson",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.ResponsiblePerson; },
						DicomDataFormatHelper.PersonNameFormatter
					)
				);

			_annotationItems.Add
				(
					new DicomAnnotationItem<string>
					(
						"Dicom.Patient.ResponsibleOrganization",
						resolver,
						delegate(Frame frame) { return frame.ParentImageSop.ResponsibleOrganization; },
						DicomDataFormatHelper.RawStringFormat
					)
				);

			_annotationItems.Add
				(
					new CodeSequenceAnnotationItem
					(
						"Dicom.Patient.PatientSpecies",
						resolver,
						DicomTags.PatientSpeciesCodeSequence,
						DicomTags.PatientSpeciesDescription
					)
				);

			_annotationItems.Add
				(
					new CodeSequenceAnnotationItem
					(
						"Dicom.Patient.PatientBreed",
						resolver,
						DicomTags.PatientBreedCodeSequence,
						DicomTags.PatientBreedDescription
					)
				);
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}
	}
}

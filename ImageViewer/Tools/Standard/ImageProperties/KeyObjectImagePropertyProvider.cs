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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.Dicom.Iod.Macros.DocumentRelationship;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.ImageProperties
{
	[ExtensionOf(typeof (ImagePropertyProviderExtensionPoint))]
	public class KeyObjectImagePropertyProvider : IImagePropertyProvider
	{
		public KeyObjectImagePropertyProvider()
		{
		}

		#region IImagePropertyProvider Members

		public IImageProperty[] GetProperties(IPresentationImage image)
		{
			List<IImageProperty> properties = new List<IImageProperty>();

			if (image != null && image.ParentDisplaySet != null)
			{
				IImageViewer viewer = image.ImageViewer;
				if (viewer != null)
				{
					IDicomDisplaySetDescriptor descriptor = image.ParentDisplaySet.Descriptor as IDicomDisplaySetDescriptor;
					if (descriptor != null && descriptor.SourceSeries != null)
					{
						string uid = descriptor.SourceSeries.SeriesInstanceUid;
						if (!String.IsNullOrEmpty(uid))
						{
							StudyTree studyTree = viewer.StudyTree;
							Series keyObjectSeries = studyTree.GetSeries(uid);
							if (keyObjectSeries != null && keyObjectSeries.Sops.Count > 0)
							{
								Sop keyObjectSop = keyObjectSeries.Sops[0];
								if (keyObjectSop.SopClassUid == SopClass.KeyObjectSelectionDocumentStorageUid)
								{
									KeyObjectSelectionDocumentIod iod = new KeyObjectSelectionDocumentIod(keyObjectSop);
									SrDocumentContentModuleIod content = iod.SrDocumentContent;
									GeneralEquipmentModuleIod equipment = iod.GeneralEquipment;

									if (content != null)
									{
										string codeValue = "";
										CodeSequenceMacro conceptSequence = content.ConceptNameCodeSequence;
										if (conceptSequence != null)
										{
											KeyObjectSelectionDocumentTitle documentTitle = KeyObjectSelectionDocumentTitleContextGroup.LookupTitle(conceptSequence);
											if (documentTitle != null)
												codeValue = documentTitle.ToString();
										}

										string documentDescription = "";
										IContentSequence[] contentSequences = content.ContentSequence ?? new IContentSequence[0];
										for (int i = contentSequences.Length - 1; i >= 0; --i)
										{
											IContentSequence contentSequence = contentSequences[i];
											CodeSequenceMacro sequenceMacro = contentSequence.ConceptNameCodeSequence;
											if (sequenceMacro != null && sequenceMacro.CodeValue == KeyObjectSelectionCodeSequences.KeyObjectDescription.CodeValue)
											{
												documentDescription = contentSequence.TextValue;
												break;
											}
										}

										properties.Add(
											new ImageProperty("KeyImageDocumentTitle",
															  SR.CategoryKeyImageSeries,
											                  SR.NameKeyImageDocumentTitle,
											                  SR.DescriptionKeyImageDocumentTitle,
											                  codeValue));

										properties.Add(
											new ImageProperty("KeyImageDocumentDescription",
															  SR.CategoryKeyImageSeries,
											                  SR.NameKeyImageDocumentDescription,
											                  SR.DescriptionKeyImageDocumentDescription,
											                  documentDescription));

										properties.Add(
											new ImageProperty("KeyImageEquipmentManufacturer",
															  SR.CategoryKeyImageEquipment,
											                  SR.NameManufacturer,
											                  SR.DescriptionManufacturer,
											                  equipment.Manufacturer ?? ""));
										properties.Add(
											new ImageProperty("KeyImageEquipmentManufacturersModelName", 
															  SR.CategoryKeyImageEquipment,
											                  SR.NameManufacturersModelName,
											                  SR.DescriptionManufacturersModelName,
											                  equipment.ManufacturersModelName ?? ""));
										properties.Add(
											new ImageProperty("KeyImageEquipmentSoftwareVersions",
															  SR.CategoryKeyImageEquipment,
											                  SR.NameSoftwareVersions,
											                  SR.DescriptionSoftwareVersions,
											                  equipment.SoftwareVersions ?? ""));
									}
								}
							}
						}
					}
				}
			}

			return properties.ToArray();
		}

		#endregion
	}
}
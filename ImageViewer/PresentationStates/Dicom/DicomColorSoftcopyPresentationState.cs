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
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Iods;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	[Cloneable]
	internal sealed class DicomColorSoftcopyPresentationState : DicomSoftcopyPresentationStateBase<DicomColorPresentationImage>
	{
		public static readonly SopClass SopClass = SopClass.ColorSoftcopyPresentationStateStorageSopClass;

		public DicomColorSoftcopyPresentationState() : base(SopClass) {}

		public DicomColorSoftcopyPresentationState(DicomFile dicomFile) : base(SopClass, dicomFile) {}

		public DicomColorSoftcopyPresentationState(DicomAttributeCollection dataSource) : base(SopClass, dataSource) {}

		public DicomColorSoftcopyPresentationState(IDicomAttributeProvider dataSource) : base(SopClass, ShallowCopyDataSource(dataSource)) {}

		private DicomColorSoftcopyPresentationState(DicomColorSoftcopyPresentationState source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		#region Serialization Support

		protected override void PerformTypeSpecificSerialization(DicomPresentationImageCollection<DicomColorPresentationImage> images)
		{
			IOverlayMapping overlayMapping;
			ColorSoftcopyPresentationStateIod iod = new ColorSoftcopyPresentationStateIod(base.DataSet);
			this.SerializePresentationStateRelationship(iod.PresentationStateRelationship, images);
			this.SerializePresentationStateShutter(iod.PresentationStateShutter);
			this.SerializeDisplayShutter(iod.DisplayShutter, images);
			this.SerializeOverlayPlane(iod.OverlayPlane, out overlayMapping, images);
			this.SerializeOverlayActivation(iod.OverlayActivation, overlayMapping, images);
			this.SerializeBitmapDisplayShutter(iod.BitmapDisplayShutter, overlayMapping, images);
			this.SerializeDisplayedArea(iod.DisplayedArea, images);
			this.SerializeGraphicAnnotation(iod.GraphicAnnotation, images);
			this.SerializeSpatialTransform(iod.SpatialTransform, images);
			this.SerializeGraphicLayer(iod.GraphicLayer, images);
			this.SerializeIccProfile(iod.IccProfile);
		}

		private void SerializeIccProfile(IccProfileModuleIod module)
		{
			// NOTE: Not supported
		}

		#endregion

		#region Deserialization Support

		protected override void PerformTypeSpecificDeserialization(DicomPresentationImageCollection<DicomColorPresentationImage> images)
		{
			ColorSoftcopyPresentationStateIod iod = new ColorSoftcopyPresentationStateIod(base.DataSet);

			foreach (DicomColorPresentationImage image in images)
			{
				RectangleF displayedArea;
				this.DeserializeSpatialTransform(iod.SpatialTransform, image);
				this.DeserializeDisplayedArea(iod.DisplayedArea, out displayedArea, image);
				this.DeserializeGraphicLayer(iod.GraphicLayer, image);
				this.DeserializeGraphicAnnotation(iod.GraphicAnnotation, displayedArea, image);
				this.DeserializeOverlayPlane(iod.OverlayPlane, image);
				this.DeserializeOverlayActivation(iod.OverlayActivation, image);
				this.DeserializeBitmapDisplayShutter(iod.BitmapDisplayShutter, image);
				this.DeserializeDisplayShutter(iod.DisplayShutter, image);
			}
		}

		#endregion

		#region IDicomAttributeProvider Copy Method

		private static DicomAttributeCollection ShallowCopyDataSource(IDicomAttributeProvider source)
		{
			if (source is DicomAttributeCollection)
				return (DicomAttributeCollection) source;

			// a shallow copy is sufficient - even if the provider is a sop object that can be user-disposed, it
			// provides an indexer to get dicom attribute objects which will not be disposed if we have a reference to it
			DicomAttributeCollection collection = new DicomAttributeCollection();

			foreach (uint tag in PatientModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in ClinicalTrialSubjectModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in GeneralStudyModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in PatientStudyModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in ClinicalTrialStudyModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in GeneralSeriesModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in ClinicalTrialSeriesModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in PresentationSeriesModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in GeneralEquipmentModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in PresentationStateIdentificationModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in PresentationStateRelationshipModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in PresentationStateShutterModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in DisplayShutterModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in BitmapDisplayShutterModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in OverlayPlaneModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in OverlayActivationModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in DisplayedAreaModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in GraphicAnnotationModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in SpatialTransformModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in GraphicLayerModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in IccProfileModuleIod.DefinedTags)
				collection[tag] = source[tag];

			foreach (uint tag in SopCommonModuleIod.DefinedTags)
				collection[tag] = source[tag];

			return collection;
		}

		#endregion
	}
}
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

using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// The interface for all DICOM-based Presentation Images.
	/// </summary>
	public interface IDicomPresentationImage :
		IPresentationImage,
		IImageSopProvider,
		IAnnotationLayoutProvider,
		IImageGraphicProvider,
		IApplicationGraphicsProvider,
		IOverlayGraphicsProvider,
		ISpatialTransformProvider,
		IPresentationStateProvider,
		IPatientPresentationProvider,
		IPatientCoordinateMappingProvider
	{
		/// <summary>
		/// Gets direct access to the presentation image's collection of domain-level graphics.
		/// Consider using <see cref="DicomGraphicsPlane.GetDicomGraphicsPlane(IDicomPresentationImage)"/> instead.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Use <see cref="DicomGraphics"/> to add DICOM-defined graphics that you want to
		/// overlay the image at the domain-level. These graphics are rendered
		/// before any <see cref="IApplicationGraphicsProvider.ApplicationGraphics"/>
		/// and before any <see cref="IOverlayGraphicsProvider.OverlayGraphics"/>.
		/// </para>
		/// <para>
		/// This property gives direct access to all the domain-level graphics of a DICOM presentation image.
		/// However, most of the graphics concepts defined in the DICOM Standard are already supported
		/// by the <see cref="DicomGraphicsPlane"/> which inserts itself into this domain-level collection.
		/// Consider using <see cref="DicomGraphicsPlane.GetDicomGraphicsPlane(IDicomPresentationImage)"/> to get
		/// a reference to a usable DicomGraphicsPlane object instead, since that provides all the logical support
		/// for layer activation and shutters in addition to enumerating all domain-level graphics. This property
		/// may change, be deprecated, and even outright removed in a future framework release.
		/// </para>
		/// </remarks>
		GraphicCollection DicomGraphics { get; }
	}
}
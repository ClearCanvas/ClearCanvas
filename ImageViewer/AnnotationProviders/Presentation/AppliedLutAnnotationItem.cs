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
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	/// <summary>
	/// Describes whatever Lut is currently applied to a presentation image.
	/// </summary>
	/// <remarks>
	/// At first glance, you might think this belongs in the Dicom namespace within this project.
	/// However, the information in the Dicom header only applies to the initial presentation of
	/// the image and is not representative of the current state of the image in the viewport.
	/// The user could have changed the W/L, applied a custom Data Lut, or a Lut from a related
	/// Grayscale Presentation State object could be applied.
	/// </remarks>
	internal sealed class AppliedLutAnnotationItem : AnnotationItem
	{
		public AppliedLutAnnotationItem()
			: base("Presentation.AppliedLut", new AnnotationResourceResolver(typeof(AppliedLutAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage == null)
				return String.Empty;

			IVoiLutProvider image = presentationImage as IVoiLutProvider;

			if (image == null || !image.VoiLutManager.Enabled)
				return String.Empty;

			IVoiLut voiLut = image.VoiLutManager.VoiLut;
			if (voiLut == null)
				return String.Empty;

			return voiLut.GetDescription();
		}
	}
}

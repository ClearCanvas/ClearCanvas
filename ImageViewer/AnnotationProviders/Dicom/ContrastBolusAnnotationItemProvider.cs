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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	/// <summary>
	/// Defines annotation items used in the Contrast/Bolus Module (C.7.6.4) and the Enhanced Contrast/Bolus Module (C.7.6.4b).
	/// </summary>
	[ExtensionOf(typeof (AnnotationItemProviderExtensionPoint))]
	public class ContrastBolusAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public ContrastBolusAnnotationItemProvider()
			: base("AnnotationItemProviders.Dicom.ContrastBolus", new AnnotationResourceResolver(typeof (ContrastBolusAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>();

			AnnotationResourceResolver resolver = new AnnotationResourceResolver(this);

			_annotationItems.Add
				(
					new CodeSequenceAnnotationItem
						(
						"Dicom.ContrastBolus.Agent",
						resolver,
						DicomTags.ContrastBolusAgentSequence,
						DicomTags.ContrastBolusAgent
						)
				);

			// N.B.: The contrast/bolus items defined here are only for those that are defined as part of (enhanced) contrast/bolus module C.7.6.4 and C.7.6.4b
			// Annotation items for contrast/bolus tags in the X-Ray 3D Shared Acquisition macro should be defined separately in the DX or another X-Ray specific provider
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}
	}
}
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
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[ExtensionOf(typeof (AnnotationItemProviderExtensionPoint))]
	public class FusionImageAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;

		public FusionImageAnnotationItemProvider()
			: base("AnnotationItemProviders.AdvancedImaging.Fusion", SR.LabelAdvancedImageFusion)
		{
			_annotationItems = new List<IAnnotationItem>();
			_annotationItems.Add(new MismatchedFrameOfReferenceFusionImageAnnotationItem());
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}

		internal class MismatchedFrameOfReferenceFusionImageAnnotationItem : AnnotationItem
		{
			public MismatchedFrameOfReferenceFusionImageAnnotationItem()
				: base("AdvancedImaging.Fusion.MismatchedFrameOfReference", @"LabelMismatchedFrameOfReference", @"LabelMismatchedFrameOfReference") {}

			public override string GetAnnotationText(IPresentationImage presentationImage)
			{
				if (presentationImage is FusionPresentationImage)
				{
					var fusionImage = (FusionPresentationImage) presentationImage;
					if (fusionImage.OverlayFrameData.BaseFrame.FrameOfReferenceUid != fusionImage.OverlayFrameData.OverlayFrameOfReferenceUid)
					{
						return SR.CodeMismatchedFrameOfReference;
					}
				}
				return string.Empty;
			}
		}
	}
}
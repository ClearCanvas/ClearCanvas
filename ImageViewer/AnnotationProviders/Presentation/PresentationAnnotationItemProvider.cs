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
//using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	[ExtensionOf(typeof(AnnotationItemProviderExtensionPoint))]
	public class PresentationAnnotationItemProvider : AnnotationItemProvider
	{
		private readonly List<IAnnotationItem> _annotationItems;
		
		public PresentationAnnotationItemProvider()
			: base("AnnotationItemProviders.Presentation", new AnnotationResourceResolver(typeof(PresentationAnnotationItemProvider).Assembly))
		{
			_annotationItems = new List<IAnnotationItem>
			                       {
			                           new ZoomAnnotationItem(),
			                           new AppliedLutAnnotationItem(),
			                           new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Left),
			                           new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Top),
			                           new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Right),
			                           new DirectionalMarkerAnnotationItem(DirectionalMarkerAnnotationItem.ImageEdge.Bottom),
			                           new DFOVAnnotationItem(),
			                           new ImageCalibrationAnnotationItem(),
			                           new DisplaySetDescriptionAnnotationItem(),
			                           new DisplaySetNumberAnnotationItem(),
									   new LossyImagePresentationAnnotationItem()
			                       };
		}

		public override IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return _annotationItems;
		}
	}
}

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
using System.Linq;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation
{
	internal class DisplaySetDescriptionAnnotationItem : AnnotationItem
	{
		public DisplaySetDescriptionAnnotationItem()
			: base("Presentation.DisplaySetDescription", new AnnotationResourceResolver(typeof(DisplaySetDescriptionAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage != null && presentationImage.ParentDisplaySet != null)
				return presentationImage.ParentDisplaySet.Description ?? "";
			else
				return "";
		}
	}
	internal class DisplaySetNumberAnnotationItem : AnnotationItem
	{
		public DisplaySetNumberAnnotationItem()
			: base("Presentation.DisplaySetNumber", new AnnotationResourceResolver(typeof(DisplaySetNumberAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage != null && presentationImage.ParentDisplaySet != null)
			{
				if (presentationImage.ParentDisplaySet.ParentImageSet != null)
				{
					return String.Format(SR.FormatDisplaySetNumberAndCount, presentationImage.ParentDisplaySet.Number,
						presentationImage.ParentDisplaySet.ParentImageSet.DisplaySets.Count);
				}
				else if (presentationImage.ImageViewer != null)
				{
					// try to find a corresponding display set in the logical workspace with a matching UID, and let display set number reflect that
					// this happens particularly when the image is part of a generated display set derived from a display set based on stored DICOM SOP instances
					var displaySetUid = presentationImage.ParentDisplaySet.Uid ?? string.Empty;
					var sourceDisplaySet = presentationImage.ImageViewer.LogicalWorkspace.ImageSets.SelectMany(s => s.DisplaySets).FirstOrDefault(s => s.Uid == displaySetUid);
					if (sourceDisplaySet != null)
						return string.Format(SR.FormatDisplaySetNumberAndCount, sourceDisplaySet.Number, sourceDisplaySet.ParentImageSet.DisplaySets.Count);
				}
				return presentationImage.ParentDisplaySet.Number.ToString(System.Globalization.CultureInfo.InvariantCulture);
			}
			else
			{
				return "";
			}
		}
	}
}

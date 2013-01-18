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
using System.Text;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal class InstanceNumberAnnotationItem : AnnotationItem
	{
		public InstanceNumberAnnotationItem()
			: base("Dicom.GeneralImage.InstanceNumber", new AnnotationResourceResolver(typeof(InstanceNumberAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			IImageSopProvider provider = presentationImage as IImageSopProvider;
			if (provider == null)
				return "";

            Frame frame = provider.Frame;

            string str;
			
			if (frame.ParentImageSop.ParentSeries != null)
			{
				// if the image is part of the study tree, let the image count reflect the number of instances in the parent series
				str = String.Format(SR.FormatImageNumberAndCount, frame.ParentImageSop.InstanceNumber, frame.ParentImageSop.ParentSeries.Sops.Count);
			}
			else if (presentationImage.ParentDisplaySet != null)
			{
				// if the image is not part of the study tree, fallback to using the number of images in the display set
				// this happens particularly when the image is part of a generated display set rather than a stored DICOM SOP instance
				str = string.Format(SR.FormatImageNumberAndCount, frame.ParentImageSop.InstanceNumber, presentationImage.ParentDisplaySet.PresentationImages.Count);
			}
			else
			{
				str = frame.ParentImageSop.InstanceNumber.ToString(System.Globalization.CultureInfo.InvariantCulture);
			}

            if (frame.ParentImageSop.NumberOfFrames > 1)
            {
                string frameString = String.Format(
					SR.FormatFrameNumberAndCount,
                    frame.FrameNumber,
                    frame.ParentImageSop.NumberOfFrames);

                str += " " + frameString;
            }

            return str;
        }
	}
}

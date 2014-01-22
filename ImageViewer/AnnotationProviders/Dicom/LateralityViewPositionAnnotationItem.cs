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
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal class LateralityViewPositionAnnotationItem : AnnotationItem
	{
		private readonly bool _showLaterality;
		private readonly bool _showViewPosition;

		public LateralityViewPositionAnnotationItem(string identifier, bool showLaterality, bool showViewPosition)
			: base(identifier, new AnnotationResourceResolver(typeof (LateralityViewPositionAnnotationItem).Assembly))
		{
			Platform.CheckTrue(showViewPosition || showLaterality, "At least one of showLaterality and showViewPosition must be true.");

			_showLaterality = showLaterality;
			_showViewPosition = showViewPosition;
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			string nullString = SR.ValueNil;

			IImageSopProvider provider = presentationImage as IImageSopProvider;
			if (provider == null)
				return "";

			string laterality = null;
			if (_showLaterality)
				laterality = provider.Frame.Laterality;

			string viewPosition = null;
			if (_showViewPosition)
			{
				viewPosition = provider.Frame.ViewPosition;
				if (string.IsNullOrEmpty(viewPosition))
				{
					//TODO: later, we could translate to ACR MCQM equivalent, at least for mammo.
					viewPosition = CodeSequenceAnnotationItem.FormatCodeSequence(provider.Frame, DicomTags.ViewCodeSequence, null);
				}
			}

			string str = "";
			if (_showLaterality && _showViewPosition)
			{
				if (string.IsNullOrEmpty(laterality))
					laterality = nullString;
				if (string.IsNullOrEmpty(viewPosition))
					viewPosition = nullString;

				if (laterality == nullString && viewPosition == nullString)
					str = ""; // if both parts are null then just show one hyphen (rather than -/-)
				else
					str = String.Format(SR.FormatLateralityViewPosition, laterality, viewPosition);
			}
			else if (_showLaterality)
			{
				str = laterality;
			}
			else if (_showViewPosition)
			{
				str = viewPosition;
			}

			return str;
		}
	}
}
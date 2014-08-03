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

using System.Linq;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.Configuration
{
	internal sealed class ToolModalityBehaviorHelper
	{
		internal const string ModalityBreastTomosynthesis = @"MG (BT)";

		private readonly IImageViewer _imageViewer;

		public ToolModalityBehaviorHelper(IImageViewer imageViewer)
		{
			_imageViewer = imageViewer;
		}

		public ToolModalityBehavior Behavior
		{
			get { return ToolSettings.DefaultInstance.CachedToolModalityBehavior.GetEntryOrDefault(SelectedModality); }
		}

		private string SelectedModality
		{
			get
			{
				var selectedImage = _imageViewer.SelectedPresentationImage;
				if (selectedImage == null)
					return string.Empty;

				var parentDisplaySet = selectedImage.ParentDisplaySet;
				if (parentDisplaySet != null)
				{
					var dicomDescriptor = parentDisplaySet.Descriptor as IDicomDisplaySetDescriptor;
					if (dicomDescriptor != null && dicomDescriptor.SourceSeries != null)
					{
						var series = _imageViewer.StudyTree.GetSeries(dicomDescriptor.SourceSeries.SeriesInstanceUid ?? string.Empty);
						if (series != null)
						{
							var firstSop = series.Sops.FirstOrDefault();
							if (firstSop != null)
							{
								var sopClassUid = firstSop.SopClassUid;
								if (sopClassUid == SopClass.KeyObjectSelectionDocumentStorageUid)
									return @"KO";
								else if (sopClassUid == SopClass.BreastTomosynthesisImageStorageUid)
									return ModalityBreastTomosynthesis;
							}
						}
					}
				}

				var imageSopProvider = selectedImage as IImageSopProvider;
				if (imageSopProvider == null) return string.Empty;
				return imageSopProvider.Sop.SopClassUid == SopClass.BreastTomosynthesisImageStorageUid ? ModalityBreastTomosynthesis : imageSopProvider.ImageSop.Modality;
			}
		}
	}
}
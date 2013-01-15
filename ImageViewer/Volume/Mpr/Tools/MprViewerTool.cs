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

using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	public abstract class MprViewerTool : MouseImageViewerTool
	{
		protected MprViewerTool() {}

		public new MprViewerComponent ImageViewer
		{
			get { return base.ImageViewer as MprViewerComponent; }
		}

		protected MprSliceSop SelectedMprSliceSop
		{
			get
			{
				IImageSopProvider selectedImageSopProvider = base.SelectedImageSopProvider;
				if (selectedImageSopProvider != null)
					return selectedImageSopProvider.ImageSop as MprSliceSop;
				return null;
			}
		}

		protected IMprSliceSet SelectedMprSliceSet
		{
			get
			{
				MprSliceSop selectedMprSliceSop = this.SelectedMprSliceSop;
				if (selectedMprSliceSop != null)
					return selectedMprSliceSop.Parent;
				return null;
			}
		}

		protected IMprVolume SelectedMprVolume
		{
			get
			{
				IMprSliceSet selectedMprSliceSet = this.SelectedMprSliceSet;
				if (selectedMprSliceSet != null)
					return selectedMprSliceSet.Parent;
				return null;
			}
		}
	}
}
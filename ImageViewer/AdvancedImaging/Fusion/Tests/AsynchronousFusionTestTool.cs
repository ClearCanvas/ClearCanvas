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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	[MenuAction("alpha", "global-menus/MenuDebug/MenuFusion/(Async) Unload PET Volume", "DropThisVolume")]
	[MenuAction("bravo", "global-menus/MenuDebug/MenuFusion/(Async) Unload Selected Fused Image", "DropThisSlice")]
	[MenuAction("charlie", "global-menus/MenuDebug/MenuFusion/(Async) Unload Fused Display Set", "DropAllSlices")]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	internal class AsynchronousFusionTestTool : ImageViewerTool
	{
		public void DropThisVolume()
		{
			var image = base.SelectedPresentationImage as FusionPresentationImage;
			if (image != null)
			{
				Unload(image.OverlayFrameData.OverlayData);
			}
		}

		public void DropThisSlice()
		{
			var image = base.SelectedPresentationImage as FusionPresentationImage;
			if (image != null)
			{
				Unload(image.OverlayFrameData);
			}
		}

		public void DropAllSlices()
		{
			var image = base.SelectedPresentationImage as FusionPresentationImage;
			if (image != null)
			{
				foreach (FusionPresentationImage singleImage in base.SelectedPresentationImage.ParentDisplaySet.PresentationImages)
				{
					Unload(singleImage.OverlayFrameData);
				}
			}
		}

		private static void Unload(ILargeObjectContainer loc)
		{
			loc.Unload();
		}
	}
}

#endif
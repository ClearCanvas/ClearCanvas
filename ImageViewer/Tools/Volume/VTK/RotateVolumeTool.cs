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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using vtk;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	[MouseToolButton(XMouseButtons.Left, false)]
	[MenuAction("activate", "imageviewer-contextmenu/Rotate Volume", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarsVolume/RotateVolume", "Select", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[Tooltip("activate", "Rotate Volume")]
	[IconSet("activate", "Icons.CreateVolumeToolSmall.png", "Icons.CreateVolumeToolLarge.png", "Icons.CreateVolumeToolLarge.png")]
	[GroupHint("activate", "Tools.VolumeImage.Manipulation.Rotate")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class RotateVolumeTool : MouseImageViewerTool
	{
		public RotateVolumeTool()
		{
			this.CursorToken = new CursorToken("Icons.CreateVolumeToolSmall.png", this.GetType().Assembly);
		}

		private vtkGenericRenderWindowInteractor GetInteractor(IPresentationImage selectedImage)
		{
			if (selectedImage == null)
				return null;

			VolumePresentationImage image = selectedImage as VolumePresentationImage;

			if (image == null)
				return null;

			VolumePresentationImageRenderer renderer = image.ImageRenderer as VolumePresentationImageRenderer;

			if (renderer == null)
				return null;

			vtkGenericRenderWindowInteractor interactor = vtkGenericRenderWindowInteractor.SafeDownCast(renderer.Interactor);

			return interactor;
		}

		#region IMouseButtonHandler Members

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			IPresentationImage selectedImage = this.Context.Viewer.SelectedPresentationImage;

			vtkGenericRenderWindowInteractor interactor = GetInteractor(selectedImage);

			if (interactor == null)
				return false;

			interactor.SetEventPositionFlipY(mouseInformation.Location.X, mouseInformation.Location.Y);
			interactor.LeftButtonPressEvent();

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			IPresentationImage selectedImage = this.Context.Viewer.SelectedPresentationImage;

			vtkGenericRenderWindowInteractor interactor = GetInteractor(selectedImage);

			if (interactor == null)
				return false;

			interactor.SetEventPositionFlipY(mouseInformation.Location.X, mouseInformation.Location.Y);
			interactor.MouseMoveEvent();

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			IPresentationImage selectedImage = this.Context.Viewer.SelectedPresentationImage;

			vtkGenericRenderWindowInteractor interactor = GetInteractor(selectedImage);

			if (interactor == null)
				return false;

			interactor.SetEventPositionFlipY(mouseInformation.Location.X, mouseInformation.Location.Y);
			interactor.LeftButtonReleaseEvent();

			return false;
		}

		public override void Cancel()
		{
			IPresentationImage selectedImage = this.Context.Viewer.SelectedPresentationImage;

			vtkGenericRenderWindowInteractor interactor = GetInteractor(selectedImage);

			if (interactor == null)
				return;

			interactor.LeftButtonReleaseEvent();
		}

		#endregion
	}
}
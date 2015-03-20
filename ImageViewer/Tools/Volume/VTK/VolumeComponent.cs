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
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Volume.VTK
{
	/// <summary>
	/// Extension point for views onto <see cref="VolumeComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class VolumeComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	/// <summary>
	/// VolumeComponent class
	/// </summary>
	[AssociateView(typeof (VolumeComponentViewExtensionPoint))]
	public class VolumeComponent : ImageViewerToolComponent
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public VolumeComponent(IDesktopWindow desktopWindow)
			: base(desktopWindow) {}

		public bool CreateVolumeEnabled
		{
			get
			{
				if (this.ImageViewer == null)
				{
					return false;
				}
				else
				{
					if (this.ImageViewer.SelectedTile == null)
						return false;
					else
						return !(this.ImageViewer.SelectedPresentationImage is VolumePresentationImage);
				}
			}
		}

		public bool VolumeSettingsEnabled
		{
			get
			{
				if (this.ImageViewer == null)
				{
					return false;
				}
				else
				{
					if (this.ImageViewer.SelectedTile == null)
						return false;
					else
						return this.ImageViewer.SelectedPresentationImage is VolumePresentationImage;
				}
			}
		}

		public GraphicCollection VolumeGraphics
		{
			get
			{
				if (this.ImageViewer == null)
					return null;

				if (this.ImageViewer.SelectedPresentationImage == null)
					return null;

				IAssociatedTissues volume = this.ImageViewer.SelectedPresentationImage as IAssociatedTissues;

				if (volume == null)
					return null;

				return volume.TissueLayers;
			}
		}

		public void CreateVolume()
		{
			if (this.ImageViewer == null)
				return;

			if (this.ImageViewer.SelectedImageBox == null)
				return;

			IDisplaySet selectedDisplaySet = this.ImageViewer.SelectedImageBox.DisplaySet;
			VolumePresentationImage image = new VolumePresentationImage(selectedDisplaySet);

			AddTissueLayers(image);

			IDisplaySet displaySet = new DisplaySet(String.Format("{0} (3D)", selectedDisplaySet.Name), String.Format("VTK.{0}", Guid.NewGuid().ToString()));
			displaySet.PresentationImages.Add(image);
			this.ImageViewer.LogicalWorkspace.ImageSets[0].DisplaySets.Add(displaySet);

			IImageBox imageBox = this.ImageViewer.SelectedImageBox;
			imageBox.DisplaySet = displaySet;
			imageBox.Draw();
			imageBox[0, 0].Select();

			NotifyAllPropertiesChanged();
		}

		protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
			if (e.DeactivatedImageViewer != null)
				e.DeactivatedImageViewer.EventBroker.DisplaySetSelected -= OnDisplaySetSelected;

			if (e.ActivatedImageViewer != null)
				e.ActivatedImageViewer.EventBroker.DisplaySetSelected += OnDisplaySetSelected;

			NotifyAllPropertiesChanged();
		}

		private void OnDisplaySetSelected(object sender, DisplaySetSelectedEventArgs e)
		{
			NotifyPropertyChanged("CreateVolumeEnabled");
			NotifyPropertyChanged("VolumeSettingsEnabled");

			NotifyAllPropertiesChanged();
		}

		private void AddTissueLayers(VolumePresentationImage image)
		{
			GraphicCollection layers = image.TissueLayers;

			TissueSettings tissue = new TissueSettings();
			tissue.SelectPreset("Bone");
			layers.Add(new VolumeGraphic(tissue));

			tissue = new TissueSettings();
			tissue.SelectPreset("Blood");
			layers.Add(new VolumeGraphic(tissue));
		}
	}
}
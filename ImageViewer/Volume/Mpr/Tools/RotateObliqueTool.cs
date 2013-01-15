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

#if EXPERIMENTAL_TOOLS

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	//TODO JY
	[MouseToolButton(XMouseButtons.Left, false)]
	[MenuAction("activate", "imageviewer-contextmenu/MenuVolume/Rotate Oblique", "Apply", Flags = ClickActionFlags.CheckAction)]
	//[ButtonAction("activate", "global-toolbars/ToolbarsMpr/Rotate Oblique", "Apply", Flags = ClickActionFlags.CheckAction)]
	[IconSet("activate", IconScheme.Colour, "Icons.RotateObliqueToolLarge.png", "Icons.RotateObliqueToolMedium.png", "Icons.RotateObliqueToolSmall.png")]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[VisibleStateObserver("activate", "Visible", "VisibleChanged")]

	[GroupHint("activate", "Tools.Mpr.Manipulation.Rotate")]

	[ExtensionOf(typeof(MprViewerToolExtensionPoint))]
	public partial class RotateObliqueTool : MprViewerTool
	{
		private PinwheelGraphic _currentPinwheelGraphic;
		private bool _rotatingGraphic = false;
		private int _rotationAxis = -1;

		private bool _visible;
		public event EventHandler VisibleChanged;	

		public RotateObliqueTool()
		{
		}

		public bool Visible
		{
			get { return _visible; }
			set
			{
				if (_visible != value)
				{
					_visible = value;
					EventsHelper.Fire(VisibleChanged, this, EventArgs.Empty);
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();

			Visible = true;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			_rotatingGraphic = false;

			if (_currentPinwheelGraphic != null)
			{
				_currentPinwheelGraphic.CoordinateSystem = CoordinateSystem.Destination;
				_rotatingGraphic = _currentPinwheelGraphic.HitTest(mouseInformation.Location);
				_currentPinwheelGraphic.ResetCoordinateSystem();
			}

			return _rotatingGraphic;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			return _rotatingGraphic = false;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_rotationAxis < 0)
				return base.Track(mouseInformation);

			if (_rotatingGraphic)
			{
				_currentPinwheelGraphic.CoordinateSystem = CoordinateSystem.Destination;
				PointF rotationAnchor = _currentPinwheelGraphic.RotationAnchor;
				PointF vertex = _currentPinwheelGraphic.Anchor;
				PointF mouse = mouseInformation.Location;
				double angle = Vector.SubtendedAngle(mouse, vertex, rotationAnchor);

				MprDisplaySet obliqueDisplaySet = base.GetObliqueDisplaySet();
				int rotationX = obliqueDisplaySet.RotateAboutX;
				int rotationY = obliqueDisplaySet.RotateAboutY;
				int rotationZ = obliqueDisplaySet.RotateAboutZ;

				if (_rotationAxis == 0)
				{
					rotationX += (int)angle;
					_currentPinwheelGraphic.Rotation = rotationX;
				}
				else if (_rotationAxis == 1)
				{
					rotationY += (int)angle;
					_currentPinwheelGraphic.Rotation = rotationY;
				}
				else
				{
					rotationZ += (int)angle;
					_currentPinwheelGraphic.Rotation = rotationZ;
				}

				_currentPinwheelGraphic.ResetCoordinateSystem();
				_currentPinwheelGraphic.Draw();

				obliqueDisplaySet.Rotate(rotationX, rotationY, rotationZ);
				return true;
			}

			return false;
		}

		private void OnActivationChanged(object sender, System.EventArgs e)
		{
			base.ActivationChanged -= new System.EventHandler(OnActivationChanged);
			RemovePinwheelGraphic();
			UpdateRotationAxis();
		}

		protected override void OnTileSelected(object sender, TileSelectedEventArgs e)
		{
			RemovePinwheelGraphic();
			UpdateRotationAxis();
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			RemovePinwheelGraphic();
			UpdateRotationAxis();

			if (Visible && Active)
			{
				AddPinwheelGraphic();
			}
		}

		public void Apply()
		{
			if (Active)
			{
				Active = false;
			}
			else if (Visible)
			{
				base.Select();

				base.ActivationChanged += new System.EventHandler(OnActivationChanged);

				UpdateRotationAxis();
				AddPinwheelGraphic();
			}
		}

		private void AddPinwheelGraphic()
		{
			IPresentationImage selectedImage = base.SelectedPresentationImage;
			if (selectedImage == null)
				return;

			if (!base.IsMprImage(selectedImage))
				return;

			MprDisplaySet displaySet = (MprDisplaySet)selectedImage.ParentDisplaySet;
			if (displaySet.Identifier == MprDisplaySetIdentifier.Oblique)
				return;

			IOverlayGraphicsProvider overlayProvider = selectedImage as IOverlayGraphicsProvider;
			IImageGraphicProvider imageGraphicProvider = selectedImage as IImageGraphicProvider;
			
			if (overlayProvider != null && imageGraphicProvider != null)
			{
				_currentPinwheelGraphic = new PinwheelGraphic();

				int width = imageGraphicProvider.ImageGraphic.Columns;
				int height = imageGraphicProvider.ImageGraphic.Rows;

				overlayProvider.OverlayGraphics.Add(_currentPinwheelGraphic);
				_currentPinwheelGraphic.CoordinateSystem = CoordinateSystem.Source;
				_currentPinwheelGraphic.Rotation = GetRotationAngle();
				_currentPinwheelGraphic.Draw();
			}
		}

		private void RemovePinwheelGraphic()
		{
			if (_currentPinwheelGraphic != null)
			{
				IPresentationImage image = _currentPinwheelGraphic.ParentPresentationImage;
				((CompositeGraphic)_currentPinwheelGraphic.ParentGraphic).Graphics.Remove(_currentPinwheelGraphic);
				image.Draw();

				_currentPinwheelGraphic.Dispose();
			}

			_currentPinwheelGraphic = null;
		}

		private void UpdateRotationAxis()
		{
			_rotationAxis = -1;

			IPresentationImage selectedImage = base.Context.Viewer.SelectedPresentationImage;
			if (selectedImage == null)
				return;

			MprDisplaySet displaySet = selectedImage.ParentDisplaySet as MprDisplaySet;
			if (displaySet == null)
				return;

			if (displaySet.Identifier == MprDisplaySetIdentifier.Identity)
				_rotationAxis = 0; //x
			else if (displaySet.Identifier == MprDisplaySetIdentifier.OrthoX)
				_rotationAxis = 1; //y
			else if (displaySet.Identifier == MprDisplaySetIdentifier.OrthoY)
				_rotationAxis = 2; //z
		}

		private int GetRotationAngle()
		{
			MprDisplaySet obliqueDisplaySet = base.GetObliqueDisplaySet();
			int rotationX = obliqueDisplaySet.RotateAboutX;
			int rotationY = obliqueDisplaySet.RotateAboutY;
			int rotationZ = obliqueDisplaySet.RotateAboutZ;

			if (_rotationAxis == 0)
				return rotationX;
			else if (_rotationAxis == 1)
				return rotationY;
			else if (_rotationAxis == 2)
				return rotationZ;

			return 0;
		}
	}
}

#endif
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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuSpatialLocator", "Select", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuSynchronization/MenuSpatialLocator", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarSynchronization/ToolbarSpatialLocator", "Select", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", "Icons.SpatialLocatorToolSmall.png", "Icons.SpatialLocatorToolMedium.png", "Icons.SpatialLocatorToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Synchronization.SpatialLocator")]
	
	[MouseToolButton(XMouseButtons.Right, false)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
    public partial class SpatialLocatorTool : MouseImageViewerTool
	{
		#region Private Fields

		private SynchronizationToolCoordinator _coordinator;

		private DicomImagePlane _referencePlane;
		private readonly List<CrossHair> _crosshairs;
		private bool _inUse;

		#endregion

		public SpatialLocatorTool()
			: base(SR.PrefixSpatialLocatorTool)
		{
			_inUse = false;
			_crosshairs = new List<CrossHair>();

			this.CursorToken = new CursorToken("SpatialLocatorCursor.png", this.GetType().Assembly);
		}

		#region Tool Overrides

		public override void Initialize()
        {
            base.Initialize();
			
			_coordinator = SynchronizationToolCoordinator.Get(base.ImageViewer);
        	_coordinator.SetSpatialLocatorTool(this);
        }

		protected override void Dispose(bool disposing)
		{
			_coordinator.Release();

			base.Dispose(disposing);
		}

		#endregion

		#region Private Methods

		private void GetPlaneClosestToReferenceImagePoint(PointF referenceImagePoint,
							IEnumerable<DicomImagePlane> targetImagePlanes,
							out DicomImagePlane closestTargetImagePlane,
							out PointF closestTargetImagePoint)
		{
			closestTargetImagePlane = null;
			closestTargetImagePoint = PointF.Empty;

			float distanceToClosestImagePlane = float.MaxValue;

			Vector3D referencePositionPatient = _referencePlane.ConvertToPatient(referenceImagePoint);

			foreach (DicomImagePlane targetImagePlane in targetImagePlanes)
			{
				float halfThickness = Math.Abs(targetImagePlane.Thickness / 2);
				float halfSpacing = Math.Abs(targetImagePlane.Spacing / 2);
				float toleranceDistanceToImagePlane = Math.Max(halfThickness, halfSpacing);

				if (_referencePlane.IsInSameFrameOfReference(targetImagePlane))
				{
					if (toleranceDistanceToImagePlane > 0)
					{
						Vector3D positionTargetImagePlane = targetImagePlane.ConvertToImagePlane(referencePositionPatient);
						float distanceToTargetImagePlane = Math.Abs(positionTargetImagePlane.Z);

						if (distanceToTargetImagePlane <= toleranceDistanceToImagePlane && distanceToTargetImagePlane < distanceToClosestImagePlane)
						{
							distanceToClosestImagePlane = distanceToTargetImagePlane;
							//The coordinates need to be converted to pixel coordinates because right now they are in mm.
							closestTargetImagePoint = targetImagePlane.ConvertToImage(new PointF(positionTargetImagePlane.X, positionTargetImagePlane.Y));
							closestTargetImagePlane = targetImagePlane;
						}
					}
				}
			}
		}

		private CrossHair GetCrosshair(IImageBox imageBox)
		{
			CrossHair crossHair = _crosshairs.Find(
				delegate(CrossHair test)
					{
						return test.ImageBox == imageBox;
					});

			if (crossHair == null)
			{
				crossHair = new CrossHair(imageBox, this);
				_crosshairs.Add(crossHair);
			}

			return crossHair;
		}

		private IEnumerable<DicomImagePlane> GetTargetImagePlanes(IImageBox imageBox)
		{
			for (int i = imageBox.DisplaySet.PresentationImages.Count - 1; i >= 0; --i)
			{
				DicomImagePlane targetImagePlane = DicomImagePlane.FromImage(imageBox.DisplaySet.PresentationImages[i]);
				if (targetImagePlane != null && _referencePlane.IsInSameFrameOfReference(targetImagePlane))
					yield return targetImagePlane;
			}
		}

		private IEnumerable<IImageBox> GetTargetImageBoxes()
		{
			foreach (IImageBox imageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox.DisplaySet != null && !IsReferenceImageBox(imageBox))
					yield return imageBox;
			}
		}

		private bool IsReferenceImageBox(IImageBox imageBox)
		{
			return imageBox == _referencePlane.SourceImage.ParentDisplaySet.ImageBox;
		}

		private void UpdateCrossHair(CrossHair crossHair, PointF referenceImagePoint)
		{
			DicomImagePlane closestTargetPlane;
			PointF closestTargetImagePoint;
			GetPlaneClosestToReferenceImagePoint(referenceImagePoint, GetTargetImagePlanes(crossHair.ImageBox),
												out closestTargetPlane, out closestTargetImagePoint);

			if (closestTargetPlane == null)
			{
				crossHair.Image = null;
			}
			else 
			{
				crossHair.Image = closestTargetPlane.SourceImage;
				crossHair.ImagePoint = closestTargetImagePoint;
			}
		}

		private void UpdateCrosshairs(Point destinationPoint)
		{
			PointF referenceImagePoint = _referencePlane.SourceImageTransform.ConvertToSource(destinationPoint);

			foreach (IImageBox imageBox in GetTargetImageBoxes())
				UpdateCrossHair(GetCrosshair(imageBox), referenceImagePoint);

			_coordinator.OnSpatialLocatorCrosshairsUpdated();
		}

		#endregion

		#region Mouse Handler Methods

		private bool Start()
		{
			_referencePlane = DicomImagePlane.FromImage(base.SelectedPresentationImage);
			return _referencePlane != null;
		}

		private void Stop()
		{
			_referencePlane = null;
			_inUse = false;

			foreach (CrossHair crosshair in _crosshairs)
				crosshair.Image = null;

			_coordinator.OnSpatialLocatorStopped();

			foreach (CrossHair crosshair in _crosshairs)
				crosshair.Dispose();

			_crosshairs.Clear();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			_inUse = Start();
			if (_inUse)
				UpdateCrosshairs(mouseInformation.Location);

			return _inUse;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (_inUse)
				UpdateCrosshairs(mouseInformation.Location);

			return _inUse;
        }

		public override bool Stop(IMouseInformation mouseInformation)
		{
			Cancel();
			return false;
        }

		public override void Cancel()
		{
			Stop();
		}

		#endregion

		#region Internal Methods (for mediator)

		internal IEnumerable<IImageBox> GetImageBoxesToRedraw()
		{
			foreach (CrossHair crosshair in _crosshairs)
			{
				if (crosshair.Dirty)
					yield return crosshair.ImageBox;
			}
		}

		#endregion
	}
}

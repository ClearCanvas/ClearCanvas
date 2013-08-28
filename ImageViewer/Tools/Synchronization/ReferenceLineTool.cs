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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuReferenceLines", "Toggle", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
	[MenuAction("activate", "global-menus/MenuTools/MenuSynchronization/MenuReferenceLines", "Toggle", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarSynchronization/ToolbarReferenceLines", "Toggle", "ReferenceLineDropDownMenuModel", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActiveChanged")]
	[Tooltip("activate", "TooltipReferenceLines")]
	[IconSet("activate", "Icons.CurrentReferenceLineToolSmall.png", "Icons.CurrentReferenceLineToolMedium.png", "Icons.CurrentReferenceLineToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Synchronization.ReferenceLines.Current")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ReferenceLineTool : ImageViewerTool
	{
		#region ReferenceLine class

		private class ReferenceLine
		{
			public readonly PointF StartPoint;
			public readonly PointF EndPoint;
			public readonly string Label;

			public ReferenceLine(PointF startPoint, PointF endPoint, string label)
			{
				this.StartPoint = startPoint;
				this.EndPoint = endPoint;
				this.Label = label;
			}
		}

		#endregion

		#region Private Fields

		private DicomImagePlane _currentReferenceImagePlane;
		
		private bool _active;
		private event EventHandler _activeChanged;

		private SynchronizationToolCoordinator _coordinator;

		private static readonly float _oneDegreeInRadians = (float)(Math.PI / 180);

		private IResourceResolver _resolver = new ApplicationThemeResourceResolver(typeof(ReferenceLineTool).Assembly);
		private ActionModelRoot _dropDownMenuModel;

		#endregion

		public ReferenceLineTool()
		{
			_active = false;
		}

		#region Tool Overrides

		public override void Initialize()
		{
			base.Initialize();

			_coordinator = SynchronizationToolCoordinator.Get(base.ImageViewer);
			_coordinator.SetReferenceLineTool(this);

			base.ImageViewer.EventBroker.ImageDrawing += OnImageDrawing;
		}

		protected override void Dispose(bool disposing)
		{
			base.ImageViewer.EventBroker.ImageDrawing -= OnImageDrawing;

			_coordinator.Release();
			_coordinator = null;
			_currentReferenceImagePlane = null;

			base.Dispose(disposing);
		}

		#endregion

		#region Action Properties, Events, Methods

		public bool Active
		{
			get { return _active; }
			set
			{
				if (_active == value)
					return;

				_active = value;
				EventsHelper.Fire(_activeChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler ActiveChanged
		{
			add { _activeChanged += value; }
			remove { _activeChanged -= value; }
		}

		public ActionModelNode ReferenceLineDropDownMenuModel
		{
			get
			{
				if (_dropDownMenuModel == null)
				{
					_dropDownMenuModel = new ActionModelRoot();
					ClickAction action = new ClickAction("showFirstAndLastReferenceLines",
						new ActionPath("reference-line-dropdown/ShowFirstAndLastReferenceLines", _resolver),
						ClickActionFlags.CheckAction, _resolver);

					action.Checked = ShowFirstAndLastReferenceLines;
					action.Label = SR.ShowFirstAndLastReferenceLines;
					action.SetClickHandler(delegate { ToggleShowFirstAndLastReferenceLines(); });

					_dropDownMenuModel.InsertAction(action);
				}

				return _dropDownMenuModel;
			}
		}

		public bool ShowFirstAndLastReferenceLines
		{
			get
			{
                return SynchronizationToolSettings.DefaultInstance.ShowFirstAndLastReferenceLines;
			}
			set
			{
                SynchronizationToolSettings.DefaultInstance.ShowFirstAndLastReferenceLines = value;
                SynchronizationToolSettings.DefaultInstance.Save();

				ClickAction showFirstAndLastAction = (ClickAction)((ActionNode)(ReferenceLineDropDownMenuModel.ChildNodes[0])).Action;
				showFirstAndLastAction.Checked = ShowFirstAndLastReferenceLines;
			}
		}

		public void Toggle()
		{
			Active = !Active;
			RefreshAllReferenceLines();
			_coordinator.OnRefreshedReferenceLines();
		}

		private void ToggleShowFirstAndLastReferenceLines()
		{
			ShowFirstAndLastReferenceLines = !ShowFirstAndLastReferenceLines;

			RefreshAllReferenceLines();
			_coordinator.OnRefreshedReferenceLines();
		}

		#endregion

		private IPresentationImage CurrentReferenceImage
		{
			get
			{
				if (_currentReferenceImagePlane != null)
					return _currentReferenceImagePlane.SourceImage;
				
				return null;
			}
		}

		#region Private Methods

		private void OnImageDrawing(object sender, ImageDrawingEventArgs e)
		{
			//make the reference lines on any image invisible when the tool is not active.
			if (!Active)
				RefreshReferenceLines(e.PresentationImage);
		}

		private IEnumerable<IPresentationImage> GetAllVisibleImages()
		{
			foreach (IImageBox imageBox in this.Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				foreach (ITile tile in imageBox.Tiles)
				{
					if (tile.PresentationImage != null)
						yield return tile.PresentationImage;
				}
			}
		}

		private IEnumerable<DicomImagePlane> GetPlanesParallelToReferencePlane()
		{
			foreach (IPresentationImage image in CurrentReferenceImage.ParentDisplaySet.PresentationImages)
			{
				DicomImagePlane plane = DicomImagePlane.FromImage(image);
				if (plane != null)
				{
					if (_currentReferenceImagePlane.IsInSameFrameOfReference(plane) &&
						_currentReferenceImagePlane.IsParallelTo(plane, _oneDegreeInRadians))
					{
						yield return plane;
					}
				}
			}
		}

		private static ReferenceLine GetReferenceLine(DicomImagePlane referenceImagePlane, DicomImagePlane targetImagePlane)
		{
			// if planes are parallel within tolerance, then they do not intersect and thus no reference lines should be shown
			float parallelTolerance = SynchronizationToolSettingsHelper.Default.ParallelPlanesToleranceAngleRadians;
			if (referenceImagePlane.IsParallelTo(targetImagePlane, parallelTolerance))
				return null;

			Vector3D intersectionPatient1, intersectionPatient2;
			if (!referenceImagePlane.GetIntersectionPoints(targetImagePlane, out intersectionPatient1, out intersectionPatient2))
				return null;

			Vector3D intersectionImagePlane1 = targetImagePlane.ConvertToImagePlane(intersectionPatient1);
			Vector3D intersectionImagePlane2 = targetImagePlane.ConvertToImagePlane(intersectionPatient2);

			//The coordinates need to be converted to pixel coordinates because right now they are in mm.
			PointF intersectionImage1 = targetImagePlane.ConvertToImage(new PointF(intersectionImagePlane1.X, intersectionImagePlane1.Y));
			PointF intersectionImage2 = targetImagePlane.ConvertToImage(new PointF(intersectionImagePlane2.X, intersectionImagePlane2.Y));
			string label = referenceImagePlane.InstanceNumber.ToString();

			return new ReferenceLine(intersectionImage1, intersectionImage2, label);
		}

		private void GetFirstAndLastReferenceLines(DicomImagePlane targetImagePlane, out ReferenceLine firstReferenceLine, out ReferenceLine lastReferenceLine)
		{
			firstReferenceLine = lastReferenceLine = null;

			float firstReferenceImageZComponent = float.MaxValue;
			float lastReferenceImageZComponent = float.MinValue;

			// 1. Find all images in the same plane as the current reference image.
			foreach (DicomImagePlane parallelPlane in GetPlanesParallelToReferencePlane())
			{
				// 2. Use the Image Position (in the coordinate system of the Image Plane without moving the origin!) 
				//    to determine the first and last reference line.  By transforming the Image Position (Patient) to 
				//    the coordinate system of the image plane, we can then simply take the 2 images with
				//    the smallest and largest z-components, respectively, as the 'first' and 'last' reference images.

				// < keeps the first image as close to the beginning of the display set as possible.
				if (parallelPlane.PositionImagePlaneTopLeft.Z < firstReferenceImageZComponent)
				{
					ReferenceLine referenceLine = GetReferenceLine(parallelPlane, targetImagePlane);
					if (referenceLine != null)
					{
						firstReferenceImageZComponent = parallelPlane.PositionImagePlaneTopLeft.Z;
						firstReferenceLine = referenceLine;
					}
				}

				// >= keeps the last image as close to the end of the display set as possible.
				if (parallelPlane.PositionImagePlaneTopLeft.Z >= lastReferenceImageZComponent)
				{
					ReferenceLine referenceLine = GetReferenceLine(parallelPlane, targetImagePlane);
					if (referenceLine != null)
					{
						lastReferenceImageZComponent = parallelPlane.PositionImagePlaneTopLeft.Z;
						lastReferenceLine = referenceLine;
					}
				}
			}
		}

		private IEnumerable<ReferenceLine> GetAllReferenceLines(DicomImagePlane targetImagePlane)
		{
			ReferenceLine firstReferenceLine = null;
			ReferenceLine lastReferenceLine = null;
			if (ShowFirstAndLastReferenceLines)
				GetFirstAndLastReferenceLines(targetImagePlane, out firstReferenceLine, out lastReferenceLine);

			if (firstReferenceLine != null)
				yield return firstReferenceLine;

			if (lastReferenceLine != null)
				yield return lastReferenceLine;

			//return 'current' last, so it draws on top of the others.
			ReferenceLine currentReferenceLine = GetReferenceLine(_currentReferenceImagePlane, targetImagePlane);
			if (currentReferenceLine != null)
				yield return currentReferenceLine;
		}

		private void RefreshReferenceLines(IPresentationImage targetImage)
		{
			DicomImagePlane targetImagePlane = DicomImagePlane.FromImage(targetImage);
			if (targetImagePlane == null)
				return;

			ReferenceLineCompositeGraphic referenceLineCompositeGraphic = _coordinator.GetReferenceLineCompositeGraphic(targetImage);
			if (referenceLineCompositeGraphic == null)
				return;

			bool showReferenceLines = this.Active && _currentReferenceImagePlane != null && 
				_currentReferenceImagePlane.IsInSameFrameOfReference(targetImagePlane);

			if (!showReferenceLines)
			{
				referenceLineCompositeGraphic.HideAllReferenceLines();
				return;
			}

			int i = 0;
			foreach (ReferenceLine referenceLine in GetAllReferenceLines(targetImagePlane))
			{
				ReferenceLineGraphic referenceLineGraphic = referenceLineCompositeGraphic[i++];
				referenceLineGraphic.Point1 = referenceLine.StartPoint;
				referenceLineGraphic.Point2 = referenceLine.EndPoint;
				referenceLineGraphic.Text = referenceLine.Label;
				referenceLineGraphic.Visible = true;
			}

			// make any that aren't valid invisible.
			for (int j = i; j < referenceLineCompositeGraphic.Graphics.Count; ++j)
				referenceLineCompositeGraphic[j].Visible = false;
		}

		private void SetCurrentReferencePlane()
		{
			if (CurrentReferenceImage == this.SelectedPresentationImage)
				return;

			_currentReferenceImagePlane = DicomImagePlane.FromImage(this.SelectedPresentationImage);
			if (_currentReferenceImagePlane == null)
				return;

			ReferenceLineCompositeGraphic referenceLineCompositeGraphic =
				_coordinator.GetReferenceLineCompositeGraphic(CurrentReferenceImage);

			//Hide the current image's reference lines
			if (referenceLineCompositeGraphic != null)
				referenceLineCompositeGraphic.HideAllReferenceLines();
		}

		#endregion

		#region Internal Methods (for mediator)

		internal void RefreshAllReferenceLines()
		{
			SetCurrentReferencePlane();

			foreach (IPresentationImage targetImage in GetAllVisibleImages())
			{
				if (targetImage != CurrentReferenceImage)
					RefreshReferenceLines(targetImage);
			}
		}

		internal IEnumerable<IPresentationImage> GetImagesToRedraw()
		{
			foreach (IPresentationImage image in GetAllVisibleImages())
			{
				ReferenceLineCompositeGraphic graphic = _coordinator.GetReferenceLineCompositeGraphic(image);
				if (graphic != null && graphic.Dirty)
					yield return image;
			}
		}

		#endregion
	}
}

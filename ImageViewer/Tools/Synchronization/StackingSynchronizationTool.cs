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
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	[MenuAction("synchronize", "imageviewer-contextmenu/MenuSynchronizeStacking", "ToggleSynchronize", Flags = ClickActionFlags.CheckAction, InitiallyAvailable = false)]
	[MenuAction("synchronize", "global-menus/MenuTools/MenuSynchronization/MenuSynchronizeStacking", "ToggleSynchronize", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("synchronize", "global-toolbars/ToolbarSynchronization/ToolbarSynchronizeStacking", "ToggleSynchronize", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("synchronize", "SynchronizeActive", "SynchronizeActiveChanged")]
	[Tooltip("synchronize", "TooltipSynchronizeStacking")]
	[IconSet("synchronize", "Icons.SynchronizeToolSmall.png", "Icons.SynchronizeToolMedium.png", "Icons.SynchronizeToolLarge.png")]
	[GroupHint("synchronize", "Tools.Image.Manipulation.Stacking.Synchronize")]

	[MenuAction("linkStudies", "imageviewer-contextmenu/MenuSynchronizeStackingLinkStudies", "ToggleLinkStudies", InitiallyAvailable = false)]
	[MenuAction("linkStudies", "global-menus/MenuTools/MenuSynchronization/MenuSynchronizeStackingLinkStudies", "ToggleLinkStudies")]
	[ButtonAction("linkStudies", "global-toolbars/ToolbarSynchronization/ToolbarSynchronizeStackingLinkStudies", "ToggleLinkStudies")]
	[EnabledStateObserver("linkStudies", "LinkStudiesEnabled", "LinkStudiesEnabledChanged")]
	[LabelValueObserver("linkStudies", "LinkStudiesLabel", "StudiesLinkedChanged")]
	[IconSetObserver("linkStudies", "LinkStudiesIconSet", "StudiesLinkedChanged")]
	[TooltipValueObserver("linkStudies", "LinkStudiesTooltip", "StudiesLinkedChanged")]
	[GroupHint("linkStudies", "Tools.Image.Manipulation.Stacking.LinkStudies")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public partial class StackingSynchronizationTool : ImageViewerTool
	{
		#region Private Fields

		private bool _synchronizeActive;
		private event EventHandler _synchronizeActiveChanged;

		private bool _linkStudiesEnabled;
		private event EventHandler _linkStudiesEnabledChanged;

		private bool _studiesLinked;
		private event EventHandler _studiesLinkedChanged;

		private readonly IconSet _linkStudiesIconSet;
		private readonly IconSet _unlinkStudiesIconSet;
		
		private SynchronizationToolCoordinator _coordinator;

		private FrameOfReferenceCalibrator _frameOfReferenceCalibrator;
		private bool _deferSynchronizeUntilDisplaySetChanged;

		private readonly List<IImageBox> _imageBoxesToDraw = new List<IImageBox>();
		
		#endregion

		public StackingSynchronizationTool()
		{
			_deferSynchronizeUntilDisplaySetChanged = false;

			_synchronizeActive = false;
			_linkStudiesEnabled = false;

			_studiesLinked = true;
			_linkStudiesIconSet = new IconSet("Icons.LinkStudiesToolSmall.png", "Icons.LinkStudiesToolMedium.png", "Icons.LinkStudiesToolLarge.png");
			_unlinkStudiesIconSet = new IconSet("Icons.UnlinkStudiesToolSmall.png", "Icons.UnlinkStudiesToolMedium.png", "Icons.UnlinkStudiesToolLarge.png");

			ResetFrameOfReferenceCalibrations();
		}

		#region Tool Overrides

		public override void Initialize()
		{
			base.Initialize();

			base.ImageViewer.EventBroker.DisplaySetChanging += OnDisplaySetChanging;
			base.ImageViewer.EventBroker.DisplaySetChanged += OnDisplaySetChanged;
			base.ImageViewer.PhysicalWorkspace.LayoutCompleted += OnLayoutCompleted;
			SynchronizationToolSettingsHelper.Default.PropertyChanged += OnSynchronizationToolSettingsPropertyChanged;

			_coordinator = SynchronizationToolCoordinator.Get(base.ImageViewer);
			_coordinator.SetStackingSynchronizationTool(this);
		}

		protected override void Dispose(bool disposing)
		{
			SynchronizationToolSettingsHelper.Default.PropertyChanged -= OnSynchronizationToolSettingsPropertyChanged;
			base.ImageViewer.EventBroker.DisplaySetChanging -= OnDisplaySetChanging;
			base.ImageViewer.EventBroker.DisplaySetChanged -= OnDisplaySetChanged;
			base.ImageViewer.PhysicalWorkspace.LayoutCompleted -= OnLayoutCompleted;

			_coordinator.Release();

			base.Dispose(disposing);
		}

		#endregion

		#region Action Methods, Properties, Events

		public bool SynchronizeActive
		{
			get { return _synchronizeActive; }
			set
			{
				if (_synchronizeActive == value)
					return;

				_synchronizeActive = value;
				EventsHelper.Fire(_synchronizeActiveChanged, this, EventArgs.Empty);
				
				LinkStudiesEnabled = _synchronizeActive;
			}
		}

		public event EventHandler SynchronizeActiveChanged
		{
			add { _synchronizeActiveChanged += value; }
			remove { _synchronizeActiveChanged -= value; }
		}

		public bool LinkStudiesEnabled
		{
			get { return _linkStudiesEnabled; }
			set
			{
				if (value && !SynchronizeActive)
					value = false;

				if (_linkStudiesEnabled == value)
					return;

				if (!value)
				{
					ResetFrameOfReferenceCalibrations();
					StudiesLinked = true;
				}

				_linkStudiesEnabled = value;
				EventsHelper.Fire(_linkStudiesEnabledChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler LinkStudiesEnabledChanged
		{
			add { _linkStudiesEnabledChanged += value; }
			remove { _linkStudiesEnabledChanged -= value; }
		}

		public bool StudiesLinked
		{
			get { return _studiesLinked; }
			set
			{
				if (value && !SynchronizeActive)
					value = true;

				if (_studiesLinked == value)
					return;

				_studiesLinked = value;
				EventsHelper.Fire(_studiesLinkedChanged, this, EventArgs.Empty);
			}
		}

		public event EventHandler StudiesLinkedChanged
		{
			add { _studiesLinkedChanged += value; }
			remove { _studiesLinkedChanged -= value; }
		}

		public IconSet LinkStudiesIconSet
		{
			get
			{
				return _studiesLinked ? _unlinkStudiesIconSet : _linkStudiesIconSet;
			}	
		}

		public string LinkStudiesLabel
		{
			get
			{
				return _studiesLinked ? SR.LabelUnlinkStudies : SR.LabelLinkStudies;
			}
		}

		public string LinkStudiesTooltip
		{
			get
			{
				return _studiesLinked ? SR.LabelUnlinkStudies : SR.LabelLinkStudies;
			}
		}

		private void ToggleSynchronize()
		{
			SynchronizeActive = !SynchronizeActive;
			if (SynchronizeActive)
			{
				SynchronizeAllImageBoxes();
				_coordinator.OnSynchronizedImageBoxes();
			}
		}

		private void ToggleLinkStudies()
		{
			StudiesLinked = !StudiesLinked;
			if (StudiesLinked)
			{
				CalibrateFrameOfReferenceForVisibleImageBoxes();
				SynchronizeAllImageBoxes(); 
				_coordinator.OnSynchronizedImageBoxes();
			}
		}

		#endregion

		#region Event Handlers

		private void OnLayoutCompleted(object sender, EventArgs e)
		{
			//this is the best we can do in this situation.
			if (SynchronizeActive)
			{
				SynchronizeAllImageBoxes();
				_coordinator.OnSynchronizedImageBoxes();
			}
		}

		private void OnDisplaySetChanging(object sender, DisplaySetChangingEventArgs e)
		{
			_deferSynchronizeUntilDisplaySetChanged = true;
		}

		private void OnDisplaySetChanged(object sender, DisplaySetChangedEventArgs e)
		{
			_deferSynchronizeUntilDisplaySetChanged = false;

			SynchronizeNewDisplaySet(e.NewDisplaySet);
			_coordinator.OnSynchronizedImageBoxes();
		}

		private void OnSynchronizationToolSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			// if the tolerance angle setting changes, force a recalibration
			if (e.PropertyName == "ParallelPlanesToleranceAngleRadians")
			{
				ResetFrameOfReferenceCalibrations();
			}
		}

		#endregion

		#region Private Methods

		private static IEnumerable<DicomImagePlane> GetAllImagePlanes(IImageBox imageBox)
		{
			if (imageBox.DisplaySet != null)
			{
				for (int index = imageBox.DisplaySet.PresentationImages.Count - 1; index >= 0; --index)
				{
					DicomImagePlane targetPlane = DicomImagePlane.FromImage(imageBox.DisplaySet.PresentationImages[index]);
					if (targetPlane != null)
						yield return targetPlane;
				}
			}
		}

		private void ResetFrameOfReferenceCalibrations()
		{
			_frameOfReferenceCalibrator = new FrameOfReferenceCalibrator(SynchronizationToolSettingsHelper.Default.ParallelPlanesToleranceAngleRadians);
		}

		private void CalibrateFrameOfReferenceForVisibleImageBoxes()
		{
			foreach (IImageBox referenceImageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
			{
				DicomImagePlane referencePlane = DicomImagePlane.FromImage(referenceImageBox.TopLeftPresentationImage);
				if (referencePlane != null)
				{
					foreach (IImageBox imageBox in GetTargetImageBoxes(referenceImageBox))
					{
						DicomImagePlane targetPlane = DicomImagePlane.FromImage(imageBox.TopLeftPresentationImage);
						if (targetPlane != null)
							_frameOfReferenceCalibrator.Calibrate(referencePlane, targetPlane);
					}
				}
			}
		}

		private DicomImagePlane GetClosestParallelImagePlane(DicomImagePlane referenceImagePlane, IEnumerable<DicomImagePlane> targetImagePlanes)
		{
			DicomImagePlane closestImagePlane = null;
			float distanceToClosestImagePlane = float.MaxValue;

			foreach (DicomImagePlane targetImagePlane in targetImagePlanes)
			{
				if (targetImagePlane.IsParallelTo(referenceImagePlane, SynchronizationToolSettingsHelper.Default.ParallelPlanesToleranceAngleRadians))
				{
					bool sameFrameOfReference = referenceImagePlane.IsInSameFrameOfReference(targetImagePlane);
					if (this.StudiesLinked || sameFrameOfReference)
					{
						Vector3D calibratedOffset = _frameOfReferenceCalibrator.GetOffset(referenceImagePlane, targetImagePlane) ?? Vector3D.Null;

						Vector3D distanceToImagePlane = referenceImagePlane.PositionPatientCenterOfImage + calibratedOffset - targetImagePlane.PositionPatientCenterOfImage;

						float absoluteDistanceToImagePlane = Math.Abs(distanceToImagePlane.Magnitude);

						if (absoluteDistanceToImagePlane < distanceToClosestImagePlane)
						{
							distanceToClosestImagePlane = absoluteDistanceToImagePlane;
							closestImagePlane = targetImagePlane;
						}
					}
				}
			}

			return closestImagePlane;
		}

		private IEnumerable<IImageBox> GetTargetImageBoxes(IImageBox referenceImageBox)
		{
			foreach (IImageBox imageBox in this.Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox != referenceImageBox && imageBox.DisplaySet != null && imageBox.DisplaySet.PresentationImages.Count > 1)
					yield return imageBox;
			}
		}

		private void SynchronizeImageBox(IImageBox referenceImageBox, IImageBox targetImageBox)
		{
			if (referenceImageBox.TopLeftPresentationImage == null)
				return;

			if (targetImageBox.TopLeftPresentationImage == null)
				return;

			DicomImagePlane referenceImagePlane = DicomImagePlane.FromImage(referenceImageBox.TopLeftPresentationImage);
			if (referenceImagePlane == null)
				return;

			IEnumerable<DicomImagePlane> targetImagePlanes = GetAllImagePlanes(targetImageBox);
			DicomImagePlane targetImagePlane = GetClosestParallelImagePlane(referenceImagePlane, targetImagePlanes);
			if (targetImagePlane == null)
				return;

			int lastIndex = targetImageBox.TopLeftPresentationImageIndex;
			targetImageBox.TopLeftPresentationImage = targetImagePlane.SourceImage;

			if (lastIndex != targetImageBox.TopLeftPresentationImageIndex)
			{
				if (!_imageBoxesToDraw.Contains(targetImageBox))
					_imageBoxesToDraw.Add(targetImageBox);
			}
		}

		private void SynchronizeAllImageBoxes()
		{
			if (!SynchronizeActive)
				return;

			IImageBox selectedImageBox = this.Context.Viewer.SelectedImageBox;

			foreach (IImageBox imageBox in this.Context.Viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox != selectedImageBox)
				{
					//Synchronize everything with everything else, but never with the selected (do it last).
					foreach (IImageBox targetImageBox in GetTargetImageBoxes(imageBox))
					{
						if (targetImageBox != selectedImageBox)
							SynchronizeImageBox(imageBox, targetImageBox);
					}
				}
			}

			if (selectedImageBox == null)
				return;

			//Synchronize with the selected.
			foreach (IImageBox targetImageBox in GetTargetImageBoxes(selectedImageBox))
				SynchronizeImageBox(selectedImageBox, targetImageBox);
		}

		private void SynchronizeNewDisplaySet(IDisplaySet newDisplaySet)
		{
			if (!SynchronizeActive || newDisplaySet == null)
				return;

			IImageBox changedImageBox = newDisplaySet.ImageBox;
			IImageBox selectedImageBox = this.Context.Viewer.SelectedImageBox;

			if (selectedImageBox == null || selectedImageBox == changedImageBox)
			{
				// if there is no selected image box or the new one is selected, try
				// to sync it up with the other ones.

				// Do a reverse synchronization; sync the newly selected one with all the others.
				foreach (IImageBox imageBox in GetTargetImageBoxes(changedImageBox))
				{
					if (imageBox != selectedImageBox)
						SynchronizeImageBox(imageBox, changedImageBox);
				}
			}
			else
			{
				SynchronizeImageBox(selectedImageBox, changedImageBox);
			}
		}

		#endregion

		#region Internal Methods (for mediator)

		internal void SynchronizeImageBoxes()
		{
			if (!SynchronizeActive || _deferSynchronizeUntilDisplaySetChanged)
				return;

			IImageBox selectedImageBox = this.Context.Viewer.SelectedImageBox;
			if (selectedImageBox == null)
				return;

			foreach (IImageBox targetImageBox in GetTargetImageBoxes(selectedImageBox))
				SynchronizeImageBox(selectedImageBox, targetImageBox);
		}

		internal IEnumerable<IImageBox> GetImageBoxesToDraw()
		{
			foreach (IImageBox imageBox in _imageBoxesToDraw)
				yield return imageBox;

			// once we've given the list to the mediator, clear it.
			_imageBoxesToDraw.Clear();
		}

		#endregion
	}
}

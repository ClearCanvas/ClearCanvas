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
using System.Drawing;
using System.Linq;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.PresentationStates.Dicom
{
	partial class DicomGraphicAnnotation
	{
		[Cloneable]
		[DicomSerializableGraphicAnnotation(typeof (DicomGraphicAnnotationSerializer))]
		private class ElementGraphic : ControlGraphic, IRoiGraphic, IAnnotationGraphic
		{
			private event EventHandler _roiChanged;

			[CloneIgnore]
			private Roi _roi;

			[CloneIgnore]
			private volatile SynchronizationContext _uiThreadContext;

			[CloneIgnore]
			private readonly object _syncLock = new object();

			[CloneIgnore]
			private int? _lastChange;

			[CloneIgnore]
			private ICalloutGraphic _calloutGraphic;

			public ElementGraphic(IGraphic subjectGraphic)
				: base(subjectGraphic)
			{
				Initialize();
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			private ElementGraphic(ElementGraphic source, ICloningContext context)
				: base(source, context)
			{
				context.CloneFields(source, this);
			}

			private void Initialize()
			{
				Subject.VisualStateChanged += OnSubjectVisualStateChanged;
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					_uiThreadContext = null;
					lock (_syncLock)
					{
						_lastChange = null;
					}

					Subject.VisualStateChanged -= OnSubjectVisualStateChanged;
				}
				base.Dispose(disposing);
			}

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				Callout = Graphics.OfType<ICalloutGraphic>().FirstOrDefault();

				Initialize();
			}

			/// <summary>
			/// Gets the <see cref="ICalloutGraphic"/> associated with the subject of interest.
			/// </summary>
			public ICalloutGraphic Callout
			{
				get { return _calloutGraphic; }
				set
				{
					if (_calloutGraphic != value)
					{
						PointF? location = null;
						if (_calloutGraphic != null)
						{
							_calloutGraphic.TextLocationChanged -= OnCalloutLocationChanged;
							location = _calloutGraphic.WithSourceCoordinates(g => g.TextLocation);
						}

						_calloutGraphic = value;

						if (location.HasValue && _calloutGraphic != null)
						{
							_calloutGraphic.WithSourceCoordinates(g => g.TextLocation = location.Value);
							_calloutGraphic.TextLocationChanged += OnCalloutLocationChanged;
						}
					}
				}
			}

			private void OnSubjectVisualStateChanged(object sender, VisualStateChangedEventArgs e)
			{
				if (!(sender is ICalloutGraphic || e.Graphic is ICalloutGraphic))
				{
					if (e.PropertyKind == VisualStatePropertyKind.Geometry || e.PropertyKind == VisualStatePropertyKind.Unspecified)
						OnSubjectChanged();
				}
			}

			public Roi Roi
			{
				get { return _roi ?? (_roi = Subject.GetRoi()); }
			}

			public event EventHandler RoiChanged
			{
				add { _roiChanged += value; }
				remove { _roiChanged -= value; }
			}

			private void OnRoiChanged()
			{
				EventsHelper.Fire(_roiChanged, this, EventArgs.Empty);
			}

			public void Refresh()
			{
				OnSubjectChanged();
				Draw();
			}

			#region ROI Update Handling

			/// <summary>
			/// Called when properties on the <see cref="ControlGraphic.Subject"/> have changed.
			/// </summary>
			private void OnSubjectChanged()
			{
				// do not respond to subject change events for ROI recalculation unless the graphic was unlocked
				if (!Enabled) return;

				if (DecoratedGraphic is IControlGraphic && SynchronizationContext.Current != null)
				{
					//we can't use the DelayedEventPublisher because that relies on the sync context,
					//and we use graphics on worker threads for avi export ... so, we'll just do it custom.
					lock (_syncLock)
					{
						_lastChange = Environment.TickCount;
					}

					if (_uiThreadContext == null)
					{
						_uiThreadContext = SynchronizationContext.Current;
						ThreadPool.QueueUserWorkItem(DelayedEventThread);
					}

					Analyze(true);
				}
				else
				{
					// the roi is inactive, focused or selected, but not actively
					// moving or stretching; just do the calculation immediately.
					Analyze(false);
				}

				OnRoiChanged();
				SetCalloutEndPoint();
			}

			private void DelayedEventThread(object nothing)
			{
				SynchronizationContext uiThreadContext;

				while (null != (uiThreadContext = _uiThreadContext))
				{
					int? lastChange;
					lock (_syncLock)
					{
						lastChange = _lastChange;
					}
					if (lastChange == null)
						break;

					var timeDiff = Math.Abs(Environment.TickCount - lastChange.Value);
					if (timeDiff >= 300)
					{
						uiThreadContext.Post(s => OnDelayedRoiChanged(), null);
						break;
					}

					Thread.Sleep(5);
				}
			}

			private void OnDelayedRoiChanged()
			{
				if (_uiThreadContext == null)
					return;

				_uiThreadContext = null;
				_lastChange = null;

				Analyze(false);
				Draw();
			}

			private void Analyze(bool responsive)
			{
				_roi = Subject.GetRoi();

				var currentCallout = Callout;
				var roiCallout = currentCallout as RoiCalloutGraphic;
				if (roiCallout == null)
				{
					if (currentCallout != null)
					{
						Graphics.Remove(currentCallout);
						currentCallout.Dispose();
					}
					roiCallout = new RoiCalloutGraphic {Color = Color};
					Graphics.Add(roiCallout);
					Callout = roiCallout;
				}
				roiCallout.Update(_roi, responsive ? RoiAnalysisMode.Responsive : RoiAnalysisMode.Normal);
			}

			#endregion

			#region Callout Location Handling

			private void SetCalloutEndPoint()
			{
				// We're attaching the callout to the ROI, so make sure the two
				// graphics are in the same coordinate system before we do that.
				// This sets all the graphics coordinate systems to be the same.

				CoordinateSystem = CoordinateSystem;
				try
				{
					Callout.AnchorPoint = Subject.GetClosestPoint(Callout.TextLocation);
				}
				finally
				{
					ResetCoordinateSystem();
				}
			}

			private void OnCalloutLocationChanged(object sender, EventArgs e)
			{
				SetCalloutEndPoint();
			}

			#endregion

			#region Image Calibration Handling

			protected override void OnParentPresentationImageChanged(IPresentationImage oldParentPresentationImage, IPresentationImage newParentPresentationImage)
			{
				var sopProvider = oldParentPresentationImage as IImageSopProvider;
				if (sopProvider != null)
					sopProvider.Frame.NormalizedPixelSpacing.Calibrated -= OnNormalizedPixelSpacingCalibrated;

				base.OnParentPresentationImageChanged(oldParentPresentationImage, newParentPresentationImage);

				sopProvider = newParentPresentationImage as IImageSopProvider;
				if (sopProvider != null)
					sopProvider.Frame.NormalizedPixelSpacing.Calibrated += OnNormalizedPixelSpacingCalibrated;
			}

			private void OnNormalizedPixelSpacingCalibrated(object sender, EventArgs e)
			{
				Refresh();
			}

			#endregion
		}
	}
}
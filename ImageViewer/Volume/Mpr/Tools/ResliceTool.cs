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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	partial class ResliceToolGroup
	{
		[MenuAction("activate", "mprviewer-reslicemenu/MenuReslice", "Select", Flags = ClickActionFlags.CheckAction)]
		[MouseButtonIconSet("activate", "Icons.ResliceToolLarge.png", "Icons.ResliceToolMedium.png", "Icons.ResliceToolSmall.png")]
		[CheckedStateObserver("activate", "Active", "ActivationChanged")]
		[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
		[LabelValueObserver("activate", "Label", "SliceSetChanged")]
		[GroupHint("activate", "Tools.Volume.MPR.Reslicing")]
		[Persistent("activate", false)]
		[MouseToolButton(XMouseButtons.Left, false)]
		private class ResliceTool : MprViewerTool, IMemorable
		{
			private ResliceToolGraphic _resliceGraphic;
			private InteractivePolylineGraphicBuilder _lineGraphicBuilder;

			private object _originalResliceToolsState;

			private Color _hotColor = Color.SkyBlue;
			private Color _normalColor = Color.CornflowerBlue;

			private ResliceToolGroup _resliceToolGroup;

			private int _lastTopLeftPresentationImageIndex = -1;

			public ResliceTool(ResliceToolGroup resliceToolGroup)
			{
				base.Behaviour |= MouseButtonHandlerBehaviour.SuppressOnTileActivate;

				_resliceToolGroup = resliceToolGroup;
			}

			public ResliceToolGroup ToolGroup
			{
				get { return _resliceToolGroup; }
			}

			public string Label
			{
				get
				{
					if (_sliceSet != null)
					{
						if (this.SliceImageBox != null)
							return string.Format(SR.MenuResliceFor, this.SliceImageBox.DisplaySet.Description);
					}
					return string.Empty;
				}
			}

			public Color HotColor
			{
				get { return _hotColor; }
				set { _hotColor = value; }
			}

			public Color NormalColor
			{
				get { return _normalColor; }
				set { _normalColor = value; }
			}

			private void OnImageViewerImageBoxDrawing(object sender, ImageBoxDrawingEventArgs e)
			{
				IImageBox imageBox = this.SliceImageBox;

				IDisplaySet containingDisplaySet = null;
				if (_resliceGraphic.ParentPresentationImage != null)
					containingDisplaySet = _resliceGraphic.ParentPresentationImage.ParentDisplaySet;

				if (containingDisplaySet != null && containingDisplaySet.ImageBox == e.ImageBox)
				{
					// translocate the graphic if the user is stacking through the display set that the graphic sits in
					// do not add this command to history - the stack command generates the actual action command
					if (_lastTopLeftPresentationImageIndex != e.ImageBox.TopLeftPresentationImageIndex)
					{
						_lastTopLeftPresentationImageIndex = e.ImageBox.TopLeftPresentationImageIndex;
						TranslocateGraphic(_resliceGraphic, e.ImageBox.TopLeftPresentationImage);
					}
				}
				else if (imageBox != null && imageBox == e.ImageBox)
				{
					// we're stacking on the set we control, so make sure the colourised display set name is replicated
					IPresentationImage firstReslicedImage = imageBox.TopLeftPresentationImage;
					ColorizeDisplaySetDescription(firstReslicedImage, this.NormalColor);

					// and realign the slice line with the stacked position
					if (_resliceGraphic.ParentPresentationImage != null && _resliceGraphic.ParentPresentationImage != firstReslicedImage)
					{
						_resliceGraphic.SetLine(imageBox.TopLeftPresentationImage, _resliceGraphic.ParentPresentationImage);
						_resliceGraphic.Draw();
					}
				}
			}

			protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
			{
				base.OnPresentationImageSelected(sender, e);

				IImageBox imageBox = this.SliceImageBox;

				IDisplaySet selectedDisplaySet = null;
				if (this.SelectedPresentationImage != null)
					selectedDisplaySet = this.SelectedPresentationImage.ParentDisplaySet;

				// only allow tool if we're in a MprViewerComponent, and we're not going to be operating on ourself!
				base.Enabled = (this.ImageViewer != null) &&
							   (imageBox != null && selectedDisplaySet != null && selectedDisplaySet != imageBox.DisplaySet);
			}

			#region Controlled SliceSet

			public event EventHandler SliceSetChanged;

			private IMprStandardSliceSet _sliceSet;
			private IImageBox _sliceImageBox;

			public IMprStandardSliceSet SliceSet
			{
				get { return _sliceSet; }
				set
				{
					if (_sliceSet != value)
					{
						_sliceSet = value;
						this.OnSliceSetChanged();
					}
				}
			}

			/// <summary>
			/// Gets the <see cref="IImageBox"/> containing the slicing we control.
			/// </summary>
			public IImageBox SliceImageBox
			{
				get
				{
					if (_sliceImageBox == null)
					{
						_sliceImageBox = FindImageBox(this.SliceSet, this.ImageViewer);
					}
					return _sliceImageBox;
				}
			}

			protected virtual void OnSliceSetChanged()
			{
				_sliceImageBox = null;
				EventsHelper.Fire(this.SliceSetChanged, this, EventArgs.Empty);
			}

			#endregion

			private void RemoveGraphicBuilder()
			{
				if (_lineGraphicBuilder != null)
				{
					_lineGraphicBuilder.GraphicComplete -= OnGraphicBuilderDone;
					_lineGraphicBuilder.GraphicCancelled -= OnGraphicBuilderCancelled;
					_lineGraphicBuilder = null;
				}
			}

			public override void Initialize()
			{
				base.Initialize();

				if (this.SliceImageBox == null)
					throw new InvalidOperationException("Tool has nothing to control because the specified slice set is not visible.");

				_resliceGraphic = new ResliceToolGraphic(this);
				_resliceGraphic.Color = this.NormalColor;
				_resliceGraphic.HotColor = this.HotColor;
				_resliceGraphic.Points.PointChanged += OnAnchorPointChanged;
				_resliceGraphic.Text = this.SliceImageBox.DisplaySet.Description;

				// draw the reslice graphic on the first imagebox that isn't showing the slicing this tool controls and is not parallel
				foreach (IImageBox imageBox in this.ImageViewer.PhysicalWorkspace.ImageBoxes)
				{
					if (imageBox != this.SliceImageBox)
					{
						if (_resliceGraphic.SetLine(this.SliceImageBox.TopLeftPresentationImage, imageBox.TopLeftPresentationImage))
						{
							TranslocateGraphic(_resliceGraphic, imageBox.TopLeftPresentationImage);
							break;
						}
					}
				}
				ColorizeDisplaySetDescription(this.SliceImageBox.TopLeftPresentationImage, this.NormalColor);

				this.ImageViewer.EventBroker.ImageBoxDrawing += OnImageViewerImageBoxDrawing;
			}

			protected override void Dispose(bool disposing)
			{
				this.ImageViewer.EventBroker.ImageBoxDrawing -= OnImageViewerImageBoxDrawing;

				if (disposing)
				{
					if (this.SliceSet != null)
					{
						this.SliceSet = null;
					}

					if (_resliceGraphic != null)
					{
						TranslocateGraphic(_resliceGraphic, null);
						_resliceGraphic.Points.PointChanged -= OnAnchorPointChanged;
						_resliceGraphic.Dispose();
						_resliceGraphic = null;
					}
				}

				_resliceToolGroup = null;
				base.Dispose(disposing);
			}

			public override bool Start(IMouseInformation mouseInformation)
			{
				// don't let the tool start if we're disabled!
				if (!this.Enabled)
					return false;

				base.Start(mouseInformation);

				if (_lineGraphicBuilder != null)
					return _lineGraphicBuilder.Start(mouseInformation);

				IPresentationImage image = mouseInformation.Tile.PresentationImage;

				IOverlayGraphicsProvider provider = image as IOverlayGraphicsProvider;
				if (provider == null)
					return false;

				// this memento will be consumed when the graphic builder is completed or cancelled
				_originalResliceToolsState = _resliceToolGroup.ToolGroupState.CreateMemento();

				TranslocateGraphic(_resliceGraphic, this.SelectedPresentationImage);

				// The interactive graphic builders typically operate on new, pristine graphics
				// Since our graphic isn't new, clear the points from it! (Otherwise you'll end up with a polyline)
				_resliceGraphic.Points.Clear();

				_lineGraphicBuilder = new InteractivePolylineGraphicBuilder(2, _resliceGraphic);
				_lineGraphicBuilder.GraphicComplete += OnGraphicBuilderDone;
				_lineGraphicBuilder.GraphicCancelled += OnGraphicBuilderCancelled;

				if (_lineGraphicBuilder.Start(mouseInformation))
				{
					return true;
				}

				this.Cancel();
				return false;
			}

			private void OnGraphicBuilderDone(object sender, GraphicEventArgs e)
			{
				if (base.ImageViewer.CommandHistory != null)
				{
					DrawableUndoableCommand compositeCommand = new DrawableUndoableCommand(this.ImageViewer.PhysicalWorkspace);
					compositeCommand.Name = SR.CommandMprReslice;

					MemorableUndoableCommand toolGroupStateCommand = new MemorableUndoableCommand(_resliceToolGroup.ToolGroupState);
					toolGroupStateCommand.BeginState = _originalResliceToolsState;
					toolGroupStateCommand.EndState = _resliceToolGroup.ToolGroupState.CreateMemento();
					compositeCommand.Enqueue(toolGroupStateCommand);

					base.ImageViewer.CommandHistory.AddCommand(compositeCommand);
				}

				_originalResliceToolsState = null;

				RemoveGraphicBuilder();

				_lastTopLeftPresentationImageIndex = this.SliceImageBox.TopLeftPresentationImageIndex;
			}

			private void OnGraphicBuilderCancelled(object sender, GraphicEventArgs e)
			{
				_resliceToolGroup.ToolGroupState.SetMemento(_originalResliceToolsState);

				_originalResliceToolsState = null;

				RemoveGraphicBuilder();

				this.ImageViewer.PhysicalWorkspace.Draw();
			}

			private void OnAnchorPointChanged(object sender, IndexEventArgs e)
			{
				IDrawable reslicedResult = this.Reslice();

				if (reslicedResult != null)
					reslicedResult.Draw();
			}

			private IDrawable Reslice()
			{
				Vector3D _startPatient = null;
				Vector3D _endPatient = null;

				IImageSopProvider imageSopProvider = _resliceGraphic.ParentPresentationImage as IImageSopProvider;

				// there must be two points already...
				if (_resliceGraphic.Points.Count > 1 && imageSopProvider != null)
				{
					_resliceGraphic.CoordinateSystem = CoordinateSystem.Destination;

					PointF start = _resliceGraphic.SpatialTransform.ConvertToSource(_resliceGraphic.Points[0]);
					PointF end = _resliceGraphic.SpatialTransform.ConvertToSource(_resliceGraphic.Points[1]);

					_resliceGraphic.ResetCoordinateSystem();

					_startPatient = imageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(start);
					_endPatient = imageSopProvider.Frame.ImagePlaneHelper.ConvertToPatient(end);
				}

				if (_startPatient == null || _endPatient == null)
					return null;

				if ((_startPatient - _endPatient).Magnitude < 5*imageSopProvider.Frame.NormalizedPixelSpacing.Row)
					return null;

				// set the new slice plane, which will regenerate the corresponding display set
				SetSlicePlane(this.SliceSet, _resliceGraphic.ParentPresentationImage, _startPatient, _endPatient);

				ColorizeDisplaySetDescription(this.SliceImageBox.TopLeftPresentationImage, this.NormalColor);

				return this.SliceImageBox.TopLeftPresentationImage;
			}

			public override bool Track(IMouseInformation mouseInformation)
			{
				if (_lineGraphicBuilder != null)
					return _lineGraphicBuilder.Track(mouseInformation);

				return false;
			}

			public override bool Stop(IMouseInformation mouseInformation)
			{
				if (_lineGraphicBuilder == null)
					return false;

				if (_lineGraphicBuilder.Stop(mouseInformation))
				{
					return true;
				}

				return false;
			}

			public override void Cancel()
			{
				if (_lineGraphicBuilder == null)
					return;

				_lineGraphicBuilder.Cancel();
			}

			public override CursorToken GetCursorToken(Point point)
			{
				if (_lineGraphicBuilder != null)
					return _lineGraphicBuilder.GetCursorToken(point);

				return base.GetCursorToken(point);
			}

			/// <summary>
			/// Gets the image on which the slice line is defined.
			/// </summary>
			public IPresentationImage ReferenceImage
			{
				get
				{
					if (_resliceGraphic.ParentPresentationImage == null || _resliceGraphic.ParentPresentationImage.ParentDisplaySet == null)
						return null;
					return _resliceGraphic.ParentPresentationImage; 
				}
			}

			/// <summary>
			/// Sets the image on which the slice line is defined.
			/// </summary>
			/// <param name="referenceImage"></param>
			/// <returns>True if the reference image was successfully changed; False otherwise (e.g. the specified image does not intersect the sliced images)</returns>
			public bool SetReferenceImage(IPresentationImage referenceImage)
			{
				if (_resliceGraphic.ParentPresentationImage != referenceImage)
				{
					if (referenceImage == null)
					{
						// if we change the reference image to nothing, hide the graphic
						TranslocateGraphic(_resliceGraphic, null);
						return true;
					}
					else if (_resliceGraphic.SetLine(this.SliceImageBox.TopLeftPresentationImage, referenceImage))
					{
						// if we change the reference image to something and we know how they intersect, move the graphic over
						TranslocateGraphic(_resliceGraphic, referenceImage);
						return true;
					}
					else
					{
						// if we change the reference image to something and they don't actually intersect (it happens in odd cases), hide the graphic but report failure
						TranslocateGraphic(_resliceGraphic, null);
						return false;
					}
				}
				return false;
			}

			#region IMemorable

			public object CreateMemento()
			{
				return new ImageHint(this.ReferenceImage);
			}

			public void SetMemento(object memento)
			{
				ImageHint state = memento as ImageHint;
				if (state == null)
					return;

				if (this.SliceSet != null && !this.SliceSet.IsReadOnly)
				{
					// don't check for null - if the hint image doesn't exist, then we're going to hide the graphic
					this.SetReferenceImage(state.Image);
				}
			}

			#endregion
		}
	}
}
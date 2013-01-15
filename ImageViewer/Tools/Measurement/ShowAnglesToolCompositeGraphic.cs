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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	partial class ShowAnglesTool
	{
		[Cloneable]
		internal partial class ShowAnglesToolCompositeGraphic : CompositeGraphic
		{
			// This must not be CloneIgnore because we lack framework support to pick the correct cloned instance from the parent image (#6668)
			private IPointsGraphic _selectedLine;

			[CloneIgnore]
			private ShowAnglesTool _ownerTool;

			[CloneIgnore]
			private bool _isDirty = true;

			[CloneIgnore]
			private readonly bool _sourceGraphicVisible = true;

			public ShowAnglesToolCompositeGraphic(ShowAnglesTool ownerTool) : base()
			{
				_ownerTool = ownerTool;
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected ShowAnglesToolCompositeGraphic(ShowAnglesToolCompositeGraphic source, ICloningContext context) : base()
			{
				context.CloneFields(source, this);

				_sourceGraphicVisible = source.Visible;
			}

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				this.Visible = _sourceGraphicVisible;
			}

			protected override void Dispose(bool disposing)
			{
				_ownerTool = null;

				base.Dispose(disposing);
			}

			public override bool Visible
			{
				get
				{
					if (_ownerTool != null && !_ownerTool.ShowAngles)
						return false;
					return base.Visible;
				}
				set { base.Visible = value; }
			}

			#region Unit Test Support

#if UNIT_TESTS
			internal IPointsGraphic SelectedLine
			{
				get { return _selectedLine; }
			}
#endif

			#endregion

			protected override void OnParentPresentationImageChanged(IPresentationImage oldParentPresentationImage, IPresentationImage newParentPresentationImage)
			{
				if (oldParentPresentationImage != null)
				{
					IOverlayGraphicsProvider overlayGraphicsProvider = oldParentPresentationImage as IOverlayGraphicsProvider;
					if (overlayGraphicsProvider != null)
					{
						overlayGraphicsProvider.OverlayGraphics.ItemChanging -= OnOverlayGraphicsItemRemoved;
						overlayGraphicsProvider.OverlayGraphics.ItemRemoved -= OnOverlayGraphicsItemRemoved;
					}
				}

				base.OnParentPresentationImageChanged(oldParentPresentationImage, newParentPresentationImage);

				if (newParentPresentationImage != null)
				{
					IOverlayGraphicsProvider overlayGraphicsProvider = newParentPresentationImage as IOverlayGraphicsProvider;
					if (overlayGraphicsProvider != null)
					{
						overlayGraphicsProvider.OverlayGraphics.ItemRemoved += OnOverlayGraphicsItemRemoved;
						overlayGraphicsProvider.OverlayGraphics.ItemChanging += OnOverlayGraphicsItemRemoved;
					}
				}

				_isDirty = true;
			}

			private void OnOverlayGraphicsItemRemoved(object sender, ListEventArgs<IGraphic> e)
			{
				IPointsGraphic lineGraphic = GetLine(e.Item);
				if (ReferenceEquals(_selectedLine, lineGraphic))
				{
					this.Select(null);
				}
			}

			public void Select(IGraphic graphic)
			{
				if (graphic != null)
				{
					Platform.CheckFalse(ReferenceEquals(graphic.ParentPresentationImage, null), "Supplied graphic must be on the same presentation image.");
					Platform.CheckTrue(ReferenceEquals(graphic.ParentPresentationImage, this.ParentPresentationImage), "Supplied graphic must be on the same presentation image.");
				}

				IPointsGraphic value = GetLine(graphic);
				if (_selectedLine != value)
				{
					if (_selectedLine != null)
					{
						_selectedLine.Points.PointAdded -= OnSelectedLinePointChanged;
						_selectedLine.Points.PointChanged -= OnSelectedLinePointChanged;
						_selectedLine.Points.PointRemoved -= OnSelectedLinePointChanged;
						_selectedLine.Points.PointsCleared -= OnSelectedLinePointsCleared;
					}

					_selectedLine = value;

					if (_selectedLine != null)
					{
						_selectedLine.Points.PointAdded += OnSelectedLinePointChanged;
						_selectedLine.Points.PointChanged += OnSelectedLinePointChanged;
						_selectedLine.Points.PointRemoved += OnSelectedLinePointChanged;
						_selectedLine.Points.PointsCleared += OnSelectedLinePointsCleared;
					}

					Visible = _selectedLine != null;
					_isDirty = true;
				}
			}

			public override void OnDrawing()
			{
				base.OnDrawing();

				if (!_isDirty)
					return;

				IOverlayGraphicsProvider overlayGraphicsProvider = this.ParentPresentationImage as IOverlayGraphicsProvider;
				if (overlayGraphicsProvider == null)
					return;

				IList<ShowAnglesToolGraphic> freeAngleGraphics = CollectionUtils.Cast<ShowAnglesToolGraphic>(this.Graphics);

				if (this.Visible && _selectedLine != null && _selectedLine.Points.Count == 2)
				{
					_selectedLine.CoordinateSystem = CoordinateSystem.Source;
					try
					{
						foreach (IGraphic otherLineGraphic in overlayGraphicsProvider.OverlayGraphics)
						{
							IPointsGraphic otherLine = GetLine(otherLineGraphic);
							if (otherLine != null && !ReferenceEquals(otherLine, _selectedLine) && otherLine.Points.Count == 2)
							{
								ShowAnglesToolGraphic showAnglesToolGraphic;
								if (freeAngleGraphics.Count > 0)
									freeAngleGraphics.Remove(showAnglesToolGraphic = freeAngleGraphics[0]);
								else
									this.Graphics.Add(showAnglesToolGraphic = new ShowAnglesToolGraphic());

								showAnglesToolGraphic.CoordinateSystem = otherLine.CoordinateSystem = CoordinateSystem.Source;
								try
								{
									showAnglesToolGraphic.SetEndpoints(_selectedLine.Points[0], _selectedLine.Points[1], otherLine.Points[0], otherLine.Points[1]);
								}
								finally
								{
									showAnglesToolGraphic.ResetCoordinateSystem();
									otherLine.ResetCoordinateSystem();
								}
							}
						}
					}
					finally
					{
						_selectedLine.ResetCoordinateSystem();
					}
				}

				foreach (IGraphic freeAngleGraphic in freeAngleGraphics)
				{
					this.Graphics.Remove(freeAngleGraphic);
					freeAngleGraphic.Dispose();
				}
			}

			private void OnSelectedLinePointsCleared(object sender, EventArgs e)
			{
				_isDirty = true;
			}

			private void OnSelectedLinePointChanged(object sender, IndexEventArgs e)
			{
				_isDirty = true;
			}

			private static IPointsGraphic GetLine(IGraphic graphic)
			{
				if (graphic is IControlGraphic)
					return ((IControlGraphic) graphic).Subject as IPointsGraphic;
				else
					return graphic as IPointsGraphic;
			}
		}
	}
}
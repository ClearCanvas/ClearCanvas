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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	partial class ResliceToolGroup
	{
		private class ResliceToolGraphic : CompositeGraphic, IPointsGraphic, IMemorable
		{
			private LineGraphic _lineGraphic;
			private SliceControlGraphic _sliceControlGraphic;
			private StandardStatefulGraphic _stateControlGraphic;
			private ResliceTool _owner;
			private object _controlGraphicBeginState;

			public ResliceToolGraphic(ResliceTool owner)
			{
				LineGraphic lineGraphic = new LineGraphic();
				MprMoveControlGraphic moveControlGraphic = new MprMoveControlGraphic(lineGraphic);
				moveControlGraphic.UndoableOperationStart += OnControlGraphicUndoableOperationStart;
				moveControlGraphic.UndoableOperationStop += OnControlGraphicUndoableOperationStop;
				moveControlGraphic.UndoableOperationCancel += OnControlGraphicUndoableOperationCancel;
				MprLineStretchControlGraphic lineControlGraphic = new MprLineStretchControlGraphic(moveControlGraphic);
				lineControlGraphic.UndoableOperationStart += OnControlGraphicUndoableOperationStart;
				lineControlGraphic.UndoableOperationStop += OnControlGraphicUndoableOperationStop;
				lineControlGraphic.UndoableOperationCancel += OnControlGraphicUndoableOperationCancel;
				SliceControlGraphic sliceControlGraphic = new SliceControlGraphic(lineControlGraphic, this);
				StandardStatefulGraphic statefulGraphic = new StandardStatefulGraphic(sliceControlGraphic);
				statefulGraphic.State = statefulGraphic.CreateInactiveState();
				base.Graphics.Add(statefulGraphic);

				_stateControlGraphic = statefulGraphic;
				_sliceControlGraphic = sliceControlGraphic;
				_lineGraphic = lineGraphic;
				_owner = owner;
			}

			protected override void Dispose(bool disposing)
			{
				_owner = null;
				_lineGraphic = null;
				_sliceControlGraphic = null;
				_stateControlGraphic = null;

				base.Dispose(disposing);
			}

			public IPointsList Points
			{
				get { return _lineGraphic.Points; }
			}

			public Color Color
			{
				get { return _stateControlGraphic.InactiveColor; }
				set
				{
					_stateControlGraphic.InactiveColor = _stateControlGraphic.SelectedColor = value;
					if (!_stateControlGraphic.Focussed)
						_stateControlGraphic.Color = _sliceControlGraphic.Color = _lineGraphic.Color = value;
				}
			}

			public Color HotColor
			{
				get { return _stateControlGraphic.FocusColor; }
				set
				{
					_stateControlGraphic.FocusColor = _stateControlGraphic.FocusSelectedColor = value;
					if (_stateControlGraphic.Focussed)
						_stateControlGraphic.Color = _sliceControlGraphic.Color = _lineGraphic.Color = value;
				}
			}

			public LineStyle LineStyle
			{
				get { return _lineGraphic.LineStyle; }
				set { _lineGraphic.LineStyle = value; }
			}

			public string Text
			{
				get { return _sliceControlGraphic.Text; }
				set { _sliceControlGraphic.Text = value; }
			}

			/// <summary>
			/// Sets this line segment's end points to be the intersection of the <paramref name="sliceImage"/> on the infinite plane of the <paramref name="referenceImage"/>.
			/// </summary>
			/// <remarks>
			/// In practice, a call to this method should have <paramref name="referenceImage"/> being equal to this line's <see cref="Graphic.ParentPresentationImage"/> or,
			/// if not, then followed by a call to <see cref="ResliceToolGroup.TranslocateGraphic">translocate</see> the line to the <paramref name="referenceImage"/>.
			/// Any other use may constitute a defect involving reference line bounds, and you should thusly inspect your code and explain why this is the case.
			/// </remarks>
			/// <param name="sliceImage">The slice whose position is controlled by this line.</param>
			/// <param name="referenceImage">The image on which the line is (or is to be) defined.</param>
			/// <returns>Returns true if a solution exists and the end points were updated.</returns>
			public bool SetLine(IPresentationImage sliceImage, IPresentationImage referenceImage)
			{
				DicomImagePlane sliceImagePlane = DicomImagePlane.FromImage(sliceImage);
				DicomImagePlane referenceImagePlane = DicomImagePlane.FromImage(referenceImage);

				if (!sliceImagePlane.IsParallelTo(referenceImagePlane, 1f))
				{
					Vector3D imagePt1 = referenceImagePlane.ConvertToImagePlane(sliceImagePlane.PositionPatientTopLeft);
					Vector3D imagePt2 = referenceImagePlane.ConvertToImagePlane(sliceImagePlane.PositionPatientTopRight);

					this.Points.SuspendEvents();
					this.Points.Clear();
					this.Points.Add(referenceImagePlane.ConvertToImage(new PointF(imagePt1.X, imagePt1.Y)));
					this.Points.Add(referenceImagePlane.ConvertToImage(new PointF(imagePt2.X, imagePt2.Y)));
					this.Points.ResumeEvents();
					return true;
				}
				return false;
			}

			#region IMemorable Members

			public object CreateMemento()
			{
				// this graphic is memorable, yes, but it's not the position of the line that is important, but rather the entire MPR state
				// the graphic only implements IMemorable because the move and drag endpoint control graphics will automatically insert
				// history commands for these operations for us.
				return _owner.ToolGroup.ToolGroupState.CreateMemento();
			}

			public void SetMemento(object obj)
			{
				_owner.ToolGroup.ToolGroupState.SetMemento(obj);
			}

			private void OnControlGraphicUndoableOperationStart(object sender, EventArgs e)
			{
				_controlGraphicBeginState = this.CreateMemento();
			}

			private void OnControlGraphicUndoableOperationCancel(object sender, EventArgs e)
			{
				_controlGraphicBeginState = null;
			}

			private void OnControlGraphicUndoableOperationStop(object sender, EventArgs e)
			{
				if (base.ImageViewer.CommandHistory != null)
				{
					DrawableUndoableCommand compositeCommand = new DrawableUndoableCommand(this.ImageViewer.PhysicalWorkspace);
					compositeCommand.Name = ((ControlGraphic) sender).CommandName;

					MemorableUndoableCommand toolGroupStateCommand = new MemorableUndoableCommand(this);
					toolGroupStateCommand.BeginState = _controlGraphicBeginState;
					toolGroupStateCommand.EndState = this.CreateMemento();
					compositeCommand.Enqueue(toolGroupStateCommand);

					base.ImageViewer.CommandHistory.AddCommand(compositeCommand);
				}

				_controlGraphicBeginState = null;
			}

			#endregion

			#region SliceControlGraphic

			private class SliceControlGraphic : ControlGraphic
			{
				private InvariantTextPrimitive _textGraphic;
				private ResliceToolGraphic _topParent;

				public SliceControlGraphic(IGraphic subject, ResliceToolGraphic topParent) : base(subject)
				{
					_topParent = topParent;
					_textGraphic = new InvariantTextPrimitive();
					_textGraphic.BoundingBoxChanged += TextGraphic_BoundingBoxChanged;
					base.Graphics.Add(_textGraphic);
					base.DecoratedGraphic.VisualStateChanged += DecoratedGraphic_VisualStateChanged;
				}

				protected override void Dispose(bool disposing)
				{
					base.DecoratedGraphic.VisualStateChanged -= DecoratedGraphic_VisualStateChanged;

					if (_textGraphic != null)
					{
						_textGraphic.BoundingBoxChanged -= TextGraphic_BoundingBoxChanged;
						_textGraphic = null;
					}

					_topParent = null;

					base.Dispose(disposing);
				}

				public string Text
				{
					get { return _textGraphic.Text; }
					set { _textGraphic.Text = value; }
				}

				public override void OnDrawing()
				{
					KeepTextInsideClientRectangle();
					base.OnDrawing();
				}

				protected override void OnColorChanged()
				{
					base.OnColorChanged();
					_textGraphic.Color = this.Color;
				}

				private void TextGraphic_BoundingBoxChanged(object sender, RectangleChangedEventArgs e)
				{
					KeepTextInsideClientRectangle();
				}

				private void DecoratedGraphic_VisualStateChanged(object sender, VisualStateChangedEventArgs e)
				{
					base.DecoratedGraphic.CoordinateSystem = CoordinateSystem.Destination;
					_textGraphic.CoordinateSystem = CoordinateSystem.Destination;
					try
					{
						RectangleF rectangle = base.DecoratedGraphic.BoundingBox;
						_textGraphic.Location = new PointF(rectangle.Right, rectangle.Top);
					}
					finally
					{
						_textGraphic.ResetCoordinateSystem();
						base.DecoratedGraphic.ResetCoordinateSystem();
					}
				}

				private void KeepTextInsideClientRectangle()
				{
					this.CoordinateSystem = CoordinateSystem.Destination;
					try
					{
						PointF startPoint, endPoint;
						float lengthOfLineThroughTextBox;
						if (!GetTextBoxAdjustmentParameters(out startPoint, out endPoint, out lengthOfLineThroughTextBox))
						{
							_textGraphic.Location = _topParent.Points[0];
							return;
						}

						Vector3D lineDirection = new Vector3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, 0F);

						float ratioLengthOfLineThroughTextBox = lengthOfLineThroughTextBox/lineDirection.Magnitude;

						SizeF textEdgeOffset = new SizeF(ratioLengthOfLineThroughTextBox*lineDirection.X, ratioLengthOfLineThroughTextBox*lineDirection.Y);

						SizeF textAnchorPointOffset = new SizeF(textEdgeOffset.Width/2F, textEdgeOffset.Height/2F);

						Vector3D lineUnit = lineDirection.Normalize();

						// extend the endpoint of the line by the distance to the outside text edge.
						endPoint = PointF.Add(endPoint, textEdgeOffset);
						// add an additional 5 pixel offset so we don't push back as far as the start point.
						endPoint = PointF.Add(endPoint, new SizeF(5F*lineUnit.X, 5F*lineUnit.Y));

						SizeF clientEdgeOffset = Size.Empty;

						// find the intersection of the extended line segment and either of the left or bottom client edge.
						PointF? intersectionPoint = GetClientRightOrBottomEdgeIntersectionPoint(startPoint, endPoint);
						if (intersectionPoint != null)
						{
							Vector3D clientEdgeOffsetVector = new Vector3D(endPoint.X - intersectionPoint.Value.X, endPoint.Y - intersectionPoint.Value.Y, 0);
							//don't allow the text to be pushed back past the start point.
							if (clientEdgeOffsetVector.Magnitude > lineDirection.Magnitude)
								clientEdgeOffsetVector = lineDirection;

							clientEdgeOffset = new SizeF(clientEdgeOffsetVector.X, clientEdgeOffsetVector.Y);
						}

						// offset by the distance from the extended endpoint to the client rectangle edge.
						endPoint = PointF.Subtract(endPoint, clientEdgeOffset);
						// offset again by half the distance necessary to keep the text box inside the client rectangle
						endPoint = PointF.Subtract(endPoint, textAnchorPointOffset);

						// this aligns the text edge with the client edge in the case where the line intersects the client edge.
						_textGraphic.Location = endPoint;
					}
					finally
					{
						this.ResetCoordinateSystem();
					}
				}

				private bool GetTextBoxAdjustmentParameters(out PointF startPoint, out PointF endPoint, out float lengthOfLineThroughTextBox)
				{
					startPoint = _topParent.Points[0];
					endPoint = _topParent.Points[1];
					lengthOfLineThroughTextBox = 0;

					Vector3D lineDirection = new Vector3D(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, 0F);

					if (Vector3D.AreEqual(lineDirection, Vector3D.Null))
						return false;

					Vector3D lineUnit = lineDirection.Normalize();

					Vector3D xUnit = new Vector3D(1F, 0, 0);
					Vector3D yUnit = new Vector3D(0, 1F, 0);

					float cosThetaX = Math.Abs(xUnit.Dot(lineUnit));
					float cosThetaY = Math.Abs(yUnit.Dot(lineUnit));

					float textWidth = _textGraphic.BoundingBox.Width;
					float textHeight = _textGraphic.BoundingBox.Height;

					if (cosThetaX >= cosThetaY)
					{
						// the distance along the line to where we want the outside right edge of the text to be.
						lengthOfLineThroughTextBox = cosThetaX*textWidth;
						if (lineDirection.X < 0)
						{
							startPoint = _topParent.Points[1];
							endPoint = _topParent.Points[0];
						}
					}
					else
					{
						// the distance along the line to where we want the outside bottom edge of the text to be.
						lengthOfLineThroughTextBox = cosThetaY*textHeight;
						if (lineDirection.Y < 0)
						{
							startPoint = _topParent.Points[1];
							endPoint = _topParent.Points[0];
						}
					}

					return true;
				}

				private PointF? GetClientRightOrBottomEdgeIntersectionPoint(PointF lineSegmentStartPoint, PointF lineSegmentEndPoint)
				{
					Rectangle clientRectangle = base.ParentPresentationImage.ClientRectangle;

					PointF clientTopRight = new PointF(clientRectangle.Right, clientRectangle.Top);
					PointF clientBottomLeft = new PointF(clientRectangle.Left, clientRectangle.Bottom);
					PointF clientBottomRight = new PointF(clientRectangle.Right, clientRectangle.Bottom);

					PointF intersectionPoint;
					if (Vector.IntersectLineSegments(lineSegmentStartPoint, lineSegmentEndPoint, clientTopRight, clientBottomRight, out intersectionPoint))
					{
						return intersectionPoint;
					}
					else if (Vector.IntersectLineSegments(lineSegmentStartPoint, lineSegmentEndPoint, clientBottomLeft, clientBottomRight, out intersectionPoint))
					{
						return intersectionPoint;
					}
					else
					{
						return null;
					}
				}
			}

			#endregion

			#region Custom Line

			private class LineGraphic : ArrowGraphic
			{
				private LineSegmentGraphicPointsAdapter _lineAdapter;

				public LineGraphic()
				{
					_lineAdapter = new LineSegmentGraphicPointsAdapter(this);
				}

				protected override void Dispose(bool disposing)
				{
					if (disposing && _lineAdapter != null)
					{
						_lineAdapter.Dispose();
						_lineAdapter = null;
					}
					base.Dispose(disposing);
				}

				public IPointsList Points
				{
					get { return _lineAdapter; }
				}

				public override void Move(SizeF delta)
				{
					_lineAdapter.SuspendEvents();
					try
					{
						base.Move(delta);
					}
					finally
					{
						_lineAdapter.ResumeEvents();
						_lineAdapter.FirePointsChanged();
					}
				}

				/// <summary>
				/// Adapter class to map an <see cref="ILineSegmentGraphic"/> as the <see cref="IPointsList"/> exposed by an <see cref="IPointsGraphic"/>.
				/// </summary>
				private class LineSegmentGraphicPointsAdapter : IPointsList, IDisposable
				{
					public event EventHandler<IndexEventArgs> PointAdded;
					public event EventHandler<IndexEventArgs> PointChanged;
					public event EventHandler<IndexEventArgs> PointRemoved;
					public event EventHandler PointsCleared;

					private ILineSegmentGraphic _lineGraphic;
					private int _pointCount = 0;
					private bool _enableInternalEvent = true;
					private bool _eventsEnabled = true;

					public LineSegmentGraphicPointsAdapter(ILineSegmentGraphic lineGraphic)
					{
						_lineGraphic = lineGraphic;
						_lineGraphic.Point1Changed += _lineGraphic_Point1Changed;
						_lineGraphic.Point2Changed += _lineGraphic_Point2Changed;
					}

					public void Dispose()
					{
						if (_lineGraphic != null)
						{
							_lineGraphic.Point1Changed -= _lineGraphic_Point1Changed;
							_lineGraphic.Point2Changed -= _lineGraphic_Point2Changed;
							_lineGraphic = null;
						}
					}

					private void _lineGraphic_Point2Changed(object sender, PointChangedEventArgs e)
					{
						if (_enableInternalEvent && _pointCount > 1)
							this.NotifyPointChanged(1);
					}

					private void _lineGraphic_Point1Changed(object sender, PointChangedEventArgs e)
					{
						if (_enableInternalEvent && _pointCount > 0)
							this.NotifyPointChanged(0);
					}

					private void NotifyPointChanged(int index)
					{
						if (_eventsEnabled)
							EventsHelper.Fire(this.PointChanged, this, new IndexEventArgs(index));
					}

					private void NotifyPointAdded(int index)
					{
						if (_eventsEnabled)
							EventsHelper.Fire(this.PointAdded, this, new IndexEventArgs(index));
					}

					private void NotifyPointRemoved(int index)
					{
						if (_eventsEnabled)
							EventsHelper.Fire(this.PointRemoved, this, new IndexEventArgs(index));
					}

					private void NotifyPointsCleared()
					{
						if (_eventsEnabled)
							EventsHelper.Fire(this.PointsCleared, this, EventArgs.Empty);
					}

					public void FirePointsChanged()
					{
						this.NotifyPointChanged(0);
					}

					#region IPointsList Members

					public bool IsClosed
					{
						get { return false; }
					}

					public void SuspendEvents()
					{
						_eventsEnabled = false;
					}

					public void ResumeEvents()
					{
						_eventsEnabled = true;
					}

					public int IndexOf(PointF item)
					{
						if (_pointCount > 0 && FloatComparer.AreEqual(item, _lineGraphic.Point1))
							return 0;
						else if (_pointCount > 1 && FloatComparer.AreEqual(item, _lineGraphic.Point2))
							return 1;
						return -1;
					}

					public void Insert(int index, PointF item)
					{
						_enableInternalEvent = false;
						try
						{
							if (index == 0)
							{
								_lineGraphic.Point2 = _lineGraphic.Point1;
								_lineGraphic.Point1 = item;
							}
							else if (index == 1)
							{
								_lineGraphic.Point2 = item;
							}
							++_pointCount;
							this.NotifyPointAdded(index);
						}
						finally
						{
							_enableInternalEvent = true;
						}
					}

					public void RemoveAt(int index)
					{
						_enableInternalEvent = false;
						try
						{
							if (index == 0)
								_lineGraphic.Point1 = _lineGraphic.Point2;
							_pointCount--;
							this.NotifyPointRemoved(index);
						}
						finally
						{
							_enableInternalEvent = true;
						}
					}

					public PointF this[int index]
					{
						get
						{
							if (index >= _pointCount)
								throw new IndexOutOfRangeException();
							else if (index == 0)
								return _lineGraphic.Point1;
							else if (index == 1)
								return _lineGraphic.Point2;
							return PointF.Empty;
						}
						set
						{
							if (index >= _pointCount)
								throw new IndexOutOfRangeException();
							else if (index == 0)
								_lineGraphic.Point1 = value;
							else if (index == 1)
								_lineGraphic.Point2 = value;
							// no event needs to be explicitly fired here - our hooked event handlers will fire one for us
						}
					}

					public void Add(PointF item)
					{
						_enableInternalEvent = false;
						try
						{
							if (_pointCount == 0)
								_lineGraphic.Point1 = item;
							else if (_pointCount == 1)
								_lineGraphic.Point2 = item;
							++_pointCount;
							this.NotifyPointAdded(_pointCount - 1);
						}
						finally
						{
							_enableInternalEvent = true;
						}
					}

					public void AddRange(IEnumerable<PointF> points)
					{
						foreach (var point in points)
							Add(point);
					}

					public void Clear()
					{
						_pointCount = 0;
						this.NotifyPointsCleared();
					}

					public bool Contains(PointF item)
					{
						return this.IndexOf(item) >= 0;
					}

					public void CopyTo(PointF[] array, int arrayIndex)
					{
						if (_pointCount > 0)
							array[arrayIndex++] = _lineGraphic.Point1;
						if (_pointCount > 1)
							array[arrayIndex++] = _lineGraphic.Point2;
						for (int n = 2; n < _pointCount; n++)
							array[arrayIndex++] = PointF.Empty;
					}

					public int Count
					{
						get { return _pointCount; }
					}

					public bool IsReadOnly
					{
						get { return false; }
					}

					public bool Remove(PointF item)
					{
						int index = this.IndexOf(item);
						if (index >= 0)
							this.RemoveAt(index);
						return index >= 0;
					}

					public IEnumerator<PointF> GetEnumerator()
					{
						if (_pointCount > 0)
							yield return _lineGraphic.Point1;
						if (_pointCount > 1)
							yield return _lineGraphic.Point2;
						for (int n = 2; n < _pointCount; n++)
							yield return PointF.Empty;
					}

					IEnumerator IEnumerable.GetEnumerator()
					{
						return this.GetEnumerator();
					}

					#endregion
				}
			}

			#endregion
		}
	}
}
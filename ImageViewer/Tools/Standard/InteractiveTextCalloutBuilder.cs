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
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.RoiGraphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	internal class InteractiveTextCalloutBuilder : InteractiveTextGraphicBuilder
	{
		private int clickIndex = 0;

		public InteractiveTextCalloutBuilder(UserCalloutGraphic textCalloutGraphic) : base(textCalloutGraphic) {}

		internal new UserCalloutGraphic Graphic
		{
			get { return (UserCalloutGraphic) base.Graphic; }
		}

		protected override ITextGraphic FindTextGraphic()
		{
			IGraphic graphic = this.Graphic;
			while (graphic != null && !(graphic is UserCalloutGraphic))
				graphic = graphic.ParentGraphic;
			if (graphic is UserCalloutGraphic)
				return new TextGraphicProxy((UserCalloutGraphic) graphic);
			return null;
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (clickIndex == 0)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.AnchorPoint = mouseInformation.Location;
				this.Graphic.TextLocation = mouseInformation.Location;
				this.Graphic.ResetCoordinateSystem();
			}
			else if (clickIndex == 1)
			{
				this.NotifyGraphicComplete();
			}
			else
			{
				return false;
			}

			clickIndex++;
			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (clickIndex == 1)
			{
				this.Graphic.CoordinateSystem = CoordinateSystem.Destination;
				this.Graphic.TextLocation = mouseInformation.Location;
				this.Graphic.ResetCoordinateSystem();
				this.Graphic.Draw();
			}

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			return true;
		}

		/// <summary>
		/// Proxy class
		/// </summary>
		private class TextGraphicProxy : ITextGraphic
		{
			private readonly UserCalloutGraphic _graphic;

			public TextGraphicProxy(UserCalloutGraphic calloutGraphic)
			{
				this._graphic = calloutGraphic;
			}

			#region ITextGraphic Members

			public string Text
			{
				get { return this._graphic.Text; }
				set { this._graphic.Text = value; }
			}

			public float SizeInPoints
			{
				get { return this._graphic.FontSize; }
				set { this._graphic.FontSize = value; }
			}

			public string Font
			{
				get { return this._graphic.FontName; }
				set { this._graphic.FontName = value; }
			}

			public SizeF Dimensions
			{
				get { return this._graphic.TextBoundingBox.Size; }
			}

			public PointF Location
			{
				get { return this._graphic.TextLocation; }
				set { this._graphic.TextLocation = value; }
			}

			#endregion

			#region IVectorGraphic Members

			public Color Color
			{
				get { return this._graphic.Color; }
				set { this._graphic.Color = value; }
			}

			public LineStyle LineStyle
			{
				get { return this._graphic.LineStyle; }
				set { this._graphic.LineStyle = value; }
			}

			#endregion

			#region IGraphic Members

			public IGraphic ParentGraphic
			{
				get { return this._graphic.ParentGraphic; }
			}

			public IPresentationImage ParentPresentationImage
			{
				get { return this._graphic.ParentPresentationImage; }
			}

			public IImageViewer ImageViewer
			{
				get { return this._graphic.ImageViewer; }
			}

			public bool Visible
			{
				get { return this._graphic.Visible; }
				set { this._graphic.Visible = value; }
			}

			public RectangleF BoundingBox
			{
				get { return this._graphic.TextBoundingBox; }
			}

			public CoordinateSystem CoordinateSystem
			{
				get { return this._graphic.CoordinateSystem; }
				set { this._graphic.CoordinateSystem = value; }
			}

			public SpatialTransform SpatialTransform
			{
				get { return this._graphic.SpatialTransform; }
			}

			public string Name
			{
				get { return this._graphic.Name; }
				set { this._graphic.Name = value; }
			}

			public bool HitTest(Point point)
			{
				return this._graphic.HitTest(point);
			}

			public PointF GetClosestPoint(PointF point)
			{
				return this._graphic.GetClosestPoint(point);
			}

			public void Move(SizeF delta)
			{
				this._graphic.Move(delta);
			}

			public void ResetCoordinateSystem()
			{
				this._graphic.ResetCoordinateSystem();
			}

			IGraphic IGraphic.Clone()
			{
				throw new NotSupportedException("Cloning an IGraphic through a proxy can lead to invalid references.");
			}

			public Roi GetRoi()
			{
				return this._graphic.GetRoi();
			}

			public event VisualStateChangedEventHandler VisualStateChanged
			{
				add { this._graphic.VisualStateChanged += value; }
				remove { this._graphic.VisualStateChanged -= value; }
			}

			#endregion

			#region IDrawable Members

			public event EventHandler Drawing
			{
				add { this._graphic.Drawing += value; }
				remove { this._graphic.Drawing -= value; }
			}

			public void Draw()
			{
				this._graphic.Draw();
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				this._graphic.Dispose();
			}

			#endregion
		}
	}
}
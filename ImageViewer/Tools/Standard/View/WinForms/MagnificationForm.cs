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
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Rendering;
using DrawMode = ClearCanvas.ImageViewer.Rendering.DrawMode;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	internal partial class MagnificationForm : Form
	{
        private readonly Point _startPointDesktop;
        private Point _startPointTile;
	    
        private IRenderingSurface _surface;

        private readonly RenderMagnifiedImage _render;

        public MagnificationForm(PresentationImage image, Point startPointTile, RenderMagnifiedImage render)
		{
			InitializeComponent();

            Visible = false;
			this.DoubleBuffered = false;
			this.SetStyle(ControlStyles.DoubleBuffer, false);
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

			if (Form.ActiveForm != null)
				this.Owner = Form.ActiveForm;

			_startPointTile = startPointTile;
            _render = render;

            _surface = image.ImageRenderer.CreateRenderingSurface(Handle, ClientRectangle.Width, ClientRectangle.Height, RenderingSurfaceType.Onscreen);
			_surface.Invalidated += OnSurfaceInvalidated;

            _startPointDesktop = Centre = Cursor.Position;
		}

		public void UpdateMousePosition(Point positionTile)
		{
			Size offsetFromStartTile = new Size(positionTile.X - _startPointTile.X, positionTile.Y - _startPointTile.Y);
			Point pointDesktop = _startPointDesktop;
			pointDesktop.Offset(offsetFromStartTile.Width, offsetFromStartTile.Height);
			Centre = pointDesktop;
		}

		private Point Centre
		{
			get
			{
				Point location = Location;
				location.Offset(Width / 2, Height / 2);
				return location;
			}	
			set
			{
				value.Offset(-Width / 2, -Height / 2);
				if (value != Location)
					base.Location = value;
			}
		}

		private void OnSurfaceInvalidated(object sender, EventArgs e)
		{
			Invalidate();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			//base.OnPaintBackground(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
            if (_surface != null)
            {
                _surface.ContextID = e.Graphics.GetHdc();
                try
                {
                    var args = new DrawArgs(_surface, new WinFormsScreenProxy(Screen.FromControl(this)), DrawMode.Refresh);
                    _render(args);
                }
                finally
                {
                    e.Graphics.ReleaseHdc(_surface.ContextID);
                }
            }

		    //base.OnPaint(e);
        }

		protected override void OnVisibleChanged(System.EventArgs e)
		{
			base.OnVisibleChanged(e);
			RenderImage();
		}

		protected override void OnLocationChanged(System.EventArgs e)
		{
			base.OnLocationChanged(e);

			RenderImage();

			if (base.Visible && base.Owner != null)
				base.Owner.Update(); //update owner's invalidated region(s)
		}

        private void RenderImage()
        {
            if (_surface == null)
                return;

            using (var graphics = base.CreateGraphics())
            {
                _surface.ContextID = graphics.GetHdc();
                try
                {
                    var args = new DrawArgs(_surface, new WinFormsScreenProxy(Screen.FromControl(this)), DrawMode.Render);
                    _render(args);
                    args = new DrawArgs(_surface, new WinFormsScreenProxy(Screen.FromControl(this)), DrawMode.Refresh);
                    _render(args);
                }
                finally 
                {
                    graphics.ReleaseHdc(_surface.ContextID);
                }
            }
        }
        
        private void DisposeSurface()
		{
			if (_surface != null)
			{
				_surface.Invalidated -= OnSurfaceInvalidated;
                _surface.Dispose();
                _surface = null;
			}
		}
	}
}
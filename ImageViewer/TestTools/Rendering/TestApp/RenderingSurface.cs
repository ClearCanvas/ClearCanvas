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
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace ClearCanvas.ImageViewer.TestTools.Rendering.TestApp
{
	public partial class RenderingSurface : UserControl, INotifyPropertyChanged
	{
		private readonly TestImageRenderer _renderer;
		private int _draws;
		private TimeSpan _time;

		public RenderingSurface()
		{
			InitializeComponent();
			_renderer = new TestImageRenderer();
			DoubleBuffered = !_renderer.CustomBackBuffer;
		}

		public IList<PixelFormat> GetPixelFormats()
		{
			return _renderer.GetPixelFormats();
		}

		public IList<GraphicsSource> GetGraphicsSources()
		{
			return _renderer.GetGraphicsSources();
		}

		public PixelFormat Format
		{
			get { return _renderer.Format; }
			set
			{
				_renderer.Format = value;
				Invalidate();
				FirePropertyChanged("PixelFormat");
			}
		}

		public GraphicsSource Source
		{
			get { return _renderer.Source; }
			set
			{
				_renderer.Source = value;
				Invalidate();
				FirePropertyChanged("GraphicsSource");
			}
		}

		public bool CustomBackBuffer
		{
			get { return _renderer.CustomBackBuffer; }
			set
			{
				_renderer.CustomBackBuffer = value;
				DoubleBuffered = !_renderer.CustomBackBuffer;

				Invalidate();
				FirePropertyChanged("CustomBackBuffer");
			}
		}

		public bool UseBufferedGraphics
		{
			get { return _renderer.UseBufferedGraphics; }
			set
			{
				_renderer.UseBufferedGraphics = value;
				Invalidate();
				FirePropertyChanged("UseBufferedGraphics");
			}
		}

		public Bitmap Bitmap
		{
			get { return _renderer.CustomImage; }
			set 
			{ 
				_renderer.CustomImage = value;
				Invalidate();
			}
		}

		public int Draws
		{
			get { return _draws; }	
		}

		public string Time
		{
			get { return _time.TotalSeconds.ToString("F2"); }	
		}

		public void ClearStats()
		{
			_draws = 0;
			_time = TimeSpan.Zero;
			_renderer.ResetStats();

			FirePropertyChanged("Draws");
			FirePropertyChanged("Time");
		}

		public void ReportStats()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat("Render: {0:F2}", _renderer.RenderTime.TotalSeconds);
			builder.AppendLine();
			builder.AppendFormat("Blit: {0:F2}", _renderer.BlitTime.TotalSeconds);
			MessageBox.Show(this, builder.ToString());
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			Invalidate();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			try
			{
				DateTime start = DateTime.Now;
				_renderer.RenderTo(e.Graphics, this.Size);
				
				DateTime end = DateTime.Now;
				TimeSpan duration = end.Subtract(start);
				_time = _time.Add(duration);
				++_draws;

				FirePropertyChanged("Draws");
				FirePropertyChanged("Time");
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			base.OnPaint(e);
		}

		private void FirePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}

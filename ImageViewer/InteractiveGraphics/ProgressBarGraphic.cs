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
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Enumeration of defined progress bar graphic styles.
	/// </summary>
	public enum ProgressBarGraphicStyle
	{
		/// <summary>
		/// Specifies a progress bar graphic where the current progress value is indicated by a number of discrete blocks.
		/// </summary>
		Blocks,

		/// <summary>
		/// Specifies a progress bar graphic where the current progress value is indicated by a contiguous bar.
		/// </summary>
		Continuous,

		/// <summary>
		/// Specifies a progress bar graphic where a graphical element moves repeatedly from left to right to indicate that work is in progress.
		/// </summary>
		Marquee,

		/// <summary>
		/// Specifies a progress bar graphic where a graphical element moves back and forth to indicate that work is in progress.
		/// </summary>
		Scanner
	}

	/// <summary>
	/// A graphic depicting a progress bar for the indication of work in progress.
	/// </summary>
	[Cloneable(true)]
	public abstract partial class ProgressBarGraphic : CompositeGraphic
	{
		//TODO (CR Sept 2010): I think inheritance is the wrong model here.  Seems like each type of
		//progress bar only overrides one method.  If the contents of that method were part of a "style"
		//class, then there would only need to be a single ProgressBarGraphic with a settable Style property.

		/// <summary>
		/// Creates a new <see cref="ProgressBarGraphic"/> in the specified style.
		/// </summary>
		/// <param name="style">The <see cref="ProgressBarGraphicStyle">style</see> of the <see cref="ProgressBarGraphic"/> to be created.</param>
		/// <returns>A new <see cref="ProgressBarGraphic"/> in the specified style.</returns>
		public static ProgressBarGraphic Create(ProgressBarGraphicStyle style)
		{
			switch (style)
			{
				case ProgressBarGraphicStyle.Continuous:
					return new ContinuousProgressBarGraphic();
				case ProgressBarGraphicStyle.Marquee:
					return new MarqueeProgressBarGraphic();
				case ProgressBarGraphicStyle.Scanner:
					return new ScannerProgressBarGraphic();
				case ProgressBarGraphicStyle.Blocks:
				default:
					return new BlocksProgressBarGraphic();
			}
		}

		private float _progress;

		//TODO (CR Sept 2010): follow the "typical" progress bar API where you can set the min/max/step?

		/// <summary>
		/// Gets or sets the current progress value as a fractional number between 0 and 1, inclusive.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the value is not between 0 and 1, inclusive.</exception>
		public float Progress
		{
			get { return _progress; }
			set
			{
				Platform.CheckTrue(value >= 0f && value <= 1f, "Progress must be between 0 and 1, inclusive.");
				if (_progress != value)
				{
					_progress = value;
					this.OnProgressChanged();
				}
			}
		}

		/// <summary>
		/// Gets or sets the current progress value as an integer between 0 and 100, inclusive.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the value is not between 0 and 100, inclusive.</exception>
		public int ProgressInt
		{
			get { return ProgressPercent; }
			set { ProgressPercent = value; }
		}

		/// <summary>
		/// Gets or sets the current progress value as an integer between 0 and 100, inclusive.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown if the value is not between 0 and 100, inclusive.</exception>
		public int ProgressPercent
		{
			get { return (int) (100*this.Progress); }
			set
			{
				Platform.CheckTrue(value >= 0 && value <= 100, "ProgressInt must be between 0 and 100, inclusive.");
				this.Progress = value/100f;
			}
		}

		/// <summary>
		/// Gets a value indicating the animation <see cref="ProgressBarGraphicStyle">style</see> of this progress bar.
		/// </summary>
		public abstract ProgressBarGraphicStyle Style { get; }

		/// <summary>
		/// Gets the size of this progress bar graphic.
		/// </summary>
		public abstract Size Size { get; }

		/// <summary>
		/// Called when the current progress value has changed.
		/// </summary>
		protected virtual void OnProgressChanged()
		{
			this.Update();
		}

		/// <summary>
		/// Called by the framework just before the progress bar is rendered.
		/// </summary>
		public override void OnDrawing()
		{
			if (base.Graphics.Count == 0)
			{
				base.Graphics.Add(CreateImageGraphic());
			}

			base.OnDrawing();
		}

		/// <summary>
		/// Forces an update of the progress bar bitmap.
		/// </summary>
		protected void Update()
		{
			DisposeAndClear(base.Graphics);
		}

		/// <summary>
		/// Draws an image centred on the specified graphics context.
		/// </summary>
		/// <param name="g">The graphics context on which the image is to be centred and drawn.</param>
		/// <param name="image">The image to be centred and drawn.</param>
		protected void DrawImageCentered(System.Drawing.Graphics g, Image image)
		{
			Size bounds = this.Size;
			Size size = image.Size;
			Point offset = new Point((bounds.Width - size.Width)/2, (bounds.Height - size.Height)/2);
			g.DrawImageUnscaledAndClipped(image, new Rectangle(offset, image.Size));
		}

		/// <summary>
		/// Called to render a progress bar depicting the specified progress value.
		/// </summary>
		/// <param name="progress">The progress value for which a progress bar is to be rendered.</param>
		/// <param name="g">The graphics context on which the progress bar is to be rendered.</param>
		protected abstract void RenderProgressBar(float progress, System.Drawing.Graphics g);

		#region Base Rendering Support

		private ColorImageGraphic CreateImageGraphic()
		{
			byte[] pixelData;
			using (Bitmap buffer = new Bitmap(this.Size.Width, this.Size.Height))
			{
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(buffer))
				{
					RenderProgressBar(_progress, g);
				}
				pixelData = GetPixelData(buffer);
			}
			ColorImageGraphic imageGraphic = new ColorImageGraphic(this.Size.Height, this.Size.Width, pixelData);
			return imageGraphic;
		}

		private static byte[] GetPixelData(Bitmap bitmap)
		{
			byte[] pixelData;
			BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			try
			{
				int length = data.Stride*data.Height;
				pixelData = new byte[length];
				Marshal.Copy(data.Scan0, pixelData, 0, length);
			}
			finally
			{
				bitmap.UnlockBits(data);
			}
			return pixelData;
		}

		private static void DisposeAndClear<T>(ICollection<T> collection) where T : IDisposable
		{
			var items = new List<T>(collection);
			collection.Clear();
			foreach (T item in items)
				item.Dispose();
		}

		#endregion

		#region Image Resource Sharing

		[ThreadStatic]
		private static Dictionary<string, Image> _cachedImageResources;

		/// <summary>
		/// Gets a statically cached image resource.
		/// </summary>
		internal static Image GetImageResource(string resourceName)
		{
			// simple static resource caching - the progress bar graphical elements only total about 6 kilobytes
			if (_cachedImageResources == null) _cachedImageResources = new Dictionary<string, Image>();
			if (!_cachedImageResources.ContainsKey(resourceName))
			{
				var resourceResolver = new ApplicationThemeResourceResolver(Assembly.GetExecutingAssembly());
				var image = Image.FromStream(resourceResolver.OpenResource(resourceName));
				_cachedImageResources.Add(resourceName, image);
			}
			return _cachedImageResources[resourceName];
		}

		#endregion
	}
}
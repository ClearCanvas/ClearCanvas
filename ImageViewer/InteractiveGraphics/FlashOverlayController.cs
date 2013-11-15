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
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// Controller for generating colour <see cref="ImageGraphic"/>s that flash up on an
	/// <see cref="IPresentationImage"/>'s overlay and disappear shortly thereafter.
	/// </summary>
	/// <example lang="CS">
	/// <![CDATA[
	/// FlashOverlayController controller = new FlashOverlayController("Icons.CreateKeyImageToolLarge.png", new ResourceResolver(this.GetType(), false));
	/// controller.Flash(base.SelectedPresentationImage);
	/// ]]>
	/// </example>
	public class FlashOverlayController
	{
		private readonly byte[] _pixelData;
		private readonly int _rows, _columns;

		/// <summary>
		/// Constructs a default controller.
		/// </summary>
		/// <remarks>
		/// If a base class uses this form of the constructor then it must also override
		/// the <see cref="GetPixelData"/>, <see cref="Rows"/> and <see cref="Columns"/> virtual members.
		/// </remarks>
		protected FlashOverlayController()
		{
			FlashSpeed = 500;

			_pixelData = null;
			_rows = _columns = 0;
		}

		/// <summary>
		/// Constructs a controller that uses a 32-bit colour ARGB image specified by the provided pixel data.
		/// </summary>
		/// <param name="colorPixelData">The 32-bit colour ARGB pixel data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="colorPixelData"/> is NULL.</exception>
		public FlashOverlayController(ColorPixelData colorPixelData)
			: this()
		{
			Platform.CheckForNullReference(colorPixelData, "colorPixelData");

			_pixelData = colorPixelData.Raw;
			_rows = colorPixelData.Rows;
			_columns = colorPixelData.Columns;
		}

		/// <summary>
		/// Constructs a controller that uses the 32-bit colour ARGB bitmap specified by the resource name and resource resolver.
		/// </summary>
		/// <param name="iconSet">The <see cref="IconSet"/> resource to use as the overlay.</param>
		/// <param name="resourceResolver">A resource resolver for the icon set.</param>
		/// <param name="iconSize">Optionally specifies the size of the icon resource to use (defaults to <see cref="IconSize.Large"/>).</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="iconSet"/> or <paramref name="resourceResolver"/> is NULL.</exception>
		public FlashOverlayController(IconSet iconSet, IResourceResolver resourceResolver, IconSize iconSize = IconSize.Large)
			: this()
		{
			Platform.CheckForNullReference(iconSet, "iconSet");

			using (var img = iconSet.CreateIcon(iconSize, resourceResolver))
			{
				var bmp = img as Bitmap;
				if (bmp != null)
				{
					_pixelData = ExtractBitmap(bmp, out _rows, out _columns);
				}
				else
				{
					using (var bitmap = new Bitmap(img))
						_pixelData = ExtractBitmap(bitmap, out _rows, out _columns);
				}
			}
		}

		/// <summary>
		/// Constructs a controller that uses the 32-bit colour ARGB bitmap specified by the resource name and resource resolver.
		/// </summary>
		/// <param name="resourceName">The partially or fully qualified name of the resource to access.</param>
		/// <param name="resourceResolver">A resource resolver for the resource.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="resourceName"/> is empty or NULL, or <paramref name="resourceResolver"/> is NULL.</exception>
		public FlashOverlayController(string resourceName, IResourceResolver resourceResolver)
			: this()
		{
			Platform.CheckForEmptyString(resourceName, "resourceName");
			Platform.CheckForNullReference(resourceResolver, "resourceResolver");

			using (Bitmap bitmap = new Bitmap(resourceResolver.OpenResource(resourceName)))
			{
				_pixelData = ExtractBitmap(bitmap, out _rows, out _columns);
			}
		}

		private static byte[] ExtractBitmap(Bitmap bitmap, out int rows, out int columns)
		{
			BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			try
			{
				int length = data.Stride*data.Height;
				var pixelData = new byte[length];
				rows = data.Height;
				columns = data.Width;
				Marshal.Copy(data.Scan0, pixelData, 0, length);
				return pixelData;
			}
			finally
			{
				bitmap.UnlockBits(data);
			}
		}

		/// <summary>
		/// Gets the pixel data of the flash overlay in 32-bit ARGB format.
		/// </summary>
		/// <returns>An ARGB byte array.</returns>
		protected virtual byte[] GetPixelData()
		{
			return _pixelData;
		}

		/// <summary>
		/// Gets the number of rows in the flash overlay.
		/// </summary>
		protected virtual int Rows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the number of columns in the flash overlay.
		/// </summary>
		protected virtual int Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// Gets or sets the delay time of the flash overlay in milliseconds.
		/// </summary>
		public int FlashSpeed { get; set; }

		/// <summary>
		/// Flashes the overlay on the specified image.
		/// </summary>
		/// <param name="image">The image on which to display the overlay. The image must implement <see cref="IOverlayGraphicsProvider"/>.</param>
		/// <param name="message">An optional message to accompany the overlay.</param>
		public void Flash(IPresentationImage image, string message = null)
		{
			DoFlash(image, SynchronizationContext.Current, message);
		}

		private void DoFlash(IPresentationImage image, SynchronizationContext syncContext, string message)
		{
			if (!(image is IOverlayGraphicsProvider))
				return;

			GraphicCollection overlayGraphics = ((IOverlayGraphicsProvider) image).OverlayGraphics;

			// Prevent multiple flash graphics per image.
			var existing = overlayGraphics.OfType<FlashOverlayGraphic>().FirstOrDefault(g => g.Controller == this);
			if (existing != null)
				return;

			// Very little utility in flashing if nobody's looking at it.
			if (syncContext == null)
				return;

			var flashOverlayGraphic = new FlashOverlayGraphic(this, message);
			overlayGraphics.Add(flashOverlayGraphic);
			image.Draw();

			ThreadPool.QueueUserWorkItem(
				delegate
					{
						Thread.Sleep(FlashSpeed);
						syncContext.Post(delegate
						                 	{
						                 		if (flashOverlayGraphic.IsDisposed)
						                 			return;

						                 		overlayGraphics.Remove(flashOverlayGraphic);
						                 		image.Draw();
						                 	}, null);
					}, null);
		}

		private class FlashOverlayGraphic : CompositeGraphic
		{
			private readonly int _rows;
			private readonly int _columns;

			public FlashOverlayGraphic(FlashOverlayController controller, string message)
			{
				Controller = controller;

				Graphics.Add(new ColorImageGraphic(_rows = controller.Rows, _columns = controller.Columns, controller.GetPixelData));

				if (!string.IsNullOrWhiteSpace(message))
				{
					const int vOffset = 25; // fixed offset because we currently don't have a way to measure text size until after rendering once...
					Graphics.Add(new InvariantTextPrimitive(message)
					             	{
					             		Location = new PointF(_columns/2f, _rows + vOffset),
					             		Color = Color.WhiteSmoke,
					             		SizeInPoints = 20
					             	});
				}
			}

			public FlashOverlayController Controller { get; private set; }
			public bool IsDisposed { get; private set; }

			protected override SpatialTransform CreateSpatialTransform()
			{
				return new InvariantSpatialTransform(this);
			}

			public override void OnDrawing()
			{
				if (base.ParentPresentationImage != null)
				{
					var clientRectangle = base.ParentPresentationImage.ClientRectangle;
					var spatialTransform = base.SpatialTransform;
					spatialTransform.TranslationX = (clientRectangle.Width - _columns)/2f;
					spatialTransform.TranslationY = (clientRectangle.Height - _rows)/2f;
				}
				base.OnDrawing();
			}

			protected override void Dispose(bool disposing)
			{
				IsDisposed = true;
				base.Dispose(disposing);
			}
		}
	}
}
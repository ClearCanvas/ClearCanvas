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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations.Utilities;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Thumbnails
{
    #region Bitmap Converter

    public delegate IImageData BitmapConverter(Bitmap bitmap);

    public class BitmapConverters
    {
        public static BitmapConverter Null = bitmap => new CloneableImageData<Bitmap>(bitmap);
        public static BitmapConverter MemoryStream = ToMemoryStreamImageData;

        private static IImageData ToMemoryStreamImageData(Bitmap bitmap)
        {
            var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            return new MemoryStreamImageData(stream);
        }
    }

    #endregion

    #region Thumbnail Factory

    public interface IThumbnailFactory<T> where T : class
    {
        IImageData CreateDummy(string message, Size size);
        IImageData CreateImage(T sourceItem, Size size);
        IImageData CreateError(Size size);
    }

    public class ThumbnailFactory : IThumbnailFactory<IPresentationImage>
    {
        private readonly BitmapConverter _converter;

        public ThumbnailFactory()
            : this(BitmapConverters.Null)
        {
        }

        public ThumbnailFactory(BitmapConverter converter)
        {
            Platform.CheckForNullReference(converter, "converter");
            _converter = converter;
        }

        public IImageData CreateDummy(string message, Size size)
        {
            return _converter(CreateDummy(message, size.Width, size.Height));
        }

        public IImageData CreateImage(IPresentationImage image, Size size)
        {
            return _converter(CreateThumbnail(image, size.Width, size.Height));
        }

        public IImageData CreateError(Size size)
        {
            return _converter(CreateError(size.Width, size.Height));
        }

        public static Bitmap CreateDummy(string message, int width, int height)
        {
            var bmp = new Bitmap(width, height);
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bmp))
            {
                using (Brush brush = new SolidBrush(Color.WhiteSmoke))
                {
                    var fontSize = Math.Min(width, height)/10f;
                    //naively, assume message should be 1/10th the size of the thumbnail.
                    using (var font = new Font("Arial", fontSize))
                    {
                        using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        {
                            graphics.DrawString(message, font, brush, new Rectangle(0, 0, width, height), format);
                            DrawBorder(graphics, width, height);
                        }
                    }
                }
            }
            return bmp;
        }

        public static Bitmap CreateThumbnail(IPresentationImage image, int width, int height)
        {
            var visibilityHelper = new TextOverlayVisibilityHelper(image);
            visibilityHelper.Hide();

            var bitmap = CreateThumbnailImage(image, width, height);
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
                DrawBorder(graphics, width, height);

            visibilityHelper.Restore();
            return bitmap;
        }

        public static Bitmap CreateError(int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
            {
                graphics.FillRectangle(Brushes.Black, 0, 0, width, height);
                graphics.DrawLine(Pens.WhiteSmoke, 0, 0, width, height);
                graphics.DrawLine(Pens.WhiteSmoke, 0, height, width, 0);
            }
            return bitmap;
        }

        private static Bitmap CreateThumbnailImage(IPresentationImage presentationImage, int width, int height)
        {
			if (!(presentationImage is PresentationImage))
				return presentationImage.DrawToBitmap(width, height);

			var image = (PresentationImage) presentationImage;
			var bmp = new Bitmap(width, height);

			using (var graphics = System.Drawing.Graphics.FromImage(bmp))
			{
			    var contextId = graphics.GetHdc();
			    var dpi = 512/Math.Min(width, height); //Assume 512 corresponds to "normal" screen dpi

			    try
			    {
			        using (var surface = image.ImageRenderer.CreateRenderingSurface(IntPtr.Zero, bmp.Width, bmp.Height, RenderingSurfaceType.Offscreen))
			        {
			            surface.ContextID = contextId;
			            surface.ClipRectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);

			            var drawArgs = new DrawArgs(surface, null, DrawMode.Render) {Dpi = dpi, Tag = bmp};
			            image.Draw(drawArgs);
			            drawArgs = new DrawArgs(surface, null, DrawMode.Refresh) {Dpi = dpi, Tag = bmp};
			            image.Draw(drawArgs);
			        }
			    }
                finally
                {
                    graphics.ReleaseHdc(contextId);
                }
            }
			return bmp;
        }

        private static void DrawBorder(System.Drawing.Graphics graphics, int width, int height)
        {
            using (var pen = new Pen(Color.DarkGray))
            {
                graphics.DrawRectangle(pen, 0, 0, width - 1, height - 1);
            }
        }
    }

    #endregion
}

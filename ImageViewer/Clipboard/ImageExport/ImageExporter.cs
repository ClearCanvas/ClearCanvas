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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.StudyManagement;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	public abstract class ImageExporter : IImageExporter
	{
		private readonly string _identifier;
		private readonly string _description;
		private readonly string[] _fileExtensions;

		protected ImageExporter(string identifier, string description, string[] fileExtensions)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			Platform.CheckForEmptyString(description, "description");
			Platform.CheckForNullReference(fileExtensions, "fileExtension");
			if (fileExtensions.Length == 0)
				throw new ArgumentException("The exporter must have at least one associated file extension.");

			IResourceResolver resolver = new ResourceResolver(this.GetType().Assembly);
			_identifier = identifier;
			_description = resolver.LocalizeString(description);
			_fileExtensions = fileExtensions;
		}

		#region IImageExporter Members

		public string Identifier
		{
			get { return _identifier; }
		}

		public string Description
		{
			get { return _description; }
		}

		public string[] FileExtensions
		{
			get { return _fileExtensions; }
		}

		public abstract void Export(IPresentationImage image, string filePath, ExportImageParams exportParams);

		#endregion

		public static Bitmap DrawToBitmap(IPresentationImage image, ExportImageParams exportParams)
		{
			Platform.CheckForNullReference(image, "image");
			Platform.CheckForNullReference(exportParams, "exportParams");

			if (!(image is ISpatialTransformProvider))
				throw new ArgumentException("The image must have a valid ImageSpatialTransform in order to be exported.");

			if (exportParams.ExportOption == ExportOption.TrueSize)
			{
				var imageSopProvider = image as IImageSopProvider;
				var pixelSpacing = imageSopProvider == null ? null : imageSopProvider.Frame.NormalizedPixelSpacing;
				if (pixelSpacing == null || pixelSpacing.IsNull)
					throw new ArgumentException("The image does not contain pixel spacing information.  TrueSize export is not possible.");
			}

			IImageSpatialTransform transform = ((ISpatialTransformProvider) image).SpatialTransform as IImageSpatialTransform;
			if (transform == null)
				throw new ArgumentException("The image must have a valid ImageSpatialTransform in order to be exported.");

			if (exportParams.ExportOption == ExportOption.TrueSize)
				return DrawTrueSizeImageToBitmap(image, exportParams.OutputSize, exportParams.Dpi);

			if (exportParams.SizeMode == SizeMode.Scale)
			{
				// TODO: Refactor ImageExporter, so there only the displayRectangle and OutputRectangle are provided
				//		Scale can be automatically figured out.
				//		A "Padded" option can be provided to distinguish between the current Fixed and ScaleToFit options
				// TODO: Refactor ImageExporter, so there are separate exporters for each ExportOption.
				//		The ExportImageParams is getting too many options and not all of them are applicable to each exporter
				//		Instead, each exporter should have its own parameters.

				if (exportParams.ExportOption == ExportOption.Wysiwyg)
				{
					return DrawWysiwygImageToBitmap(image, exportParams.DisplayRectangle, exportParams.Scale, exportParams.Dpi);
				}
				else
				{
					return DrawCompleteImageToBitmap(image, exportParams.Scale, exportParams.Dpi);
				}
			}
			else if (exportParams.SizeMode == SizeMode.ScaleToFit)
			{
				if (exportParams.ExportOption == ExportOption.Wysiwyg)
				{
					var scale = ScaleToFit(exportParams.DisplayRectangle.Size, exportParams.OutputSize);
					return DrawWysiwygImageToBitmap(image, exportParams.DisplayRectangle, scale, exportParams.Dpi);
				}
				else
				{
					var scale = ScaleToFit(image.SceneSize, exportParams.OutputSize);
					return DrawCompleteImageToBitmap(image, scale, exportParams.Dpi);
				}
			}
			else
			{
				Bitmap paddedImage = new Bitmap(exportParams.OutputSize.Width, exportParams.OutputSize.Height);
				using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(paddedImage))
				{
					// paint background
					using (Brush b = new SolidBrush(exportParams.BackgroundColor))
					{
						graphics.FillRectangle(b, new Rectangle(Point.Empty, exportParams.OutputSize));
					}

					// paint image portion
					Bitmap bmp;
					if (exportParams.ExportOption == ExportOption.Wysiwyg)
					{
						float scale = ScaleToFit(exportParams.DisplayRectangle.Size, exportParams.OutputSize);
						bmp = DrawWysiwygImageToBitmap(image, exportParams.DisplayRectangle, scale, exportParams.Dpi);
					}
					else
					{
						float scale = ScaleToFit(image.SceneSize, exportParams.OutputSize);
						bmp = DrawCompleteImageToBitmap(image, scale, exportParams.Dpi);
					}
						graphics.DrawImageUnscaledAndClipped(bmp, new Rectangle(CenterRectangles(bmp.Size, exportParams.OutputSize), bmp.Size));
					bmp.Dispose();
				}
				
				return paddedImage;
			}
		}

		private static Bitmap DrawCompleteImageToBitmap(IPresentationImage image, float scale, float dpi)
		{
			IImageSpatialTransform transform = (IImageSpatialTransform)((ISpatialTransformProvider)image).SpatialTransform;
			object restoreMemento = transform.CreateMemento();
			try
			{
				Rectangle imageRectangle = new Rectangle(new Point(0, 0), image.SceneSize);

				transform.Initialize();
				transform.ScaleToFit = false;
				transform.Scale = scale;
				RectangleF displayRectangle = transform.ConvertToDestination(imageRectangle);
				int width = (int) Math.Round(displayRectangle.Width);
				int height = (int) Math.Round(displayRectangle.Height);

				transform.ScaleToFit = true;
				return ImageDrawToBitmap(image, width, height, dpi);
			}
			finally
			{
				transform.SetMemento(restoreMemento);
			}
		}

		private static Bitmap DrawWysiwygImageToBitmap(IPresentationImage image, Rectangle displayRectangle, float scale, float dpi)
		{
			IImageSpatialTransform transform = (IImageSpatialTransform) ((ISpatialTransformProvider) image).SpatialTransform;
			object restoreMemento = transform.CreateMemento();
			try
			{
				int width = (int) (displayRectangle.Width*scale);
				int height = (int) (displayRectangle.Height*scale);

				transform.Scale *= scale;

				return ImageDrawToBitmap(image, width, height, dpi);
			}
			finally
			{
				transform.SetMemento(restoreMemento);
			}
		}

		private static double Magnitude(double x, double y)
		{
			return Math.Sqrt(x*x + y*y);
		}

		private static Bitmap DrawTrueSizeImageToBitmap(IPresentationImage image, Size outputSize, float dpi)
		{
			const double mmPerInch = 25.4;
			const int pxLength = 100;

			var transform = (IImageSpatialTransform) ((ISpatialTransformProvider) image).SpatialTransform;
			var restoreMemento = transform.CreateMemento();
			try
			{
				var frame = ((IImageSopProvider) image).Frame;
				var pixelSpacing = frame.NormalizedPixelSpacing;

				// to compute the correct transform scale for true size export at the specified DPI (i.e. pixel spacing)
				// we need to determine the current effective pixel spacing of the destination (post-transform) image
				// then you can compute the value by which to further adjust the transform scale
				// so that the destination image achieves the desired pixel spacing
				// for a mathematical proof of this algorithm, see jasper

				// we start by turning off scale to fit and fix the scale so it's not too small that it introduces floating point error
				transform.ScaleToFit = false;
				transform.Scale = 1;

				// choose an arbitrary non-trivial vector in the current destination image
				var dstVector = new SizeF(pxLength, 0);

				// now convert it back to source coordinates
				var srcVector = transform.ConvertToSource(dstVector);

				// compute the physical length of the vector
				var mmLength = Magnitude(srcVector.Width*pixelSpacing.Column, srcVector.Height*pixelSpacing.Row);

				// since we know the length of the vector in destination pixels, we can compute the effective pixel spacing
				// a single value will suffice because the destination image has the pixel aspect ratio normalized to be isotropic!
				var effectivePixelSpacing = mmLength/pxLength;

				// thus the effective DPI of the destination image right now is...
				var effectiveDpi = (float) (mmPerInch/effectivePixelSpacing);

				// since, at the current scale, the transform gives you effective DPI,
				// you can adjust the scale by DPI / effective DPI to achieve the desired DPI
				transform.Scale *= dpi/effectiveDpi;

				// we also want to constrain the output to not render any unnecessary padding
				var imageBounds = transform.ConvertToDestination(new SizeF(frame.Columns, frame.Rows));
				var imageSize = Size.Ceiling(new SizeF(Math.Abs(imageBounds.Width), Math.Abs(imageBounds.Height)));
				return ImageDrawToBitmap(image, Math.Min(imageSize.Width, outputSize.Width), Math.Min(imageSize.Height, outputSize.Height), dpi);
			}
			finally
			{
				transform.SetMemento(restoreMemento);
			}
		}

		private static Bitmap ImageDrawToBitmap(IPresentationImage presentationImage, int width, int height, float dpi)
		{
			if (!(presentationImage is PresentationImage))
				return presentationImage.DrawToBitmap(width, height);

			var image = (PresentationImage) presentationImage;
			var bmp = new Bitmap(width, height);

			var graphics = System.Drawing.Graphics.FromImage(bmp);
			var contextId = graphics.GetHdc();
			try
			{
				using (var surface = image.ImageRenderer.CreateRenderingSurface(IntPtr.Zero, bmp.Width, bmp.Height, RenderingSurfaceType.Offscreen))
				{
					surface.ContextID = contextId;
					surface.ClipRectangle = new Rectangle(0, 0, bmp.Width, bmp.Height);

					var drawArgs = new DrawArgs(surface, null, DrawMode.Render) {Dpi = dpi, Tag = bmp};
					image.Draw(drawArgs);
                    drawArgs = new DrawArgs(surface, null, DrawMode.Refresh) { Dpi = dpi, Tag = bmp};
					image.Draw(drawArgs);
				}
			}
			finally
			{
				graphics.ReleaseHdc(contextId);
				graphics.Dispose();
			}
			return bmp;
		}

		private static float ScaleToFit(Size source, SizeF destination)
		{
			if (source.Width == 0 || source.Height == 0)
				return 1;

			float aW = destination.Width/source.Width;
			float aH = destination.Height/source.Height;
			if (!FloatComparer.IsGreaterThan(aW * source.Height, destination.Height))
				return aW;
			else
				return aH;
		}

		private static Point CenterRectangles(Size source, Size destination)
		{
			return new Point((destination.Width - source.Width)/2, (destination.Height - source.Height)/2);
		}

		protected static void Export(IPresentationImage image, string filePath, ExportImageParams exportParams, ImageFormat imageFormat)
		{
			using (Bitmap bmp = DrawToBitmap(image, exportParams))
			{
				// try to save the dpi in the meta header for image types that support it (the GDI+ implementation of TIFF encoder doesn't even though TIFF itself supports it)
				bmp.SetResolution(exportParams.Dpi, exportParams.Dpi);

				Export(bmp, filePath, imageFormat);
			}
		}

		protected static void Export(IPresentationImage image, string filePath, ExportImageParams exportParams, ImageCodecInfo encoder, EncoderParameters encoderParameters)
		{
			using (Bitmap bmp = DrawToBitmap(image, exportParams))
			{
				// try to save the dpi in the meta header for image types that support it (the GDI+ implementation of TIFF encoder doesn't even though TIFF itself supports it)
				bmp.SetResolution(exportParams.Dpi, exportParams.Dpi);

				Export(bmp, filePath, encoder, encoderParameters);
			}
		}

		protected static void Export(Image image, string filePath, ImageFormat imageFormat)
		{
			Platform.CheckForNullReference(image, "image");
			Platform.CheckForNullReference(imageFormat, "imageFormat");
			Platform.CheckForEmptyString(filePath, "filePath");

			image.Save(filePath, imageFormat);
		}

		protected static void Export(Image image, string filePath, ImageCodecInfo encoder, EncoderParameters encoderParameters)
		{
			Platform.CheckForNullReference(image, "image");
			Platform.CheckForEmptyString(filePath, "filePath");
			Platform.CheckForNullReference(encoder, "encoder");
			Platform.CheckForNullReference(encoderParameters, "encoderParameters");

			image.Save(filePath, encoder, encoderParameters);
		}

		/// <summary>
		/// Creates a clone of a <see cref="IPresentationImage"/> suitable for static export.
		/// </summary>
		/// <remarks>
		/// Functionally, this method is similar to <see cref="IPresentationImage.Clone"/>.
		/// However, this method also fills the <see cref="IPresentationImage.ParentDisplaySet"/>
		/// property with a static <see cref="IDisplaySet"/> instance whose properties are a snapshot
		/// of the source image's parent display set at the time the clone was created.
		/// This makes the resulting images suitable for export with features like annotation overlay
		/// enabled, as certain items (specifically, display set description and number) depend on
		/// this information.
		/// </remarks>
		/// <param name="presentationImage">The source image to be cloned.</param>
		/// <returns>A clone of the source image.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="presentationImage"/> is null.</exception>
		public static IPresentationImage ClonePresentationImage(IPresentationImage presentationImage)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");
			var imageClone = presentationImage.Clone();
			try
			{
				var parentDisplaySet = presentationImage.ParentDisplaySet;
				if (parentDisplaySet != null)
				{
					var descriptor = parentDisplaySet.Descriptor;
					if (descriptor != null)
						parentDisplaySet = new DisplaySet(new BasicDisplaySetDescriptor {Description = descriptor.Description, Name = descriptor.Name, Number = descriptor.Number, Uid = descriptor.Uid});
					else
						parentDisplaySet = new DisplaySet(parentDisplaySet.Name, parentDisplaySet.Uid);
					parentDisplaySet.PresentationImages.Add(imageClone);
				}
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Failed to create a dummy parent display set for the cloned presentation image");
			}
			return imageClone;
		}
	}
}
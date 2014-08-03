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
using System.Runtime.InteropServices;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.Macros;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Rendering;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	internal static class Utilities
	{
		/// <summary>
		/// Converts <see cref="NormalizedPixelSpacingCalibrationType"/> to <see cref="PixelSpacingCalibrationType"/>.
		/// </summary>
		public static PixelSpacingCalibrationType ToPixelSpacingCalibrationType(this NormalizedPixelSpacingCalibrationType type)
		{
			switch (type)
			{
				case NormalizedPixelSpacingCalibrationType.Geometry:
					return PixelSpacingCalibrationType.Geometry;
				case NormalizedPixelSpacingCalibrationType.Fiducial:
					return PixelSpacingCalibrationType.Fiducial;
				default:
					return PixelSpacingCalibrationType.None;
			}
		}

		/// <summary>
		/// Converts <see cref="NormalizedPixelSpacingCalibrationType"/> to <see cref="PixelSpacingCalibrationType"/>.
		/// </summary>
		public static string GetDescription(this NormalizedPixelSpacingCalibrationType type)
		{
			switch (type)
			{
				case NormalizedPixelSpacingCalibrationType.Manual:
					return "Manual";
				case NormalizedPixelSpacingCalibrationType.CrossSectionalSpacing:
					return "Actual";
				case NormalizedPixelSpacingCalibrationType.Detector:
					return "Detector";
				case NormalizedPixelSpacingCalibrationType.Geometry:
					return "Geometry";
				case NormalizedPixelSpacingCalibrationType.Fiducial:
					return "Fiducial";
				case NormalizedPixelSpacingCalibrationType.Magnified:
					return "Magnified";
				case NormalizedPixelSpacingCalibrationType.None:
					return string.Empty;
				case NormalizedPixelSpacingCalibrationType.Unknown:
				default:
					return "Unknown";
			}
		}

		/// <summary>
		/// Format the <see cref="Vector3D"/> as a string of form X\Y\Z
		/// </summary>
		public static string ToDicomAttributeString(this Vector3D vector3D)
		{
			return String.Format(@"{0:G12}\{1:G12}\{2:G12}", vector3D.X, vector3D.Y, vector3D.Z);
		}

		/// <summary>
		/// Format the <see cref="SizeF"/> as a string of form HEIGHT\WIDTH
		/// </summary>
		public static string ToDicomAttributeString(this SizeF size)
		{
			return String.Format(@"{0:G12}\{1:G12}", size.Height, size.Width);
		}

		public static void SetStringValue2(this DicomAttribute dicomAttribute, string value)
		{
			if (!string.IsNullOrEmpty(value))
				dicomAttribute.SetStringValue(value);
			else
				dicomAttribute.SetEmptyValue();
		}

		/// <summary>
		/// Reads the ARGB pixel data from a <see cref="Bitmap"/> into the specified address
		/// </summary>
		public static void CopyArgbDataTo(this Bitmap bitmap, IntPtr pPixelData)
		{
			var bitmapData = bitmap.LockBits(new Rectangle(new Point(), bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			try
			{
				var pSrc = bitmapData.Scan0;
				var srcStride = bitmapData.Stride;
				var pDst = pPixelData;
				var dstStride = Math.Abs(srcStride);
				for (var n = 0; n < bitmapData.Height; ++n)
				{
					CopyMemory(pDst, pSrc, (uint) dstStride);
					pDst += dstStride;
					pSrc += srcStride;
				}
			}
			finally
			{
				bitmap.UnlockBits(bitmapData);
			}
		}

		/// <summary>
		/// Forces a render of the specified image and setting the image's client rectangle property at the same time.
		/// </summary>
		public static Bitmap RenderImage(this IPresentationImage image, Rectangle clientRectangle)
		{
			// we can't just call image.DrawToBitmap because we also want to force the client rectangle of the image
			// which wouldn't be set correctly because the image has possibly never been drawn in a Tile before

			var presentationImage = image as PresentationImage;
			if (presentationImage != null)
			{
				var bmp = new Bitmap(clientRectangle.Width, clientRectangle.Height, PixelFormat.Format32bppArgb);
				var graphics = System.Drawing.Graphics.FromImage(bmp);
				var contextId = graphics.GetHdc();
				try
				{
					using (var surface = presentationImage.ImageRenderer.CreateRenderingSurface(IntPtr.Zero, clientRectangle.Width, clientRectangle.Height, RenderingSurfaceType.Offscreen))
					{
						surface.ContextID = contextId;
						surface.ClipRectangle = new Rectangle(0, 0, clientRectangle.Width, clientRectangle.Height);

						var drawArgs = new DrawArgs(surface, null, DrawMode.Render);
						presentationImage.Draw(drawArgs);
						drawArgs = new DrawArgs(surface, null, DrawMode.Refresh);
						presentationImage.Draw(drawArgs);
					}
				}
				finally
				{
					graphics.ReleaseHdc(contextId);
					graphics.Dispose();
				}
				return bmp;
			}
			else
			{
				throw new NotSupportedException("Image type is not supported");
			}
		}

		[DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
		private static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);
	}
}
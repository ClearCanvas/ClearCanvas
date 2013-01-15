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

using ClearCanvas.Common;
using System;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines the methods and properties of the data source for a <see cref="Frame"/>.
	/// </summary>
	public interface ISopFrameData : IDisposable
	{
		/// <summary>
		/// Gets the parent <see cref="ISopDataSource"/> to which this frame belongs.
		/// </summary>
		ISopDataSource Parent { get; }

		/// <summary>
		/// Gets the 1-based numeric identifier of this frame.
		/// </summary>
		int FrameNumber { get; }

		/// <summary>
		/// Gets pixel data in normalized form (8 or 16-bit grayscale, or 32-bit ARGB).
		/// </summary>
		/// <remarks>
		/// <i>Normalized</i> pixel data means that:
		/// <list type="Bullet">
		/// <item>
		/// <description>Grayscale pixel data has embedded overlays removed and each pixel value
		/// is padded so that it can be cast directly to the appropriate type (e.g. byte, sbyte, ushort, short).</description>
		/// </item>
		/// <item>
		/// <description>Colour pixel data is always converted into ARGB format.</description>
		/// </item>
		/// <item>
		/// <description>Pixel data is always uncompressed.</description>
		/// </item>
		/// </list>
		/// <para>
		/// Ensuring that the pixel data always meets the above criteria
		/// allows clients to easily consume pixel data without having
		/// to worry about the the multitude of DICOM photometric interpretations
		/// and transfer syntaxes.
		/// </para>
		/// <para>
		/// Pixel data is reloaded when this method is called after a 
		/// call to <see cref="Unload"/>.
		/// </para>
		/// </remarks>		
		byte[] GetNormalizedPixelData();

		/// <summary>
		/// Gets the normalized overlay data buffer for a particular overlay group (8-bit grayscale).
		/// </summary>
		/// <remarks>
		/// <para>
		/// <i>Normalized</i> overlay data means that the 1-bit overlay pixel data is extracted and
		/// unpacked as necessary to form an 8-bit-per-pixel buffer with values of either 0 or 255.
		/// </para>
		/// <para>
		/// Ensuring that the overlay data always meets the above criteria allows clients to easily
		/// consume overlay data without having to worry about the storage of overlay data, whether
		/// embedded in unused bits of the pixel data or in a separate packed bits buffer.
		/// </para>
		/// <para>
		/// Overlay data is reloaded when this method is called after a call to <see cref="Unload"/>.
		/// The pixel data will also be reloaded if this method is called before
		/// <see cref="GetNormalizedPixelData"/> and there are overlays stored in unused bits of the
		/// pixel data.
		/// </para>
		/// </remarks>
		/// <param name="overlayNumber">The 1-based overlay plane number.</param>
		/// <returns>A byte buffer containing the normalized overlay pixel data.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="overlayNumber"/> is not a positive non-zero number.</exception>
		byte[] GetNormalizedOverlayData(int overlayNumber);

		/// <summary>
		/// Unloads any cached byte buffers owned by this <see cref="ISopFrameData"/>.
		/// </summary>
		/// <remarks>
		/// It is sometimes necessary to manage the memory used by unloading the pixel data. 
		/// Calling this method will not necessarily result in an immediate decrease in memory
		/// usage, since it merely releases the reference to the pixel data; it is up to the
		/// garbage collector to free the memory.  Calling <see cref="GetNormalizedPixelData"/>
		/// will reload the pixel data.
		/// </remarks>
		void Unload();
	}

	/// <summary>
	/// Base implementation of <see cref="ISopFrameData"/>.
	/// </summary>
	public abstract class SopFrameData : ISopFrameData
	{
		private readonly SopDataSource _parent;
		private readonly int _frameNumber;

		/// <summary>
		/// Constructs a new <see cref="SopFrameData"/>.
		/// </summary>
		/// <param name="frameNumber">The 1-based number of this frame.</param>
		/// <param name="parent">The parent <see cref="ISopDataSource"/> that this frame belongs to.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="frameNumber"/> is zero or negative.</exception>
		protected SopFrameData(int frameNumber, SopDataSource parent)
		{
			Platform.CheckForNullReference(parent, "parent");
			Platform.CheckPositive(frameNumber, "frameNumber");

			_parent = parent;
			_frameNumber = frameNumber;
		}

		/// <summary>
		/// Gets the parent <see cref="ISopDataSource"/> to which this frame belongs.
		/// </summary>
		public ISopDataSource Parent
		{
			get { return _parent; }
		}

		/// <summary>
		/// Gets the 1-based number of this frame.
		/// </summary>
		public int FrameNumber
		{
			get { return _frameNumber; }
		}

		/// <summary>
		/// Gets pixel data in normalized form (8 or 16-bit grayscale, or 32-bit ARGB).
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// <i>Normalized</i> pixel data means that:
		/// <list type="Bullet">
		/// <item>
		/// <description>Grayscale pixel data has embedded overlays removed and each pixel value
		/// is padded so that it can be cast directly to the appropriate type (e.g. byte, sbyte, ushort, short).</description>
		/// </item>
		/// <item>
		/// <description>Colour pixel data is always converted into ARGB format.</description>
		/// </item>
		/// <item>
		/// <description>Pixel data is always uncompressed.</description>
		/// </item>
		/// </list>
		/// <para>
		/// Ensuring that the pixel data always meets the above criteria
		/// allows clients to easily consume pixel data without having
		/// to worry about the the multitude of DICOM photometric interpretations
		/// and transfer syntaxes.
		/// </para>
		/// <para>
		/// Pixel data is reloaded when this method is called after a 
		/// call to <see cref="ISopFrameData.Unload"/>.
		/// </para>
		/// </remarks>		
		public abstract byte[] GetNormalizedPixelData();

		/// <summary>
		/// Gets the normalized overlay data buffer for a particular overlay group (8-bit grayscale).
		/// </summary>
		/// <remarks>
		/// <para>
		/// <i>Normalized</i> overlay data means that the 1-bit overlay pixel data is extracted and
		/// unpacked as necessary to form an 8-bit-per-pixel buffer with values of either 0 or 255.
		/// </para>
		/// <para>
		/// Ensuring that the overlay data always meets the above criteria allows clients to easily
		/// consume overlay data without having to worry about the storage of overlay data, whether
		/// embedded in unused bits of the pixel data or in a separate packed bits buffer.
		/// </para>
		/// <para>
		/// Overlay data is reloaded when this method is called after a call to <see cref="ISopFrameData.Unload"/>.
		/// The pixel data will also be reloaded if this method is called before
		/// <see cref="ISopFrameData.GetNormalizedPixelData"/> and there are overlays stored in unused bits of the
		/// pixel data.
		/// </para>
		/// </remarks>
		/// <param name="overlayNumber">The 1-based overlay plane number.</param>
		/// <returns>A byte buffer containing the normalized overlay pixel data.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="overlayNumber"/> is not a positive non-zero number.</exception>
		public abstract byte[] GetNormalizedOverlayData(int overlayNumber);

		/// <summary>
		/// Unloads any cached byte buffers owned by this <see cref="ISopFrameData"/>.
		/// </summary>
		/// <remarks>
		/// It is sometimes necessary to manage the memory used by unloading the pixel data. 
		/// Calling this method will not necessarily result in an immediate decrease in memory
		/// usage, since it merely releases the reference to the pixel data; it is up to the
		/// garbage collector to free the memory.  Calling <see cref="ISopFrameData.GetNormalizedPixelData"/>
		/// will reload the pixel data.
		/// </remarks>
		public abstract void Unload();

		/// <summary>
		/// Releases resources owned by this <see cref="SopFrameData"/>.
		/// </summary>
		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e, "An unexpected error has occurred while disposing the frame data.");
			}
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		/// <param name="disposing">A value indicating whether or not the object is being disposed.</param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
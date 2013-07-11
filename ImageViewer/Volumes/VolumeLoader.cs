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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volumes
{
	/// <summary>
	/// Represents the callback method to receive progress notification when loading a <see cref="Volume"/>.
	/// </summary>
	/// <param name="completedSubOperations">The number of suboperations completed.</param>
	/// <param name="totalSubOperations">The total number of suboperations.</param>
	public delegate void CreateVolumeProgressCallback(int completedSubOperations, int totalSubOperations);

	partial class Volume
	{
		/// <summary>
		/// Creates a <see cref="Volume"/> from the images of the specified <see cref="IDisplaySet"/>.
		/// </summary>
		/// <param name="displaySet">The <see cref="IDisplaySet"/> from which to create the <see cref="Volume"/>.</param>
		/// <returns>A <see cref="Volume"/> constructed from the specified inputs.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="displaySet"/> is NULL.</exception>
		/// <exception cref="CreateVolumeException">Thrown if the specified inputs cannot be constructed into a <see cref="Volume"/>.</exception>
		public static Volume Create(IDisplaySet displaySet)
		{
			return Create(displaySet, null);
		}

		/// <summary>
		/// Creates a <see cref="Volume"/> from the images of the specified <see cref="IDisplaySet"/>.
		/// </summary>
		/// <param name="displaySet">The <see cref="IDisplaySet"/> from which to create the <see cref="Volume"/>.</param>
		/// <param name="callback">A <see cref="CreateVolumeProgressCallback"/> to receive progress notification while the <see cref="Volume"/> is being loaded.</param>
		/// <returns>A <see cref="Volume"/> constructed from the specified inputs.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="displaySet"/> is NULL.</exception>
		/// <exception cref="CreateVolumeException">Thrown if the specified inputs cannot be constructed into a <see cref="Volume"/>.</exception>
		public static Volume Create(IDisplaySet displaySet, CreateVolumeProgressCallback callback)
		{
			Platform.CheckForNullReference(displaySet, "displaySet");
			List<Frame> frames = new List<Frame>();

			foreach (IPresentationImage image in displaySet.PresentationImages)
			{
				IImageSopProvider imageSopProvider = image as IImageSopProvider;
				if (imageSopProvider == null)
					throw new ArgumentException("Images must be valid IImageSopProviders.");
				frames.Add(imageSopProvider.Frame);
			}
			return Create(frames, callback);
		}

		/// <summary>
		/// Creates a <see cref="Volume"/> from the specified <see cref="Frame"/>s.
		/// </summary>
		/// <param name="frames">The <see cref="Frame"/>s from which to create the <see cref="Volume"/>.</param>
		/// <returns>A <see cref="Volume"/> constructed from the specified inputs.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="frames"/> is NULL.</exception>
		/// <exception cref="CreateVolumeException">Thrown if the specified inputs cannot be constructed into a <see cref="Volume"/>.</exception>
		public static Volume Create(IEnumerable<Frame> frames)
		{
			return Create(frames, null);
		}

		/// <summary>
		/// Creates a <see cref="Volume"/> from the specified <see cref="Frame"/>s.
		/// </summary>
		/// <param name="frames">The <see cref="Frame"/>s from which to create the <see cref="Volume"/>.</param>
		/// <param name="callback">A <see cref="CreateVolumeProgressCallback"/> to receive progress notification while the <see cref="Volume"/> is being loaded.</param>
		/// <returns>A <see cref="Volume"/> constructed from the specified inputs.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="frames"/> is NULL.</exception>
		/// <exception cref="CreateVolumeException">Thrown if the specified inputs cannot be constructed into a <see cref="Volume"/>.</exception>
		public static Volume Create(IEnumerable<Frame> frames, CreateVolumeProgressCallback callback)
		{
			Platform.CheckForNullReference(frames, "frames");
			using (VolumeBuilder builder = new VolumeBuilder(frames, callback))
			{
				return builder.Build();
			}
		}

		/// <summary>
		/// Creates a <see cref="Volume"/> from the <see cref="Frame"/>s referenced by the specified <see cref="IFrameReference"/>s.
		/// </summary>
		/// <remarks>
		/// This method does not take ownership of the <see cref="IFrameReference"/>s. The calling code is still responsible for disposing the frames.
		/// </remarks>
		/// <param name="frames">The <see cref="IFrameReference"/> from which to create the <see cref="Volume"/>. Does <b>NOT</b> take ownership of the references.</param>
		/// <returns>A <see cref="Volume"/> constructed from the specified inputs.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="frames"/> is NULL.</exception>
		/// <exception cref="CreateVolumeException">Thrown if the specified inputs cannot be constructed into a <see cref="Volume"/>.</exception>
		public static Volume Create(IEnumerable<IFrameReference> frames)
		{
			return Create(frames, null);
		}

		/// <summary>
		/// Creates a <see cref="Volume"/> from the <see cref="Frame"/>s referenced by the specified <see cref="IFrameReference"/>s.
		/// </summary>
		/// <remarks>
		/// This method does not take ownership of the <see cref="IFrameReference"/>s. The calling code is still responsible for disposing the frames.
		/// </remarks>
		/// <param name="frames">The <see cref="IFrameReference"/> from which to create the <see cref="Volume"/>. Does <b>NOT</b> take ownership of the references.</param>
		/// <param name="callback">A <see cref="CreateVolumeProgressCallback"/> to receive progress notification while the <see cref="Volume"/> is being loaded.</param>
		/// <returns>A <see cref="Volume"/> constructed from the specified inputs.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="frames"/> is NULL.</exception>
		/// <exception cref="CreateVolumeException">Thrown if the specified inputs cannot be constructed into a <see cref="Volume"/>.</exception>
		public static Volume Create(IEnumerable<IFrameReference> frames, CreateVolumeProgressCallback callback)
		{
			Platform.CheckForNullReference(frames, "frames");
			using (VolumeBuilder builder = new VolumeBuilder(frames, callback))
			{
				return builder.Build();
			}
		}

		/// <summary>
		/// Creates a <see cref="Volume"/> from the specified DICOM files.
		/// </summary>
		/// <param name="filenames">The DICOM files from which to create the <see cref="Volume"/>.</param>
		/// <returns>A <see cref="Volume"/> constructed from the specified inputs.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="filenames"/> is NULL.</exception>
		/// <exception cref="CreateVolumeException">Thrown if the specified inputs cannot be constructed into a <see cref="Volume"/>.</exception>
		public static Volume Create(IEnumerable<string> filenames)
		{
			return Create(filenames, null);
		}

		/// <summary>
		/// Creates a <see cref="Volume"/> from the specified DICOM files.
		/// </summary>
		/// <param name="filenames">The DICOM files from which to create the <see cref="Volume"/>.</param>
		/// <param name="callback">A <see cref="CreateVolumeProgressCallback"/> to receive progress notification while the <see cref="Volume"/> is being loaded.</param>
		/// <returns>A <see cref="Volume"/> constructed from the specified inputs.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="filenames"/> is NULL.</exception>
		/// <exception cref="CreateVolumeException">Thrown if the specified inputs cannot be constructed into a <see cref="Volume"/>.</exception>
		public static Volume Create(IEnumerable<string> filenames, CreateVolumeProgressCallback callback)
		{
			Platform.CheckForNullReference(filenames, "filenames");
			List<ImageSop> loadedImageSops = new List<ImageSop>();
			try
			{
				foreach (string fileName in filenames)
				{
					try
					{
						loadedImageSops.Add(new ImageSop(fileName));
					}
					catch (Exception ex)
					{
						throw new ArgumentException(string.Format("Files must be valid DICOM image SOPs. File: {0}", fileName), ex);
					}
				}

				List<Frame> frames = new List<Frame>();
				foreach (ImageSop sop in loadedImageSops)
					frames.AddRange(sop.Frames);
				return Create(frames, callback);
			}
			finally
			{
				foreach (ImageSop sop in loadedImageSops)
					sop.Dispose();
			}
		}
	}
}
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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volumes
{
	/// <summary>
	/// Represents an implementation of a reslicer algorithm for <see cref="Volume"/>s.
	/// </summary>
	public interface IVolumeSlicerCore : IDisposable
	{
		/// <summary>
		/// Gets the pixel data for the specified slice.
		/// </summary>
		/// <param name="imagePositionPatient">The image position, in patient coordinates, of the requested slice.</param>
		/// <returns>The pixel data buffer for the requested slice.</returns>
		byte[] CreateSlicePixelData(Vector3D imagePositionPatient);
	}

	/// <summary>
	/// Base implementation of a reslicer algorithm for <see cref="Volume"/>s.
	/// </summary>
	public abstract class VolumeSlicerCore : IVolumeSlicerCore
	{
		private readonly IVolumeReference _volumeReference;
		private readonly VolumeSliceArgs _args;

		/// <summary>
		/// Initializes the instance of <see cref="VolumeSlicerCore"/>.
		/// </summary>
		/// <param name="volumeReference">The <see cref="IVolumeReference"/> of the volume.</param>
		/// <param name="args">The requested slicer algorithm parameters.</param>
		/// <exception cref="ArgumentNullException">Thrown if either <paramref name="volumeReference"/> or <paramref name="args"/> are null.</exception>
		protected VolumeSlicerCore(IVolumeReference volumeReference, VolumeSliceArgs args)
		{
			Platform.CheckForNullReference(volumeReference, "volume");
			Platform.CheckForNullReference(args, "args");

			_volumeReference = volumeReference;
			_args = args;
		}

		~VolumeSlicerCore()
		{
			try
			{
				Dispose(false);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Exception thrown in finalizer");
			}
		}

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Exception thrown in Dispose");
			}
		}

		/// <summary>
		/// Called on <see cref="Dispose"/> or <see cref="Finalize"/>.
		/// </summary>
		/// <param name="disposing">True if being called from <see cref="Dispose"/>; False if the object is being finalized.</param>
		protected virtual void Dispose(bool disposing) {}

		/// <summary>
		/// Gets the requested output row orientation in patient coordinates.
		/// </summary>
		public Vector3D RowOrientationPatient
		{
			get { return _args.RowOrientationPatient; }
		}

		/// <summary>
		/// Gets the requested output column orientation in patient coordinates.
		/// </summary>
		public Vector3D ColumnOrientationPatient
		{
			get { return _args.ColumnOrientationPatient; }
		}

		/// <summary>
		/// Gets the requested output row spacing.
		/// </summary>
		public float RowSpacing
		{
			get { return _args.RowSpacing; }
		}

		/// <summary>
		/// Gets the requested output column spacing.
		/// </summary>
		public float ColumnSpacing
		{
			get { return _args.ColumnSpacing; }
		}

		/// <summary>
		/// Gets the requested output number of rows.
		/// </summary>
		public int Rows
		{
			get { return _args.Rows; }
		}

		/// <summary>
		/// Gets the requested output number of columns.
		/// </summary>
		public int Columns
		{
			get { return _args.Columns; }
		}

		/// <summary>
		/// Gets the requested thickness, in patient units, represented by each slice.
		/// </summary>
		public float SliceThickness
		{
			get { return _args.SliceThickness; }
		}

		/// <summary>
		/// Gets the requested number of subsamples when performing the reslicing.
		/// </summary>
		public int Subsamples
		{
			get { return _args.Subsamples; }
		}

		/// <summary>
		/// Gets the requested interpolation method to use when performing the reslicing.
		/// </summary>
		public VolumeInterpolationMode Interpolation
		{
			get { return _args.Interpolation; }
		}

		/// <summary>
		/// Gets the requested projection method to use to aggregate subsamples when performing the reslicing.
		/// </summary>
		public VolumeProjectionMode Projection
		{
			get { return _args.Projection; }
		}

		/// <summary>
		/// Gets the <see cref="IVolumeReference"/> of the source <see cref="Volume"/> being resliced.
		/// </summary>
		public IVolumeReference VolumeReference
		{
			get { return _volumeReference; }
		}

		public abstract byte[] CreateSlicePixelData(Vector3D imagePositionPatient);

		#region Static Factory Methods

		private static List<IVolumeSlicerCoreProvider> _coreProviders;

		/// <summary>
		/// Creates an implementation of <see cref="IVolumeSlicerCore"/> for slicing the requested volume.
		/// </summary>
		/// <param name="volumeReference">Reference to the <see cref="Volume"/> to be resliced.</param>
		/// <param name="args">The requested reslicing parameters.</param>
		public static IVolumeSlicerCore Create(IVolumeReference volumeReference, VolumeSliceArgs args)
		{
			if (_coreProviders == null)
			{
				try
				{
					_coreProviders = new VolumeSlicerCoreProviderExtensionPoint().CreateExtensions().Cast<IVolumeSlicerCoreProvider>().ToList();
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Error, ex, "Failed to initialize implementations on VolumeSlicerCoreProviderExtensionPoint");
				}
			}

			if (_coreProviders == null || !_coreProviders.Any())
			{
#if UNIT_TESTS
				// ensures the VTK version is loaded if possible
				TypeRef.Get("ClearCanvas.ImageViewer.Vtk.VtkVolumeSlicerCoreProvider, ClearCanvas.ImageViewer.VTK").Resolve();

				// ensures that unit tests running under a custom extension factory will always have a slicer implementation where available
				_coreProviders = Platform.PluginManager.ExtensionPoints
				                         .First(e => e.ExtensionPointClass == typeof (VolumeSlicerCoreProviderExtensionPoint))
				                         .ListExtensions().Select(x => Activator.CreateInstance(x.ExtensionClass.Resolve()))
				                         .Cast<IVolumeSlicerCoreProvider>().ToList();

				if (_coreProviders == null || !_coreProviders.Any())
					throw new NotSupportedException("No implementations of IVolumeSlicerCoreProvider were found.");
#else
				throw new NotSupportedException("No implementations of IVolumeSlicerCoreProvider were found.");
#endif
			}

			try
			{
				return _coreProviders.First(c => c.IsSupported(args)).CreateSlicerCore(volumeReference, args);
			}
			catch (Exception ex)
			{
				throw new NotSupportedException("No suitable implementation of IVolumeSlicerCoreProvider was found.", ex);
			}
		}

		#endregion
	}
}
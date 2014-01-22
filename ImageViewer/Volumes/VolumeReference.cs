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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Volumes
{
	/// <summary>
	/// Represents a reference to a <see cref="Volumes.Volume"/>.
	/// </summary>
	/// <remarks>
	/// Although you can simply hold an instance of <see cref="Volumes.Volume"/> directly, it is recommended
	/// practice to indirectly reference the <see cref="Volumes.Volume"/> by holding a <see cref="IVolumeReference"/>
	/// instead, as this allows code to be written in such a way that would support the use of cached,
	/// memory-managed volumes using the <see cref="VolumeCache"/> mechanism (where you must use indirect
	/// references as the underlying <see cref="Volumes.Volume"/> instance may change unexpectedly).
	/// </remarks>
	public interface IVolumeReference : IVolumeHeader, IDisposable
	{
		/// <summary>
		/// Gets the referenced <see cref="Volumes.Volume"/> instance.
		/// </summary>
		Volume Volume { get; }

		/// <summary>
		/// Clones an existing <see cref="IVolumeReference"/> to create a new reference instance.
		/// </summary>
		IVolumeReference Clone();
	}

	partial class Volume
	{
		/// <summary>
		/// Implements <see cref="IVolumeReference"/>
		/// </summary>
		/// <remarks>
		/// Although the volume itself no longer holds any unmanaged resources, and thus we can rely on the finalizer to appropriately handle cleanup,
		/// we encourage client code to use references rather hold the volume directly, since then the code is theoretically capable of
		/// using the volume cache and memory management system, which uses references by necessity.
		/// </remarks>
		private class VolumeReference : VolumeHeaderBase, IVolumeReference
		{
			private Volume _volume;

			public VolumeReference(Volume volume)
			{
				_volume = volume;
				_volume.OnReferenceCreated();
			}

			public Volume Volume
			{
				get { return _volume; }
			}

			protected override VolumeHeaderData VolumeHeaderData
			{
				get { return _volume._volumeHeaderData; }
			}

			public IVolumeReference Clone()
			{
				return _volume.CreateReference();
			}

			public void Dispose()
			{
				if (_volume != null)
				{
					_volume.OnReferenceDisposed();
					_volume = null;
				}
			}
		}

		private readonly object _syncLock = new object();
		private int _referenceCount = 0;
		private bool _selfDisposed = false;

		private void OnReferenceDisposed()
		{
			lock (_syncLock)
			{
				if (_referenceCount > 0)
					--_referenceCount;

				if (_referenceCount == 0 && _selfDisposed)
					DisposeInternal();
			}
		}

		private void OnReferenceCreated()
		{
			lock (_syncLock)
			{
				if (_referenceCount == 0 && _selfDisposed)
					throw new ObjectDisposedException(typeof (Volume).FullName);

				++_referenceCount;
			}
		}

		private void DisposeInternal()
		{
			try
			{
				Dispose(true);
				Disposed = true;
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		/// <summary>
		/// Gets a value indicating whether or not the object has already been disposed.
		/// </summary>
		protected bool Disposed { get; private set; }

		/// <summary>
		/// Creates a reference to this <see cref="Volume"/>.
		/// </summary>
		public IVolumeReference CreateReference()
		{
			if (Disposed) throw new ObjectDisposedException(typeof (Volume).FullName);
			return new VolumeReference(this);
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			lock (_syncLock)
			{
				_selfDisposed = true;

				//Only dispose for real when self has been disposed and all the references have been disposed.
				if (_referenceCount == 0)
					DisposeInternal();
			}
		}
	}
}
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
	public interface IVolumeReference : IDisposable
	{
		Volume Volume { get; }

		/// <summary>
		/// Clones an existing <see cref="IVolumeReference"/>, creating a new transient reference.
		/// </summary>
		IVolumeReference Clone();
	}

	partial class Volume
	{
		private class VolumeReference : IVolumeReference
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

			public IVolumeReference Clone()
			{
				return _volume.CreateTransientReference();
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
		private int _transientReferenceCount = 0;
		private bool _selfDisposed = false;

		private void OnReferenceDisposed()
		{
			lock (_syncLock)
			{
				if (_transientReferenceCount > 0)
					--_transientReferenceCount;

				if (_transientReferenceCount == 0 && _selfDisposed)
					DisposeInternal();
			}
		}

		private void OnReferenceCreated()
		{
			lock (_syncLock)
			{
				if (_transientReferenceCount == 0 && _selfDisposed)
					throw new ObjectDisposedException("The underlying sop data source has already been disposed.");

				++_transientReferenceCount;
			}
		}

		private void DisposeInternal()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

	    // TODO (CR Apr 2013): Now that there's no VTK objects being held, we should be able to get rid of these "references" and also get rid of the IDisposable implementation.

		/// <summary>
		/// Creates a new 'transient reference' to this <see cref="Volume"/>.
		/// </summary>
		/// <remarks>See <see cref="IVolumeReference"/> for a detailed explanation of 'transient references'.</remarks>
		public IVolumeReference CreateTransientReference()
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

				//Only dispose for real when self has been disposed and all the transient references have been disposed.
				if (_transientReferenceCount == 0)
					DisposeInternal();
			}
		}
	}
}
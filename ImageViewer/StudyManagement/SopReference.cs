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
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Interface defining a 'transient reference' to a <see cref="Sop"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In a Framework or SDK, managed objects often implement the
	/// <see cref="IDisposable"/> pattern even when they don't explicitly contain
	/// any unmanaged resources.  This is done in cases where it is possible, or perhaps very likely,
	/// that a derived class would in fact have unmanaged resources that need
	/// to be disposed or cleaned up.  Such is the case with classes like <see cref="ISopDataSource"/>.
	/// </para>
	/// <para>
	/// Also, occasionally, the clear 'owner' of an object, particularly one that may be
	/// passed around from object to object, is not easy to determine, or simply doesn't exist.
	/// For managed objects that contain no unmanaged resources, this doesn't matter because
	/// the object(s) can simply be discarded and left for the garbage collector to
	/// clean up.  But what about objects that must be disposed, but have no clear owner?
	/// One might argue that this points to a design flaw, and that may very well be correct in most cases.
	/// However, one totally valid case of this is objects that are cached for reasons of
	/// memory conservation, like <see cref="ISopDataSource"/>, that must also implement
	/// <see cref="IDisposable"/>.  How do you determine when to properly dispose these objects
	/// when there is no one parent container that disposes them when it is itself disposed
	/// (for example, the <see cref="ImageViewerComponent"/> or it's <see cref="StudyTree"/>)?
	/// The only plausible way is to implement reference counting on the cached objects and only truly perform
	/// the disposal when the reference count goes to zero.
	/// </para>
	/// <para>
	/// To solve all of these issues, enter the 'transient reference'.  So, how does it work?
	/// Basically, each transient reference object can itself be 'owned' by another object and disposed
	/// at the time the owning object is disposed.  This essentially allows many objects to reference
	/// the same shared/cached object, while also solving the problem of ownership.  Each object owns
	/// its reference object, and the object it points to doesn't actually have to be explicitly owned at all!
	/// Instead, all the entities that own a transient reference essentially share ownership of the referenced
	/// object, and only once all the transient references are disposed is the underlying referenced object
	/// disposed.  It's reference counting, but way better.  Because the 'reference count' is always equal
	/// to the total number of transient reference objects, things are suddenly much easier to manage.
	/// </para>
	/// <para>
	/// In the viewer framework, we don't typically work directly with <see cref="ISopDataSource"/>s because
	/// the interface is far too basic to have to work with directly.  Instead, we work with <see cref="Sop"/>,
	/// <see cref="ImageSop"/> and <see cref="Frame"/> objects that are simply bridges (see Bridge Design Pattern)
	/// to <see cref="ISopDataSource"/> and <see cref="ISopFrameData"/>.  The <see cref="Sop"/> class is also the entity responsible for
	/// transparently managing the caching of the <see cref="ISopDataSource"/>s passed to its constructor,
	/// therefore we work with 'transient references' to <see cref="Sop"/>s and <see cref="Frame"/>s instead
	/// of <see cref="ISopDataSource"/> and <see cref="ISopFrameData"/>.
	/// </para>
	/// <para>
	/// The recommended practice, when taking a reference to a <see cref="Sop"/> outside of it's current owner
	/// and holding a reference to it (like in the Clipboard, for example) is to get a <see cref="ISopReference">transient reference</see>
	/// to the <see cref="Sop"/> or <see cref="Frame"/>.  When you are done with the transient reference, call Dispose on it.
	/// The object that owns the <see cref="Sop"/>, normally the <see cref="ImageViewerComponent"/> will dispose the
	/// <see cref="Sop"/>, but internally, the <see cref="Sop"/> will not do any cleanup until all transient references to it
	/// are also disposed.  If you create a <see cref="Sop"/> yourself, it is good practice to dispose the <see cref="Sop"/>
	/// <b>and</b> all it's transient references in order to ensure the <see cref="ISopDataSource"/> is released
	/// from the cache.
	/// </para>
	/// </remarks>
	public interface ISopReference : ISopProvider, IDisposable
	{
		/// <summary>
		/// Clones an existing <see cref="ISopReference"/>, creating a new transient reference.
		/// </summary>
		ISopReference Clone();
	}

	public partial class Sop
	{
		private class SopReference : ISopReference
		{
			private Sop _sop;

			public SopReference(Sop sop)
			{
				_sop = sop;
				_sop.OnReferenceCreated();
			}

			#region ISopProvider Members

			public Sop Sop
			{
				get { return _sop; }
			}

			#endregion

			#region ICachedSop Members

			public ISopReference Clone()
			{
				return _sop.CreateTransientReference();
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				if (_sop != null)
				{
					_sop.OnReferenceDisposed();
					_sop = null;
				}
			}

			#endregion
		}

		#region Sop Stuff for Transient References

		private readonly object _syncLock = new object();
		private int _transientReferenceCount = 0;
		private bool _selfDisposed = false;

		private void OnReferenceDisposed()
		{
			lock(_syncLock)
			{
				if (_transientReferenceCount > 0)
					--_transientReferenceCount;

				if (_transientReferenceCount == 0 && _selfDisposed)
					DisposeInternal();
			}
		}
		
		private void OnReferenceCreated()
		{
			lock(_syncLock)
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
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		/// <summary>
		/// Creates a new 'transient reference' to this <see cref="Sop"/>.
		/// </summary>
		/// <remarks>See <see cref="ISopReference"/> for a detailed explanation of 'transient references'.</remarks>
		public ISopReference CreateTransientReference()
		{
			return new SopReference(this);
		}

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern.
		/// </summary>
		public void Dispose()
		{
			lock(_syncLock)
			{
				_selfDisposed = true;

				//Only dispose for real when self has been disposed and all the transient references have been disposed.
				if (_transientReferenceCount == 0)
					DisposeInternal();
			}
		}

		#endregion
	}
}
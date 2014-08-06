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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Defines an extension point for extensions that listen for entity change-sets.
	/// </summary>
	[ExtensionPoint]
	public class EntityChangeSetListenerExtensionPoint : ExtensionPoint<IEntityChangeSetListener>
	{
	}

	/// <summary>
	/// Publishes entity change-sets to listeners.
	/// </summary>
	public class EntityChangeSetPublisher
	{
		private readonly List<IEntityChangeSetListener> _listeners;

		/// <summary>
		/// Constructor
		/// </summary>
		public EntityChangeSetPublisher()
			: this(LoadExtensions())
		{
		}

		public EntityChangeSetPublisher(IEnumerable<IEntityChangeSetListener> listeners)
		{
			_listeners = new List<IEntityChangeSetListener>(listeners);
		}

		/// <summary>
		/// Gets a value indicating whether this instance has any listeners.
		/// </summary>
		public bool HasListeners
		{
			get { return _listeners.Count > 0; }
		}

		/// <summary>
		/// Publishes the pre-commit notification.
		/// </summary>
		/// <param name="args"></param>
		public void PreCommit(EntityChangeSetPreCommitArgs args)
		{
			foreach (var listener in _listeners)
			{
				// do not catch exceptions thrown by PreCommit listeners
				// if any listener throws an exception, the commit should be aborted
				listener.PreCommit(args);
			}
		}

		/// <summary>
		/// Publishes the post-commit notification.
		/// </summary>
		/// <param name="args"></param>
		public void PostCommit(EntityChangeSetPostCommitArgs args)
		{
			foreach (var listener in _listeners)
			{
				try
				{
					// might as well catch and log any exceptions thrown by PostCommit listeners,
					// because it is too late to abort the Commit
					listener.PostCommit(args);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Error, e);
				}
			}
		}

		private static IEnumerable<IEntityChangeSetListener> LoadExtensions()
		{
			return new TypeSafeEnumerableWrapper<IEntityChangeSetListener>(
				new EntityChangeSetListenerExtensionPoint().CreateExtensions());
		}
	}
}

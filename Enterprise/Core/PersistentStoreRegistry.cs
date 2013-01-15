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

namespace ClearCanvas.Enterprise.Core
{
	[ExtensionPoint]
	public class PersistentStoreExtensionPoint : ExtensionPoint<IPersistentStore>
	{
	}

	/// <summary>
	/// Static class for obtaining singleton persistent store objects.
	/// </summary>
	public static class PersistentStoreRegistry
	{
		private static readonly object _syncLock = new object();
		private static volatile IPersistentStore _defaultStore;

		/// <summary>
		/// Gets the default persistent store, creating it if not yet created.
		/// </summary>
		/// <remarks>
		/// This method can safely be called from multiple threads.
		/// </remarks>
		/// <returns></returns>
		public static IPersistentStore GetDefaultStore()
		{
			if (_defaultStore == null)
			{
				lock (_syncLock)
				{
					if(_defaultStore == null)
					{
						// for now, just look for a single extension and treat it as the "default" store
						// in future, there could conceivably be a number of different persistent stores,
						// in which case we will need to use a different mechanism (config file or something)
						var store = (IPersistentStore)(new PersistentStoreExtensionPoint()).CreateExtension();
						store.Initialize();

						// assign to static variable here, after initialization is complete
						_defaultStore = store;
					}
				}
			}
			return _defaultStore;
		}
	}
}

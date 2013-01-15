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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common
{
    /// <summary>
    /// The default implementation of <see cref="IExtensionFactory"/> that creates extensions from
    /// the set of plugins discovered at runtime.
    /// </summary>
    /// <remarks>
    /// This class is safe for use by mutliple concurrent threads.
    /// </remarks>
    internal class DefaultExtensionFactory : IExtensionFactory
    {
    	private volatile IDictionary<Type, List<ExtensionInfo>> _extensionMap;
		private readonly object _syncLock = new object();

        internal DefaultExtensionFactory()
        {
        }

        #region IExtensionFactory Members

        /// <summary>
        /// Creates one of each type of object that extends the input <paramref name="extensionPoint" />, 
        /// matching the input <paramref name="filter" />; creates a single extension if <paramref name="justOne"/> is true.
        /// </summary>
        /// <param name="extensionPoint">The <see cref="ExtensionPoint"/> to create extensions for.</param>
        /// <param name="filter">The filter used to match each extension that is discovered.</param>
        /// <param name="justOne">Indicates whether or not to return only the first matching extension that is found.</param>
        /// <returns></returns>
        public object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
        {
            // get subset of applicable extensions
            var extensions = ListExtensionsHelper(extensionPoint, filter);

            // attempt to instantiate the extension classes
            var createdObjects = new List<object>();
            foreach (var extension in extensions)
            {
                if (justOne && createdObjects.Count > 0)
                    break;

                try
                {
                    // instantiate
                    var o = Activator.CreateInstance(extension.ExtensionClass);
                    createdObjects.Add(o);
                }
                catch (Exception e)
                {
                    // instantiation failed
					// this should not be considered an exceptional circumstance
					// instantiation may fail by design in some cases (e.g extension is designed only to run on a particular platform)
					Platform.Log(LogLevel.Debug, e);
                }
            }

            return createdObjects.ToArray();
        }

        /// <summary>
        /// Gets metadata describing all enabled extensions of the input <paramref name="extensionPoint"/>, 
        /// matching the given <paramref name="filter"/>.
        /// </summary>
        /// <param name="extensionPoint">The <see cref="ExtensionPoint"/> whose extension metadata is to be retrieved.</param>
        /// <param name="filter">An <see cref="ExtensionFilter"/> used to filter out extensions with particular characteristics.</param>
        /// <returns></returns>
        public ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
        {
        	return ListExtensionsHelper(extensionPoint, filter).ToArray();
        }

        #endregion

		private List<ExtensionInfo> ListExtensionsHelper(ExtensionPoint extensionPoint, ExtensionFilter filter)
		{
			// ensure extension map has been constructed
			BuildExtensionMapOnce();

			List<ExtensionInfo> extensions;
			if (_extensionMap.TryGetValue(extensionPoint.GetType(), out extensions))
			{
				return CollectionUtils.Select(extensions,
					extension => extension.Enabled && extension.Authorized && (filter == null || filter.Test(extension)));
			}
			return new List<ExtensionInfo>();
		}

		private void BuildExtensionMapOnce()
		{
			// build extension map if not already built
			// note that this is the only place where we need to lock, because once built, map is safe for concurrent readers
			if (_extensionMap != null)
				return;

			lock(_syncLock)
			{
				if(_extensionMap == null)
				{
					// group extensions by extension point
					// (note that grouping preserves the order of the original Extensions list)
					_extensionMap = CollectionUtils.GroupBy(Platform.PluginManager.Extensions, ext => ext.PointExtended);
				}
			}
		}
    }
}

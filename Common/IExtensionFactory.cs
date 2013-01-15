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

namespace ClearCanvas.Common
{
    /// <summary>
    /// Interface defining a factory for extensions of arbitrary <see cref="ExtensionPoint"/>s.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface are expected to be thread-safe.
    /// </remarks>
	public interface IExtensionFactory
    {
		/// <summary>
		/// Creates one of each type of object that extends the input <paramref name="extensionPoint" />, 
		/// matching the input <paramref name="filter" />; creates a single extension if <paramref name="justOne"/> is true.
		/// </summary>
		/// <param name="extensionPoint">The <see cref="ExtensionPoint"/> to create extensions for.</param>
		/// <param name="filter">The filter used to match each extension that is discovered.</param>
		/// <param name="justOne">Indicates whether or not to return only the first matching extension that is found.</param>
		/// <returns></returns>
		object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne);

		/// <summary>
		/// Gets metadata describing all extensions of the input <paramref name="extensionPoint"/>, 
		/// matching the given <paramref name="filter"/>.
		/// </summary>
		/// <param name="extensionPoint">The <see cref="ExtensionPoint"/> whose extension metadata is to be retrieved.</param>
		/// <param name="filter">An <see cref="ExtensionFilter"/> used to filter out extensions with particular characteristics.</param>
		/// <returns></returns>
        ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter);
    }
}

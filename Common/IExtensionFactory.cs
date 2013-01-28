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
		/// Creates instances of available extensions that extend the specified extension point and match the specified filter.
		/// </summary>
		/// <param name="extensionPoint">The extension point for which to create extensions.</param>
		/// <param name="filter">An <see cref="ExtensionFilter"/> used to limit the result set to extensions with particular characteristics.</param>
		/// <param name="justOne">Indicates whether or not to return only the first matching extension that is found.</param>
		/// <returns>A set of extension instances.</returns>
		/// <remarks>
		/// Available extensions are those which are both enabled and licensed.
		/// If <paramref name="justOne"/> is true, the first matching extension that is successfully instantiated is returned,
		/// an no other extensions are instantiated.
		/// </remarks>
		object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne);

		/// <summary>
		/// Lists all available extensions for the specified <paramref name="extensionPoint"/> that match the specified <paramref name="filter"/>.
		/// </summary>
		/// <param name="extensionPoint">The extension point for which to retrieve a list of extensions.</param>
		/// <param name="filter">An <see cref="ExtensionFilter"/> used to limit the result set to extensions with particular characteristics.</param>
		/// <returns>A list of <see cref="ExtensionInfo"/> objects describing available extensions.</returns>
		/// <remarks>
		/// Available extensions are those which are both enabled and licensed.
		/// </remarks>
		ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter);
    }
}

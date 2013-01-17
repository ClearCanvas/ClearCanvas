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

namespace ClearCanvas.Common
{
    /// <summary>
    /// Extension point interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface provides a means for a client of an extension point to reference
    /// the extension point and call methods on it without knowing the type of the extension point class.
	/// </para>
	/// <para>
    /// Extension point classes should never implement this interface directly.
    /// Instead, subclass <see cref="ExtensionPoint" />.
	/// </para>
    /// </remarks>
    public interface IExtensionPoint
    {
        /// <summary>
        /// Lists the available extensions.
        /// </summary>
        /// <returns>An array of <see cref="ExtensionInfo" /> objects describing the available extensions.</returns>
        /// <remarks>
        /// Available extensions are those that are both enabled and licensed for use.
        /// </remarks>
        ExtensionInfo[] ListExtensions();

        /// <summary>
        /// Lists the available extensions, that also match the specified <see cref="ExtensionFilter"/>.
        /// </summary>
        /// <returns>An array of <see cref="ExtensionInfo" /> objects describing the available extensions.</returns>
		/// <remarks>
		/// Available extensions are those that are both enabled and licensed for use.
		/// </remarks>
		ExtensionInfo[] ListExtensions(ExtensionFilter filter);

        /// <summary>
        /// Lists the available extensions that match the specified filter.
        /// </summary>
		/// <remarks>
		/// Available extensions are those that are both enabled and licensed for use.
		/// </remarks>
		ExtensionInfo[] ListExtensions(Predicate<ExtensionInfo> filter);
 
        /// <summary>
        /// Instantiates one extension.
        /// </summary>
		/// <returns>A reference to the extension.</returns>
		/// <exception cref="NotSupportedException">Failed to instantiate an extension.</exception>
		/// <remarks>
        /// If more than one extension exists, then the type of the extension that is
        /// returned is non-deterministic.  If no extensions exist that can be successfully
        /// instantiated, an exception is thrown. Note that only extensions that are enabled
        /// and licensed are considered.
        /// </remarks>
        object CreateExtension();

        /// <summary>
        /// Instantiates an extension that also matches the specified <see cref="ExtensionFilter" />.
        /// </summary>
		/// <returns>A reference to the extension.</returns>
		/// <exception cref="NotSupportedException">Failed to instantiate an extension.</exception>
		/// <remarks>
        /// If more than one extension exists, then the type of the extension that is
        /// returned is non-deterministic.  If no extensions exist that can be successfully
		/// instantiated, an exception is thrown. Note that only extensions that are enabled
		/// and licensed are considered.
        /// </remarks>
        object CreateExtension(ExtensionFilter filter);

        /// <summary>
        /// Instantiates an extension that matches the specified filter.
        /// </summary>
		/// <returns>A reference to the extension.</returns>
		/// <exception cref="NotSupportedException">Failed to instantiate an extension.</exception>
		/// <remarks>
		/// If more than one extension exists, then the type of the extension that is
		/// returned is non-deterministic.  If no extensions exist that can be successfully
		/// instantiated, an exception is thrown. Note that only extensions that are enabled
		/// and licensed are considered.
		/// </remarks>
		object CreateExtension(Predicate<ExtensionInfo> filter);
        
        /// <summary>
        /// Instantiates each available extension.
        /// </summary>
        /// <remarks>
        /// Attempts to instantiate each available extension.  If an extension fails to instantiate
		/// for any reason, the failure is logged and it is ignored. Note that only extensions that are enabled
		/// and licensed are considered.
        /// </remarks>
        /// <returns>An array of references to the created extensions.  If no extensions were created
        /// the array will be empty.</returns>
        object[] CreateExtensions();

        /// <summary>
        /// Instantiates each available extension that also matches the specified <see cref="ExtensionFilter" />.
        /// </summary>
        /// <remarks>
        /// Attempts to instantiate each matching extension.  If an extension fails to instantiate
		/// for any reason, the failure is logged and it is ignored. Note that only extensions that are enabled
		/// and licensed are considered.
        /// </remarks>
        /// <returns>An array of references to the created extensions.  If no extensions were created
        /// the array will be empty.</returns>
        object[] CreateExtensions(ExtensionFilter filter);

        /// <summary>
        /// Instantiates each available extension that matches the specified filter.
        /// </summary>
		/// <remarks>
		/// Attempts to instantiate each matching extension.  If an extension fails to instantiate
		/// for any reason, the failure is logged and it is ignored. Note that only extensions that are enabled
		/// and licensed are considered.
		/// </remarks>
		/// <returns>An array of references to the created extensions.  If no extensions were created
		/// the array will be empty.</returns>
		object[] CreateExtensions(Predicate<ExtensionInfo> filter);
    }
}

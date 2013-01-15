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

using System.Drawing;
using System.IO;
using System.Reflection;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Extension point for <see cref="IApplicationThemeResourceProvider"/> implementations.
	/// </summary>
	/// <remarks>
	/// <para>Extensions of this extension point may not provide an empty or NULL value for <see cref="IApplicationThemeResourceProvider.Id"/>.</para>
	/// <para>Consider subclassing <see cref="ApplicationThemeResourceProviderBase"/> instead of implementing the <see cref="IApplicationThemeResourceProvider"/> interface directly.</para>
	/// </remarks>
	/// <seealso cref="ApplicationThemeManager"/>
	/// <seealso cref="ApplicationThemeResourceProviderBase"/>
	/// <seealso cref="IApplicationThemeResourceProvider"/>
	[ExtensionPoint]
	public sealed class ApplicationThemeResourceProviderExtensionPoint : ExtensionPoint<IApplicationThemeResourceProvider> {}

	/// <summary>
	/// Defines the methods and properties for accessing GUI element style information.
	/// </summary>
	/// <remarks>
	/// Consider subclassing <see cref="ApplicationThemeResourceProviderBase"/> instead of implementing this interface directly.
	/// </remarks>
	/// <seealso cref="ApplicationThemeManager"/>
	/// <seealso cref="ApplicationThemeResourceProviderBase"/>
	/// <seealso cref="ApplicationThemeResourceProviderExtensionPoint"/>
	public interface IApplicationThemeResourceProvider
	{
		/// <summary>
		/// Gets a string that identifies the application theme.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the localized name of the application theme.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a localized description of the application theme.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the resource name of an icon for the application theme.
		/// </summary>
		string Icon { get; }

		/// <summary>
		/// Gets the color scheme associated with the application theme.
		/// </summary>
		IApplicationThemeColors Colors { get; }

		/// <summary>
		/// Checks whether or not the application theme provides a themed replacement for the specified resource name.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>True if the application theme provides a themed replacement; False otherwise.</returns>
		bool HasResource(string resourceFullName, Assembly originalAssemblyHint);

		/// <summary>
		/// Gets a <see cref="Stream"/> to the themed replacement for the specified resource name provided by the application theme.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>A new <see cref="Stream"/> for the resource if the application theme provides a themed replacement; NULL otherwise.</returns>
		Stream OpenResource(string resourceFullName, Assembly originalAssemblyHint);
	}

	/// <summary>
	/// Defines the color scheme associated with an application theme.
	/// </summary>
	public interface IApplicationThemeColors
	{
		/// <summary>
		/// Gets the base standard colour.
		/// </summary>
		Color StandardColorBase { get; }

		/// <summary>
		/// Gets a dark variant of the standard colour.
		/// </summary>
		Color StandardColorDark { get; }

		/// <summary>
		/// Gets a light variant of the standard colour.
		/// </summary>
		Color StandardColorLight { get; }
	}
}
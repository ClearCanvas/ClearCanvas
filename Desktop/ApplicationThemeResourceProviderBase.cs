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
using System.Drawing;
using System.IO;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Base abstract implementation of <see cref="IApplicationThemeResourceProvider"/>.
	/// </summary>
	/// <remarks>
	/// The default implementation assumes that all themed resources are embedded in the assembly in which the implementing type is defined.
	/// The mapping of the original resource name to the local name of the resource can be overriden using <see cref="MapResourceName"/>.
	/// </remarks>
	/// <seealso cref="ApplicationThemeManager"/>
	/// <seealso cref="ApplicationThemeResourceProviderExtensionPoint"/>
	/// <seealso cref="IApplicationThemeResourceProvider"/>
	public abstract class ApplicationThemeResourceProviderBase : IApplicationThemeResourceProvider
	{
		private readonly IResourceResolver _resourceResolver;
		private readonly IApplicationThemeColors _colors;
		private readonly string _id;
		private readonly string _name;
		private readonly string _description;
		private readonly string _icon;

		/// <summary>
		/// Initializes the <see cref="ApplicationThemeResourceProviderBase"/>.
		/// </summary>
		/// <param name="id">The string that identifies the theme.</param>
		/// <param name="name">The name of the theme. May be a key to a string resource in an SR table in the same assembly as the implementing type.</param>
		/// <param name="description">A description of the theme. May be a key to a string resource in an SR table in the same assembly as the implementing type.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="id"/> is NULL or empty.</exception>
		protected ApplicationThemeResourceProviderBase(string id, string name, string description)
			: this(id, name, description, null) {}

		/// <summary>
		/// Initializes the <see cref="ApplicationThemeResourceProviderBase"/>.
		/// </summary>
		/// <param name="id">The string that identifies the theme.</param>
		/// <param name="name">The name of the theme. May be a key to a string resource in an SR table in the same assembly as the implementing type.</param>
		/// <param name="description">A description of the theme. May be a key to a string resource in an SR table in the same assembly as the implementing type.</param>
		/// <param name="icon">A resource name to an icon for the theme in the same assembly as the implementing type. May be NULL or empty if no icon is available.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="id"/> is NULL or empty.</exception>
		protected ApplicationThemeResourceProviderBase(string id, string name, string description, string icon)
		{
			Platform.CheckForEmptyString(id, @"id");
			_id = id;
			_name = !string.IsNullOrEmpty(name) ? name : id;
			_description = description ?? string.Empty;
			_icon = icon ?? string.Empty;
			_resourceResolver = new ResourceResolver(GetType(), false);
			_colors = new ApplicationThemeColors(this);
		}

		public string Id
		{
			get { return _id; }
		}

		public string Name
		{
			get { return _resourceResolver.LocalizeString(_name); }
		}

		public string Description
		{
			get { return _resourceResolver.LocalizeString(_description); }
		}

		public string Icon
		{
			get { return _icon; }
		}

		public IApplicationThemeColors Colors
		{
			get { return _colors; }
		}

		/// <summary>
		/// Gets the base standard colour.
		/// </summary>
		protected virtual Color StandardColorBase
		{
			get { return Color.FromArgb(124, 177, 221); }
		}

		/// <summary>
		/// Gets a dark variant of the standard colour.
		/// </summary>
		protected virtual Color StandardColorDark
		{
			get { return Color.FromArgb(61, 152, 209); }
		}

		/// <summary>
		/// Gets a light variant of the standard colour.
		/// </summary>
		protected virtual Color StandardColorLight
		{
			get { return Color.FromArgb(186, 210, 236); }
		}

		/// <summary>
		/// Called by the default implementations of <see cref="HasResource"/> and <see cref="OpenResource"/> to map a requested resource to
		/// an expected resource name local to the assembly of the implementing type.
		/// </summary>
		/// <remarks>
		/// <para>The default implementation prepends the theme <see cref="Id"/> as a namespace qualifier to <paramref name="resourceFullName"/>.</para>
		/// <para>The expected resource name returned by this function need not actually exist.</para>
		/// </remarks>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>The expected resource name, local to the implementing type, of a themed replacement for the specified resource.</returns>
		protected virtual string MapResourceName(string resourceFullName, Assembly originalAssemblyHint)
		{
			return _id + '.' + resourceFullName;
		}

		/// <summary>
		/// Checks whether or not the application theme provides a themed replacement for the specified resource name.
		/// </summary>
		/// <remarks>
		/// The default implementation calls <see cref="MapResourceName"/> to map the requested resource to the expected resource name
		/// local to the assembly of the implementing type, if the resource is available as part of this theme.
		/// </remarks>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>True if the application theme provides a themed replacement; False otherwise.</returns>
		public virtual bool HasResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			try
			{
				var resourceName = MapResourceName(resourceFullName, originalAssemblyHint);
				string resolvedResourceName;
				return _resourceResolver.TryResolveResource(resourceName, out resolvedResourceName);
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Gets a <see cref="Stream"/> to the themed replacement for the specified resource name provided by the application theme.
		/// </summary>
		/// <remarks>
		/// The default implementation calls <see cref="MapResourceName"/> to map the requested resource to the expected resource name
		/// local to the assembly of the implementing type, if the resource is available as part of this theme.
		/// </remarks>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>A new <see cref="Stream"/> for the resource if the application theme provides a themed replacement; NULL otherwise.</returns>
		public virtual Stream OpenResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			try
			{
				var resourceName = MapResourceName(resourceFullName, originalAssemblyHint);
				Stream resourceStream;
				if (_resourceResolver.TryOpenResource(resourceName, out resourceStream))
					return resourceStream;

				return null;
			}
			catch
			{
				return null;
			}
		}

		private class ApplicationThemeColors : IApplicationThemeColors
		{
			private readonly ApplicationThemeResourceProviderBase _owner;

			public ApplicationThemeColors(ApplicationThemeResourceProviderBase owner)
			{
				_owner = owner;
			}

			public Color StandardColorBase
			{
				get { return _owner.StandardColorBase; }
			}

			public Color StandardColorDark
			{
				get { return _owner.StandardColorDark; }
			}

			public Color StandardColorLight
			{
				get { return _owner.StandardColorLight; }
			}
		}
	}
}
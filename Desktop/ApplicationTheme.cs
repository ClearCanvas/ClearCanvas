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
using System.Drawing;
using System.IO;
using System.Reflection;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Provides GUI element style information so as to provide a consistent look and feel for the application.
	/// The information includes alternative resource graphics and a matching colour scheme.
	/// </summary>
	/// <remarks>
	/// Individual instances of <see cref="ApplicationTheme"/> are managed by the <see cref="ApplicationThemeManager"/>.
	/// </remarks>
	/// <seealso cref="ApplicationThemeManager"/>
	public sealed class ApplicationTheme
	{
		private readonly IList<IApplicationThemeResourceProvider> _providers;
		private readonly string _id;

		/// <summary>
		/// Initializes a new <see cref="ApplicationTheme"/>.
		/// </summary>
		/// <param name="id">The ID of the <see cref="ApplicationTheme"/> to be initialized.</param>
		/// <param name="providers">A collection of <see cref="IApplicationThemeResourceProvider"/> implementations that, combined, provide the style information.</param>
		internal ApplicationTheme(string id, IEnumerable<IApplicationThemeResourceProvider> providers)
		{
			_id = id;
			_providers = new List<IApplicationThemeResourceProvider>(providers).AsReadOnly();
		}

		/// <summary>
		/// Gets a string that identifies the <see cref="ApplicationTheme"/>.
		/// </summary>
		public string Id
		{
			get { return _id; }
		}

		/// <summary>
		/// Gets the localized name of the <see cref="ApplicationTheme"/>.
		/// </summary>
		public string Name
		{
			get { return _providers[0].Name; }
		}

		/// <summary>
		/// Gets a localized description of the <see cref="ApplicationTheme"/>.
		/// </summary>
		public string Description
		{
			get { return _providers[0].Description; }
		}

		/// <summary>
		/// Gets the resource name of an icon for the <see cref="ApplicationTheme"/>.
		/// </summary>
		/// <seealso cref="GetIcon"/>
		public string Icon
		{
			get { return _providers[0].Icon; }
		}

		/// <summary>
		/// Gets the color scheme associated with the <see cref="ApplicationTheme"/>.
		/// </summary>
		public IApplicationThemeColors Colors
		{
			get { return _providers[0].Colors; }
		}

		/// <summary>
		/// Gets the icon for the <see cref="ApplicationTheme"/>.
		/// </summary>
		/// <returns>A new <see cref="Stream"/> for the icon.</returns>
		/// <seealso cref="Icon"/>
		public Stream GetIcon()
		{
			if (string.IsNullOrEmpty(_providers[0].Icon))
				return null;

			var resourceResolver = new ResourceResolver(_providers[0].GetType(), false);
			return resourceResolver.OpenResource(_providers[0].Icon);
		}

		/// <summary>
		/// Checks whether or not the <see cref="ApplicationTheme"/> provides a themed replacement for the specified resource name.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>True if the <see cref="ApplicationTheme"/> provides a themed replacement; False otherwise.</returns>
		public bool HasResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			foreach (var provider in _providers)
				if (provider.HasResource(resourceFullName, originalAssemblyHint)) return true;
			return false;
		}

		/// <summary>
		/// Gets a <see cref="Stream"/> to the themed replacement for the specified resource name provided by the <see cref="ApplicationTheme"/>.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>A new <see cref="Stream"/> for the resource if the <see cref="ApplicationTheme"/> provides a themed replacement; NULL otherwise.</returns>
		public Stream OpenResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			Stream stream;
			foreach (var provider in _providers)
				if ((stream = provider.OpenResource(resourceFullName, originalAssemblyHint)) != null) return stream;
			return null;
		}

		#region Static Helpers

		/// <summary>
		/// Gets or sets the current <see cref="ApplicationTheme"/> in use by the desktop application framework.
		/// </summary>
		/// <remarks>
		/// This property is a synonym for <see cref="Application.CurrentUITheme"/>.
		/// </remarks>
		public static ApplicationTheme CurrentTheme
		{
			get { return Application.CurrentUITheme; }
			set { Application.CurrentUITheme = value; }
		}

		/// <summary>
		/// Gets or sets the current <see cref="ApplicationTheme"/> ID for the desktop application framework.
		/// </summary>
		/// <remarks>
		/// When reading this property, the value returned will be the ID of the current <see cref="ApplicationTheme"/> in use by the desktop application framework.
		/// When writing to this property, the current <see cref="ApplicationTheme"/> will be set to the defined theme with the matching ID, or the default theme if there are no matching themes.
		/// </remarks>
		public static string CurrentThemeId
		{
			get { return ApplicationThemeManager.CurrentThemeId; }
			set { ApplicationThemeManager.CurrentThemeId = value; }
		}

		/// <summary>
		/// Gets a collection of <see cref="ApplicationTheme"/>s available in the installation.
		/// </summary>
		public static ICollection<ApplicationTheme> Themes
		{
			get { return ApplicationThemeManager.Themes; }
		}

		/// <summary>
		/// Checks whether or not an <see cref="ApplicationTheme"/> with the given ID is available in the installation.
		/// </summary>
		/// <param name="id">The ID of the <see cref="ApplicationTheme"/> to be checked.</param>
		/// <returns>True if an <see cref="ApplicationTheme"/> with the given ID is available; False otherwise.</returns>
		public static bool IsThemeDefined(string id)
		{
			return ApplicationThemeManager.IsThemeDefined(id);
		}

		/// <summary>
		/// Gets the <see cref="ApplicationTheme"/> with the given ID.
		/// </summary>
		/// <param name="id">The ID of the <see cref="ApplicationTheme"/> to be retrieved.</param>
		/// <returns>The <see cref="ApplicationTheme"/> with the given ID, or NULL if an <see cref="ApplicationTheme"/> with the given ID is not available.</returns>
		public static ApplicationTheme GetTheme(string id)
		{
			return ApplicationThemeManager.GetTheme(id);
		}

		/// <summary>
		/// Creates a new user-defined <see cref="ApplicationTheme"/>.
		/// </summary>
		/// <param name="id">The ID of the <see cref="ApplicationTheme"/> to be created.</param>
		/// <param name="provider">An <see cref="IApplicationThemeResourceProvider"/> implementation that provides the style information.</param>
		/// <param name="additionalProviders">Additional <see cref="IApplicationThemeResourceProvider"/> implementations.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="provider"/> is NULL.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="id"/> is NULL or empty.</exception>
		public static ApplicationTheme CreateTheme(string id, IApplicationThemeResourceProvider provider, params IApplicationThemeResourceProvider[] additionalProviders)
		{
			Platform.CheckForNullReference(provider, "provider");

			var providers = new List<IApplicationThemeResourceProvider> {provider};
			if (additionalProviders != null) providers.AddRange(additionalProviders);
			return CreateTheme(id, providers);
		}

		/// <summary>
		/// Creates a new user-defined <see cref="ApplicationTheme"/>.
		/// </summary>
		/// <param name="id">The ID of the <see cref="ApplicationTheme"/> to be created.</param>
		/// <param name="providers">A collection of <see cref="IApplicationThemeResourceProvider"/> implementations that provide the style information.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="providers"/> is NULL.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="id"/> is NULL or empty, or <paramref name="providers"/> does not contain at least one provider.</exception>
		public static ApplicationTheme CreateTheme(string id, IEnumerable<IApplicationThemeResourceProvider> providers)
		{
			Platform.CheckForNullReference(providers, "providers");
			Platform.CheckTrue(CollectionUtils.FirstElement(providers) != null, "The providers collection must contain at least one provider.");
			Platform.CheckForEmptyString(id, "id");

			return new ApplicationTheme(id, providers);
		}

		#endregion

		#region Default Application Theme

		/// <summary>
		/// Gets an <see cref="ApplicationTheme"/> representing the default application style.
		/// </summary>
		public static readonly ApplicationTheme DefaultApplicationTheme = new ApplicationTheme(string.Empty, new[] {new DefaultApplicationThemeResourceProvider()});

		/// <summary>
		/// A default theme resource provider, which is really just a placeholder that provides no alternative resources and only the basic stock "ClearCanvas Blue" colour scheme
		/// </summary>
		private sealed class DefaultApplicationThemeResourceProvider : IApplicationThemeResourceProvider
		{
			private readonly DefaultApplicationThemeColors _colors = new DefaultApplicationThemeColors();

			public string Id
			{
				get { return string.Empty; }
			}

			public string Name
			{
				get { return SR.LabelDefault; }
			}

			public string Description
			{
				get { return SR.DescriptionDefaultTheme; }
			}

			public string Icon
			{
				get { return string.Empty; }
			}

			public IApplicationThemeColors Colors
			{
				get { return _colors; }
			}

			public bool HasResource(string resourceFullName, Assembly originalAssemblyHint)
			{
				return false;
			}

			public Stream OpenResource(string resourceFullName, Assembly originalAssemblyHint)
			{
				return null;
			}

			private class DefaultApplicationThemeColors : IApplicationThemeColors
			{
				public Color StandardColorBase
				{
					get { return Color.FromArgb(124, 177, 221); }
				}

				public Color StandardColorDark
				{
					get { return Color.FromArgb(61, 152, 209); }
				}

				public Color StandardColorLight
				{
					get { return Color.FromArgb(186, 210, 236); }
				}
			}
		}

		#endregion
	}
}
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
	/// Manages the <see cref="ApplicationTheme"/>s available in the application based on the enabled <see cref="IApplicationThemeResourceProvider"/> extensions in the plugins.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In order to provide for individual plugins providing all or part of a theme, the <see cref="ApplicationTheme"/>
	/// class is not designed to be directly extensible. Instead, plugins should implement <see cref="IApplicationThemeResourceProvider"/>s
	/// for the desired theme ID via the <see cref="ApplicationThemeResourceProviderExtensionPoint"/>.
	/// The <see cref="ApplicationThemeManager"/> will automatically combine all <see cref="IApplicationThemeResourceProvider"/> extensions together
	/// by matching ID to produce appropriate <see cref="ApplicationTheme"/> instances.
	/// </para>
	/// <para>
	/// In the event that multiple <see cref="IApplicationThemeResourceProvider"/>s try to provide for the same requested theme resource,
	/// extension ordering will determine the result of the request operation.
	/// </para>
	/// <para>
	/// All explicitly defined application themes must have a non-empty theme ID. The empty theme ID maps to the <see cref="DefaultTheme"/>, which
	/// provides a pass through to the original resources defined in the respective plugin assemblies, as well as the basic blue colour scheme.
	/// </para>
	/// </remarks>
	/// <seealso cref="ApplicationTheme"/>
	/// <seealso cref="ApplicationThemeResourceProviderExtensionPoint"/>
	/// <seealso cref="IApplicationThemeResourceProvider"/>
	public static class ApplicationThemeManager
	{
		private static readonly Dictionary<string, ApplicationTheme> _themes;

		static ApplicationThemeManager()
		{
			var themeResourceProviders = new Dictionary<string, List<IApplicationThemeResourceProvider>>();
			foreach (IApplicationThemeResourceProvider theme in new ApplicationThemeResourceProviderExtensionPoint().CreateExtensions())
			{
				if (!string.IsNullOrEmpty(theme.Id))
				{
					if (!themeResourceProviders.ContainsKey(theme.Id))
						themeResourceProviders.Add(theme.Id, new List<IApplicationThemeResourceProvider>());
					themeResourceProviders[theme.Id].Add(theme);
				}
			}
			_themes = CollectionUtils.Map(themeResourceProviders, kvp => new KeyValuePair<string, ApplicationTheme>(kvp.Key, new ApplicationTheme(kvp.Key, kvp.Value)));
			_themes.Add(string.Empty, ApplicationTheme.DefaultApplicationTheme);
		}

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
			get { return CurrentTheme.Id; }
			set { CurrentTheme = GetTheme(value); }
		}

		/// <summary>
		/// Gets an <see cref="ApplicationTheme"/> representing the default application style.
		/// </summary>
		public static ApplicationTheme DefaultTheme
		{
			get { return ApplicationTheme.DefaultApplicationTheme; }
		}

		/// <summary>
		/// Gets a collection of <see cref="ApplicationTheme"/>s available in the installation.
		/// </summary>
		public static ICollection<ApplicationTheme> Themes
		{
			get { return _themes.Values; }
		}

		/// <summary>
		/// Checks whether or not an <see cref="ApplicationTheme"/> with the given ID is available in the installation.
		/// </summary>
		/// <param name="id">The ID of the <see cref="ApplicationTheme"/> to be checked.</param>
		/// <returns>True if an <see cref="ApplicationTheme"/> with the given ID is available; False otherwise.</returns>
		public static bool IsThemeDefined(string id)
		{
			return _themes.ContainsKey(id ?? string.Empty);
		}

		/// <summary>
		/// Gets the <see cref="ApplicationTheme"/> with the given ID.
		/// </summary>
		/// <param name="id">The ID of the <see cref="ApplicationTheme"/> to be retrieved.</param>
		/// <returns>The <see cref="ApplicationTheme"/> with the given ID, or NULL if an <see cref="ApplicationTheme"/> with the given ID is not available.</returns>
		public static ApplicationTheme GetTheme(string id)
		{
			ApplicationTheme theme;
			return _themes.TryGetValue(id ?? string.Empty, out theme) ? theme : null;
		}

		/// <summary>
		/// Checks whether or not the <see cref="CurrentTheme"/> provides a themed replacement for the specified resource name.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <returns>True if the <see cref="CurrentTheme"/> provides a themed replacement; False otherwise.</returns>
		public static bool HasResource(string resourceFullName)
		{
			return HasResource(resourceFullName, null);
		}

		/// <summary>
		/// Checks whether or not the <see cref="CurrentTheme"/> provides a themed replacement for the specified resource name.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>True if the <see cref="CurrentTheme"/> provides a themed replacement; False otherwise.</returns>
		public static bool HasResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			try
			{
				var currentTheme = CurrentTheme;
				return currentTheme != null && currentTheme.HasResource(resourceFullName, originalAssemblyHint);
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "An exception was thrown while processing an application theme.");
				return false;
			}
		}

		/// <summary>
		/// Opens the themed replacement for the specified resource name provided by the <see cref="CurrentTheme"/> as a <see cref="Stream"/>.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <returns>A new <see cref="Stream"/> for the resource if the <see cref="CurrentTheme"/> provides a themed replacement; NULL otherwise.</returns>
		public static Stream OpenResource(string resourceFullName)
		{
			return OpenResource(resourceFullName, null);
		}

		/// <summary>
		/// Opens the themed replacement for the specified resource name provided by the <see cref="CurrentTheme"/> as a <see cref="Stream"/>.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>A new <see cref="Stream"/> for the resource if the <see cref="CurrentTheme"/> provides a themed replacement; NULL otherwise.</returns>
		public static Stream OpenResource(string resourceFullName, Assembly originalAssemblyHint)
		{
			try
			{
				var currentTheme = CurrentTheme;
				return currentTheme != null ? currentTheme.OpenResource(resourceFullName, originalAssemblyHint) : null;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "An exception was thrown while processing an application theme.");
				return null;
			}
		}

		/// <summary>
		/// Opens the themed replacement for the specified image resource provided by the <see cref="CurrentTheme"/> as an <see cref="Image"/>.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the image resource being requested.</param>
		/// <returns>A new <see cref="Image"/> for the resource if the <see cref="CurrentTheme"/> provides a themed replacement; NULL otherwise.</returns>
		public static Image OpenImage(string resourceFullName)
		{
			return OpenImage(resourceFullName, null);
		}

		/// <summary>
		/// Opens the themed replacement for the specified image resource provided by the <see cref="CurrentTheme"/> as an <see cref="Image"/>.
		/// </summary>
		/// <param name="resourceFullName">The fully-qualified name of the image resource being requested.</param>
		/// <param name="originalAssemblyHint">The original assembly in which the image resource was defined, if known. May be NULL if unknown.</param>
		/// <returns>A new <see cref="Image"/> for the resource if the <see cref="CurrentTheme"/> provides a themed replacement; NULL otherwise.</returns>
		public static Image OpenImage(string resourceFullName, Assembly originalAssemblyHint)
		{
			try
			{
				using (var stream = OpenResource(resourceFullName, originalAssemblyHint))
				using (var image = Image.FromStream(stream))
					return new Bitmap(image); // #9261 - must return a new clone of image to avoid GDI resource stream sharing exception
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "An exception was thrown while processing an application theme.");
				return null;
			}
		}
	}
}
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
using System.Resources;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// An <see cref="ApplicationTheme"/>-enabled implementation of <see cref="IResourceResolver"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Resolves resources by searching the set of assemblies (specified in the constructor)
	/// in order for a matching resource. If the application is currently using a <see cref="ApplicationTheme"/>
	/// and the theme provides a matching resource, the themed resource will be returned.
	/// </para>
	/// <para>Instances of this class are immutable and thread-safe.</para>
	/// </remarks>
	/// <seealso cref="ApplicationTheme"/>
	/// <seealso cref="ApplicationThemeManager"/>
	public class ApplicationThemeResourceResolver : ResourceResolver
	{
		private bool _useApplicationTheme = true;

		/// <summary>
		/// Constructs a resource resolver that will look in the specified set of assemblies for resources.
		/// </summary>
		/// <param name="assemblies">The set of assemblies to search.</param>
		public ApplicationThemeResourceResolver(Assembly[] assemblies)
			: base(assemblies) {}

		/// <summary>
		/// Constructs an object that will search the specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly to search.</param>
		public ApplicationThemeResourceResolver(Assembly assembly)
			: base(assembly) {}

		/// <summary>
		/// Constructs a resource resolver that will look in the specified assembly for resources.
		/// </summary>
		/// <param name="assembly">The assembly to search.</param>
		/// <param name="fallback">The fallback <see cref="IResourceResolver"/> to use when an object cannot be resolved by this resolver.</param>
		public ApplicationThemeResourceResolver(Assembly assembly, IResourceResolver fallback)
			: base(assembly, fallback) {}

		/// <summary>
		/// Constructs a resource resolver that will look in the specified set of assemblies for resources.
		/// </summary>
		/// <param name="assemblies">Assemblies covered by this resolver.</param>
		/// <param name="fallback">A fallback resolver, that will be invoked if resources are not found in the specified assemblies.</param>
		public ApplicationThemeResourceResolver(Assembly[] assemblies, IResourceResolver fallback)
			: base(assemblies, fallback) {}

		/// <summary>
		/// Constructs a resource resolver that will find resources in the assembly containing the specified type,
		/// and optionally those assemblies containing its base types.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="walkInheritanceChain">If true, assemblies containing base types of the specified type will also be included.</param>
		public ApplicationThemeResourceResolver(Type type, bool walkInheritanceChain)
			: base(type, walkInheritanceChain) {}

		/// <summary>
		/// Constructs a resource resolver that will find resources in the assembly containing the specified type,
		/// and optionally those assemblies containing its base types.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="walkInheritanceChain">If true, assemblies containing base types of the specified type will also be included.</param>
		/// <param name="fallback"></param>
		public ApplicationThemeResourceResolver(Type type, bool walkInheritanceChain, IResourceResolver fallback)
			: base(type, walkInheritanceChain, fallback) {}

		/// <summary>
		/// Gets or sets a value controlling whether or not the current application theme will be used when resolving resources with this <see cref="IResourceResolver"/>.
		/// </summary>
		public bool UseApplicationTheme
		{
			get { return _useApplicationTheme; }
			set { _useApplicationTheme = value; }
		}

		/// <summary>
		/// Attempts to resolve and open an image resource from the specified name, which may be partially
		/// qualified or entirely unqualified, by searching the assemblies associated with this <see cref="ResourceResolver"/> in order.
		/// </summary>
		/// <remarks>
		/// Since application themes typically override graphical resources in the form of images, this helper method is provided to
		/// invoke <see cref="IResourceResolver.OpenResource"/> and automatically reconstitute the <see cref="Stream"/> into an <see cref="Image"/>.
		/// </remarks>
		/// <param name="resourceName">A partially qualified or unqualified resource name.</param>
		/// <returns>The loaded resource image.</returns>
		/// <exception cref="MissingManifestResourceException">if the resource name could not be resolved.</exception>
		public Image OpenImage(string resourceName)
		{
			using (var stream = OpenResource(resourceName))
			using (var image = Image.FromStream(stream))
				return new Bitmap(image); // #9261 - must return a new clone of image to avoid GDI resource stream sharing exception
		}

		protected override Stream OpenResource(string resourceFullName, Assembly assembly)
		{
			var resource = UseApplicationTheme ? ApplicationThemeManager.OpenResource(resourceFullName, assembly) : null;
			return resource ?? base.OpenResource(resourceFullName, assembly);
		}
	}
}
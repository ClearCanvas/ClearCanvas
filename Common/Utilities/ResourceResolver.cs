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
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Default implementation of <see cref="IResourceResolver"/>; finds resources embedded in assemblies.
	/// </summary>
	/// <remarks>
	/// Resolves resources by searching the set of assemblies (specified in the constructor)
	/// in order for a matching resource.  Instances of this class are immutable and thread-safe.
	/// </remarks>
	public class ResourceResolver : IResourceResolver
	{
		/// <summary>
		/// Cache of string resource managers for each assembly.
		/// </summary>
		/// <remarks>
		/// This field is accessed in only one method, GetStringResourceManagers().  This 
		/// is important from a thread-sync point of view.
		/// </remarks>
		private static readonly Dictionary<Assembly, List<ResourceManager>> _mapStringResourceManagers = new Dictionary<Assembly, List<ResourceManager>>();

		/// <summary>
		/// Assemblies to search for resources.
		/// </summary>
		private readonly Assembly[] _assemblies;

		/// <summary>
		/// A fallback resolver, used when a resource cannot be resolved by this resolver.
		/// </summary>
		private readonly IResourceResolver _fallbackResolver;

		/// <summary>
		/// Constructs a resource resolver that will look in the specified set of assemblies for resources.
		/// </summary>
		/// <param name="assemblies">The set of assemblies to search.</param>
		public ResourceResolver(Assembly[] assemblies)
			: this(assemblies, null) {}

		/// <summary>
		/// Constructs an object that will search the specified assembly.
		/// </summary>
		/// <param name="assembly">The assembly to search.</param>
		public ResourceResolver(Assembly assembly)
			: this(new[] {assembly}) {}

		/// <summary>
		/// Constructs a resource resolver that will look in the specified assembly for resources.
		/// </summary>
		/// <param name="assembly">The assembly to search.</param>
		/// <param name="fallback">The fallback <see cref="IResourceResolver"/> to use when an object cannot be resolved by this resolver.</param>
		public ResourceResolver(Assembly assembly, IResourceResolver fallback)
			: this(new[] {assembly}, fallback) {}

		/// <summary>
		/// Constructs a resource resolver that will look in the specified set of assemblies for resources.
		/// </summary>
		/// <param name="assemblies">Assemblies covered by this resolver.</param>
		/// <param name="fallback">A fallback resolver, that will be invoked if resources are not found in the specified assemblies.</param>
		public ResourceResolver(Assembly[] assemblies, IResourceResolver fallback)
		{
			_assemblies = assemblies;
			_fallbackResolver = fallback;
		}

		/// <summary>
		/// Constructs a resource resolver that will find resources in the assembly containing the specified type,
		/// and optionally those assemblies containing its base types.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="walkInheritanceChain">If true, assemblies containing base types of the specified type will also be included.</param>
		public ResourceResolver(Type type, bool walkInheritanceChain)
			: this(GetAssembliesForType(type, walkInheritanceChain)) {}

		/// <summary>
		/// Constructs a resource resolver that will find resources in the assembly containing the specified type,
		/// and optionally those assemblies containing its base types.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="walkInheritanceChain">If true, assemblies containing base types of the specified type will also be included.</param>
		/// <param name="fallback"></param>
		public ResourceResolver(Type type, bool walkInheritanceChain, IResourceResolver fallback)
			: this(GetAssembliesForType(type, walkInheritanceChain), fallback) {}

		/// <summary>
		/// Attempts to localize the specified unqualified string resource key
		/// by searching the set of assemblies associated with this <see cref="ResourceResolver"/> in order.
		/// </summary>
		/// <remarks>
		/// Searches the assemblies for resources ending in "SR.resources", and searches those resources
		/// for a string matching the specified key.
		/// </remarks>
		/// <param name="unqualifiedStringKey">The string resource key to search for.  Must not be qualified.</param>
		/// <returns>The localized string, or the argument unchanged if the key could not be found.</returns>
		public string LocalizeString(string unqualifiedStringKey)
		{
			if (string.IsNullOrEmpty(unqualifiedStringKey))
				return unqualifiedStringKey;

			// search the assemblies in order
			foreach (Assembly asm in _assemblies)
			{
				try
				{
					string localized = LocalizeString(unqualifiedStringKey, asm);
					if (localized != null)
						return localized;
				}
				catch (Exception)
				{
					// failed to resolve in the specified assembly
				}
			}

			// try the fallback
			if (_fallbackResolver != null)
				return _fallbackResolver.LocalizeString(unqualifiedStringKey);

			// return the unresolved string if not resolved
			return unqualifiedStringKey;
		}

		/// <summary>
		/// Attempts to resolve a resource name from the specified name, which may be partially
		/// qualified or entirely unqualified.
		/// </summary>
		/// <param name="resourceName">A partially qualified or unqualified resource name.</param>
		/// <param name="resolvedResourceName">The qualified resource name.</param>
		/// <returns>Whether or not a resource was found.</returns>
		public bool TryResolveResource(string resourceName, out string resolvedResourceName)
		{
			Platform.CheckForEmptyString(resourceName, @"resourceName");
			foreach (var asm in _assemblies)
			{
				var result = ResolveResource(resourceName, asm);
				if (result != null)
				{
					resolvedResourceName = result;
					return true;
				}
			}

			// try the fallback
			if (_fallbackResolver != null)
				return _fallbackResolver.TryResolveResource(resourceName, out resolvedResourceName);

			resolvedResourceName = null;
			return false;
		}

		/// <summary>
		/// Attempts to resolve a resource name from the specified name, which may be partially
		/// qualified or entirely unqualified.
		/// </summary>
		/// <param name="resourceName">A partially qualified or unqualified resource name.</param>
		/// <returns>A qualified resource name, if found, otherwise an exception is thrown.</returns>
		/// <exception cref="MissingManifestResourceException">if the resource name could not be resolved.</exception>
		public string ResolveResource(string resourceName)
		{
			string resolvedResourceName;
			if (TryResolveResource(resourceName, out resolvedResourceName))
				return resolvedResourceName;

			throw new MissingManifestResourceException(string.Format(SR.ExceptionResourceNotFound, resourceName));
		}

		/// <summary>
		/// Attempts to resolve and open a resource from the specified name, which may be partially
		/// qualified or entirely unqualified.
		/// </summary>
		/// <param name="resourceName">A partially qualified or unqualified resource name.</param>
		/// <param name="resourceStream">The resource as a <see cref="Stream"/>.</param>
		/// <returns>True, if a resource is found, otherwise False.</returns>
		public bool TryOpenResource(string resourceName, out Stream resourceStream)
		{
			Platform.CheckForEmptyString(resourceName, @"resourceName");
			foreach (var asm in _assemblies)
			{
				var result = ResolveResource(resourceName, asm);
				if (result != null)
				{
					resourceStream = OpenResource(result, asm);
					return true;
				}
			}

			// try the fallback
			if (_fallbackResolver != null)
				return _fallbackResolver.TryOpenResource(resourceName, out resourceStream);

			resourceStream = null;
			return false;
		}

		/// <summary>
		/// Attempts to resolve and open a resource from the specified name, which may be partially
		/// qualified or entirely unqualified.
		/// </summary>
		/// <param name="resourceName">A partially qualified or unqualified resource name.</param>
		/// <returns>The resource as a <see cref="Stream"/>.</returns>
		/// <exception cref="MissingManifestResourceException">if the resource name could not be resolved.</exception>
		public Stream OpenResource(string resourceName)
		{
			Stream resourceStream;
			if (TryOpenResource(resourceName, out resourceStream))
				return resourceStream;

			throw new MissingManifestResourceException(string.Format(SR.ExceptionResourceNotFound, resourceName));
		}

		/// <summary>
		/// Returns the set of resources whose name matches the specified regular expression.
		/// </summary>
		/// <param name="regex">A regular expression to be used to select matching resources.</param>
		/// <returns>An array of fully qualified resource names that match the specified regular expression.</returns>
		public string[] FindResources(Regex regex)
		{
			Platform.CheckForNullReference(regex, @"regex");

			var matches = new List<string>();
			foreach (var asm in _assemblies)
				matches.AddRange(CollectionUtils.Select(asm.GetManifestResourceNames(), result => regex.Match(result).Success));

			// include the fallback
			if (_fallbackResolver != null)
				matches.AddRange(_fallbackResolver.FindResources(regex));

			return CollectionUtils.Unique(matches).ToArray();
		}

		/// <summary>
		/// Attempts to resolve a fully qualified resource name from the specified name, which may be partially qualified or entirely unqualified.
		/// </summary>
		/// <param name="resourceName">A partially qualified or unqualified resource name.</param>
		/// <param name="assembly">The assembly in which the resource is to be resolved.</param>
		/// <returns>A qualified resource name, if found, otherwise an exception is thrown.</returns>
		protected virtual string ResolveResource(string resourceName, Assembly assembly)
		{
			//resources are qualified internally in the manifest with '.' characters, so let's first try
			//to find a resource that matches 'exactly' by preceding the name with a '.'
			var exactMatch = String.Format(".{0}", resourceName);
			foreach (var match in GetResourcesEndingWith(assembly, exactMatch))
				return match;

			//next we'll just try to find the first match ending with the resource name.
			foreach (var match in GetResourcesEndingWith(assembly, resourceName))
				return match;

			return null;
		}

		/// <summary>
		/// Attempts to open the resource with the specified fully-qualified name.
		/// </summary>
		/// <param name="resourceFullName">A partially qualified or unqualified resource name.</param>
		/// <param name="assembly">The assembly from which the resource is to be opened.</param>
		/// <returns>The loaded resource stream.</returns>
		protected virtual Stream OpenResource(string resourceFullName, Assembly assembly)
		{
			// Assembly type is thread-safe, so this call is ok
			return assembly.GetManifestResourceStream(resourceFullName);
		}

		/// <summary>
		/// Attempts to localize the specified string table key from the specified assembly, checking all
		/// string resource files in arbitrary order.
		/// </summary>
		/// <remarks>
		/// The first match is returned, or null if no matches are found.
		/// </remarks>
		/// <param name="stringTableKey">The string table key to localize.</param>
		/// <param name="assembly">The assembly to look in.</param>
		/// <returns>The first string table entry that matches the specified key, or null if no matches are found.</returns>
		protected virtual string LocalizeString(string stringTableKey, Assembly assembly)
		{
			foreach (var resourceManager in GetStringResourceManagers(assembly))
			{
				// resource managers are thread-safe (according to MSDN)
				var resolved = resourceManager.GetString(stringTableKey);
				if (resolved != null)
					return resolved;
			}
			return null;
		}

		/// <summary>
		/// Returns a list of <see cref="ResourceManager"/>s, one for each string resource file that is present
		/// in the specified assembly.
		/// </summary>
		/// <remarks>
		/// The returned <see cref="ResourceManager"/>s can be used to localize strings.
		/// </remarks>
		private static List<ResourceManager> GetStringResourceManagers(Assembly asm)
		{
			List<ResourceManager> resourceManagers;

			// look for a cached copy
			lock (_mapStringResourceManagers)
			{
				//List<ResourceManager> resourceManagers;
				if (_mapStringResourceManagers.TryGetValue(asm, out resourceManagers))
					return resourceManagers;
			}

			// no cached copy, so create
			resourceManagers = new List<ResourceManager>();
			foreach (string stringResource in GetResourcesEndingWith(asm, "SR.resources"))
			{
				resourceManagers.Add(new ResourceManager(stringResource.Replace(".resources", ""), asm));
			}

			// update the cache
			lock (_mapStringResourceManagers)
			{
				// note: another thread may have written to the cache in the interim, but it really doesn't matter
				// we can just overwrite it
				_mapStringResourceManagers[asm] = resourceManagers;
				return resourceManagers;
			}
		}

		/// <summary>
		/// Searches the specified assembly for resource files whose names end with the specified string.
		/// </summary>
		/// <param name="asm">The assembly to search.</param>
		/// <param name="endingWith">The string to match the end of the resource name with.</param>
		private static string[] GetResourcesEndingWith(Assembly asm, string endingWith)
		{
			List<string> stringResources = new List<string>();

			// Assembly type is thread-safe, so this call is ok
			foreach (string resName in asm.GetManifestResourceNames())
			{
				if (resName.EndsWith(endingWith))
					stringResources.Add(resName);
			}
			return stringResources.ToArray();
		}

		/// <summary>
		/// Returns the set of assemblies containing the specified type and all of its base types.
		/// </summary>
		private static Assembly[] GetAssembliesForType(Type type, bool walkInheritanceChain)
		{
			List<Assembly> assemblies = new List<Assembly>();
			while (type != typeof (object))
			{
				assemblies.Add(type.Assembly);
				if (!walkInheritanceChain)
					break;
				type = type.BaseType;
			}
			return CollectionUtils.Unique(assemblies).ToArray();
		}
	}
}
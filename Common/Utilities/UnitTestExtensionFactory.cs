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

#if UNIT_TESTS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ClearCanvas.Common.Utilities
{
	//TODO (CR Sept 2010): move to Tests namespace?

	/// <summary>
	/// An <see cref="IExtensionFactory"/> that returns only extensions that have been explicitly mapped.
	/// </summary>
	/// <remarks>
	/// This <see cref="IExtensionFactory"/> is useful in unit test scenarios where precise control over
	/// creation of extensions is required. Simply create an instance of this class and map individual
	/// extension types to <see cref="IExtensionPoint"/> types.
	/// </remarks>
	public class UnitTestExtensionFactory : IExtensionFactory, IDictionary<Type, Type>
	{
		private readonly Dictionary<Type, List<ExtensionDescriptor>> _extensionMap = new Dictionary<Type, List<ExtensionDescriptor>>();
		private const string _exceptionMustImplementInterface = "Extension point class must implement IExtensionPoint";

		/// <summary>
		/// Instantiates an empty <see cref="UnitTestExtensionFactory"/>.
		/// </summary>
		public UnitTestExtensionFactory() {}

		/// <summary>
		/// Instantiates an <see cref="UnitTestExtensionFactory"/> with the provided <paramref name="extensionMap">extensions map</paramref>.
		/// </summary>
		public UnitTestExtensionFactory(IEnumerable<KeyValuePair<Type, IEnumerable<Type>>> extensionMap)
		{
			if (extensionMap != null)
			{
				foreach (var entry in extensionMap)
				{
					foreach (var type in entry.Value)
						Define(entry.Key, type);
				}
			}
		}

		/// <summary>
		/// Instantiates an <see cref="UnitTestExtensionFactory"/> with the provided <paramref name="extensionMap">extensions map</paramref>.
		/// </summary>
		public UnitTestExtensionFactory(IEnumerable<KeyValuePair<Type, Type>> extensionMap)
		{
			if (extensionMap != null)
			{
				foreach (var entry in extensionMap)
					Define(entry.Key, entry.Value);
			}
		}

		/// <summary>
		/// Defines a type as an extension of the specified <see cref="IExtensionPoint"/> type.
		/// </summary>
		/// <param name="extensionPoint">The type of the <see cref="IExtensionPoint"/>.</param>
		/// <param name="extension">The type of the extension.</param>
		public void Define(Type extensionPoint, Type extension)
		{
			if (!typeof (IExtensionPoint).IsAssignableFrom(extensionPoint))
				throw new ArgumentException(_exceptionMustImplementInterface, "extensionPoint");

			if (!_extensionMap.ContainsKey(extensionPoint))
				_extensionMap.Add(extensionPoint, new List<ExtensionDescriptor>());
			_extensionMap[extensionPoint].Add(new ExtensionDescriptor(extensionPoint, extension));
		}

		/// <summary>
		/// Defines a factory method as an extension of the specified <see cref="IExtensionPoint"/> type.
		/// </summary>
		/// <param name="extensionPoint">The type of the <see cref="IExtensionPoint"/>.</param>
		/// <param name="factoryMethod">The factory method to create the extension.</param>
		public void Define<T>(Type extensionPoint, Func<T> factoryMethod)
		{
			if (!typeof (IExtensionPoint).IsAssignableFrom(extensionPoint))
				throw new ArgumentException(_exceptionMustImplementInterface, "extensionPoint");

			if (!_extensionMap.ContainsKey(extensionPoint))
				_extensionMap.Add(extensionPoint, new List<ExtensionDescriptor>());
			_extensionMap[extensionPoint].Add(new ExtensionDescriptor(extensionPoint, typeof (T), () => (object) factoryMethod.Invoke()));
		}

		/// <summary>
		/// Undefines all extensions for the specified <see cref="IExtensionPoint"/> type.
		/// </summary>
		/// <param name="extensionPoint">The type of the <see cref="IExtensionPoint"/>.</param>
		/// <returns>True if any extensions were undefined; False otherwise.</returns>
		public bool UndefineAll(Type extensionPoint)
		{
			if (!typeof (IExtensionPoint).IsAssignableFrom(extensionPoint))
				throw new ArgumentException(_exceptionMustImplementInterface, "extensionPoint");
			return _extensionMap.Remove(extensionPoint);
		}

		/// <summary>
		/// Checks if there are any extensions defined for the specified <see cref="IExtensionPoint"/> type.
		/// </summary>
		/// <param name="extensionPoint">The type of the <see cref="IExtensionPoint"/>.</param>
		/// <returns>True if extensions are undefined; False otherwise.</returns>
		public bool HasExtensions(Type extensionPoint)
		{
			if (!typeof (IExtensionPoint).IsAssignableFrom(extensionPoint))
				throw new ArgumentException(_exceptionMustImplementInterface, "extensionPoint");
			return _extensionMap.ContainsKey(extensionPoint);
		}

		/// <summary>
		/// Gets a list of <see cref="IExtensionPoint"/> types for which extensions have been defined.
		/// </summary>
		public ICollection<Type> ExtensionPoints
		{
			get { return _extensionMap.Keys; }
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
		public virtual object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
		{
			var extensions = ListExtensionsCore(extensionPoint, filter);
			if (justOne) extensions = extensions.Take(1);
			return extensions.Select(x => x.CreateInstance()).ToArray();
		}

		/// <summary>
		/// Gets metadata describing all extensions of the input <paramref name="extensionPoint"/>, 
		/// matching the given <paramref name="filter"/>.
		/// </summary>
		/// <param name="extensionPoint">The <see cref="ExtensionPoint"/> whose extension metadata is to be retrieved.</param>
		/// <param name="filter">An <see cref="ExtensionFilter"/> used to filter out extensions with particular characteristics.</param>
		/// <returns></returns>
		public virtual ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
		{
			return ListExtensionsCore(extensionPoint, filter).Select(x => x.Info).ToArray();
		}

		private IEnumerable<ExtensionDescriptor> ListExtensionsCore(ExtensionPoint extensionPoint, ExtensionFilter filter)
		{
			if (extensionPoint == null)
				throw new ArgumentNullException("extensionPoint");

			var extensionPointType = extensionPoint.GetType();
			if (!_extensionMap.ContainsKey(extensionPointType))
				return Enumerable.Empty<ExtensionDescriptor>();

			var extensions = _extensionMap[extensionPointType].Select(x => x);
			if (filter != null) extensions = extensions.Where(x => filter.Test(x.Info));
			return extensions;
		}

		#endregion

		#region IDictionary<Type, Type> Members

		/// <summary>
		/// See <see cref="Define"/>.
		/// </summary>
		/// <remarks>
		/// This is separately declared here to support collection initializer syntax.
		/// </remarks>
		public void Add(Type key, Type value)
		{
			Define(key, value);
		}

		bool IDictionary<Type, Type>.ContainsKey(Type key)
		{
			return HasExtensions(key);
		}

		ICollection<Type> IDictionary<Type, Type>.Keys
		{
			get { return ExtensionPoints; }
		}

		bool IDictionary<Type, Type>.Remove(Type key)
		{
			return UndefineAll(key);
		}

		bool IDictionary<Type, Type>.TryGetValue(Type key, out Type value)
		{
			List<ExtensionDescriptor> list;
			bool result = _extensionMap.TryGetValue(key, out list);
			if (result)
			{
				value = list.Select(x => x.Type).FirstOrDefault();
				return value != null;
			}
			value = null;
			return false;
		}

		ICollection<Type> IDictionary<Type, Type>.Values
		{
			get { return _extensionMap.Values.SelectMany(x => x).Select(x => x.Type).ToList().AsReadOnly(); }
		}

		Type IDictionary<Type, Type>.this[Type key]
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		#endregion

		#region ICollection<KeyValuePair<Type,Type>> Members

		void ICollection<KeyValuePair<Type, Type>>.Add(KeyValuePair<Type, Type> item)
		{
			Define(item.Key, item.Value);
		}

		public void Clear()
		{
			_extensionMap.Clear();
		}

		bool ICollection<KeyValuePair<Type, Type>>.Contains(KeyValuePair<Type, Type> item)
		{
			return _extensionMap.ContainsKey(item.Key);
		}

		void ICollection<KeyValuePair<Type, Type>>.CopyTo(KeyValuePair<Type, Type>[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		int ICollection<KeyValuePair<Type, Type>>.Count
		{
			get { return ((IDictionary<Type, Type>) this).Values.Count; }
		}

		bool ICollection<KeyValuePair<Type, Type>>.IsReadOnly
		{
			get { return false; }
		}

		bool ICollection<KeyValuePair<Type, Type>>.Remove(KeyValuePair<Type, Type> item)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region IEnumerable<KeyValuePair<Type,Type>> Members

		public IEnumerator<KeyValuePair<Type, Type>> GetEnumerator()
		{
			return _extensionMap.SelectMany(x => x.Value.Select(y => new KeyValuePair<Type, Type>(x.Key, y.Type))).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region ExtensionDescriptor Class

		private class ExtensionDescriptor
		{
			private readonly Type _extensionType;
			private readonly Func<object> _creator;
			private readonly ExtensionInfo _info;

			public ExtensionDescriptor(Type extensionPoint, Type extension, Func<object> creator = null)
			{
				_info = new ExtensionInfo(extension, extensionPoint, extension.Name, extension.AssemblyQualifiedName, true);
				_extensionType = extension;
				_creator = creator;
			}

			public Type Type
			{
				get { return _extensionType; }
			}

			public ExtensionInfo Info
			{
				get { return _info; }
			}

			public object CreateInstance()
			{
				return _creator != null ? _creator.Invoke() : Activator.CreateInstance(_extensionType);
			}
		}

		#endregion
	}
}

#endif
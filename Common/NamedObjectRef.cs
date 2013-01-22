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
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Base class for <see cref="TypeRef"/> and <see cref="AssemblyRef"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public abstract class NamedObjectRef<T>: ISerializable
		where T: class
	{
		private static readonly Dictionary<string, T> _resolutionCache = new Dictionary<string, T>();
		private readonly string _name;
		private T _obj;

		protected NamedObjectRef(string name, T obj)
		{
			_name = name;
			_obj = obj;
		}

		protected NamedObjectRef(string name)
		{
			_name = name;
		}

		protected NamedObjectRef(SerializationInfo info, StreamingContext context)
		{
			_name = (string)info.GetValue("name", typeof(string));
		}

		public override string ToString()
		{
			return _name;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("name", _name, typeof(string));
		}

		/// <summary>
		/// Resolve the reference and returns the referenced object.
		/// </summary>
		/// <returns></returns>
		public T Resolve()
		{
			if (_obj != null)
				return _obj;

			lock (_resolutionCache)
			{
				if (_resolutionCache.TryGetValue(_name, out _obj))
					return _obj;

				_obj = ResolveObject(_name);
				_resolutionCache.Add(_name, _obj);
				return _obj;
			}
		}

		protected bool EqualsHelper(NamedObjectRef<T> other)
		{
			if (ReferenceEquals(this, other)) return true;
			if (ReferenceEquals(null, other)) return false;

			return _name == other._name;
		}

		protected bool EqualsHelper(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;

			var other = obj as NamedObjectRef<T>;
			if (ReferenceEquals(null, other)) return false;

			return _name == other._name;
		}

		protected int HashCodeHelper()
		{
			return _name.GetHashCode();
		}

		protected string Name
		{
			get { return _name; }
		}

		protected abstract T ResolveObject(string name);
	}

	/// <summary>
	/// Represents a reference to a .NET type.
	/// </summary>
	[Serializable]
	public sealed class TypeRef : NamedObjectRef<Type>, IEquatable<TypeRef>
	{
		private string _typeFullName;

		public TypeRef(Type type)
			: base(GetAssemblyQualifiedName(type), type)
		{
		}

		public TypeRef(string typeName)
			:base(typeName)
		{
		}

		private TypeRef(SerializationInfo info, StreamingContext context)
			:base(info, context)
		{
		}

		/// <summary>
		/// Gets the full name of the referenced type.
		/// </summary>
		public string FullName
		{
			get { return _typeFullName ?? (_typeFullName = this.Name.Split(',').First()); }
		}

		/// <summary>
		/// Converts a <see cref="Type"/> to a <see cref="TypeRef"/>.
		/// </summary>
		public static implicit operator TypeRef(Type type)
		{
			return new TypeRef(type);
		}

		public override bool Equals(object obj)
		{
			return EqualsHelper(obj);
		}

		public override int GetHashCode()
		{
			return HashCodeHelper();
		}

		public bool Equals(TypeRef other)
		{
			return EqualsHelper(other);
		}

		public static bool operator ==(TypeRef left, TypeRef right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(TypeRef left, TypeRef right)
		{
			return !Equals(left, right);
		}

		protected override Type ResolveObject(string name)
		{
			return Type.GetType(name, true);
		}

		private static string GetAssemblyQualifiedName(Type type)
		{
			return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
		}
	}

	[Serializable]
	public class AssemblyRef : NamedObjectRef<Assembly>, IEquatable<AssemblyRef>
	{
		private Func<string, Assembly> _resolver;

		public AssemblyRef(Assembly assembly)
			: base(assembly.GetName().Name, assembly)
		{
		}

		protected AssemblyRef(SerializationInfo info, StreamingContext context)
			:base(info, context)
		{
		}

		public override bool Equals(object obj)
		{
			return EqualsHelper(obj);
		}

		public override int GetHashCode()
		{
			return HashCodeHelper();
		}

		public bool Equals(AssemblyRef other)
		{
			return EqualsHelper(other);
		}

		public static bool operator ==(AssemblyRef left, AssemblyRef right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(AssemblyRef left, AssemblyRef right)
		{
			return !Equals(left, right);
		}

		protected override Assembly ResolveObject(string name)
		{
			return _resolver(name);
		}

		internal void SetResolver(Func<string, Assembly> resolver)
		{
			_resolver = resolver;
		}
	}

}

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
using System.Reflection;
using System.Collections.Generic;
using ClearCanvas.Common;
using System.Diagnostics;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Constructed and used by the <see cref="CloneBuilder"/>; allows
	/// objects being cloned to have control over when their fields
	/// are cloned.
	/// </summary>
	public interface ICloningContext
	{
		/// <summary>
		/// Clones <paramref name="source"/>'s fields and populates
		/// the corresponding fields in <paramref name="destination"/>.
		/// </summary>
		/// <remarks>
		///	The generic type parameter tells the <see cref="CloneBuilder"/>
		/// what type's fields are to be cloned.  It is a <b>contract breakage</b>
		/// to specify a type other than the one currently making the call
		/// to <see cref="CloneFields{T}"/>.
		/// </remarks>
		/// <typeparam name="T">The type for which fields are to be cloned.</typeparam>
		/// <param name="source">The source object.</param>
		/// <param name="destination">The destination object (or clone).</param>
		void CloneFields<T>(T source, T destination) where T : class;
	}

	#region CloneCreatedEventArgs

	/// <summary>
	/// Can be used to notify listeners of objects being cloned.
	/// </summary>
	/// <remarks>
	/// No events are fired by the <see cref="CloneBuilder"/>; this
	/// class exists for convenience so cloneable objects can notify
	/// listeners when they are cloned.
	/// </remarks>
	public class CloneCreatedEventArgs : EventArgs
	{
		private readonly object _source;
		private readonly object _clone;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CloneCreatedEventArgs(object source, object clone)
		{
			Platform.CheckForNullReference(source, "sourceObject");
			Platform.CheckForNullReference(clone, "clonedObject");

			_source = source;
			_clone = clone;
		}

		/// <summary>
		/// The object that has been cloned.
		/// </summary>
		public object Source
		{
			get { return _source; }
		}

		/// <summary>
		/// The clone.
		/// </summary>
		public object Clone
		{
			get { return _clone; }
		}
	}

	#endregion

	#region Attributes

	/// <summary>
	/// Decorates a class as being cloneable.
	/// </summary>
	/// <remarks>
	/// When decorated with this attribute, a class must:
	/// <list type="bullet">
	/// <item>
	/// Have a default constructor (can be private, protected or public) with an optional 
	/// <see cref="CloneInitializeAttribute"/>-decorated method.
	/// <see cref="CloneableAttribute.UseDefaultConstructor"/> 
	/// must be true, or an exception will be thrown.
	/// </item>
	/// <item>
	///	<para>
	/// Have a cloning constructor, which has the following signature:
	/// </para>
	/// <para>
	/// - ClassType(ClassType source, ICloningContext context)
	/// </para>
	///	<para>
	/// The cloning constructor should have protected or private access and must call
	/// the base class' cloning constructor if one exists, <b>or none of the base classes
	/// with cloning constructors will be cloned correctly</b>.
	/// </para>
	/// </item>
	/// </list>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class CloneableAttribute : Attribute
	{
		/// <summary>
		/// Specifies that the class' default constructor should be used when cloning.
		/// </summary>
		public readonly bool UseDefaultConstructor;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks>
		/// Use of this constructor implies that the class being cloned has a cloning constructor.
		/// See <see cref="CloneableAttribute"/> for more details.
		/// </remarks>
		public CloneableAttribute()
		{
			this.UseDefaultConstructor = false;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <remarks>
		/// See <see cref="CloneableAttribute"/> for more details.
		/// </remarks>
		public CloneableAttribute(bool useDefaultConstructor)
		{
			this.UseDefaultConstructor = useDefaultConstructor;
		}
	}

	/// <summary>
	/// Decorates a field belonging to a cloneable class; the field will not be cloned.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class CloneIgnoreAttribute : Attribute
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public CloneIgnoreAttribute(){ }
	}

	/// <summary>
	/// Decorates a field belonging to a cloneable class; for reference types,
	/// the object reference will be copied rather than a clone.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class CloneCopyReferenceAttribute : Attribute
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public CloneCopyReferenceAttribute(){ }
	}

	/// <summary>
	/// Decorates the clone-initialization method of a cloneable class;  the
	/// method will only be called when <see cref="CloneableAttribute.UseDefaultConstructor"/>
	/// is true, otherwise a cloning constructor must exist.
	/// </summary>
	/// <remarks>
	/// <para>
	///	The clone-initalization method must have the same signature as the cloning
	/// constructor, with a void return type.  See <see cref="CloneableAttribute"/> for more details
	/// on the cloning constructor's signature.
	/// </para>
	/// <para>
	/// The method should have private access.
	/// </para>
	/// <para>
	/// The clone-initialization method is called for each class in the hierarchy beginning
	/// with the lowest level cloneable class, and ending with the highest.
	/// </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class CloneInitializeAttribute : Attribute
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public CloneInitializeAttribute(){ }
	}
	
	/// <summary>
	/// Decorates the clone-complete method of a cloneable class; the method will
	/// be called after construction (or initialization) for each cloneable class
	/// in the hierarchy.
	/// </summary>
	/// <remarks>
	/// <para>
	///	The clone-complete method must have a void signature (no argument, no return value).
	/// The method should have private access.
	/// </para>
	/// <para>
	/// the clone-complete method is called for each class in the hierarchy
	/// beginning with the lowest level cloneable class, and ending with the highest.
	/// </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class OnCloneCompleteAttribute : Attribute
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public OnCloneCompleteAttribute() { }
	}

	#endregion

	#region Exceptions

	/// <summary>
	/// Exception thrown by the <see cref="CloneBuilder"/>.
	/// </summary>
	public sealed class CloningException : Exception
	{
		internal CloningException(string message)
			:base(message)
		{
		}

		internal CloningException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	#endregion

	/// <summary>
	/// A static helper class that builds a clone of a class decorated
	/// with the <see cref="CloneableAttribute"/> attribute.
	/// </summary>
	public static class CloneBuilder
	{
		private class CloningContext : ICloningContext
		{
			#region ICloningContext Members

			public void CloneFields<T>(T source, T destination) where T : class
			{
				CloneBuilder.CloneFields(source, destination, typeof(T));
			}

			#endregion
		}

		private static readonly CloningContext _context = new CloningContext();

		/// <summary>
		/// Creates a clone of the <paramref name="source"/> object.
		/// </summary>
		/// <remarks>
		/// Returns null if the class is not cloneable.
		/// </remarks>
		public static object Clone(object source)
		{
			try
			{
				return CloneInternal(source);
			}
			catch(CloningException)
			{
				throw;
			}
			catch(Exception e)
			{
				throw new CloningException(
					"An unexpected exception has occurred; see the InnerException for more details.", e);
			}
		}

		private static object CloneInternal(object source)
		{
			Platform.CheckForNullReference(source, "source");

			object[] attributes = source.GetType().GetCustomAttributes(typeof (CloneableAttribute), false);
			if (attributes.Length == 0)
				return null;

			object clone;
			CloneableAttribute attribute = (CloneableAttribute) attributes[0];
			if (attribute.UseDefaultConstructor)
			{
				ConstructorInfo info = source.GetType().GetConstructor(
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
					null, Type.EmptyTypes, null);

				if (info == null)
				{
					throw new CloningException(
						"No default constructor found; the input object must have a default constructor" +
						" with the appropriate reflection permission.");
				}

				try
				{
					clone = info.Invoke(null);
				}
				catch(Exception e)
				{
					throw new CloningException(
						"An error occurred while executing the default constructor;" +
						" insure the class has the appropriate reflection permissions.", e);
				}
			}
			else
			{
				ConstructorInfo info = source.GetType().GetConstructor(
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
					null, new Type[] {source.GetType(), typeof (ICloningContext)}, null);
				if (info == null)
				{
					throw new CloningException(
						"The input object must have a constructor with the correct cloning signature and " +
						"reflection permission.  See the class documentation for more details.");
				}

				try
				{
					clone = info.Invoke(new object[] {source, _context});
				}
				catch(Exception e)
				{
					throw new CloningException(
						"An error occurred while executing the cloning constructor; " +
						"insure the class has the appropriate reflection permissions.", e);
				}
			}

			Platform.CheckForNullReference(clone, "clone");
			if (clone.GetType() != source.GetType())
			{
				throw new CloningException(
					string.Format("The cloned object is not the same type as the source object ({0} != {1}).",
				    clone.GetType().FullName, source.GetType().FullName));
			}

			//Trace.WriteLine(String.Format("CloneCreated: {0}", clone.GetType().FullName));

			foreach (Type type in WalkCloneHierarchy(clone.GetType()))
			{
				InitializeClone(source, clone, type);
				//Trace.WriteLine(String.Format("InitializeClone: {0}", type.FullName));
			}

			return clone;
		}

		//returns all the cloneable types in reverse order.
		private static IEnumerable<Type> WalkCloneHierarchy(Type type)
		{
			if (type.IsClass)
			{
				if (type.IsDefined(typeof(CloneableAttribute), false))
				{
					foreach (Type baseType in WalkCloneHierarchy(type.BaseType))
						yield return baseType;

					yield return type;
				}
			}
		}

		private static void InitializeClone(object source, object clone, Type type)
		{
			CloneableAttribute attribute =
				(CloneableAttribute) type.GetCustomAttributes(typeof (CloneableAttribute), false)[0];

			MethodInfo[] methods = type.GetMethods(BindingFlags.Public |BindingFlags.NonPublic | BindingFlags.Instance);
			if (attribute.UseDefaultConstructor)
			{
				bool foundMethod = false;
				foreach (MethodInfo method in methods)
				{
					if (method.IsDefined(typeof (CloneInitializeAttribute), false))
					{
						method.Invoke(clone, new object[]{ source, _context});
						foundMethod = true;
						break;						
					}
				}

				if (!foundMethod)
				{
					// if there was no CloneInitialize method, we assume we are just to clone the fields.
					CloneFields(source, clone, type);
				}
			}

			foreach (MethodInfo method in methods)
			{
				if (method.IsDefined(typeof(OnCloneCompleteAttribute), false))
				{
					method.Invoke(clone, Type.EmptyTypes);
					break;
				}
			}
		}

		private static void CloneFields(object source, object clone, Type type)
		{
			FieldInfo[] info = type.GetFields(BindingFlags.Public | 
												BindingFlags.NonPublic | 
												BindingFlags.Instance | 
												BindingFlags.DeclaredOnly);

			foreach (FieldInfo field in info)
			{
				if (field.IsDefined(typeof(CloneIgnoreAttribute), false))
					continue;

				// treat strings as value types
				if (field.FieldType.IsValueType || field.FieldType == typeof(string))
				{
					//set all value types to be the same.
					field.SetValue(clone, field.GetValue(source));
				}
				else //class or interface
				{
					if (!field.IsDefined(typeof(CloneCopyReferenceAttribute), false))
					{
						object fieldValue = field.GetValue(source);
						if (fieldValue != null)
						{
							if (fieldValue.GetType().IsDefined(typeof (CloneableAttribute), false))
								field.SetValue(clone, Clone(fieldValue));
							else
								field.SetValue(clone, null);
						}
						else
							field.SetValue(clone, null);
					}
					else
					{
						//copy the reference.
						field.SetValue(clone, field.GetValue(source));
					}
				}
			}
		}
	}
}
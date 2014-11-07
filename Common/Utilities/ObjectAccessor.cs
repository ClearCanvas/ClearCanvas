using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Provides high-performance dynamic access to object properties.
	/// </summary>
	/// <remarks>
	/// Both static and instance members of this class are safe for concurrent use by multiple threads.
	/// </remarks>
	public class ObjectAccessor
	{
		private static readonly ConcurrentDictionary<Type, ObjectAccessor> _typeAccessors = new ConcurrentDictionary<Type, ObjectAccessor>();

		/// <summary>
		/// Gets a cached instance of <see cref="ObjectAccessor"/> for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static ObjectAccessor For(Type type)
		{
			return _typeAccessors.GetOrAdd(type, t => new ObjectAccessor(t));
		}


		private readonly Type _type;
		private readonly Dictionary<string, PropertyAccessor> _propertyAccessors = new Dictionary<string, PropertyAccessor>();

		private ObjectAccessor(Type type)
		{
			_type = type;
		}

		/// <summary>
		/// Gets the <see cref="PropertyAccessor"/> for the specified property, or null if the property cannot be found.
		/// </summary>
		/// <param name="name">The name of the property to get.</param>
		/// <param name="includeNonPublic">True to include non-public properties in the search.</param>
		/// <returns></returns>
		[CanBeNull]
		public PropertyAccessor GetProperty(string name, bool includeNonPublic = false)
		{
			lock (_propertyAccessors)
			{
				PropertyAccessor result;
				if (_propertyAccessors.TryGetValue(name, out result))
					return result;
			}

			// do the expensive stuff outside of the lock
			var bindingFlags = BindingFlags.Instance | BindingFlags.Public;
			if (includeNonPublic)
				bindingFlags |= BindingFlags.NonPublic;

			var propertyInfo = _type.GetProperty(name, bindingFlags);
			if (propertyInfo == null)
				return null;
			
			var accessor = PropertyAccessor.Create(_type, propertyInfo);
			lock (_propertyAccessors)
			{
				// did another thread create the same accessor while we did?
				// if so, return that one (so that we're always returning the same instance)
				PropertyAccessor result;
				if (_propertyAccessors.TryGetValue(name, out result))
					return result;

				_propertyAccessors.Add(name, accessor);
				return accessor;
			}
		}
	}

	/// <summary>
	/// Provides high-performance dynamic access to a property.
	/// </summary>
	/// <remarks>
	/// Both static and instance members of this class are safe for concurrent use by multiple threads.
	/// </remarks>
	public abstract class PropertyAccessor
	{
		internal static PropertyAccessor Create(Type type, PropertyInfo propertyInfo)
		{
			var genericType = typeof(PropertyAccessor<,>).MakeGenericType(type, propertyInfo.PropertyType);
			return (PropertyAccessor)Activator.CreateInstance(genericType, BindingFlags.NonPublic|BindingFlags.Instance, null, new object[]{type, propertyInfo}, null);
		}

		/// <summary>
		/// Gets the <see cref="PropertyInfo"/> for the property associated with this accessor.
		/// </summary>
		public abstract PropertyInfo Property { get; }

		/// <summary>
		/// Gets a value indicating whether the property is readable (has a getter).
		/// </summary>
		public abstract bool IsReadable { get; }

		/// <summary>
		/// Gets a value indicating whether the property is writable (has a setter).
		/// </summary>
		public abstract bool IsWritable { get; }

		/// <summary>
		/// Gets the value of the property for the specified object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public abstract object GetValue(object obj);

		/// <summary>
		/// Sets the value of the property on the the specified object.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="value"></param>
		public abstract void SetValue(object obj, object value);
	}

	internal class PropertyAccessor<TObject, TProperty> : PropertyAccessor
	{
		private readonly Type _type;
		private readonly PropertyInfo _property;
		private readonly Func<TObject, TProperty> _getter;
		private readonly Action<TObject, TProperty> _setter;

		private PropertyAccessor(Type type, PropertyInfo propertyInfo)
		{
			_type = type;
			_property = propertyInfo;

			// init everything up front so that we're immutable and therefore thread safe
			var getMethod = propertyInfo.GetGetMethod(true);
			if (getMethod != null)
			{
				_getter = (Func<TObject, TProperty>)Delegate.CreateDelegate(typeof(Func<TObject, TProperty>), getMethod);
			}

			var setMethod = propertyInfo.GetSetMethod(true);
			if (setMethod != null)
			{
				_setter = (Action<TObject, TProperty>)Delegate.CreateDelegate(typeof(Action<TObject, TProperty>), setMethod);
			}
		}

		public override PropertyInfo Property
		{
			get { return _property; }
		}

		public override bool IsReadable
		{
			get { return _getter != null; }
		}

		public override bool IsWritable
		{
			get { return _setter != null; }
		}

		public override object GetValue(object obj)
		{
			if (_getter == null)
				throw new InvalidOperationException(string.Format("Property {0}.{1} does not have a getter.", _type.Name, _property.Name));
			return _getter((TObject)obj);
		}

		public override void SetValue(object obj, object value)
		{
			if (_setter == null)
				throw new InvalidOperationException(string.Format("Property {0}.{1} does not have a setter.", _type.Name, _property.Name));
			_setter((TObject)obj, (TProperty)value);
		}
	}
}

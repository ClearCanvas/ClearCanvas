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

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Delegate used to get a property of type <typeparamref name="T"/>.
	/// </summary>
	internal delegate T PropertyGetDelegate<T>();

	/// <summary>
	/// Delegate used to set a property of type <typeparamref name="T"/>.
	/// </summary>
	internal delegate void PropertySetDelegate<T>(T val);

	/// <summary>
	/// Provides a dynamic implementation of <see cref="IObservablePropertyBinding{T}" />.
	/// </summary>
	/// <typeparam name="T">The type of the underlying property.</typeparam>
	/// <remarks>
	/// This class can be used for runtime binding to an arbitrary property and corresponding change
	/// notification event of a target object.  Binding is accomplished using reflection, which means
	/// that compile-time knowledge of the type of the target object is not needed.
	/// </remarks>
	internal class DynamicObservablePropertyBinding<T> : IObservablePropertyBinding<T>
	{
		private PropertyGetDelegate<T> _getDelegate;
		private PropertySetDelegate<T> _setDelegate;

		private object _target;
		private string _propertyName;
		private string _propertyChangedEventName;

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="target">The target object to bind to.</param>
		/// <param name="propertyName">The name of the property to bind to on the target object.</param>
		/// <param name="propertyChangedEventName">The name of a change notification event on the target object.</param>
		public DynamicObservablePropertyBinding(object target, string propertyName, string propertyChangedEventName)
		{
			_target = target;
			_propertyName = propertyName;
			_propertyChangedEventName = propertyChangedEventName;
		}
        
		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public T PropertyValue
		{
			get
			{
				if (_getDelegate == null)
				{
					_getDelegate = CreatePropertyGetDelegate(_target, _propertyName);
				}
				return _getDelegate();
			}
			set
			{
				if (_setDelegate == null)
				{
					_setDelegate = CreatePropertySetDelegate(_target, _propertyName);
				}
				_setDelegate(value);
			}
		}

		/// <summary>
		/// Event that is fired when the <see cref="PropertyValue"/> has changed.
		/// </summary>
		public event EventHandler PropertyChanged
		{
			add
			{
				if (string.IsNullOrEmpty(_propertyChangedEventName))
					return;

				AddEventHandler(value, _target, _propertyChangedEventName);
			}
			remove
			{
				if (string.IsNullOrEmpty(_propertyChangedEventName))
					return;

				RemoveEventHandler(value, _target, _propertyChangedEventName);
			}
		}

		/// <summary>
		/// Creates a delegate that can get the specified property on the specified target object.
		/// </summary>
		/// <param name="target">The target object.</param>
		/// <param name="propertyName">The name of the property to bind.</param>
		/// <returns>A delegate that can be used to get the property.</returns>
		private static PropertyGetDelegate<T> CreatePropertyGetDelegate(object target, string propertyName)
		{
			PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);
			MethodInfo propertyGetter = propertyInfo.GetGetMethod();

			return (PropertyGetDelegate<T>)Delegate.CreateDelegate(typeof(PropertyGetDelegate<T>), target, propertyGetter);
		}

		/// <summary>
		/// Creates a delegate that can set the specified property on the specified target object.
		/// </summary>
		/// <param name="target">The target object.</param>
		/// <param name="propertyName">The name of the property to bind.</param>
		/// <returns>A delegate that can be used to set the property.</returns>
		private static PropertySetDelegate<T> CreatePropertySetDelegate(object target, string propertyName)
		{
			PropertyInfo propertyInfo = target.GetType().GetProperty(propertyName);
			MethodInfo propertySetter = propertyInfo.GetSetMethod();

			return (PropertySetDelegate<T>)Delegate.CreateDelegate(typeof(PropertySetDelegate<T>), target, propertySetter.Name);
		}

		/// <summary>
		/// Adds the specified handler to the specified event on the target object.
		/// </summary>
		/// <param name="handler">An event handler.</param>
		/// <param name="target">The target object.</param>
		/// <param name="eventName">The name of the event to add the handler to.</param>
		private static void AddEventHandler(EventHandler handler, object target, string eventName)
		{
			EventInfo eventInfo = target.GetType().GetEvent(eventName);
			eventInfo.AddEventHandler(target, handler);
		}

		/// <summary>
		/// Removes the specified handler from the specified event on the target object.
		/// </summary>
		/// <param name="handler">An event handler.</param>
		/// <param name="target">The target object.</param>
		/// <param name="eventName">The name of the event to remove the handler from.</param>
		private static void RemoveEventHandler(EventHandler handler, object target, string eventName)
		{
			EventInfo eventInfo = target.GetType().GetEvent(eventName);
			eventInfo.RemoveEventHandler(target, handler);
		}
	}
}
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
using System.ComponentModel;
using System.Reflection;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// This attribute class is used to decorate properties of other classes for use with the <see cref="SimpleSerializer"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class SimpleSerializedAttribute : Attribute
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public SimpleSerializedAttribute()
		{
		}
	}

	/// <summary>
	/// Thrown by <see cref="SimpleSerializer"/>'s public methods: <see cref="SimpleSerializer.Deserialize"/> and <see cref="SimpleSerializer.Serialize"/>.
	/// </summary>
	[Serializable]
	public sealed class SimpleSerializerException : Exception
	{
		internal SimpleSerializerException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

	/// <summary>
	/// The <see cref="SimpleSerializer"/> class can be used to serialize objects to and from a string dictionary (Property/Value pairs).
	/// </summary>
	/// <remarks>
	/// The resulting dictionary can be stored to a file or setting so that objects can be easily persisted and restored.
	/// </remarks>
	public sealed class SimpleSerializer
	{
		private SimpleSerializer()
		{
		}

		/// <summary>
		/// Populates the <paramref name="destinationObject"/>'s properties that are decorated with a <see cref="SimpleSerializedAttribute"/> 
		/// attribute using the Property/Value pairs from the input dictionary (<paramref name="sourceValues"/>).
		/// </summary>
		/// <param name="destinationObject">The object whose properties are to be initialized using the input dictionary's Property/Value pairs.</param>
		/// <param name="sourceValues">The input dictionary of Property/Value pairs.</param>
		/// <exception cref="ArgumentNullException">Thrown when either of the input values are null.</exception>
		/// <exception cref="SimpleSerializerException">Thrown when an error occurs during serialization.</exception>
		public static void Serialize(object destinationObject, IDictionary<string, string> sourceValues)
		{
			Serialize<SimpleSerializedAttribute>(destinationObject, sourceValues);
		}

		/// <summary>
		/// Populates the <paramref name="destinationObject"/>'s properties that are decorated with an attribute of type <typeparamref name="T"/>
		/// using the Property/Value pairs from the input dictionary (<paramref name="sourceValues"/>).
		/// </summary>
		/// <typeparam name="T">Must be an attribute type.</typeparam>
		/// <param name="destinationObject">The object whose properties are to be initialized using the input dictionary's Property/Value pairs.</param>
		/// <param name="sourceValues">The input dictionary of Property/Value pairs.</param>
		/// <exception cref="ArgumentNullException">Thrown when either of the input values are null.</exception>
		/// <exception cref="SimpleSerializerException">Thrown when an error occurs during serialization.</exception>
		public static void Serialize<T>(object destinationObject, IDictionary<string, string> sourceValues)
			where T : Attribute
		{
			Platform.CheckForNullReference(destinationObject, "destinationObject");
			Platform.CheckForNullReference(sourceValues, "sourceValues");

			try
			{
				PropertyInfo[] properties = destinationObject.GetType().GetProperties();

				foreach (PropertyInfo property in properties)
				{
					if (!property.IsDefined(typeof(T), false))
						continue;

					T attribute = (T)(property.GetCustomAttributes(typeof (T), false)[0]);

					Type propertyType = property.PropertyType;
					if (sourceValues.ContainsKey(property.Name))
					{
						string value = sourceValues[property.Name];

						TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
						if (converter.CanConvertFrom(typeof(string)))
						{
							property.SetValue(destinationObject, converter.ConvertFromString(null, System.Globalization.CultureInfo.InvariantCulture, value), null);
						}
						else
							throw new InvalidOperationException(String.Format(SR.ExceptionFormatCannotConvertFromStringToType, propertyType.FullName));
					}
				}
			}
			catch (Exception e)
			{
				throw new SimpleSerializerException(String.Format(SR.ExceptionFormatSerializationFailedForType, destinationObject.GetType().FullName), e);
			}
		}

		/// <summary>
		/// Constructs and returns a dictionary of Property/Value pairs from the input <paramref name="sourceObject"/>.
		/// </summary>
		/// <remarks>
		/// Those properties decorated with the <see cref="SimpleSerializedAttribute"/> attribute will have their
		/// values extracted and inserted into the resulting dictionary.
		/// </remarks>
		/// <param name="sourceObject">The object whose properties are to be extracted.</param>
		/// <exception cref="ArgumentNullException">Thrown when the input value is null.</exception>
		/// <exception cref="SimpleSerializerException">Thrown when an error occurs during deserialization.</exception>
		/// <returns>A dictionary of Property/Value pairs.</returns>
		public static IDictionary<string, string> Deserialize(object sourceObject)
		{
			return Deserialize<SimpleSerializedAttribute>(sourceObject);
		}

		/// <summary>
		/// Constructs and returns a dictionary of Property/Value pairs from the input <paramref name="sourceObject"/>.
		/// </summary>
		/// <remarks>
		/// Those properties decorated with an attribute of type <typeparamref name="T"/> will have their
		/// values extracted and inserted into the resulting dictionary.
		/// </remarks>
		/// <typeparam name="T">Must be an attribute type.</typeparam>
		/// <param name="sourceObject">The object whose properties are to be extracted.</param>
		/// <exception cref="ArgumentNullException">Thrown when the input value is null.</exception>
		/// <exception cref="SimpleSerializerException">Thrown when an error occurs during deserialization.</exception>
		/// <returns>A dictionary of Property/Value pairs.</returns>
		public static IDictionary<string, string> Deserialize<T>(object sourceObject)
			where T : Attribute
		{
			Platform.CheckForNullReference(sourceObject, "sourceObject");

			Dictionary<string, string> dictionary = new Dictionary<string, string>();

			try
			{
				PropertyInfo[] properties = sourceObject.GetType().GetProperties();
				foreach (PropertyInfo property in properties)
				{
					if (!property.IsDefined(typeof(T), false))
						continue;

					T attribute = (T)(property.GetCustomAttributes(typeof(T), false)[0]);

					Type propertyType = property.PropertyType;
					object value = property.GetValue(sourceObject, null);

					if (value == null)
						continue;

					TypeConverter converter = TypeDescriptor.GetConverter(propertyType);
					if (converter.CanConvertTo((typeof(string))))
					{
						string stringValue = null;
						stringValue = converter.ConvertToString(null, System.Globalization.CultureInfo.InvariantCulture, value);

						if (!String.IsNullOrEmpty(stringValue))
							dictionary[property.Name] = stringValue;
					}
					else
						throw new InvalidOperationException(String.Format(SR.ExceptionFormatCannotConvertFromTypeToString, propertyType.FullName));
				}

				return dictionary;
			}
			catch (Exception e)
			{
				throw new SimpleSerializerException(String.Format(SR.ExceptionFormatDeserializationFailedForType, sourceObject.GetType().FullName), e);
			}
		}
	}
}

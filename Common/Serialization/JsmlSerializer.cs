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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Serialization
{

	#region Extensiblity

	/// <summary>
	/// Defines an interface to an object that hooks into the JSML serialization process
	/// for the purpose of customizing it.
	/// </summary>
	public interface IJsmlSerializerHook
	{
		/// <summary>
		/// Called to allow the hook to perform custom serialization.
		/// </summary>
		/// <param name="context"></param>
		/// <returns>True if the hook has handled serialization, false otherwise.</returns>
		bool Serialize(IJsmlSerializationContext context);

		/// <summary>
		/// Called to allow the hook to perform custom deserialization.
		/// </summary>
		/// <param name="context"></param>
		/// <returns>True if the hook has handled deserialization, false otherwise.</returns>
		bool Deserialize(IJsmlDeserializationContext context);
	}

	/// <summary>
	/// Defines an interface to the serialization context information passed to <see cref="IJsmlSerializerHook.Serialize"/>.
	/// </summary>
	public interface IJsmlSerializationContext
	{
		/// <summary>
		/// Gets the XML writer that is being used to perform serialization.
		/// </summary>
		/// <remarks>
		/// The hook may use this object to write XML directly.
		/// </remarks>
		XmlWriter XmlWriter { get; }

		/// <summary>
		/// Gets or sets the object being serialized.
		/// </summary>
		/// <remarks>
		/// The hook may set this to a different object.
		/// </remarks>
		object Data { get; set; }

		/// <summary>
		/// Gets or sets the name of the object for serialization.
		/// </summary>
		/// <remarks>
		/// The hook may modify the name.
		/// </remarks>
		string Name { get; set; }

		/// <summary>
		/// Gets the collection of attributes that will be serialized along with this object.
		/// </summary>
		/// <remarks>
		/// The hook may add to this collection.
		/// </remarks>
		IDictionary<string, string> Attributes { get; }

		/// <summary>
		/// Allows the hook to call back into the JSML serializer.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objectName"></param>
		/// <param name="attributes"></param>
		void Serialize(object obj, string objectName, IDictionary<string, string> attributes);
	}

	/// <summary>
	/// Defines an interface to the serialization context information passed to <see cref="IJsmlSerializerHook.Deserialize"/>.
	/// </summary>
	public interface IJsmlDeserializationContext
	{
		/// <summary>
		/// Gets the XML element containing the serialized representation.
		/// </summary>
		XmlElement XmlElement { get; }

		/// <summary>
		/// Gets or sets the data-type of the object being deserialized.
		/// </summary>
		/// <remarks>
		/// The hook may set this property to modify the data type.
		/// </remarks>
		Type DataType { get; set; }

		/// <summary>
		/// Gets or sets the object that results from deserialization.
		/// </summary>
		/// <remarks>
		/// If the hook handled the deserialization, it must set this property to reference the resulting object.
		/// </remarks>
		object Data { get; set; }

		/// <summary>
		/// Allows the hook to call back into the JSML deserializer.
		/// </summary>
		/// <param name="dataType"></param>
		/// <param name="xmlElement"></param>
		/// <returns>The object resulting from deserialization.</returns>
		object Deserialize(Type dataType, XmlElement xmlElement);
	}

	/// <summary>
	/// Defines an extension point for customizing the JSML serialization/deserialization services.
	/// </summary>
	[ExtensionPoint]
	public class JsmlSerializerHookExtensionPoint : ExtensionPoint<IJsmlSerializerHook> {}

	#endregion

	/// <summary>
	/// Provides JSML serialization and deserialization services.
	/// </summary>
	public class JsmlSerializer
	{
		#region Options classes

		/// <summary>
		/// Abstract base class for serialization options.
		/// </summary>
		public abstract class OptionsBase
		{
			private class NullHook : IJsmlSerializerHook
			{
				public bool Serialize(IJsmlSerializationContext context)
				{
					return false;
				}

				public bool Deserialize(IJsmlDeserializationContext context)
				{
					return false;
				}
			}

			protected OptionsBase()
			{
				// default filters
				this.DataContractTest = m => AttributeUtils.HasAttribute<DataContractAttribute>(m, false);
				this.DataMemberTest = m => AttributeUtils.HasAttribute<DataMemberAttribute>(m, true);
				this.Hook = new NullHook();
			}

			/// <summary>
			/// Gets or sets a predicate that determines whether a member should participate in serialization.
			/// </summary>
			/// <remarks>
			/// By default, a member is considered a data-member if it has the <see cref="DataMemberAttribute"/>,
			/// but this behaviour can be changed by supplying a custom predicate here.
			/// </remarks>
			public Predicate<MemberInfo> DataMemberTest { get; set; }

			/// <summary>
			/// Gets or sets a value that indicates whether non-public members should be included.
			/// </summary>
			/// <remarks>
			/// This value is set to true by default for backwards compatibility with the behaviour of
			/// previous versions of <see cref="JsmlSerializer"/>.
			/// </remarks>
			public bool IncludeNonPublicMembers { get; set; }

			/// <summary>
			/// Gets or sets a predicate that determines whether a type is considered a data-contract.
			/// </summary>
			/// <remarks>
			/// If a type is considered a data-contract, its data-members are serialized recursively.
			/// By default, a type is considered a data-contract if it has the <see cref="DataContractAttribute"/>,
			/// but this behaviour can be changed by supplying a custom predicate here.
			/// </remarks>
			public Predicate<Type> DataContractTest { get; set; }

			/// <summary>
			/// Gets or sets an instance of <see cref="IJsmlSerializerHook"/>.
			/// </summary>
			public IJsmlSerializerHook Hook { get; set; }
		}

		/// <summary>
		/// Encapsulates options that customize the serialization process.
		/// </summary>
		public class SerializeOptions : OptionsBase
		{
			/// <summary>
			/// Defines the default options.
			/// </summary>
			internal static readonly SerializeOptions Default = new SerializeOptions();
		}

		/// <summary>
		/// Encapsulates options that customize the deserialization process.
		/// </summary>
		public class DeserializeOptions : OptionsBase
		{
			/// <summary>
			/// Defines the default options.
			/// </summary>
			internal static readonly DeserializeOptions Default = new DeserializeOptions();
		}

		#endregion

		#region Public API

		/// <summary>
		/// Serializes the specified object to JSML format, using the specified objectName as the outermost tag name.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="objectName"></param>
		/// <returns></returns>
		public static string Serialize(object obj, string objectName)
		{
			return Serialize(obj, objectName, SerializeOptions.Default);
		}

		/// <summary>
		/// Serializes the specified object to JSML format, using the specified objectName as the outermost tag name.
		/// </summary>
		public static string Serialize(object obj, string objectName, SerializeOptions options)
		{
			if (obj == null)
				return null;

			using (var sw = new StringWriter())
			{
				var writer = new XmlTextWriter(sw) {Formatting = Formatting.Indented};
				Serialize(writer, obj, objectName, options);
				writer.Close();
				return sw.ToString();
			}
		}

		/// <summary>
		/// Serializes the specified object to JSML format, using the specified objectName as the outermost tag name.
		/// </summary>
		public static void Serialize(XmlWriter writer, object obj, string objectName)
		{
			Serialize(writer, obj, objectName, SerializeOptions.Default);
		}

		/// <summary>
		/// Serializes the specified object to JSML format, using the specified objectName as the outermost tag name.
		/// </summary>
		public static void Serialize(XmlWriter writer, object obj, string objectName, SerializeOptions options)
		{
			if (obj == null)
				return;

			var serializer = new Serializer(writer, options);
			serializer.Do(obj, objectName);
		}

		/// <summary>
		/// Deserializes the specified JSML text into an object of the specified type.
		/// </summary>
		public static T Deserialize<T>(string jsml)
		{
			return (T) Deserialize(typeof (T), jsml);
		}

		/// <summary>
		/// Deserializes the specified JSML text into an object of the specified type.
		/// </summary>
		public static T Deserialize<T>(string jsml, DeserializeOptions options)
		{
			return (T) Deserialize(typeof (T), jsml, options);
		}

		/// <summary>
		/// Deserializes the specified XML Document into an object of the specified type.
		/// </summary>
		public static T Deserialize<T>(XmlDocument xmlDoc, DeserializeOptions options)
		{
			return (T) Deserialize(typeof (T), xmlDoc, options);
		}

		/// <summary>
		/// Deserializes the specified JSML text into an object of the specified type.
		/// </summary>
		public static object Deserialize(Type dataContract, string jsml)
		{
			return Deserialize(dataContract, jsml, DeserializeOptions.Default);
		}

		/// <summary>
		/// Deserializes the specified JSML text into an object of the specified type.
		/// </summary>
		public static object Deserialize(Type dataContract, string jsml, DeserializeOptions options)
		{
			if (String.IsNullOrEmpty(jsml))
				return null;

			var xmlDoc = new XmlDocument {PreserveWhitespace = true};
			xmlDoc.LoadXml(jsml);

			var deserializer = new Deserializer(options);
			return deserializer.Do(dataContract, xmlDoc.DocumentElement);
		}

		/// <summary>
		/// Deserializes the specified XML document into an object of the specified type.
		/// </summary>
		/// 
		public static object Deserialize(Type dataContract, XmlDocument xmlDoc, DeserializeOptions options)
		{
			if (xmlDoc == null)
				return null;

			xmlDoc.PreserveWhitespace = true;

			var deserializer = new Deserializer(options);
			return deserializer.Do(dataContract, xmlDoc.DocumentElement);
		}

		/// <summary>
		/// Deserializes the specified JSML text into an object of the specified type.
		/// </summary>
		public static T Deserialize<T>(XmlReader reader)
		{
			return (T) Deserialize(reader, typeof (T));
		}

		/// <summary>
		/// Deserializes the specified JSML text into an object of the specified type.
		/// </summary>
		public static T Deserialize<T>(XmlReader reader, DeserializeOptions options)
		{
			return (T) Deserialize(reader, typeof (T), options);
		}

		/// <summary>
		/// Deserializes the specified JSML text into an object of the specified type.
		/// </summary>
		public static object Deserialize(XmlReader reader, Type dataContract)
		{
			return Deserialize(reader, dataContract, DeserializeOptions.Default);
		}

		/// <summary>
		/// Deserializes the specified JSML text into an object of the specified type.
		/// </summary>
		public static object Deserialize(XmlReader reader, Type dataContract, DeserializeOptions options)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(reader);

			var deserializer = new Deserializer(options);
			return deserializer.Do(dataContract, xmlDoc.DocumentElement);
		}

		#endregion

		#region SerializerBase class

		private abstract class SerializerBase
		{
			protected OptionsBase Options { get; private set; }
			private readonly List<IJsmlSerializerHook> _hooks;

			protected SerializerBase(OptionsBase options)
			{
				_hooks = CollectionUtils.Cast<IJsmlSerializerHook>(new JsmlSerializerHookExtensionPoint().CreateExtensions());
				Options = options;
			}

			/// <summary>
			/// Get a list of properties and fields from a data contract that satisfy the predicate.
			/// </summary>
			protected static IEnumerable<IObjectMemberContext> GetDataMemberFields(object dataObject, Predicate<MemberInfo> memberTest, bool includeNonPublicMembers)
			{
				var walker = new ObjectWalker(memberTest)
				             	{
				             		IncludeNonPublicFields = includeNonPublicMembers,
				             		IncludeNonPublicProperties = includeNonPublicMembers
				             	};

				return walker.Walk(dataObject);
			}

			protected bool DoHooks(Predicate<IJsmlSerializerHook> hookAction)
			{
				// first try the "options" hook
				if (hookAction(Options.Hook))
					return true;

				// then try extension hooks
				foreach (var hook in _hooks)
				{
					if (hookAction(hook))
						return true;
				}
				return false;
			}

			protected bool IsDataContract(Type dataType)
			{
				// need to explicitly exclude enums, which can have the [DataContract] attribute applied
				// for .NET serialization, but we don't treat them as data contracts
				return !(dataType.IsEnum) && this.Options.DataContractTest(dataType);
			}
		}

		#endregion

		#region Serializer class

		private class Serializer : SerializerBase
		{
			private class SerializationContext : IJsmlSerializationContext
			{
				public SerializationContext(Serializer owner, object data, string nodeName, IDictionary<string, string> attributes)
				{
					this.Owner = owner;
					this.Data = data;
					this.Name = nodeName;
					this.Attributes = new Dictionary<string, string>(attributes);
				}

				public XmlWriter XmlWriter
				{
					get { return this.Owner._writer; }
				}

				public object Data { get; set; }
				public string Name { get; set; }
				public IDictionary<string, string> Attributes { get; private set; }

				public void Serialize(object obj, string objectName, IDictionary<string, string> attributes)
				{
					this.Owner.Do(obj, objectName, attributes);
				}

				private Serializer Owner { get; set; }
			}

			private readonly XmlWriter _writer;

			public Serializer(XmlWriter writer, SerializeOptions options)
				: base(options)
			{
				_writer = writer;
			}

			/// <summary>
			/// Serializes the specified object, enclosing it with the specified name.
			/// </summary>
			/// <param name="obj"></param>
			/// <param name="objectName"></param>
			internal void Do(object obj, string objectName)
			{
				Do(obj, objectName, new Dictionary<string, string>());
			}

			/// <summary>
			/// Serializes the specified object, enclosing it with the specified name.
			/// </summary>
			/// <param name="obj"></param>
			/// <param name="objectName"></param>
			/// <param name="attributes"></param>
			private void Do(object obj, string objectName, IDictionary<string, string> attributes)
			{
				// hooks get first chance
				var sctx = new SerializationContext(this, obj, objectName, attributes);
				if (DoHooks(h => h.Serialize(sctx)))
					return;

				// hooks may have modified these args
				obj = sctx.Data;
				objectName = sctx.Name;
				attributes = sctx.Attributes;

				if (obj == null)
					return;

				_writer.WriteStartElement(objectName);
				foreach (var kvp in attributes)
				{
					_writer.WriteAttributeString(kvp.Key, kvp.Value);
				}

				if (IsDataContract(obj.GetType()))
				{
					var dataMemberFields = new List<IObjectMemberContext>(GetDataMemberFields(obj, this.Options.DataMemberTest, this.Options.IncludeNonPublicMembers));
					if (dataMemberFields.Count > 0)
					{
						_writer.WriteAttributeString("type", "hash");
						foreach (var context in dataMemberFields)
						{
							Do(context.MemberValue, context.Member.Name);
						}
					}
				}
				else if (obj is IDictionary)
				{
					// this clause was added mainly to support serialization of ExtendedProperties, which 
					// is always a string-keyed dictionary
					// the dictionary is serialized as if it were an object and each key is a property on the object
					// jscript will not be able to distinguish that it was originally a dictionary
					// note that if the dictionary contains non-string keys, unpredictable behaviour may result
					var dic = (IDictionary) obj;
					_writer.WriteAttributeString("type", "hash");
					foreach (DictionaryEntry entry in dic)
					{
						Do(entry.Value, entry.Key.ToString());
					}
				}
				else if (obj is Enum)
				{
					_writer.WriteValue(obj.ToString());
				}
				else if (obj is string)
				{
					_writer.WriteValue(obj.ToString());
				}
				else if (obj is DateTime)
				{
					_writer.WriteValue(DateTimeUtils.FormatISO((DateTime) obj));
				}
				else if (obj is DateTime?)
				{
					_writer.WriteValue(DateTimeUtils.FormatISO(((DateTime?) obj).Value));
				}
				else if (obj is TimeSpan)
				{
					_writer.WriteValue(DateTimeUtils.FormatTimeSpan((TimeSpan) obj));
				}
				else if (obj is TimeSpan?)
				{
					_writer.WriteValue(DateTimeUtils.FormatTimeSpan(((TimeSpan?) obj).Value));
				}
				else if (obj is bool)
				{
					_writer.WriteValue((bool) obj ? "true" : "false");
				}
				else if (obj is IList)
				{
					_writer.WriteAttributeString("type", "array");
					foreach (var item in (IList) obj)
					{
						Do(item, "item");
					}
				}
				else if (obj is XmlDocument)
				{
					// this clause supports serialization of an embedded JSML document inline with the
					// output of the serializer
					_writer.WriteAttributeString("type", "hash");
					var xmlDoc = (XmlDocument) obj;
					if (xmlDoc.DocumentElement != null)
					{
						xmlDoc.DocumentElement.WriteTo(_writer);
					}
				}
				else if (obj is Guid)
				{
					_writer.WriteValue(((Guid) obj).ToString("N"));
				}
				else if (obj is IConvertible)
				{
					_writer.WriteValue(Convert.ToString(obj, CultureInfo.InvariantCulture));
				}
				else
				{
					_writer.WriteValue(obj.ToString());
				}
				_writer.WriteEndElement();
			}
		}

		#endregion

		#region Deserializer class

		private class Deserializer : SerializerBase
		{
			private class DeserializationContext : IJsmlDeserializationContext
			{
				public DeserializationContext(Deserializer owner, Type dataType, XmlElement element)
				{
					this.Owner = owner;
					this.DataType = dataType;
					this.XmlElement = element;
				}

				public XmlElement XmlElement { get; private set; }
				public Type DataType { get; set; }
				public object Data { get; set; }

				public object Deserialize(Type dataType, XmlElement xmlElement)
				{
					return this.Owner.Do(dataType, xmlElement);
				}

				private Deserializer Owner { get; set; }
			}

			public Deserializer(DeserializeOptions options)
				: base(options) {}

			/// <summary>
			/// Create an object of type 'dataType' from the xmlElement.  Recurse if the object is a DataContractBase or IList
			/// </summary>
			/// <param name="dataType"></param>
			/// <param name="xmlElement"></param>
			/// <returns></returns>
			internal object Do(Type dataType, XmlElement xmlElement)
			{
				// hooks get first chance
				var dctx = new DeserializationContext(this, dataType, xmlElement);
				if (DoHooks(h => h.Deserialize(dctx)))
					return dctx.Data;

				// hooks may have modified these args
				dataType = dctx.DataType;

				if (IsDataContract(dataType))
				{
					var dataObject = Activator.CreateInstance(dataType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);

					foreach (var context in GetDataMemberFields(dataObject, this.Options.DataMemberTest, this.Options.IncludeNonPublicMembers))
					{
						var memberElement = GetFirstElementWithTagName(xmlElement, context.Member.Name);
						if (memberElement != null)
						{
							var memberObject = Do(context.MemberType, memberElement);
							context.MemberValue = memberObject;
						}
					}
					return dataObject;
				}

				if (typeof (IDictionary).IsAssignableFrom(dataType))
				{
					// this clause was added mainly to support de-serialization of ExtendedProperties
					// note that only strongly-typed dictionaries are supported, and the key type *must* be "string",
					// and the value type must be JSML-serializable
					var dataObject = Activator.CreateInstance(dataType);
					var genericTypes = dataType.GetGenericArguments();
					var keyType = genericTypes[0];
					if (keyType != typeof (string))
						throw new NotSupportedException("Only IDictionary<string, T>, where T is a JSML-serializable type, is supported.");
					var valueType = genericTypes[1];

					foreach (XmlNode node in xmlElement.ChildNodes)
					{
						if (node is XmlElement)
						{
							var value = Do(valueType, (XmlElement) node);
							((IDictionary) dataObject).Add(node.Name, value);
						}
					}
					return dataObject;
				}

				if (dataType == typeof (string))
				{
					return xmlElement.InnerText;
				}

				if (dataType.IsEnum)
				{
					return Enum.Parse(dataType, xmlElement.InnerText, true);
				}

				if (dataType == typeof (DateTime))
				{
					return DateTimeUtils.ParseISO(xmlElement.InnerText);
				}
				if (dataType == typeof (TimeSpan))
				{
					return DateTimeUtils.ParseTimeSpan(xmlElement.InnerText);
				}

				if (dataType == typeof (bool))
				{
					return xmlElement.InnerText.Equals("true", StringComparison.InvariantCultureIgnoreCase);
				}

				if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof (Nullable<>))
				{
					// recur using the generic argument type in place of the nullable wrapper type
					return Do(dataType.GetGenericArguments()[0], xmlElement);
				}

				if (dataType.GetInterface("IList") == typeof (IList))
				{
					var nodeList = xmlElement.SelectNodes("item");
					var count = nodeList == null ? 0 : nodeList.Count;
					if (dataType.BaseType == typeof (Array))
					{
						var elementType = dataType.GetElementType();
						var dataObject = Array.CreateInstance(elementType, count);

						for (var i = 0; i < count; i++)
						{
							var item = Do(elementType, (XmlElement) nodeList[i]);
							dataObject.SetValue(item, i);
						}

						return dataObject;
					}
					else
					{
						if (!dataType.IsGenericType)
							throw new NotSupportedException("Only typed lists (IList<T>) are supported, and T must be a JSML-serializable type.");

						var elementType = dataType.GetGenericArguments()[0];
						var dataObject = Activator.CreateInstance(dataType);
						for (var i = 0; i < count; i++)
						{
							var item = Do(elementType, (XmlElement) nodeList[i]);
							((IList) dataObject).Add(item);
						}
						return dataObject;
					}
				}

				if (dataType == typeof (XmlDocument))
				{
					// this clause supports deserialization of an embedded JSML document
					var xml = xmlElement.InnerXml;
					if (!string.IsNullOrEmpty(xml))
					{
						var doc = new XmlDocument();
						doc.LoadXml(xml);
						return doc;
					}
					return null;
				}

				if (dataType == typeof (Guid))
				{
					// parse guid
					return new Guid(xmlElement.InnerText);
				}

				if (dataType.GetInterface("IConvertible") == typeof (IConvertible))
				{
					return Convert.ChangeType(xmlElement.InnerText, dataType, CultureInfo.InvariantCulture);
				}

				return xmlElement.InnerText;
			}

			private static XmlElement GetFirstElementWithTagName(XmlNode xmlElement, string tagName)
			{
				return (XmlElement) CollectionUtils.FirstElement(xmlElement.SelectNodes(tagName));
			}
		}

		#endregion
	}
}
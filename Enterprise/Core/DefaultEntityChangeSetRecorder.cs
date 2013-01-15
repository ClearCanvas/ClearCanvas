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
using System.Runtime.Serialization;
using ClearCanvas.Common.Audit;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using System.Collections;
using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// Default implementation of <see cref="IEntityChangeSetRecorder"/>.
	/// </summary>
	public class DefaultEntityChangeSetRecorder : IEntityChangeSetRecorder
	{
		#region Data contracts

		[DataContract]
		public class ChangeSetData
		{
			[DataMember]
			public string Operation;

			[DataMember]
			public List<ActionData> Actions;
		}

		[DataContract]
		public class ActionData
		{
			[DataMember]
			public string Type;

			[DataMember]
			public string Class;

			[DataMember]
			public string OID;

			[DataMember]
			public int Version;

			[DataMember]
			public Dictionary<string, PropertyData> ChangedProperties;
		}

		[DataContract]
		public class PropertyData
		{
			[DataMember]
			public object OldValue;

			[DataMember]
			public object NewValue;
		}

		#endregion

		private const int MaxStringLength = 255;
		private const string NullValue = "{null}";
		private const string MultiValued = "{multiple-values}";

		#region ObjectWriter implementation

		class ObjectWriter : IObjectWriter
		{
			private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

			void IObjectWriter.WriteProperty(string name, object value)
			{
				_data[name] = WritePropertyValue(value);
			}

			internal Dictionary<string, object> Data
			{
				get { return _data; }
			}
		}

		#endregion

		/// <summary>
		/// Obtains a data-contract instance representing the specified change-set, suitable for
		/// transmission and/or serialization to an audit log.
		/// </summary>
		/// <param name="operationName"></param>
		/// <param name="changeSet"></param>
		/// <returns></returns>
		public static ChangeSetData WriteChangeSet(string operationName, IEnumerable<EntityChange> changeSet)
		{
			var actionData = from entityChange in changeSet
							 select new ActionData
							 {
								 Type = entityChange.ChangeType.ToString(),
								 Class = EntityRefUtils.GetClass(entityChange.EntityRef).FullName,
								 OID = EntityRefUtils.GetOID(entityChange.EntityRef).ToString(),
								 Version = EntityRefUtils.GetVersion(entityChange.EntityRef),
								 ChangedProperties = WriteProperties(entityChange)
							 };

			return new ChangeSetData { Operation = StringUtilities.EmptyIfNull(operationName), Actions = actionData.ToList() };
		}


		private string _operationName;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DefaultEntityChangeSetRecorder()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="operationName"></param>
		public DefaultEntityChangeSetRecorder(string operationName)
		{
			_operationName = operationName;
		}

		/// <summary>
		/// Gets or sets a logical operation name for the operation that produced the change set.
		/// </summary>
		public string OperationName
		{
			get { return _operationName; }
			set { _operationName = value; }
		}


		#region IEntityChangeSetRecorder Members

		/// <summary>
		/// Writes an audit log entry for the specified change set.
		/// </summary>
		void IEntityChangeSetRecorder.WriteLogEntry(IEnumerable<EntityChange> changeSet, AuditLog auditLog)
		{
			var xml = JsmlSerializer.Serialize(WriteChangeSet(_operationName, changeSet), "ChangeSet");
			auditLog.WriteEntry(_operationName, xml);
		}

		#endregion

		#region Helpers

		private static Dictionary<string, PropertyData> WriteProperties(EntityChange entityChange)
		{
			var propertiesData = new Dictionary<string, PropertyData>();

			var entityClass = EntityRefUtils.GetClass(entityChange.EntityRef);
			foreach (var prop in entityChange.PropertyChanges)
			{
				var pi = entityClass.GetProperty(prop.PropertyName);

				// special handling of extended properties collections
				// note that we need to check pi != null because it may represent a field-access "property"
				// which has no corresponding .NET property
				if (pi != null && AttributeUtils.HasAttribute<ExtendedPropertiesCollectionAttribute>(pi))
				{
					var extendedProps = WriteExtendedProperties(prop, entityChange.ChangeType);
					foreach (var extendedProp in extendedProps)
					{
						propertiesData.Add(extendedProp.Key, extendedProp.Value);
					}
				}
				else
				{
					var propertyData = WriteProperty(prop.OldValue, prop.NewValue, entityChange.ChangeType);
					propertiesData.Add(prop.PropertyName, propertyData);
				}
			}
			return propertiesData;
		}

		private static PropertyData WriteProperty(object oldValue, object newValue, EntityChangeType changeType)
		{
			var data = new PropertyData();

			// for Updates and Deletes, write the old value
			if (changeType != EntityChangeType.Create)
			{
				data.OldValue = WritePropertyValue(oldValue);
			}

			// for Creates and Updates, write the new value
			if (changeType != EntityChangeType.Delete)
			{
				data.NewValue = WritePropertyValue(newValue);
			}
			return data;
		}

		private static Dictionary<string, PropertyData> WriteExtendedProperties(PropertyChange propertyChange, EntityChangeType changeType)
		{
			var result = new Dictionary<string, PropertyData>();

			var collectionName = propertyChange.PropertyName;
			var oldColl = propertyChange.OldValue == null ? new Hashtable() : (IDictionary)propertyChange.OldValue;
			var newColl = propertyChange.NewValue == null ? new Hashtable() : (IDictionary)propertyChange.NewValue;

			// obtain unique set of keys over both items
			var keys = CollectionUtils.Unique(CollectionUtils.Concat(oldColl.Keys, newColl.Keys));

			// enumerate each key
			foreach (var key in keys)
			{
				var oldValue = oldColl.Contains(key) ? oldColl[key] : null;
				var newValue = newColl.Contains(key) ? newColl[key] : null;

				// has this "property" changed?
				if (!Equals(oldValue, newValue))
				{
					var propertyName = string.Concat(collectionName, ".", key);
					var propertyData = WriteProperty(oldValue, newValue, changeType);
					result.Add(propertyName, propertyData);
				}
			}
			return result;
		}

		private static object WritePropertyValue(object propertyValue)
		{
			if (propertyValue is ICollection)
			{
				return WriteCollectionContent(propertyValue);
			}

			if (propertyValue is IAuditFormattable)
			{
				// allow value to write itself
				var formattable = propertyValue as IAuditFormattable;
				var objectWriter = new ObjectWriter();
				formattable.Write(objectWriter);
				return objectWriter.Data;
			}

			// use simple serialization
			return GetSerializedValue(propertyValue);
		}

		private static List<object> WriteCollectionContent(object collection)
		{
			return (from object item in ((IEnumerable) collection)
					select WritePropertyValue(item)).ToList();
		}

		private static string GetSerializedValue(object value)
		{
			if (value == null)
				return NullValue;

			// use ISO format for date times, because it is easy to parse
			if (value is DateTime)
				return DateTimeUtils.FormatISO((DateTime)value);

			// for entities, write the class name and object id
			if (value is Entity)
			{
				var entity = value as Entity;
				return string.Format("{0}:{1}", entity.GetClass().FullName, entity.OID);
			}

			// for enum values, write the code
			if (value is EnumValue)
				return (value as EnumValue).Code;

			// don't support dealing with nested collections at the present time
			// just write out that it is multi-valued
			if (value is ICollection)
				return MultiValued;

			// for all other values, including components (ValueObject subclasses)
			// just call ToString() and truncate to MaxStringLength chars
			var s = value.ToString();
			return (s.Length > MaxStringLength) ? s.Substring(0, MaxStringLength) : s;
		}

		#endregion

	}
}

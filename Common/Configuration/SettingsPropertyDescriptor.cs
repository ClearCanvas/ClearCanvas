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
using System.Reflection;
using ClearCanvas.Common.Utilities;
using System.Runtime.Serialization;
using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Describes a settings property.
	/// </summary>
	/// <remarks>
	/// A settings property is a single property belonging to a settings group.
	/// </remarks>
	[DataContract]
	public class SettingsPropertyDescriptor
	{
		/// <summary>
		/// Returns a list of <see cref="SettingsPropertyDescriptor"/> objects describing each property belonging
		/// to a settings group.
		/// </summary>
		/// <remarks>
		/// The specified group must refer to a locally installed settings class.
		/// </remarks>
		public static List<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group)
		{
			Platform.CheckForNullReference(group, "group");

			var settingsClass = Type.GetType(group.AssemblyQualifiedTypeName);
			if(settingsClass == null)
				throw new SettingsException(string.Format("{0} is not a locally installed settings group.", group.Name));

			return CollectionUtils.Map(SettingsClassMetaDataReader.GetSettingsProperties(settingsClass),
									   (PropertyInfo property) => new SettingsPropertyDescriptor(property));
		}


		private string _name;
		private string _typeName;
		private string _description;
		private SettingScope _scope;
		private string _defaultValue;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal SettingsPropertyDescriptor(PropertyInfo property)
			: this(SettingsClassMetaDataReader.GetName(property),
				SettingsClassMetaDataReader.GetType(property).FullName,
				SettingsClassMetaDataReader.GetDescription(property),
				SettingsClassMetaDataReader.GetScope(property),
				SettingsClassMetaDataReader.GetDefaultValue(property))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsPropertyDescriptor(string name, string typeName, string description, SettingScope scope, string defaultValue)
		{
			_name = name;
			_typeName = typeName;
			_description = description;
			_scope = scope;
			_defaultValue = defaultValue;
		}

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		[DataMember]
		public string Name
		{
			get { return _name; }
			private set { _name = value; }
		}

		/// <summary>
		/// Gets the name of the type of the property.
		/// </summary>
		[DataMember]
		public string TypeName
		{
			get { return _typeName; }
			private set { _typeName = value; }
		}

		/// <summary>
		/// Gets the description of the property.
		/// </summary>
		[DataMember]
		public string Description
		{
			get { return _description; }
			private set { _description = value; }
		}

		/// <summary>
		/// Gets the scope of the property.
		/// </summary>
		[DataMember]
		public SettingScope Scope
		{
			get { return _scope; }
			private set { _scope = value; }
		}

		/// <summary>
		/// Gets the serialized default value of the property.
		/// </summary>
		[DataMember]
		public string DefaultValue
		{
			get { return _defaultValue; }
			private set { _defaultValue = value; }
		}

		public override string ToString()
		{
			return String.Format("{0} ({1}, {2})", Name, Scope, TypeName);
		}
	}
}

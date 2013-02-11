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
			if (settingsClass == null)
				throw new SettingsException(string.Format("{0} is not a locally installed settings group.", group.Name));

			return SettingsClassMetaDataReader.GetSettingsProperties(settingsClass)
				.Select(property => new SettingsPropertyDescriptor(property)).ToList();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SettingsPropertyDescriptor(PropertyInfo property)
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
			Name = name;
			TypeName = typeName;
			Description = description;
			Scope = scope;
			DefaultValue = defaultValue;
		}

		/// <summary>
		/// Gets the name of the property.
		/// </summary>
		[DataMember]
		public string Name { get; private set; }

		/// <summary>
		/// Gets the name of the type of the property.
		/// </summary>
		[DataMember]
		public string TypeName { get; private set; }

		/// <summary>
		/// Gets the description of the property.
		/// </summary>
		[DataMember]
		public string Description { get; private set; }

		/// <summary>
		/// Gets the scope of the property.
		/// </summary>
		[DataMember]
		public SettingScope Scope { get; private set; }

		/// <summary>
		/// Gets the serialized default value of the property.
		/// </summary>
		[DataMember]
		public string DefaultValue { get; private set; }

		public override string ToString()
		{
			return String.Format("{0} ({1}, {2})", Name, Scope, TypeName);
		}
	}
}

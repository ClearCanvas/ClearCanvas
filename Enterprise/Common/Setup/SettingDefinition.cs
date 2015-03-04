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
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Setup
{
	/// <summary>
	/// Helper class for providing setting definitions to be imported at deployment time.
	/// </summary>
	[DataContract]
	public class SettingDefinition
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		// Required for JSML deserialization - do not remove
		public SettingDefinition()
		{
		}

		/// <summary>
		/// Gets the group name of the setting.
		/// </summary>
		[DataMember]
		public string Group { get; set; }

		/// <summary>
		/// Gets the version of the setting.
		/// </summary>
		[DataMember]
		public string Version { get; set; }

		/// <summary>
		/// Gets the property name of the setting.
		/// </summary>
		[DataMember]
		public string Property { get; set; }

		/// <summary>
		/// Gets the value of the setting.
		/// </summary>
		[DataMember]
		public string Value { get; set; }
	}
}

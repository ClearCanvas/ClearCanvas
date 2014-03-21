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

using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Configuration
{
	/// <summary>
	/// Helper class for providing configuration definitions to be imported at deployment time.
	/// </summary>
	[DataContract]
	public class ConfigurationDefinition
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		// Required for JSML deserialization - do not remove
		public ConfigurationDefinition()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the configuration.</param>
		/// <param name="version">The version configuration.</param>
		/// <param name="body">The body of the configuration</param>
		public ConfigurationDefinition(string name, string version, string body)
		{
			Name = name;
			Version = version;
			Body = body;
		}

		/// <summary>
		/// Gets the name of the configuration.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets the version of the configuration.
		/// </summary>
		[DataMember]
		public string Version { get; set; }

		/// <summary>
		/// Gets the body of the configuration.
		/// </summary>
		[DataMember]
		public string Body { get; set; }
	}
}

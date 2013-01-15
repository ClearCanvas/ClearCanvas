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

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Encapsulates meta-data about a configuration document.
	/// </summary>
	[DataContract]
	public class ConfigurationDocumentHeader
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="creationTime"></param>
		/// <param name="modifiedTime"></param>
		public ConfigurationDocumentHeader(ConfigurationDocumentKey key, DateTime creationTime, DateTime modifiedTime)
		{
			Key = key;
			CreationTime = creationTime;
			ModifiedTime = modifiedTime;
		}

		/// <summary>
		/// Gets the key that identifies the document.
		/// </summary>
		[DataMember]
		public ConfigurationDocumentKey Key { get; private set; }

		/// <summary>
		/// Gets the document creation time.
		/// </summary>
		[DataMember]
		public DateTime CreationTime { get; private set; }

		/// <summary>
		/// Gets the document last modification time.
		/// </summary>
		[DataMember]
		public DateTime ModifiedTime { get; private set; }
	}
}

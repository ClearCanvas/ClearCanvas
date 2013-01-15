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
using ClearCanvas.Common.Caching;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
	/// Identifies a configuration document.
	/// </summary>
	[DataContract]
	public class ConfigurationDocumentKey : IDefinesCacheKey
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="documentName"></param>
		/// <param name="version"></param>
		/// <param name="user"></param>
		/// <param name="instanceKey"></param>
		public ConfigurationDocumentKey(string documentName, Version version, string user, string instanceKey)
		{
			DocumentName = documentName;
			Version = version;
			User = user;
			InstanceKey = instanceKey;
		}

		/// <summary>
		/// Gets the name of the document.
		/// </summary>
		[DataMember]
		public string DocumentName { get; private set; }

		/// <summary>
		/// Gets the version of the document.
		/// </summary>
		[DataMember]
		public Version Version { get; private set; }

		/// <summary>
		/// Gets the owner of the document.
		/// </summary>
		[DataMember]
		public string User { get; private set; }

		/// <summary>
		/// Gets the instance key of the document.
		/// </summary>
		[DataMember]
		public string InstanceKey { get; private set; }

		#region IDefinesCacheKey Members

		/// <summary>
		/// Gets the cache key defined by this instance.
		/// </summary>
		/// <returns></returns>
		string IDefinesCacheKey.GetCacheKey()
		{
			return string.Format("{0}:{1}:{2}:{3}",
				this.DocumentName,
				this.Version,
				StringUtilities.EmptyIfNull(this.User),
				StringUtilities.EmptyIfNull(this.InstanceKey));
		}

		#endregion
	}
}

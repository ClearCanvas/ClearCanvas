#region License

// Copyright (c) 2014, ClearCanvas Inc.
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
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Common
{
	/// <summary>
	/// Data contract for specifying product and version details.
	/// </summary>
	[DataContract]
	public class ApplicationVersion
	{
		/// <summary>
		/// Gets or sets the Product Name (PN) field.
		/// </summary>
		[DataMember(Name = @"ProductName")]
		public string ProductName { get; set; }

		/// <summary>
		/// Gets or sets the Component Name (CN) field.
		/// </summary>
		[DataMember(Name = @"ComponentName")]
		public string ComponentName { get; set; }

		/// <summary>
		/// Gets or sets the Component Edition (ED) field.
		/// </summary>
		[DataMember(Name = @"ComponentEdition")]
		public string ComponentEdition { get; set; }

		/// <summary>
		/// Gets or sets the Component Version (VER) field.
		/// </summary>
		[DataMember(Name = @"ComponentVersion")]
		public string ComponentVersion { get; set; }

		/// <summary>
		/// Gets or sets the Component Version Suffix (SFX) field.
		/// </summary>
		[DataMember(Name = @"ComponentVersionSuffix")]
		public string ComponentVersionSuffix { get; set; }

		/// <summary>
		/// Gets or sets the Component Release (REL) field.
		/// </summary>
		[DataMember(Name = @"ComponentRelease")]
		public string ComponentRelease { get; set; }

		public ApplicationVersion() {}

		public ApplicationVersion(ApplicationVersion other)
		{
			if (other != null)
			{
				ProductName = other.ProductName;
				ComponentName = other.ComponentName;
				ComponentEdition = other.ComponentEdition;
				ComponentVersion = other.ComponentVersion;
				ComponentVersionSuffix = other.ComponentVersionSuffix;
				ComponentRelease = other.ComponentRelease;
			}
		}

		public ApplicationVersion Clone()
		{
			return new ApplicationVersion(this);
		}

		public override string ToString()
		{
			return string.Format(@"{{PN={0}|CN={1}|ED={2}|VER={3}|SFX={4}|REL={5}}}", ProductName, ComponentName, ComponentEdition, ComponentVersion, ComponentVersionSuffix, ComponentRelease);
		}

		public override int GetHashCode()
		{
			return 0x1D11842C
			       ^ (ProductName != null ? ProductName.GetHashCode() : 0)
			       ^ (ComponentName != null ? ComponentName.GetHashCode() : 0)
			       ^ (ComponentEdition != null ? ComponentEdition.GetHashCode() : 0)
			       ^ (ComponentVersion != null ? ComponentVersion.GetHashCode() : 0)
			       ^ (ComponentVersionSuffix != null ? ComponentVersionSuffix.GetHashCode() : 0)
			       ^ (ComponentRelease != null ? ComponentRelease.GetHashCode() : 0);
		}

		public bool Equals(ApplicationVersion other)
		{
			return !ReferenceEquals(other, null)
			       && ProductName == other.ProductName
			       && ComponentName == other.ComponentName
			       && ComponentEdition == other.ComponentEdition
			       && ComponentVersion == other.ComponentVersion
			       && ComponentVersionSuffix == other.ComponentVersionSuffix
			       && ComponentRelease == other.ComponentRelease;
		}

		public override bool Equals(object obj)
		{
			return obj is ApplicationVersion && Equals((ApplicationVersion) obj);
		}

		public static ApplicationVersion GetCurrentVersion()
		{
			return new ApplicationVersion
			       	{
			       		ProductName = ProductInformation.Product,
			       		ComponentName = ProductInformation.Component,
			       		ComponentEdition = ProductInformation.Edition,
			       		ComponentRelease = ProductInformation.Release,
			       		ComponentVersion = ProductInformation.Version.ToString(),
			       		ComponentVersionSuffix = ProductInformation.VersionSuffix
			       	};
		}
	}
}
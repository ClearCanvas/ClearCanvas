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
using System.Runtime.Serialization;
using System.Text;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin
{
	[DataContract]
	public class EnumValueAdminInfo : DataContractBase, ICloneable, IEquatable<EnumValueAdminInfo>
	{
		public EnumValueAdminInfo(string code, string value, string description, bool deactivated)
		{
			this.Code = code;
			this.Value = value;
			this.Description = description;
			this.Deactivated = deactivated;
		}

		public EnumValueAdminInfo()
		{
		}

		[DataMember]
		public string Code;

		[DataMember]
		public string Value;

		[DataMember]
		public string Description;

		[DataMember]
		public bool Deactivated;


		#region ICloneable Members

		public object Clone()
		{
			return new EnumValueAdminInfo(this.Code, this.Value, this.Description, this.Deactivated);
		}

		#endregion

		#region IEquatable<EnumValueAdminInfo>

		public bool Equals(EnumValueAdminInfo enumValueInfo)
		{
			if (ReferenceEquals(this, enumValueInfo)) return true;
			if (enumValueInfo == null) return false;
			if (!Equals(Code, enumValueInfo.Code)) return false;
			return true;
		}

		#endregion

		#region Object overrides

		public override bool Equals(object obj)
		{
			return Equals(obj as EnumValueAdminInfo);
		}

		public override int GetHashCode()
		{
			return Code.GetHashCode();
		}

		/// <summary>
		/// Return the display value
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Value;
		}

		#endregion
	}
}

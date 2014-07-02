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

namespace ClearCanvas.Enterprise.Common.Mail
{
	/// <summary>
	/// Identifies outgoing message classification of contents.
	/// </summary>
	[DataContract(Name = "Classification")]
	public enum OutgoingMailClassification
	{
		/// <summary>
		/// Indicates that the outgoing message contains normal content.
		/// </summary>
		[EnumMember]
		Normal = 0,

		/// <summary>
		/// Indicates that the outgoing message contains restricted content (such as PHI or other sensitive information).
		/// </summary>
		[EnumMember]
		Restricted = 1,

		/// <summary>
		/// Default outgoing message classification is to err on side of caustion and classify as restricted anyway.
		/// </summary>
		[EnumMember]
		Default = Restricted
	}
}
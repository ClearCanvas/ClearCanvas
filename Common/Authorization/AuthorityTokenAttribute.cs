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

namespace ClearCanvas.Common.Authorization
{
	/// <summary>
	/// Attribute used to define authority group tokens on types.
	/// </summary>
	/// <seealso cref="AuthorityGroupDefinition"/>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public class AuthorityTokenAttribute : Attribute
	{
		/// <summary>
		/// The token description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// The former identities of this token, used for purposes of migrating a token.
		/// Separate multiple former identities by a semicolon.
		/// </summary>
		public string Formerly { get; set; }
	}
}

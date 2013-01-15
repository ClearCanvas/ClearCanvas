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

namespace ClearCanvas.Common.Authorization
{
	/// <summary>
	/// Describes an authority token.
	/// </summary>
	public class AuthorityTokenDefinition
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="definingAssembly"></param>
		/// <param name="description"></param>
		/// <param name="formerIdentities"></param>
		public AuthorityTokenDefinition(string token, string definingAssembly, string description, string[] formerIdentities)
		{
			Token = token;
			DefiningAssembly = definingAssembly;
			Description = description;
			FormerIdentities = formerIdentities;
		}

		/// <summary>
		/// Gets the token string.
		/// </summary>
		public string Token { get; private set; }

		/// <summary>
		/// Gets the name of the assembly that defines the token.
		/// </summary>
		public string DefiningAssembly { get; private set; }

		/// <summary>
		/// Gets the token description.
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// Gets the set of former identities for this token, for migration purposes.
		/// </summary>
		public string[] FormerIdentities { get; private set; }
	}
}

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

namespace ClearCanvas.Common.Authorization
{
	/// <summary>
	/// Helper class for providing authority group definitions to be imported at deployment time.
	/// </summary>
	/// <seealso cref="AuthorityTokenAttribute"/>
	[DataContract]
	public class AuthorityGroupDefinition
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		// Required for JSML deserialization - do not remove
		public AuthorityGroupDefinition()
		{
			this.Tokens = new string[0];
		}


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the authority group.</param>
		/// <param name="tokens">The associated authority group tokens.</param>
		public AuthorityGroupDefinition(string name, string[] tokens)
			: this(name, name, false, tokens, false)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the authority group.</param>
		/// <param name="tokens">The associated authority group tokens.</param>
		/// <param name="dataGroup">Tells if the group is an authority group for controlling access to data.</param>
		/// <param name="description">The description of the authority group.</param>
		public AuthorityGroupDefinition(string name, string description, bool dataGroup, string[] tokens)
			: this(name, description, dataGroup, tokens, false)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the authority group.</param>
		/// <param name="tokens">The associated authority group tokens.</param>
		/// <param name="dataGroup">Tells if the group is an authority group for controlling access to data.</param>
		/// <param name="description">The description of the authority group.</param>
		/// <param name="builtIn"> </param>
		public AuthorityGroupDefinition(string name, string description, bool dataGroup, string[] tokens, bool builtIn)
		{
			Name = name;
			Tokens = tokens;
			Description = description;
			DataGroup = dataGroup;
			BuiltIn = builtIn;
		}

		/// <summary>
		/// Gets the name of the authority group.
		/// </summary>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets the name of the authority group.
		/// </summary>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Gets a bool signaling if the authority group is a built-in group.
		/// </summary>
		[DataMember]
		public bool BuiltIn { get; set; }


		/// <summary>
		/// Gets a bool signaling if the authority group is for Data access.
		/// </summary>
		[DataMember]
		public bool DataGroup { get; set; }

		/// <summary>
		/// Gets the set of tokens that are assigned to the group.
		/// </summary>
		[DataMember]
		public string[] Tokens { get; set; }
	}
}

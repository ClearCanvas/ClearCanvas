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
    /// Helper class for providing authority group definitions to be imported at deployment time.
    /// </summary>
	/// <seealso cref="AuthorityTokenAttribute"/>
	public class AuthorityGroupDefinition
    {
        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">The name of the authority group.</param>
		/// <param name="tokens">The associated authority group tokens.</param>
		/// <param name="dataGroup">Tells if the group is an authority group for controlling access to data.</param>
		/// <param name="description">The description of the authority group.</param>
        public AuthorityGroupDefinition(string name, string description, bool dataGroup, string[] tokens)
        {
            Name = name;
            Tokens = tokens;
		    Description = description;
            DataGroup = dataGroup;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the authority group.</param>
        /// <param name="tokens">The associated authority group tokens.</param>
        public AuthorityGroupDefinition(string name, string[] tokens)
        {
            Name = name;
            Tokens = tokens;
            Description = name;
            DataGroup = false;
        }

        /// <summary>
        /// Gets the name of the authority group.
        /// </summary>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// Gets the name of the authority group.
        /// </summary>
        public string Description
        {
            get; private set;
        }


        /// <summary>
        /// Gets a bool signaling if the authority group is for Data access.
        /// </summary>
        public bool DataGroup
        {
            get; private set;
        }


        /// <summary>
        /// Gets the set of tokens that are assigned to the group.
        /// </summary>
        public string[] Tokens
        {
            get; private set;
        }
    }
}

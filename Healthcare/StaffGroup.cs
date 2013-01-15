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
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// StaffGroup entity
    /// </summary>
	public partial class StaffGroup : ClearCanvas.Enterprise.Core.Entity
    {
        /// <summary>
        /// Add a member to this staff group.
        /// </summary>
        /// <param name="member"></param>
        public virtual void AddMember(Staff member)
        {
            _members.Add(member);
            member.Groups.Add(this);
        }

        /// <summary>
        /// Remove a member from this staff group.
        /// </summary>
        /// <param name="member"></param>
        public virtual void RemoveMember(Staff member)
        {
            _members.Remove(member);
            member.Groups.Remove(this);
        }


	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}
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

namespace ClearCanvas.Enterprise.Common
{
    [Serializable]
    public class AuthorityGroupIsNotEmptyException : Exception
    {
        public string GroupName { get; set; }
        public int UserCount { get; set; }

        
        public AuthorityGroupIsNotEmptyException(string groupName, int userCount)
            : base(userCount == 1 ? string.Format(SR.ExceptionAuthorityGroupIsNotEmpty_OneUser, groupName) : string.Format(SR.ExceptionAuthorityGroupIsNotEmpty_MultipleUsers, groupName, userCount))
        {
            GroupName = groupName;
            UserCount = userCount;
        }

        /// <summary>
        /// Creates an instance of <see cref="AuthorityGroupIsNotEmptyException"/> from the serialization stream.
        /// This constructor is used by the client.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public AuthorityGroupIsNotEmptyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // get the custom properties out of the serialization stream and 
            // set the object's properties
            GroupName = info.GetString("GroupName");
            UserCount = info.GetInt32("UserCount");
        }

        
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // add the custom properties into the serialization stream
            info.AddValue("GroupName", GroupName);
            info.AddValue("UserCount", UserCount);

            base.GetObjectData(info, context);
        }
    }
}
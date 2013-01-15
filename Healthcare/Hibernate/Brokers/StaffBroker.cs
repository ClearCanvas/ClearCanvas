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
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Hql;
using System.Collections;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
    public partial class StaffBroker
    {
        // JR: this is no longer needed, but left it here because it is likely we'll need to do something similar in future
        // if we end up defining a User shadow class within Healthcare

        //public Staff FindStaffForUser(string userName)
        //{
        //    HqlQuery query = new HqlQuery("from Staff s");
        //    query.Conditions.Add(new HqlCondition("s.User.UserName = ?", new object[] { userName }));

        //    IList<Staff> results = this.ExecuteHql<Staff>(query);
        //    if (results.Count > 0)
        //    {
        //        return results[0];
        //    }
        //    else
        //    {
        //        throw new EntityNotFoundException(string.Format(SR.ErrorNoStaffForUser, userName), null);
        //    }
        //}
    }
}

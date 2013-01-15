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
using ClearCanvas.Enterprise.Hibernate.Ddl.Migration;
using NHibernate.Dialect;
using ClearCanvas.Common.Utilities;
using NHibernate.Cfg;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration.Renderers
{
	/// <summary>
	/// Implementation of <see cref="IRenderer"/> for MS-SQL 2000 and greater.
	/// </summary>
    class MsSqlRenderer : Renderer
    {
		public MsSqlRenderer(Configuration config)
			:base(config)
		{
		}

        public override IEnumerable<RelationalModelChange> PreFilter(IEnumerable<RelationalModelChange> changes)
        {
            List<RelationalModelChange> filtered = new List<RelationalModelChange>(changes);
            foreach (RelationalModelChange change in changes)
            {
                // if a primary key is being added
                if (change is AddPrimaryKeyChange)
                {
                    // check if the table is being added, in which case
                    // the primary key will be embedded in the CREATE TABLE statement
                    if (IsTableAdded(changes, change.Table))
                        filtered.Remove(change);
                }

                // if a primary key is being dropped
                if (change is DropPrimaryKeyChange)
                {
                    // check if the table is being dropped, in which case
                    // the primary key will be dropped automatically
                    if (IsTableDropped(changes, change.Table))
                        filtered.Remove(change);
                }
            }

			return base.PreFilter(filtered);
        }

        private bool IsTableAdded(IEnumerable<RelationalModelChange> changes, TableInfo table)
        {
            return CollectionUtils.Contains(changes,
                delegate(RelationalModelChange c)
                {
                    return c is AddTableChange && Equals(c.Table, table);
                });
        }

        private bool IsTableDropped(IEnumerable<RelationalModelChange> changes, TableInfo table)
        {
            return CollectionUtils.Contains(changes,
                delegate(RelationalModelChange c)
                {
                    return c is DropTableChange && Equals(c.Table, table);
                });
        }
    }
}

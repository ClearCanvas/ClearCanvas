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

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer
{
    public class PersistentStoreVersionSelectCriteria : EntitySelectCriteria
    {
        public PersistentStoreVersionSelectCriteria()
            : base("DatabaseVersion_")
        { }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        [EntityFieldDatabaseMapping(TableName = "DatabaseVersion_", ColumnName = "Build_")]
        public ISearchCondition<System.String> Build
        {
            get
            {
                if (!SubCriteria.ContainsKey("Build_"))
                {
                    SubCriteria["Build_"] = new SearchCondition<System.String>("Build_");
                }
                return (ISearchCondition<System.String>)SubCriteria["Build_"];
            }
        }
        [EntityFieldDatabaseMapping(TableName = "DatabaseVersion_", ColumnName = "Major_")]
        public ISearchCondition<System.String> Major
        {
            get
            {
                if (!SubCriteria.ContainsKey("Major_"))
                {
                    SubCriteria["Major_"] = new SearchCondition<System.String>("Major_");
                }
                return (ISearchCondition<System.String>)SubCriteria["Major_"];
            }
        }
        [EntityFieldDatabaseMapping(TableName = "DatabaseVersion_", ColumnName = "Minor_")]
        public ISearchCondition<System.String> Minor
        {
            get
            {
                if (!SubCriteria.ContainsKey("Minor_"))
                {
                    SubCriteria["Minor_"] = new SearchCondition<System.String>("Minor_");
                }
                return (ISearchCondition<System.String>)SubCriteria["Minor_"];
            }
        }
        [EntityFieldDatabaseMapping(TableName = "DatabaseVersion_", ColumnName = "Revision_")]
        public ISearchCondition<System.String> Revision
        {
            get
            {
                if (!SubCriteria.ContainsKey("Revision_"))
                {
                    SubCriteria["Revision_"] = new SearchCondition<System.String>("Revision_");
                }
                return (ISearchCondition<System.String>)SubCriteria["Revision_"];
            }
        }
    }
}
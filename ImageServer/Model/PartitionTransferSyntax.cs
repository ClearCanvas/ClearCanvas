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
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model
{
	public partial class PartitionTransferSyntax
	{
        #region Private Members
        private String _description;
        private Boolean _lossless;
        private String _uid;
        #endregion

        #region Public Properties
		[EntityFieldDatabaseMapping(TableName = "PartitionTransferSyntax", ColumnName = "Description")]
        public String Description
        {
        get { return _description; }
        set { _description = value; }
        }

		[EntityFieldDatabaseMappingAttribute(TableName = "PartitionTransferSyntax", ColumnName = "Lossless")]
        public Boolean Lossless
        {
        get { return _lossless; }
        set { _lossless = value; }
        }

		[EntityFieldDatabaseMappingAttribute(TableName = "PartitionTransferSyntax", ColumnName = "Uid")]
		public String Uid
        {
        get { return _uid; }
        set { _uid = value; }
        }
        #endregion
	}
}

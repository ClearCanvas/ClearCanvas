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
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class AddressTable : Table<AddressDetail>
    {
        public AddressTable()
        {
            this.Columns.Add(new TableColumn<AddressDetail, string>(SR.ColumnType,
                delegate(AddressDetail a) { return a.Type.Value; }, 
                1.1f));
            this.Columns.Add(new TableColumn<AddressDetail, string>(SR.ColumnAddress,
                delegate(AddressDetail a) { return AddressFormat.Format(a); }, 
                2.2f));
            this.Columns.Add(new DateTableColumn<AddressDetail>(SR.ColumnExpiryDate,
                delegate(AddressDetail a) { return a.ValidRangeUntil; }, 
                0.9f));

        }
    }
}

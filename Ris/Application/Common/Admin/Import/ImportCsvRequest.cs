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
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.Import
{
    [DataContract]
    public class ImportCsvRequest : DataContractBase
    {
        public ImportCsvRequest()
        {

        }

        public ImportCsvRequest(string importer, List<string> rows)
        {
            this.Importer = importer;
            this.Rows = rows;
        }

        /// <summary>
        /// Name of the importer extension that will import the data.
        /// </summary>
        [DataMember]
        public string Importer;

        /// <summary>
        /// Collection of rows of data to import (e.g. from a CSV file).
        /// </summary>
        [DataMember]
        public List<string> Rows;
    }
}

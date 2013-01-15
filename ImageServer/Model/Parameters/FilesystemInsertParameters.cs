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
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Model.Parameters
{
    /// <summary>
    /// Input parameters for <see cref="IInsertFilesystem"/>.
    /// </summary>
    public class FilesystemInsertParameters : ProcedureParameters
    {
        public FilesystemInsertParameters()
            : base("InsertFilesystem")
        {
        }

        public FilesystemTierEnum TypeEnum
        {
            set { SubCriteria["FilesystemTierEnum"] = new ProcedureParameter<ServerEnum>("FilesystemTierEnum", value); }
        }

        public String FilesystemPath
        {
            set { SubCriteria["FilesystemPath"] = new ProcedureParameter<String>("FilesystemPath", value); }
        }

        public String Description
        {
            set { SubCriteria["Description"] = new ProcedureParameter<String>("Description", value); }
        }

        public bool Enabled
        {
            set { SubCriteria["Enabled"] = new ProcedureParameter<bool>("Enabled", value); }
        }

        public bool ReadOnly
        {
            set { SubCriteria["ReadOnly"] = new ProcedureParameter<bool>("ReadOnly", value); }
        }

        public bool WriteOnly
        {
            set { SubCriteria["WriteOnly"] = new ProcedureParameter<bool>("WriteOnly", value); }
        }

        public Decimal HighWatermark
        {
            set { SubCriteria["HighWatermark"] = new ProcedureParameter<Decimal>("HighWatermark", value); }
        }

        public Decimal LowWatermark
        {
            set { SubCriteria["LowWatermark"] = new ProcedureParameter<Decimal>("LowWatermark", value); }
        }

        public Decimal PercentFull
        {
            set { SubCriteria["PercentFull"] = new ProcedureParameter<Decimal>("PercentFull", value); }
        }
    }
}

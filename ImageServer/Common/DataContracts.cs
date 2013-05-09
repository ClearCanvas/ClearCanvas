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
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageServer.Common
{
    public static class ImageServerNamespace
    {
        public const string Value = "http://www.clearcanvas.ca/imageserver";
    }

    public abstract class ImageServerRequestItem : DataContractBase
    {


    }

    public abstract class ImageServerProgressItem : DataContractBase
    {
        public virtual string Status { get { return string.Empty; } }

        [DataMember(IsRequired = false)]
        public string StatusDetails { get; set; }

        public virtual Decimal PercentComplete { get { return new decimal(0.0); } }

        public virtual Decimal PercentFailed { get { return new decimal(0.0); } }

        [DataMember(IsRequired = true)]
        public bool IsCancelable { get; set; }
    }
}

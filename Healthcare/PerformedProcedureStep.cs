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
using ClearCanvas.Workflow;
using ClearCanvas.Common;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract class that roughly represents the notion of an IHE/DICOM General Purpose Performed Procedure Step (GP-PPS)
    /// </summary>
    public abstract class PerformedProcedureStep : PerformedStep
    {
		private IDictionary<string, string> _extendedProperties = new Dictionary<string, string>();


        public PerformedProcedureStep(Staff performingStaff)
            : base(new ProcedureStepPerformer(performingStaff))
        {
        }

        public PerformedProcedureStep(Staff performingStaff, DateTime? startTime)
            : base(new ProcedureStepPerformer(performingStaff), startTime)
        {
        }

        public PerformedProcedureStep()
        {
        }

		public virtual IDictionary<string, string> ExtendedProperties
        {
            get { return _extendedProperties; }
            protected set { _extendedProperties = value; }
        }

    }
}

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
using System.IO;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.PatientReconciliation;

namespace ClearCanvas.Healthcare.Alerts
{
    [ExtensionOf(typeof(PatientProfileAlertExtensionPoint))]
    public class ReconciliationAlert : PatientProfileAlertBase
    {
		public override string Id
		{
			get { return "ReconciliationAlert"; }
		}
		
		public override AlertNotification Test(PatientProfile profile, IPersistenceContext context)
        {
            IPatientReconciliationStrategy strategy = (IPatientReconciliationStrategy)(new PatientReconciliationStrategyExtensionPoint()).CreateExtension();

            IList<PatientProfileMatch> matches = strategy.FindReconciliationMatches(profile, context);
            if (matches.Count > 0)
            {
                return new AlertNotification(this.Id, new string[]{});
            }

            return null;
        }
    }
}

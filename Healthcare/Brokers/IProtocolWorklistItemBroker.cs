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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Brokers
{
    /// <summary>
    /// Defines an interface to a worklist item broker for protocol worklist items.
    /// </summary>
    public interface IProtocolWorklistItemBroker : IWorklistItemBroker
    {
		/// <summary>
		/// Maps the specified set of protocolling steps to a corresponding set of reporting worklist items.
		/// </summary>
		/// <param name="protocollingSteps"></param>
		/// <returns></returns>
		IList<ReportingWorklistItem> GetWorklistItems(IEnumerable<ProtocolProcedureStep> protocollingSteps, WorklistItemField timeField);

		/// <summary>
		/// Obtains a set of protocol steps that are candidates for linked protocolling to the specified assignment step.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="author"></param>
		/// <returns></returns>
		IList<ProtocolAssignmentStep> GetLinkedProtocolCandidates(ProtocolAssignmentStep step, Staff author);
	}
}

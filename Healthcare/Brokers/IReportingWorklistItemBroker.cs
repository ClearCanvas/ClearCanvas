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

using System.Collections.Generic;
using ClearCanvas.Healthcare.Workflow.Reporting;

namespace ClearCanvas.Healthcare.Brokers
{
    /// <summary>
    /// Defines an interface to a worklist item broker for registration worklist items.
    /// </summary>
    public interface IReportingWorklistItemBroker : IWorklistItemBroker
    {
        /// <summary>
        /// Maps the specified set of reporting steps to a corresponding set of reporting worklist items.
        /// </summary>
        /// <param name="reportingSteps"></param>
        /// <returns></returns>
		IList<ReportingWorklistItem> GetWorklistItems(IEnumerable<ReportingProcedureStep> reportingSteps, WorklistItemField timeField);

        /// <summary>
        /// Obtains a set of interpretation steps that are candidates for linked reporting to the specified interpretation step.
        /// </summary>
        /// <param name="step"></param>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        IList<InterpretationStep> GetLinkedInterpretationCandidates(InterpretationStep step, Staff interpreter);
    }
}

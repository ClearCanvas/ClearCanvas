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

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    /// <summary>
    /// Request object for <see cref="IReportingWorkflowService.GetPriors"/>.
    /// </summary>
    /// <remarks>
    /// </remarks>
    [DataContract]
    public class GetPriorsRequest : DataContractBase
    {
        /// <summary>
        /// A report for which relevant priors are obtained.  Only one of ReportRef and OrderRef should be set.
        /// </summary>
        [DataMember]
        public EntityRef ReportRef;

        /// <summary>
		/// An order for which relevant priors are obtained.  Only one of ReportRef and OrderRef should be set.
        /// </summary>
        [DataMember]
        public EntityRef OrderRef;

		/// <summary>
		/// Specifies whether only relevant priors should be returned, as opposed to all priors for the patient.
		/// </summary>
		[DataMember]
    	public bool RelevantOnly;

    }
}

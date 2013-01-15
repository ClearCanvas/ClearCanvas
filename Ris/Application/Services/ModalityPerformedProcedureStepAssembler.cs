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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Application.Services
{
	public class ModalityPerformedProcedureStepAssembler
	{
		public ModalityPerformedProcedureStepDetail CreateModalityPerformedProcedureStepDetail(ModalityPerformedProcedureStep mpps, IPersistenceContext context)
		{
			var assembler = new ModalityProcedureStepAssembler();

			// include the details of each MPS in the mpps summary
			var mpsDetails = CollectionUtils.Map(mpps.Activities,
				(ProcedureStep mps) => assembler.CreateProcedureStepSummary(mps.As<ModalityProcedureStep>(), context));

			var dicomSeriesAssembler = new DicomSeriesAssembler();
			var dicomSeries = dicomSeriesAssembler.GetDicomSeriesDetails(mpps.DicomSeries);

			StaffSummary mppsPerformer = null;
			var performer = mpps.Performer as ProcedureStepPerformer;
			if (performer != null)
			{
				var staffAssembler = new StaffAssembler();
				mppsPerformer = staffAssembler.CreateStaffSummary(performer.Staff, context);
			}

			return new ModalityPerformedProcedureStepDetail(
				mpps.GetRef(),
				EnumUtils.GetEnumValueInfo(mpps.State, context),
				mpps.StartTime,
				mpps.EndTime,
				mppsPerformer,
				mpsDetails,
				dicomSeries,
				ExtendedPropertyUtils.Copy(mpps.ExtendedProperties));
		}
	}
}

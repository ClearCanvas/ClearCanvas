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
using System.Linq;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Application.Services.ModalityWorkflow
{
	class ModalityWorkflowServiceRecorder
	{
		static class Operations
		{
			public const string Start = "Procedures:Start";
			public const string End = "Procedures:End";
			public const string Discontinue = "Procedures:Discontinue";
			public const string DocumentationUpdate = "PerformingDocumentation:Update";
			public const string DocumentationComplete = "PerformingDocumentation:Complete";
		}

		internal class StartProcedures : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (StartModalityProcedureStepsRequest) recorderContext.Request;

				var procedures = request.ModalityProcedureSteps.Select(r => persistenceContext.Load<ModalityProcedureStep>(r, EntityLoadFlags.None).Procedure).ToList();
				var order = procedures.First().Order;
				var patientProfile = procedures.First().PatientProfile;

				return new OperationData(Operations.Start, patientProfile, order, procedures);
			}
		}

		internal class CompleteProcedures : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (CompleteModalityPerformedProcedureStepRequest)recorderContext.Request;

				var mpps = persistenceContext.Load<ModalityPerformedProcedureStep>(request.Mpps.ModalityPerformendProcedureStepRef, EntityLoadFlags.None);
				var procedures = mpps.Activities.Select(a => a.Downcast<ModalityProcedureStep>().Procedure).ToList();
				var order = procedures.First().Order;
				var patientProfile = procedures.First().PatientProfile;

				return new OperationData(Operations.End, patientProfile, order, procedures);
			}
		}

		internal class DiscontinueProcedures : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				if (recorderContext.Request is DiscontinueModalityProcedureStepsRequest)
					return Capture((DiscontinueModalityProcedureStepsRequest) recorderContext.Request, persistenceContext);
				if (recorderContext.Request is DiscontinueModalityPerformedProcedureStepRequest)
					return Capture((DiscontinueModalityPerformedProcedureStepRequest)recorderContext.Request, persistenceContext);
				throw new InvalidOperationException("Cannot audit this request");
			}

			private static OperationData Capture(DiscontinueModalityProcedureStepsRequest request, IPersistenceContext persistenceContext)
			{
				var procedures = request.ModalityProcedureSteps.Select(r => persistenceContext.Load<ModalityProcedureStep>(r, EntityLoadFlags.None).Procedure).ToList();
				var order = procedures.First().Order;
				var patientProfile = procedures.First().PatientProfile;

				return new OperationData(Operations.Discontinue, patientProfile, order, procedures);
			}

			private static OperationData Capture(DiscontinueModalityPerformedProcedureStepRequest request, IPersistenceContext persistenceContext)
			{
				var mpps = persistenceContext.Load<ModalityPerformedProcedureStep>(request.Mpps.ModalityPerformendProcedureStepRef, EntityLoadFlags.None);
				var procedures = mpps.Activities.Select(a => a.Downcast<ModalityProcedureStep>().Procedure).ToList();
				var order = procedures.First().Order;
				var patientProfile = procedures.First().PatientProfile;

				return new OperationData(Operations.Discontinue, patientProfile, order, procedures);
			}
		}

		internal class UpdateDocumentation : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (SaveOrderDocumentationDataRequest) recorderContext.Request;
				var order = persistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.None);
				var patientProfile = order.Procedures.First().PatientProfile;

				return new OperationData(Operations.DocumentationUpdate, patientProfile, order);
			}
		}

		internal class CompleteDocumentation : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var request = (CompleteOrderDocumentationRequest)recorderContext.Request;
				var order = persistenceContext.Load<Order>(request.OrderRef, EntityLoadFlags.None);
				var patientProfile = order.Procedures.First().PatientProfile;

				return new OperationData(Operations.DocumentationComplete, patientProfile, order);
			}
		}
	}
}

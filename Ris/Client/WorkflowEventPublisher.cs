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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client
{
	public static class WorkflowEventListener
	{
		public class PatientProfileInfo
		{
			public string Id { get; set; }
			public string GivenName { get; set; }
			public string FamilyName { get; set; }
			public DateTime? BirthDate { get; set; }
			public string Sex { get; set; }

			public override string ToString()
			{
				return String.Format("{0}: {1}, {2}", Id, FamilyName, GivenName);
			}
		}

		public abstract class WorkflowEventListenerArgs
		{
			protected WorkflowEventListenerArgs(DesktopWindow desktopWindow)
			{
				DesktopWindow = desktopWindow;
			}

			public DesktopWindow DesktopWindow { get; private set; }
		}

		public class PerformedProcedureStepCompletedArgs : WorkflowEventListenerArgs
		{
			public PerformedProcedureStepCompletedArgs(DesktopWindow desktopWindow, PatientProfileInfo patientProfile, string accessionNumber, List<ModalityProcedureStepSummary> modalityProcedureSteps, DateTime completedTime)
				: base(desktopWindow)
			{
				PatientProfile = patientProfile;
				CompletedTime = completedTime;
				AccessionNumber = accessionNumber;
				ModalityProcedureSteps = modalityProcedureSteps;
			}

			public PatientProfileInfo PatientProfile { get; private set; }
			public DateTime CompletedTime { get; private set; }
			public string AccessionNumber { get; private set; }

			/// <summary>
			/// Modality procedure steps that were performed with this performed procedure step.
			/// </summary>
			public List<ModalityProcedureStepSummary> ModalityProcedureSteps;

			public string ProcedureTypes
			{
				get { return string.Join("; ", ModalityProcedureSteps.Select(mps => mps.Procedure.Type.Name)); }
			}

			public override string ToString()
			{
				return String.Format("{0}: {1}, {2}", ProcedureTypes, AccessionNumber, CompletedTime);
			}
		}

		public enum OrderSubmitType
		{
			New,
			Modify,
			Replace
		}

		public abstract class OrderEditorArgs : WorkflowEventListenerArgs
		{
			protected OrderEditorArgs(DesktopWindow desktopWindow, PatientProfileSummary patientProfile, OrderRequisition requisition, OrderSubmitType submitType)
				: base(desktopWindow)
			{
				PatientProfile = patientProfile;
				Requisition = requisition;
				SubmitType = submitType;
			}

			public PatientProfileSummary PatientProfile { get; private set; }
			public OrderRequisition Requisition { get; set; }
			public OrderSubmitType SubmitType { get; private set; }
		}

		public class OrderSubmittingArgs : OrderEditorArgs
		{
			public OrderSubmittingArgs(DesktopWindow desktopWindow, PatientProfileSummary patientProfile, OrderRequisition requisition, OrderSubmitType submitType)
				: base(desktopWindow, patientProfile, requisition, submitType)
			{
			}

			public bool Cancel { get; set; }
		}

		public class OrderSubmittedArgs : OrderEditorArgs
		{
			public OrderSubmittedArgs(DesktopWindow desktopWindow, PatientProfileSummary patientProfile, OrderRequisition requisition, OrderSubmitType submitType)
				: base(desktopWindow, patientProfile, requisition, submitType)
			{
			}
		}
	}

	public interface IWorkflowEventListener
	{
		void OrderSubmitting(WorkflowEventListener.OrderSubmittingArgs args);
		void OrderSubmitted(WorkflowEventListener.OrderSubmittedArgs args);

		void PerformedProcedureStepCompleted(WorkflowEventListener.PerformedProcedureStepCompletedArgs args);
	}

	public class WorkflowEventListenerBase : IWorkflowEventListener
	{
		public virtual void OrderSubmitting(WorkflowEventListener.OrderSubmittingArgs args)
		{
		}

		public virtual void OrderSubmitted(WorkflowEventListener.OrderSubmittedArgs args)
		{
		}

		public virtual void PerformedProcedureStepCompleted(WorkflowEventListener.PerformedProcedureStepCompletedArgs args)
		{
		}
	}

	[ExtensionPoint]
	public class WorkflowEventListenerExtensionPoint : ExtensionPoint<IWorkflowEventListener>
	{
	}

	public class WorkflowEventPublisher : IWorkflowEventListener
	{
		private static readonly WorkflowEventPublisher _instance = new WorkflowEventPublisher();

		/// <summary>
		/// Singleton static instance.
		/// </summary>
		public static WorkflowEventPublisher Instance
		{
			get { return _instance; }
		}


		private readonly object[] _listeners;

		private WorkflowEventPublisher()
		{
			_listeners = new WorkflowEventListenerExtensionPoint().CreateExtensions();
		}


		#region Implementation of IWorkflowEventListener

		public void OrderSubmitting(WorkflowEventListener.OrderSubmittingArgs args)
		{
			ForEachListener(listener => listener.OrderSubmitting(args));
		}

		public void OrderSubmitted(WorkflowEventListener.OrderSubmittedArgs args)
		{
			ForEachListener(listener => listener.OrderSubmitted(args));
		}

		public void PerformedProcedureStepCompleted(WorkflowEventListener.PerformedProcedureStepCompletedArgs args)
		{
			ForEachListener(listener => listener.PerformedProcedureStepCompleted(args));
		}

		#endregion

		private void ForEachListener(Action<IWorkflowEventListener> action)
		{
			lock(_listeners)
			{
				foreach (IWorkflowEventListener listener in _listeners)
				{
					try
					{
						action(listener);
					}
					catch (Exception e)
					{
						Platform.Log(LogLevel.Error, e);
					}
				}
			}
		}
	}
}

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
using System.Text;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="CheckInOrderComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class CheckInOrderComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// CheckInOrderComponent class
	/// </summary>
	[AssociateView(typeof(CheckInOrderComponentViewExtensionPoint))]
	public class CheckInOrderComponent : ApplicationComponent
	{
		private readonly List<ProcedureSummary> _procedures;
		private CheckInOrderTable _checkInOrderTable;
		private DateTime _checkInTime;

		/// <summary>
		/// Constructor
		/// </summary>
		internal CheckInOrderComponent(List<ProcedureSummary> procedures)
		{
			_procedures = procedures;
		}

		public override void Start()
		{
			_checkInOrderTable = new CheckInOrderTable();
			_checkInTime = Platform.Time;

			_checkInOrderTable.Items.AddRange(
				CollectionUtils.Map(_procedures,
						delegate(ProcedureSummary item)
						{
							var entry = new CheckInOrderTableEntry(item);
							entry.CheckedChanged += OrderCheckedStateChangedEventHandler;
							return entry;
						}));

			base.Start();
		}

		#region Presentation Model

		public ITable OrderTable
		{
			get { return _checkInOrderTable; }
		}

		public DateTime CheckInTime
		{
			get { return _checkInTime; }
			set { _checkInTime = value; }
		}

		public bool CheckInTimeVisible
		{
			get { return DowntimeRecovery.InDowntimeRecoveryMode; }
		}

		public bool AcceptEnabled
		{
			get { return CollectionUtils.Contains(_checkInOrderTable.Items, entry => entry.Checked); }
		}

		#endregion

		public void Accept()
		{
			var checkedEntries = CollectionUtils.Select(_checkInOrderTable.Items, entry => entry.Checked);

			if (!Confirm(checkedEntries))
				return;

			try
			{
				var checkedProcedureRefs = CollectionUtils.Map(checkedEntries, (CheckInOrderTableEntry entry) => entry.Procedure.ProcedureRef);
				Platform.GetService((IRegistrationWorkflowService service) => service.CheckInProcedure(
					new CheckInProcedureRequest(checkedProcedureRefs, 
						DowntimeRecovery.InDowntimeRecoveryMode ? (DateTime?) _checkInTime : null)));

				Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void Cancel()
		{
			Exit(ApplicationComponentExitCode.None);
		}

		private void OrderCheckedStateChangedEventHandler(object sender, EventArgs e)
		{
			NotifyPropertyChanged("AcceptEnabled");
		}

		private bool Confirm(IEnumerable<CheckInOrderTableEntry> checkedEntries)
		{
			var warnProcedures = new List<ProcedureSummary>();
			var errorProcedures = new List<ProcedureSummary>();

			// Get the list of Order EntityRef from the table
			foreach (var entry in checkedEntries)
			{
				string checkInValidationMessage;
				var result = CheckInSettings.Validate(entry.Procedure.ScheduledStartTime, _checkInTime, out checkInValidationMessage);
				switch (result)
				{
					case CheckInSettings.ValidateResult.TooEarly:
					case CheckInSettings.ValidateResult.TooLate:
						warnProcedures.Add(entry.Procedure);
						break;
					case CheckInSettings.ValidateResult.NotScheduled:
						errorProcedures.Add(entry.Procedure);
						break;
				}
			}

			if (errorProcedures.Count > 0)
			{
				var messageBuilder = new StringBuilder();
				messageBuilder.Append(SR.MessageCheckInProceduresNotScheduled);
				messageBuilder.AppendLine();
				messageBuilder.AppendLine();
				CollectionUtils.ForEach(errorProcedures, procedure => messageBuilder.AppendLine(ProcedureFormat.Format(procedure)));

				this.Host.DesktopWindow.ShowMessageBox(messageBuilder.ToString(), MessageBoxActions.Ok);
				return false;
			}

			if (warnProcedures.Count > 0)
			{
				var earlyThreshold = TimeSpan.FromMinutes(CheckInSettings.Default.EarlyCheckInWarningThreshold);
				var lateThreshold = TimeSpan.FromMinutes(CheckInSettings.Default.LateCheckInWarningThreshold);

				var messageBuilder = new StringBuilder();
				messageBuilder.AppendFormat(SR.MessageCheckInProceduresTooLateOrTooEarly,
											TimeSpanFormat.FormatDescriptive(earlyThreshold),
											TimeSpanFormat.FormatDescriptive(lateThreshold));
				messageBuilder.AppendLine();
				messageBuilder.AppendLine();
				CollectionUtils.ForEach(warnProcedures, procedure => messageBuilder.AppendLine(ProcedureFormat.Format(procedure)));
				messageBuilder.AppendLine();
				messageBuilder.Append(SR.MessageConfirmCheckInProcedures);

				if (DialogBoxAction.No == this.Host.DesktopWindow.ShowMessageBox(messageBuilder.ToString(), MessageBoxActions.YesNo))
				{
					return false;
				}
			}
			return true;
		}

	}
}

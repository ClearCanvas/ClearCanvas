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
using ClearCanvas.Common;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// ReportPart component
	/// </summary>
	public partial class ReportPart
	{
		/// <summary>
		/// Constructor used by <see cref="Report"/> class.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="index"></param>
		internal ReportPart(Report owner, int index)
		{
			_report = owner;
			_index = index;
			_extendedProperties = new Dictionary<string, string>();
			_creationTime = Platform.Time;
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		/// <summary>
		/// Gets a value indicating whether this report part is an addendum.
		/// </summary>
		public virtual bool IsAddendum
		{
			get { return _index > 0; }
		}

		/// <summary>
		/// Gets a value indicating whether this report part is modifiable,
		/// which is true if the status is either <see cref="ReportPartStatus.D"/> or <see cref="ReportPartStatus.P"/>.
		/// </summary>
		public virtual bool IsModifiable
		{
			get { return _status == ReportPartStatus.D || _status == ReportPartStatus.P; }
		}

		/// <summary>
		/// Marks this report part as being preliminary.
		/// </summary>
		public virtual void MarkPreliminary()
		{
			if (_status != ReportPartStatus.D)
				throw new WorkflowException(string.Format("Cannot transition from {0} to P", _status));

			_preliminaryTime = Platform.Time;
			SetStatus(ReportPartStatus.P);
		}

		/// <summary>
		/// Marks this report part as being complete (status Final).
		/// </summary>
		public virtual void Complete()
		{
			if (_status == ReportPartStatus.X || _status == ReportPartStatus.F)
				throw new WorkflowException(string.Format("Cannot transition from {0} to F", _status));

			_completedTime = Platform.Time;
			SetStatus(ReportPartStatus.F);
		}

		/// <summary>
		/// Marks this report part as being cancelled (status Cancelled).
		/// </summary>
		public virtual void Cancel()
		{
			if (_status == ReportPartStatus.X || _status == ReportPartStatus.F)
				throw new WorkflowException(string.Format("Cannot transition from {0} to X", _status));

			_cancelledTime = Platform.Time;
			SetStatus(ReportPartStatus.X);
		}

		/// <summary>
		/// Removes transient properties related to rejected transcriptions.  These properties may no longer be valid if a report part
		/// is re-submitted for transcription.
		/// </summary>
		public virtual void ResetTranscription()
		{
			if (!this.IsModifiable)
				throw new WorkflowException("Cannot change transcription details for a completed report part.");

			_transcriptionRejectReason = null;
			_transcriptionSupervisor = null;
		}

		/// <summary>
		/// Helper method to change the status and also notify the parent report to change its status
		/// if necessary.
		/// </summary>
		/// <param name="status"></param>
		private void SetStatus(ReportPartStatus status)
		{
			_status = status;

			_report.UpdateStatus();
		}

		/// <summary>
		/// Shifts the object in time by the specified number of minutes, which may be negative or positive.
		/// </summary>
		/// <remarks>
		/// The method is not intended for production use, but is provided for the purpose
		/// of generating back-dated data for demos and load-testing.
		/// </remarks>
		/// <param name="minutes"></param>
		protected internal virtual void TimeShift(int minutes)
		{
			_creationTime = _creationTime.AddMinutes(minutes);
			_preliminaryTime = _preliminaryTime.HasValue ? _preliminaryTime.Value.AddMinutes(minutes) : _preliminaryTime;
			_completedTime = _completedTime.HasValue ? _completedTime.Value.AddMinutes(minutes) : _completedTime;
			_cancelledTime = _cancelledTime.HasValue ? _cancelledTime.Value.AddMinutes(minutes) : _cancelledTime;
		}
	}
}
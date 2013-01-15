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

using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Protocol entity
	/// </summary>
	public partial class Protocol : ClearCanvas.Enterprise.Core.Entity
	{
		public Protocol(Procedure procedure)
			: this()
		{
			_procedures.Add(procedure);
			procedure.Protocols.Add(this);
		}

		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

		public virtual void Accept()
		{
			_status = ProtocolStatus.PR;
		}

		public virtual void Reject(ProtocolRejectReasonEnum reason)
		{
			_status = ProtocolStatus.RJ;
			_rejectReason = reason;
		}

		public virtual void Resolve()
		{
			_status = ProtocolStatus.PN;
			_rejectReason = null;
		}

		public virtual void SubmitForApproval()
		{
			_status = ProtocolStatus.AA;
		}

		public virtual void Cancel()
		{
			_status = ProtocolStatus.X;
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
			// no times to shift
		}

		/// <summary>
		/// Links a <see cref="Procedure"/> to this report, meaning that the protocol covers
		/// this radiology procedure.
		/// </summary>
		/// <param name="procedure"></param>
		protected internal virtual void LinkProcedure(Procedure procedure)
		{
			if (_procedures.Contains(procedure))
				throw new WorkflowException("The procedure is already associated with this protocol.");

			// does the procedure already have a non-new protocol?
			Protocol otherProtocol = procedure.ActiveProtocol;
			if (otherProtocol.IsNew() == false && !this.Equals(otherProtocol))
				throw new WorkflowException("Cannot link this procedure because it already has an active protocol.");

			_procedures.Add(procedure);
			procedure.Protocols.Add(this);

            // dissociate the otherProtocol from the procedure
            // (ideally we should delete otherProtocol too, but how do we do that from here?)
            otherProtocol.Procedures.Remove(procedure);
        }

		protected internal virtual bool IsNew()
		{
			if (_status != ProtocolStatus.PN)
				return false;
			if (_author != null)
				return false;
			if (!_codes.IsEmpty)
				return false;
			return true;
		}
	}
}
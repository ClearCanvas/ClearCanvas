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

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Represents the action taken by the application entity upon receiving a transfer of DICOM instances.
	/// </summary>
	/// <remarks>
	/// In actuality, each <see cref="EventReceiptAction"/> has a 1-to-1 mapping with a <see cref="EventIdentificationContentsEventActionCode"/>,
	/// but <see cref="EventReceiptAction"/> uses <see cref="DicomAuditHelper"/> to abstract away any requirement for knowledge of the
	/// underlying audit types defined in the DICOM toolkit.
	/// </remarks>
	public sealed class EventReceiptAction
	{
		/// <summary>
		/// The device does not already have these instances, and hence created new ones.
		/// </summary>
		public static readonly EventReceiptAction CreateNew = new EventReceiptAction(EventIdentificationContentsEventActionCode.C);

		/// <summary>
		/// The device already has these instances, has determined them to be no different from the arriving ones, and hence did not perform any action.
		/// </summary>
		public static readonly EventReceiptAction KeepExisting = new EventReceiptAction(EventIdentificationContentsEventActionCode.R);

		/// <summary>
		/// The device already has these instances, has determined them to be different from the arriving ones, and hence updated the existing ones.
		/// </summary>
		public static readonly EventReceiptAction UpdateExisting = new EventReceiptAction(EventIdentificationContentsEventActionCode.U);

		/// <summary>
		/// The action that the receiving device took is unknown.
		/// </summary>
		public static readonly EventReceiptAction ActionUnknown = new EventReceiptAction(EventIdentificationContentsEventActionCode.E);

		private readonly EventIdentificationContentsEventActionCode _action;

		private EventReceiptAction(EventIdentificationContentsEventActionCode action)
		{
			_action = action;
		}

		public static implicit operator EventIdentificationContentsEventActionCode(EventReceiptAction operand)
		{
			return operand._action;
		}
	}
}
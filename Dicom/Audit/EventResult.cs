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
	/// Represents the result of a particular auditable event.
	/// </summary>
	/// <remarks>
	/// In actuality, each <see cref="EventResult"/> has a 1-to-1 mapping with a <see cref="EventIdentificationContentsEventOutcomeIndicator"/>,
	/// but <see cref="EventResult"/> uses <see cref="AuditLogHelper"/> to abstract away any requirement for knowledge of the
	/// underlying audit types defined in the DICOM toolkit.
	/// </remarks>
	public sealed class EventResult
	{
		/// <summary>
		/// The auditable event completed successfully.
		/// </summary>
		public static readonly EventResult Success = new EventResult(EventIdentificationContentsEventOutcomeIndicator.Success);

		/// <summary>
		/// The auditable event finished with minor errors.
		/// </summary>
		public static readonly EventResult MinorFailure = new EventResult(EventIdentificationContentsEventOutcomeIndicator.MinorFailureActionRestarted);

		/// <summary>
		/// The auditable event finished with major errors.
		/// </summary>
		public static readonly EventResult MajorFailure = new EventResult(EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable);

		/// <summary>
		/// The auditable event finished with serious errors.
		/// </summary>
		public static readonly EventResult SeriousFailure = new EventResult(EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated);

		private readonly EventIdentificationContentsEventOutcomeIndicator _outcome;

		private EventResult(EventIdentificationContentsEventOutcomeIndicator outcome)
		{
			_outcome = outcome;
		}

		/// <summary>
		/// Converts the <paramref name="operand"/> to the equivalent <see cref="EventIdentificationContentsEventOutcomeIndicator"/>.
		/// </summary>
		public static implicit operator EventIdentificationContentsEventOutcomeIndicator(EventResult operand)
		{
			return operand._outcome;
		}

		/// <summary>
		/// Converts the <paramref name="operand"/> to the equivalent <see cref="EventResult"/>.
		/// </summary>
		public static implicit operator EventResult(EventIdentificationContentsEventOutcomeIndicator operand)
		{
			switch (operand)
			{
				case EventIdentificationContentsEventOutcomeIndicator.Success:
					return Success;
				case EventIdentificationContentsEventOutcomeIndicator.MinorFailureActionRestarted:
					return MinorFailure;
				case EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated:
					return SeriousFailure;
				case EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable:
					return MajorFailure;
				default:
					return null;
			}
		}
	}
}
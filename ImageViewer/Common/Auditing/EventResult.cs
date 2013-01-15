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

using ClearCanvas.Dicom.Audit;
using DicomEventResult = ClearCanvas.Dicom.Audit.EventResult;

namespace ClearCanvas.ImageViewer.Common.Auditing
{
	/// <summary>
	/// Represents the result of a particular auditable event.
	/// </summary>
	/// <remarks>
	/// In actuality, each <see cref="EventResult"/> has a 1-to-1 mapping with a <see cref="EventIdentificationContentsEventOutcomeIndicator"/>,
	/// but <see cref="EventResult"/> uses <see cref="AuditHelper"/> to abstract away any requirement for knowledge of the
	/// underlying audit types defined in the DICOM toolkit.
	/// </remarks>
	public class EventResult
	{
		/// <summary>
		/// The auditable event completed successfully.
		/// </summary>
		public static DicomEventResult Success
		{
			get { return DicomEventResult.Success; }
		}

		/// <summary>
		/// The auditable event finished with minor errors.
		/// </summary>
		public static DicomEventResult MinorFailure
		{
			get { return DicomEventResult.MinorFailure; }
		}

		/// <summary>
		/// The auditable event finished with major errors.
		/// </summary>
		public static DicomEventResult MajorFailure
		{
			get { return DicomEventResult.MajorFailure; }
		}

		/// <summary>
		/// The auditable event finished with serious errors.
		/// </summary>
		public static DicomEventResult SeriousFailure
		{
			get { return DicomEventResult.SeriousFailure; }
		}

		private readonly DicomEventResult _result;

		private EventResult(DicomEventResult result)
		{
			_result = result;
		}

		public static implicit operator DicomEventResult(EventResult result)
		{
			return result._result;
		}

		public static implicit operator EventResult(DicomEventResult result)
		{
			return new EventResult(result);
		}
	}
}
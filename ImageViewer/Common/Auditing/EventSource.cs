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
using DicomEventSource = ClearCanvas.Dicom.Audit.EventSource;

namespace ClearCanvas.ImageViewer.Common.Auditing
{
	/// <summary>
	/// Represents the source of a particular auditable event.
	/// </summary>
	public class EventSource
	{
		/// <summary>
		/// The source of the auditable event is the current end user.
		/// </summary>
		public static DicomEventSource CurrentUser
		{
			get { return DicomEventSource.CurrentUser; }
		}

		/// <summary>
		/// Gets a DICOM AE event source when the source of the auditable event is the current DICOM server process.
		/// </summary>
		public static DicomEventSource CurrentProcess
		{
			get { return DicomAEEventSource.CurrentProcess; }
		}

		/// <summary>
		/// A generic source for when the actual source is unknown.
		/// </summary>
		public static DicomEventSource UnknownSource
		{
			get { return DicomEventSource.UnknownSource; }
		}

		/// <summary>
		/// Gets a DICOM AE event source when the source of the auditable event is the current DICOM server process.
		/// </summary>
		/// <remarks>
		/// This object represents the configuration of the DICOM server at the time this method was called. Subsequent changes to the configuration will not affect this value, event if the audit event has not been processed yet.
		/// </remarks>
		public static DicomEventSource GetCurrentDicomAE()
		{
			return new DicomAEEventSource(AuditHelper.LocalAETitle);
		}

		/// <summary>
		/// Gets a DICOM AE event source when the source of the auditable event is a DICOM application entity.
		/// </summary>
		public static DicomEventSource GetDicomAEEventSource(string aeTitle)
		{
			return new DicomAEEventSource(aeTitle);
		}

		/// <summary>
		/// Gets a generic event source for other sources of auditable events.
		/// </summary>
		/// <param name="otherSourceName">The name of the source.</param>
		public static DicomEventSource GetOtherEventSource(string otherSourceName)
		{
			return DicomEventSource.GetOtherEventSource(otherSourceName);
		}

		/// <summary>
		/// Gets a user event source when the specified user is the source of the auditable event.
		/// </summary>
		/// <param name="userName">The username of the source.</param>
		public static DicomEventSource GetUserEventSource(string userName)
		{
			return DicomEventSource.GetUserEventSource(userName);
		}

		private readonly DicomEventSource _source;

		private EventSource(DicomEventSource source)
		{
			_source = source;
		}

		public static implicit operator DicomEventSource(EventSource source)
		{
			return source._source;
		}

		public static implicit operator EventSource(DicomEventSource source)
		{
			return new EventSource(source);
		}

		private class DicomAEEventSource : DicomEventSource
		{
			public static readonly DicomAEEventSource CurrentProcess = new DicomAEEventSource(null);

			private readonly string _aeTitle;

			public DicomAEEventSource(string aeTitle)
			{
				_aeTitle = aeTitle;
			}

			protected override DicomAuditSource AsDicomAuditSource()
			{
				return new DicomAuditSource(_aeTitle ?? AuditHelper.LocalAETitle, string.Empty, AuditSourceTypeCodeEnum.ApplicationServerProcessTierInMultiTierSystem);
			}

			protected override AuditActiveParticipant AsAuditActiveParticipant()
			{
				return new AuditProcessActiveParticipant(_aeTitle ?? AuditHelper.LocalAETitle);
			}
		}
	}
}
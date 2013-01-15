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
using System.Collections;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare
{
	public class OrderNoteboxItem
	{
		private readonly EntityRef _noteRef;
		private readonly EntityRef _orderRef;
		private readonly EntityRef _patientRef;
		private readonly EntityRef _patientProfileRef;

		private readonly PatientIdentifier _mrn;
		private readonly PersonName _patientName;
		private readonly DateTime? _dateOfBirth;
		private readonly string _accessionNumber;
		private readonly string _diagnosticServiceName;
		private readonly string _category;
		private readonly bool _urgent;
		private readonly DateTime? _postTime;
		private readonly Staff _author;
		private readonly StaffGroup _onBehalfOfGroup;
		private readonly bool _isAcknowledged;
		private readonly IList _recipients;


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="note"></param>
		/// <param name="order"></param>
		/// <param name="patient"></param>
		/// <param name="patientProfile"></param>
		/// <param name="noteAuthor"></param>
		/// <param name="recipients"></param>
		/// <param name="diagnosticServiceName"></param>
		/// <param name="isAcknowledged"></param>
		public OrderNoteboxItem(Note note, Order order, Patient patient, PatientProfile patientProfile,
			Staff noteAuthor, IList recipients,
			string diagnosticServiceName, bool isAcknowledged)
		{
			_noteRef = note.GetRef();
			_orderRef = order.GetRef();
			_patientRef = patient.GetRef();
			_patientProfileRef = patientProfile.GetRef();
			_mrn = patientProfile.Mrn;
			_patientName = patientProfile.Name;
			_dateOfBirth = patientProfile.DateOfBirth;
			_accessionNumber = order.AccessionNumber;
			_diagnosticServiceName = diagnosticServiceName;
			_category = note.Category;
			_urgent = note.Urgent;
			_postTime = note.PostTime;
			_author = noteAuthor;
			_onBehalfOfGroup = note.OnBehalfOfGroup;
			_isAcknowledged = isAcknowledged;
			_recipients = recipients;
		}

		public EntityRef NoteRef
		{
			get { return _noteRef; }
		}

		public EntityRef OrderRef
		{
			get { return _orderRef; }
		}

		public EntityRef PatientRef
		{
			get { return _patientRef; }
		}

		public EntityRef PatientProfileRef
		{
			get { return _patientProfileRef; }
		}

		public PatientIdentifier Mrn
		{
			get { return _mrn; }
		}

		public PersonName PatientName
		{
			get { return _patientName; }
		}

		public DateTime? DateOfBirth
		{
			get { return _dateOfBirth; }
		}

		public string AccessionNumber
		{
			get { return _accessionNumber; }
		}

		public string DiagnosticServiceName
		{
			get { return _diagnosticServiceName; }
		}

		public string Category
		{
			get { return _category; }
		}

		public bool Urgent
		{
			get { return _urgent; }
		}

		public DateTime? PostTime
		{
			get { return _postTime; }
		}

		public Staff Author
		{
			get { return _author; }
		}

		public StaffGroup OnBehalfOfGroup
		{
			get { return _onBehalfOfGroup; }
		}

		public bool IsAcknowledged
		{
			get { return _isAcknowledged; }
		}

		public IList Recipients
		{
			get { return _recipients; }
		}
	}
}

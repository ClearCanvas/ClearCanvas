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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class PatientNoteEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(PatientNoteEditorComponentViewExtensionPoint))]
	public class PatientNoteEditorComponent : ApplicationComponent
	{
		private readonly bool _readOnlyMode;
		private readonly PatientNoteDetail _note;
		private readonly List<PatientNoteCategorySummary> _noteCategoryChoices;

		private string _comment;
		private DateTime? _expiryDate;
		private PatientNoteCategorySummary _category;

		public PatientNoteEditorComponent(PatientNoteDetail noteDetail, List<PatientNoteCategorySummary> noteCategoryChoices)
			: this(noteDetail, noteCategoryChoices, false)
		{
		}

		public PatientNoteEditorComponent(PatientNoteDetail noteDetail, List<PatientNoteCategorySummary> noteCategoryChoices, bool readOnlyMode)
		{
			_readOnlyMode = readOnlyMode;
			_note = noteDetail;
			_noteCategoryChoices = noteCategoryChoices;

			_comment = _note.Comment;
			_expiryDate = _note.ValidRangeUntil == null ? null : (DateTime?)_note.ValidRangeUntil.Value.Date;
			_category = _note.Category;

			this.Validation.Add(new ValidationRule("ExpiryDate",
				delegate
				{
					var valid = ExpiryDate == null || ExpiryDate > Platform.Time.Date;
					return new ValidationResult(valid, SR.MessageInvalidExpiryDate);
				}));
		}

		#region Presentation Model

		public string Comment
		{
			get { return _comment; }
			set 
			{
				_comment = value;
				this.Modified = true;
			}
		}

		public bool IsCommentEditable
		{
			get { return !_readOnlyMode && IsNewItem; }
		}

		public DateTime? ExpiryDate
		{
			get { return _expiryDate; }
			set
			{
				_expiryDate = (value == null) ? null : (DateTime?)value.Value.Date;
				this.Modified = true;
			}
		}

		public bool IsExpiryDateEditable
		{
			get { return !_readOnlyMode && !IsExpired; }
		}

		public IList CategoryChoices
		{
			get { return _noteCategoryChoices; }
		}

		[ValidateNotNull]
		public PatientNoteCategorySummary Category
		{
			get { return _category; }
			set
			{
				if (_category == value)
					return;

				_category = value;

				NotifyPropertyChanged("Category");
				NotifyPropertyChanged("CategoryDescription");
				this.Modified = true;
			}
		}

		public string CategoryDescription
		{
			get { return _category == null ? "" : _category.Description; }
		}

		public string FormatNoteCategory(object item)
		{
			var noteCategory = (PatientNoteCategorySummary) item;
			return String.Format(SR.FormatNoteCategory, noteCategory.Name, noteCategory.Severity.Value);
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
		}

		public event EventHandler AcceptEnabledChanged
		{
			add { this.ModifiedChanged += value; }
			remove { this.ModifiedChanged -= value; }
		}

		public void Accept()
		{
			if (!_readOnlyMode)
			{
				if (this.HasValidationErrors)
				{
					this.ShowValidation(true);
					return;
				}

				_note.Comment = _comment;
				_note.ValidRangeUntil = _expiryDate;
				_note.Category = _category;
			}

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		private bool IsNewItem
		{
			get { return _note.CreationTime == null; }
		}

		private bool IsExpired
		{
			get { return this.ExpiryDate.HasValue && this.ExpiryDate < Platform.Time; }
		}
	}
}

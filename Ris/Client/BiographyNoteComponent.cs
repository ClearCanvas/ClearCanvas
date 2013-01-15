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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="BiographyNoteComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class BiographyNoteComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// BiographyNoteComponent class
	/// </summary>
	[AssociateView(typeof(BiographyNoteComponentViewExtensionPoint))]
	public class BiographyNoteComponent : ApplicationComponent
	{
		private List<PatientNoteDetail> _noteList;
		private readonly BiographyNoteTable _noteTable;
		private PatientNoteDetail _selectedNote;

		/// <summary>
		/// Constructor
		/// </summary>
		public BiographyNoteComponent()
		{
			_noteTable = new BiographyNoteTable(this);
		}

		public List<PatientNoteDetail> Notes
		{
			get { return _noteList; }
			set
			{
				_noteList = value;
				_noteTable.Items.Clear();
				_noteTable.Items.AddRange(value);
			}
		}

		public ITable NoteTable
		{
			get { return _noteTable; }
		}

		public ISelection SelectedNote
		{
			get { return new Selection(_selectedNote); }
			set
			{
				_selectedNote = (PatientNoteDetail)value.Item;
				NoteSelectionChanged();
			}
		}

		public void ShowNoteDetail(PatientNoteDetail notedetail)
		{
			try
			{
				var caegotryChoices = new List<PatientNoteCategorySummary> {notedetail.Category};
				var editor = new PatientNoteEditorComponent(notedetail, caegotryChoices, true);
				LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleNoteText);
			}
			catch (Exception e)
			{
				// failed to launch editor
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		private void NoteSelectionChanged()
		{
			NotifyAllPropertiesChanged();
		}
	}
}

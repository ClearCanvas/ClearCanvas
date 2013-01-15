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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class OrderNoteEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(OrderNoteEditorComponentViewExtensionPoint))]
    public class OrderNoteEditorComponent : ApplicationComponent
    {
        private readonly OrderNoteDetail _note;
		private ICannedTextLookupHandler _cannedTextLookupHandler;
    	private string _noteBody;

        public OrderNoteEditorComponent(OrderNoteDetail noteDetail)
        {
            _note = noteDetail;
        	_noteBody = noteDetail.NoteBody;
        }

		public override void Start()
		{
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);

			base.Start();
		}

        #region Presentation Model

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}
		
		[ValidateNotNull]
		public string Comment
        {
			get { return _noteBody; }
            set
            {
				_noteBody = value;
                this.Modified = true;
            }
        }

        public bool IsNewItem
        {
            get { return _note.OrderNoteRef == null; }
        }

        public void Accept()
        {
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

        	_note.NoteBody = _noteBody;
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        public bool AcceptEnabled
        {
            get { return this.Modified && this.IsNewItem; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion
    }
}

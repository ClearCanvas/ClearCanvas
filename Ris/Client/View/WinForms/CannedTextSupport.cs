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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Adds canned-text in-place lookup support to a rich text box.
    /// </summary>
    public class CannedTextSupport : IDisposable
    {
        class RichTextBoxOwner : IRichTextBoxOwner
        {
            private readonly RichTextBox _richTextBox;

            public RichTextBoxOwner(RichTextBox richTextBox)
            {
                _richTextBox = richTextBox;
            }

            #region IRichTextBoxOwner Members

            public RichTextBox GetRichTextBox()
            {
                return _richTextBox;
            }

            public void SetSelectedText(string text)
            {
                _richTextBox.SelectedText = text;
            }

            #endregion
        }

        private readonly IRichTextBoxOwner _textEditor;
        private readonly ICannedTextLookupHandler _defaultLookupHandler;
        private ICannedTextLookupHandler _handlerInUse;
        private PopupForm _popup;
        private CannedTextInplaceLookupControl _lookup;

        public CannedTextSupport(RichTextBox control, ICannedTextLookupHandler defaultLookupHandler)
            : this(new RichTextBoxOwner(control), defaultLookupHandler)
        {
        }

        public CannedTextSupport(IRichTextBoxOwner control, ICannedTextLookupHandler defaultLookupHandler)
        {
            _textEditor = control;
            _defaultLookupHandler = defaultLookupHandler;

            Initialize();
        }

        public void ShowPopup()
        {
            this.ShowPopup(null, null);
        }

        public void ShowPopup(string query, ICannedTextLookupHandler alternativeHandler)
        {
            ClosePopup();

            Point pt = _textEditor.GetRichTextBox().GetPositionFromCharIndex(_textEditor.GetRichTextBox().SelectionStart);

            _handlerInUse = alternativeHandler ?? _defaultLookupHandler;

            _lookup = new CannedTextInplaceLookupControl(_handlerInUse);
            _lookup.Cancelled += _lookup_Cancelled;
            _lookup.Committed += _lookup_Committed;

            if (!string.IsNullOrEmpty(query))
                _handlerInUse.SuggestionProvider.SetQuery(query);

            _popup = new PopupForm(_lookup, _textEditor.GetRichTextBox(), _textEditor.GetRichTextBox().PointToScreen(pt));
            _popup.ShowPopup();
        }

        public void Commit(CannedText cannedText, ICannedTextLookupHandler handler)
        {
            ClosePopup();

            if (cannedText != null)
            {
                if (cannedText.IsSnippet)
                    _textEditor.SetSelectedText(handler.GetFullText(cannedText));
                else
                    _textEditor.SetSelectedText(cannedText.Text);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _textEditor.GetRichTextBox().KeyDown -= _textEditor_KeyDown;
        }

        #endregion

        private void Initialize()
        {
            _textEditor.GetRichTextBox().KeyDown += _textEditor_KeyDown;
        }

        private void _textEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.OemPeriod)
            {
                ShowPopup();
            }
        }

        private void _lookup_Committed(object sender, EventArgs e)
        {
            CannedText cannedText = (CannedText)_lookup.Value;
            Commit(cannedText, _handlerInUse);
        }

        private void _lookup_Cancelled(object sender, EventArgs e)
        {
            ClosePopup();
        }

        private void ClosePopup()
        {
            if (_popup != null)
            {
                _popup.ClosePopup();
                _popup = null;
            }

            if (_lookup != null)
            {
                _lookup.Cancelled -= _lookup_Cancelled;
                _lookup.Committed -= _lookup_Committed;
                _lookup = null;
            }
        }
    }
}

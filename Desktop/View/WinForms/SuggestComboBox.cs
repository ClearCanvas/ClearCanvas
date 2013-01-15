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
using System.Windows.Forms;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Special combo-box that works with a <see cref="ISuggestionProvider"/> to populate the drop-down list
    /// with suggested items as the user types.
    /// </summary>
    public class SuggestComboBox : ComboBox
    {
        private ISuggestionProvider _suggestionProvider;

        private event EventHandler _valueChanged;

        #region Public properties

        /// <summary>
        /// Gets or sets the <see cref="ISuggestionProvider"/>.
        /// </summary>
        [Browsable(false)]
        public ISuggestionProvider SuggestionProvider
        {
            get { return _suggestionProvider; }
            set
            {
                if (_suggestionProvider != null)
                    _suggestionProvider.SuggestionsProvided -= ItemsProvidedEventHandler;

                _suggestionProvider = value;

                if (_suggestionProvider != null)
                {
                    _suggestionProvider.SuggestionsProvided += ItemsProvidedEventHandler;

					// Bug #2222: only call SetQuery if this is a "simple" dropdown, in which case
					// we need to populate the list immediately
					// calling it for other styles causes behaviour described in #2222,
					// and there is really no need to get suggestions until user actually types
					if (this.DropDownStyle == ComboBoxStyle.Simple)
						_suggestionProvider.SetQuery(this.Text);
                }
            }
        }

        /// <summary>
        /// Gets or sets the current value of the control.
        /// </summary>
        [Browsable(false)]
        public object Value
        {
            get { return this.SelectedItem; }
            set
            {
                if(!Equals(this.SelectedItem, value))
                {
                    // in order to set the value, the Items collection must contain the value
                    // but if the value is null, can just use an empty list
                    // (also need to check for DBNull, for some stupid reason)
                    var items = new ArrayList();
                    if(value != null && value != DBNull.Value)
                    {
                        items.Add(value);
                    }
                    else
                    {
                        // if value is null or DBNull, clear the text
                        this.Text = null;
                    }

                    UpdateListItems(items);
                    this.SelectedItem = value;

                    OnValueChanged(EventArgs.Empty);
                }

            }
        }

        /// <summary>
        /// Occurs when the <see cref="Value"/> property changes.
        /// </summary>
        [Browsable(false)]
        public event EventHandler ValueChanged
        {
            add { _valueChanged += value; }
            remove { _valueChanged -= value; }
        }

        #endregion

        #region Overrides and Helpers

        protected virtual void OnValueChanged(EventArgs args)
        {
            EventsHelper.Fire(_valueChanged, this, EventArgs.Empty);
        }

        protected override void OnCreateControl()
        {
            this.SelectedItem = null;

            base.OnCreateControl();
        }

        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            // there are 2 ways that the value can change
            // either the selection change is comitted, or the control loses focus
            OnValueChanged(EventArgs.Empty);

            base.OnSelectionChangeCommitted(e);
        }

        protected override void OnLeave(EventArgs e)
        {
			UpdateSelectionFromText();
            base.OnLeave(e);
        }

		// Defect #5876: SuggestComboxBox mishandles 'Esc' once input is resolved
		// Override this method to clear the text when the ESC key is pressed.
		// Otherwise the component hosting the combobox will eat the ESC key, 
		// and close the component if the CancelButton is defined.
		protected override bool ProcessCmdKey(ref Message msg, Keys k)
		{
			if (k == Keys.Escape)
			{
				this.Text = null;
				UpdateSelectionFromText();
				this.DroppedDown = false;
				return true;
			}

			return base.ProcessCmdKey(ref msg, k);
		}

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
				case Keys.Escape:
					this.Text = null;
            		OnSelectionChangeCommitted(e);
            		break;
                default:
                    break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnTextUpdate(EventArgs e)
        {
            base.OnTextUpdate(e);
			CursorReset();
            _suggestionProvider.SetQuery(this.Text);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // setting this to null will also unsubscribe from the last subscribed-to provider
                this.SuggestionProvider = null;
            }

            base.Dispose(disposing);
        }

        private void ItemsProvidedEventHandler(object sender, SuggestionsProvidedEventArgs e)
        {
            // Remember the current text and selection start,
            // as they exists prior to modifying the items collection
            var curText = this.Text;
            var cursorPosition = this.SelectionStart;

            if (e.Items.Count == 0)
            {
				try
				{
					this.DroppedDown = false;
					// there are no suggestions, so clear the items list
					this.Items.Clear();
					// reset text back to original text
					// and return the cursor to the original position
					this.Text = curText;
					this.SelectionStart = cursorPosition;
				}
				catch (ArgumentOutOfRangeException)
				{
					// just in case...it throws the exception while clearing the list
				}
            }
            else
            {
                // at least 1 suggestion exists

                // open the dropdown menu
                this.DroppedDown = true;

                // update list
                UpdateListItems(e.Items);

                // set cursor to the end
                this.Text = curText;
                this.SelectionStart = curText.Length;
                this.SelectionLength = 0;
            }
        }

        private void UpdateListItems(ICollection items)
        {
			this.Items.Clear();
			if (items.Count > 0)
			{
				var array = new object[items.Count];
				items.CopyTo(array, 0);
				this.Items.AddRange(array);
			}
        }

        #endregion


		private static void CursorReset()
		{
			Cursor.Current = Cursors.Default;
			Cursor.Show();
		}

		private void UpdateSelectionFromText()
		{
			try
			{
				// do a case-insensitive search
				var itemIndex = this.FindStringExact(this.Text);
				if (itemIndex > -1)
				{
					// update the selected index
					this.SelectedIndex = itemIndex;

					// also update the visible text, because the upper/lower-casing may not match
					var item = this.Items[itemIndex];
					this.Text = GetItemText(item);
				}
				else
				{
					// doesn't match any suggestions, clear the text
					this.Text = null;
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				// if the combo box is dropped down, and the control loses focus (calling OnLeave),
				// it seems WinForms throws an exception from this.Text
				// not sure why this happens, but there is really nothing that can be done in terms of recovery
			}

			// there are 2 ways that the value can change
			// either the selection change is comitted, or the control loses focus
			OnValueChanged(EventArgs.Empty);
		}
    }
}

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

/*
Copyright (c) 2006, Marc Clifton
All rights reserved.

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list
  of conditions and the following disclaimer. 

* Redistributions in binary form must reproduce the above copyright notice, this 
  list of conditions and the following disclaimer in the documentation and/or other
  materials provided with the distribution. 
 
* Neither the name of Marc Clifton nor the names of its contributors may be
  used to endorse or promote products derived from this software without specific
  prior written permission. 

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace Clifton.Windows.Forms
{
    public class NullableMaskedEdit : MaskedTextBox
    {
        protected enum SkipMode
        {
            NonLiteral,
            Literal,
        }

        public event EventHandler ValueChanged;
        public event EventHandler NullValueChanged;
        public event EventHandler NullTextDisplayValueChanged;
        public event EventHandler NullTextReturnValueChanged;

        protected object nullValue;
        protected object val;
        protected string nullTextDisplayValue;
        protected string nullTextReturnValue;
        protected string editMask;
        protected bool selectGroup;
        protected bool autoAdvance;
        protected int[] posToMaskIndex = new int[0];								// 04/19/06 - Designer support.
        protected bool badMaskChar;

        // All other characters are considered delimeters, except for...
        protected string inputMaskChars = "09#L?&CAa";
        // ...non-delimeter characters, which do not represent an input character field.
        protected string nonDelimeterChars = "<>|";

        /// <summary>
        /// Gets/sets autoAdvance
        /// </summary>
        [Category("Nullable Masked Edit")]
        [Description("If set to true, auto advances (and backspaces) over mask delimiter characters")]
        public bool AutoAdvance
        {
            get { return autoAdvance; }
            set { autoAdvance = value; }
        }

        /// <summary>
        /// Gets/sets the select group flag, which will automatically select all the
        /// prompt characters between two delimiters.
        /// </summary>
        [Category("Nullable Masked Edit")]
        [Description("If set to true, automatically selects all the input fields between a pair of delimiters as the cursor moves into a delimited group.")]
        public bool SelectGroup
        {
            get { return selectGroup; }
            set { selectGroup = value; }
        }

        /// <summary>
        /// Gets/sets the mask.  This property should be used instead of the 
        /// MaskedTextBox.Mask value.
        /// </summary>
        [Category("Nullable Masked Edit")]
        [Description("Use this property instead of the base class' Mask property to set the edit mask.")]
        public string EditMask
        {
            get { return editMask; }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }

                editMask = value;
                RecalcPositionToMaskIndices();

                // If currently focused, set the new mask right away
                // in the base class.
                //if (Focused)
                //{
                //    Mask = value;
                //}
                Mask = editMask;
            }
        }

        /// <summary>
        /// Gets/sets the value that the Text property returns for null values.  This ensures
        /// that some known string value is returned when Value is the nullValue.
        /// </summary>
        [Category("Nullable Masked Edit")]
        [Description("The string to return in the Text getter when the control's value is null.")]
        public string NullTextReturnValue
        {
            get { return nullTextReturnValue; }
            set
            {
                if (nullTextReturnValue != value)
                {
                    nullTextReturnValue = value;
                    OnNullTextReturnValueChanged();
                }
            }
        }

        /// <summary>
        /// Gets/sets the text to display in the textbox when the control does not have focus.
        /// </summary>
        [Category("Nullable Masked Edit")]
        [Description("The string to display when the control's value is null.")]
		[Localizable(true)]
        public string NullTextDisplayValue
        {
            get { return nullTextDisplayValue; }
            set
            {
                if (nullTextDisplayValue != value)
                {
                    // Save the current unfocused is-null state.
                    bool isNull = IsNull;
                    nullTextDisplayValue = value;

                    // If the control was null (and unfocused), then update
                    // the currently displayed text with the new null text display
                    // value.  This will not change the control's Text property if the
                    // null display text value is changed while the control has focus,
                    // unless the null display text value is the same as the unedited mask
                    // value.
                    if (isNull)
                    {
                        Text = nullTextDisplayValue;
                    }

                    OnNullTextDisplayValueChanged();
                }
            }
        }

        /// <summary>
        /// Returns true of the control is displaying the null text display value (which it will do
        /// when the control doesn't have focus) or is an empty string (which indicates, when it has 
        /// focus, that it is an unedited text value).
        /// </summary>
        [Browsable(false)]
        public bool IsNull
        {
            get
            {
                return (base.Text == nullTextDisplayValue) ||
                    (base.Text == null) ||
                    (RemovePromptAndDelimiters(base.Text) == String.Empty);
            }
        }

        /// <summary>
        /// Gets/sets the nullable text value of the control.  If the control's Text value is
        /// empty or the equal to the NullTextDisplayValue value, this property will return the
        /// value assigned to the NullValue property.
        /// </summary>
        [Category("Nullable Masked Edit")]
        [Description("The control's value, either the text string or, if null, the value set in the NullValue property")]
        public object Value
        {
            get { return IsNull ? nullValue : base.Text; }
            set
            {
                if (val != value)
                {
                    val = value;

                    // Update the Text property according to the val state.
                    if ((val == null) || (val == DBNull.Value))
                    {
                        Text = nullTextDisplayValue;
                    }
                    else
                    {
                        Text = val.ToString();
                    }

                    OnValueChanged();
                }
            }
        }

        /// <summary>
        /// Gets/sets the NullValue value.
        /// </summary>
        [Category("Nullable Masked Edit")]
        [Description("The object to return in the Value property when the control's value is null.")]
        public object NullValue
        {
            get { return nullValue; }
            set
            {
                if (nullValue != value)
                {
                    nullValue = value;
                    OnNullValueChanged();
                }
            }
        }

        /// <summary>
        /// If the Text value is the null text display value (which it will be when
        /// the control doesn't have focus) or an empty string (which it may be when the
        /// control does have focus) then return the null text return value.
        /// </summary>
        [Description("Returns the control's text string or the null text string, if the control's value is null.")]
        public override string Text
        {
            get { return IsNull ? nullTextReturnValue : base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public NullableMaskedEdit()
        {
            this.MaskInputRejected += delegate { badMaskChar = true; };
            // 04/19/06 - If set to true, the control displays "0" in the numeric input fields.
            // Also, SkipLiterals doesn't do what it says it's supposed to do.
            SkipLiterals = false;
        }

    	/// <summary>
        /// On enter, set the mask to the edit mask, which
        /// override any null display value text.
        /// </summary>
        protected override void OnEnter(EventArgs e)
        {
			// If the null value, then set the Text to the empty string.
			if (val == nullValue)
			{
				// Setting the Text to String.Empty doesn't set the textbox to displaying
				// the mask!  But if we set the Text to the mask, then it works.  (Actually,
				// setting the Text property to String.Empty used to work, so this is some 
				// sort of wierd indeterminate behavior.)
				var originalText = Text;
				Text = editMask;

				// Mask property must be set last.
				Mask = editMask;

				// Restore the original text
				Text = originalText;
			}
			else
			{
				// Otherwise, set it to our Value.
				Text = val.ToString();

				// Mask property must be set last.
				Mask = editMask;
			}

            base.OnEnter(e);
        }

        /// <summary>
        /// If group selection is enabled, set up selection of the group if the 
        /// cursor is positioned at the beginning of the group.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            // If AutoSelect is set, manage the implementation here.
            // We have to do this in this event, because it's overridden if we
            // do it earlier, such as in the OnEnter event.
            if (selectGroup)
            {
                SelectionStart = 0;
                SelectFrom(0);

                // Optionally, instead of the above code, you could use this code, which
                // selects the group if the cursor is currently adjacent to a delimiter.

                //// Any chars to the right of the current position?
                //if (SelectionStart < posToMaskIndex.Length)
                //{
                //    // If the cursor is position at the start of the textbox, on, or immediately
                //    // to the right of a delimiter, then select the delimited fields.
                //    if ((SelectionStart == 0) ||
                //        (posToMaskIndex[SelectionStart] < 0) ||
                //        (posToMaskIndex[SelectionStart - 1] < 0))
                //    {
                //        SelectFrom(SelectionStart);
                //    }
                //}
            }
        }

        /// <summary>
        /// On leave, clear the mask, allowing the control
        /// to display the null display value text if the
        /// control is in the null state.
        /// </summary>
        protected override void OnLeave(EventArgs e)
        {
            // Save text mask format.
            MaskFormat maskFormat = TextMaskFormat;
            // Set to exclude all.
            TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            // If the Text is an empty string, then replace with the null text display value.
            if (base.Text == String.Empty)
            {
                // The mask has to be cleared.
                Mask = String.Empty;

                // Set the textbox to display the null text display value.
                Text = nullTextDisplayValue;

                // Our control value is the null value.
                Value = nullValue;
            }
            else
            {
                // valid text, set our control value to the Text property value.
                Value = base.Text;
            }

            // Restore the text mask format.
            TextMaskFormat = maskFormat;

            base.OnLeave(e);
        }

        /// <summary>
        /// Test for the backspace keystroke, and skip the delimiter char if AutoAdvance
        /// is true.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // if in overwrite mode, then clear the block or the single character
            // to the right of the cursor, without(!) pulling the text to the right
            // of the cursor left.
            if ((e.KeyCode == Keys.Delete) && (IsOverwriteMode))
            {
                if (SelectionLength == 0)
                {
                    SelectionLength = 1;
                }

                ClearSelection();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Back)
            {
                // Clear any selection block without moving the cursor.
                if (SelectionLength > 0)
                {
                    // TODO: Implement clearing the selected area.
                    // Save text mask format.
                    ClearSelection();
                    e.Handled = true;
                }
                else if (SelectionStart > 0)
                {
                    // In overwrite mode, simply replace the character being deleted with
                    // the prompt char, rather than pulling the whole text to the right of the
                    // cursor left, which causes misalignment of the delimited fields.
                    if (IsOverwriteMode)
                    {
                        // Move left of all delimiters.  If the cursor is immediately to the right of a 
                        // delimiter, this means we will delete the first non-delimiter character to the 
                        // left.
                        while ((SelectionStart > 0) && (posToMaskIndex[SelectionStart - 1] < 0))
                        {
                            --SelectionStart;
                        }

                        if (SelectionStart > 0)
                        {
                            --SelectionStart;
                            SelectionLength = 1;
                            ClearSelection();
                            e.Handled = true;
                        }
                    }
                }

                if (!e.Handled)
                {
                    base.OnKeyDown(e);
                }

                // Automatically decrement the cursor position if deleting up to a delimeter.
                // This is visually more correct, but is done only if the programmer sets the
                // AutoAdvance property to true (so that delimiters are skipped in the forward
                // direction as well.
                if (autoAdvance && editMask.Equals(string.Empty) == false)
                {
                    while ((SelectionStart - 1 >= 0) && (posToMaskIndex[SelectionStart - 1] < 0))
                    {
                        --SelectionStart;
                    }
                }
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // If we're in overwrite mode with a block selected, then clear the block
            // but keep all other delimited data in its group.
			// Bug #1824: ignore if control key is pressed...LUIJ
			bool isControlPressed = (ModifierKeys & Keys.Control) == Keys.Control;
			if ((SelectionLength > 0) && (IsOverwriteMode) && !isControlPressed)
            {
                ClearSelection();
            }

            // Clear flag, which may be set in the OnMaskInputRejected event handler.
            badMaskChar = false;
            base.OnKeyPress(e);

            // If the character doesn't meet the mask requirements...
            if (badMaskChar)
            {
                // then see if the char is the next delimiter.  If so, position
                // to the next non-delimiter prompt field.
                int n = SelectionStart;

                while (n < posToMaskIndex.Length)
                {
                    // If there's a delimiter at the physical position...
                    if (posToMaskIndex[n] < 0)
                    {
                        // ... and it's the char the user pressed...
                        if (Mask[(-posToMaskIndex[n]) - 1] == e.KeyChar)
                        {
                            // ... go to the first prompt field position.
                            SelectionStart = Skip(n + 1, SkipMode.Literal);

                            // And if group option is set, select the group.
                            if (selectGroup)
                            {
                                SelectFrom(SelectionStart);
                            }
                        }

                        break;
                    }

                    ++n;
                }
            }
            else
            {
                // If not at the end of the mask and not the backspace key...
                if ((SelectionStart < Mask.Length) && (e.KeyChar != '\b'))
                {
                    // and positioned on a delimiter...
                    if (posToMaskIndex[SelectionStart] < 0)
                    {
                        // ...advance the selection if flag is set.
                        if (autoAdvance)
                        {
                            SelectionStart = Skip(SelectionStart + 1, SkipMode.Literal);
                        }

                        // Select the next group if flag is set.
                        if (selectGroup)
                        {
                            SelectFrom(SelectionStart);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If spaces are used in the mask, the formatting goes all wierd if you don't strip 
        /// out those spaces before re-assigning the string to the Text control.  Nasty MaskedEditControl.
        /// I should have just written one from scratch!
        /// Since we've changed the format mask to include literals and prompt chars, we should
        /// be able to remove any whitespace in the current string.  I think.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string StripSpaces(string text)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != ' ')
                {
                    sb.Append(text[i]);
                }
            }

            return sb.ToString();
        }

        protected void ClearSelection()
        {
            if (string.IsNullOrEmpty(this.Mask))
            {
                RemoveCharacters();
            }
            else
            {
                RemoveCharactersAndReplaceWithMaskPrompt();
            }
        }

        private void RemoveCharacters()
        {
            Text = base.Text.Remove(SelectionStart, SelectionLength);
        }

        private void RemoveCharactersAndReplaceWithMaskPrompt()
        {
            MaskFormat maskFormat = TextMaskFormat;
            // Set to include prompts and literals;
            TextMaskFormat = MaskFormat.IncludePromptAndLiterals;
            char[] newText = base.Text.ToCharArray();
            int savePos = SelectionStart;

            // Clear the selected non-delimiter fields.
            for (int i = SelectionStart; i < SelectionStart + SelectionLength; i++)
            {
                if (posToMaskIndex.Length > 0)
                {
                    if (posToMaskIndex[i] >= 0)
                    {
                        newText[i] = PromptChar;
                    }
                }
                else
                {
                    newText[i] = ' ';
                }
            }

            // Handle MaskedTextBox quirk when the text has whitespace.
            Text = StripSpaces(new String(newText));
            TextMaskFormat = maskFormat;
            SelectionStart = savePos;
            SelectionLength = 0;
        }

        /// <summary>
        /// Returns a string with prompt and delimiters removed, used for testing
        /// if the string is empty, and thus a null value.
        /// </summary>
        protected string RemovePromptAndDelimiters(string text)
        {
            string ret = text;

            if (TextMaskFormat != MaskFormat.ExcludePromptAndLiterals)
            {
                char[] chars = text.ToCharArray();

                for (int i = 0; (i < posToMaskIndex.Length) && (i < chars.Length); i++)
                {
                    if ((posToMaskIndex[i] < 0) || (chars[i] == PromptChar))
                    {
                        chars[i] = ' ';
                    }
                }

                ret = new String(chars).Trim();
            }

            return ret;
        }

        /// <summary>
        /// Select the group of input char fields from the current position
        /// to the next delimiter, skipping any delimiters that start at the
        /// the current position.
        /// </summary>
        protected void SelectFrom(int pos)
        {
            // Find the first non-literal character.
            pos = Skip(pos, SkipMode.Literal);
            int pos2 = Skip(pos, SkipMode.NonLiteral);

            SelectionStart = pos;
            SelectionLength = pos2 - pos;
        }

        /// <summary>
        /// Skip either literals or prompt fields, depending on the SkipMode.
        /// Return the stop position, which is 0 to n - 1 characters, 
        /// where n is the position of the stop character.
        /// </summary>
        protected int Skip(int pos, SkipMode mode)
        {
            int n = pos;
            int len = 0;

            if (mode == SkipMode.Literal)
            {
                // Skip literals.
                while ((n < posToMaskIndex.Length) && (posToMaskIndex[n] < 0))
                {
                    ++len;
                    ++n;
                }
            }
            else
            {
                // Skip non-literals (input fields).
                while ((n < posToMaskIndex.Length) && (posToMaskIndex[n] >= 0))
                {
                    ++len;
                    ++n;
                }
            }

            return pos + len;
        }

        /// <summary>
        /// Scan the mask, creating a position to mask index entry, so that for a given
        /// cursor position, we can easily locate the mask position.  This makes it easier
        /// to find the input group between two delimeters.
        /// </summary>
        protected void RecalcPositionToMaskIndices()
        {
            int n = 0;
            int q = 0;
            posToMaskIndex = new int[editMask.Length];

            while (n < editMask.Length)
            {
                char maskChar = editMask[n];
                bool isMaskChar = inputMaskChars.IndexOf(maskChar) != -1;
                bool isNonDelimiterChar = nonDelimeterChars.IndexOf(maskChar) != -1;
                posToMaskIndex[q] = -(n + 1);		// Negative value used to indicate a delimiter character at the physical position.

                if (isMaskChar)
                {
                    // The mask at the position is an input char.
                    posToMaskIndex[q++] = n++;
                }
                else if (isNonDelimiterChar)
                {
                    // The mask at the position is a non-delimiter character, like <, >, or |
                    ++n;
                }
                else
                {
                    // The mask at the position is a delimiter.
                    ++q;
                    ++n;
                    // Don't put in the array, as we'll never be positioned on this char.
                }
            }
        }

        /// <summary>
        /// Fires the ValueChanged event when the Value property is changed.
        /// </summary>
        protected virtual void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires the NullValueChanged event when the NullValue property is changed.
        /// </summary>
        protected virtual void OnNullValueChanged()
        {
            if (NullValueChanged != null)
            {
                NullValueChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires the NullTextDisplayValueChanged event when the NullTextDisplayValue property is changed.
        /// </summary>
        protected virtual void OnNullTextDisplayValueChanged()
        {
            if (NullTextDisplayValueChanged != null)
            {
                NullTextDisplayValueChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires the NullTextReturnValueChanged event when the NullTextReturnValue property is changed.
        /// </summary>
        protected virtual void OnNullTextReturnValueChanged()
        {
            if (NullTextReturnValueChanged != null)
            {
                NullTextReturnValueChanged(this, EventArgs.Empty);
            }
        }
    }
}

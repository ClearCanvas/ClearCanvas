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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// A textbox-like control that captures a key stroke from the user.
	/// </summary>
	[DefaultEvent("KeyStrokeChanged")]
	[DefaultProperty("KeyStroke")]
	public class KeyStrokeCaptureBox : Control
	{
		private event EventHandler _borderStyleChanged;
		private event EventHandler _keySeparatorChanged;
		private event EventHandler _keyStrokeChanged;
		private event EventHandler _readOnlyChanged;
		private event EventHandler _showClearButtonChanged;
		private event EventHandler _textAlignChanged;
		private event ValidateKeyStrokeEventHandler _validateKeyStrokeEventHandler;

		private readonly List<XKeys> _currentKeys = new List<XKeys>(5);
		private readonly TextBox _textBox = new XTextBox();
		private readonly ClearButton _clearButton = new ClearButton();

		private XKeys _keyStroke = XKeys.None;
		private string _keySeparator = "+";
		private bool _keyStrokeAccepted = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyStrokeCaptureBox"/> control.
		/// </summary>
		public KeyStrokeCaptureBox()
		{
			this.SuspendLayout();
			try
			{
				_textBox.ContextMenu = base.ContextMenu;
				_textBox.ContextMenuStrip = base.ContextMenuStrip;
				_textBox.Font = base.Font;
				_textBox.ShortcutsEnabled = false;
				_textBox.TextAlign = HorizontalAlignment.Center;
				_textBox.PreviewKeyDown += HandleTextBoxPreviewKeyDown;
				_textBox.KeyUp += HandleTextBoxKeyUp;
				_textBox.KeyDown += HandleTextBoxKeyDown;
				_textBox.KeyPress += HandleTextBoxKeyPress;
				_textBox.Enter += HandleTextBoxFocusChanged;
				_textBox.Leave += HandleTextBoxFocusChanged;
				_textBox.LostFocus += HandleTextBoxFocusChanged;
				_textBox.GotFocus += HandleTextBoxFocusChanged;

				_clearButton.Cursor = Cursors.Default;
				_clearButton.Visible = false;
				_clearButton.Click += HandleClearButtonClick;

				this.Controls.Add(_clearButton);
				this.Controls.Add(_textBox);
			}
			finally
			{
				this.ResumeLayout(true);
			}
		}

		#region Designer Properties

		/// <summary>
		/// Gets or sets the background color for the control.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="SystemColors.Window"/>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The background color for the control.")]
		public override Color BackColor
		{
			get { return _textBox.BackColor; }
			set
			{
				if (_textBox.BackColor != value)
				{
					_textBox.BackColor = value;
					this.OnBackColorChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Resets the <see cref="BackColor"/> property to its default value.
		/// </summary>
		public override void ResetBackColor()
		{
			if (this.ReadOnly)
				this.BackColor = SystemColors.Control;
			else
				this.BackColor = SystemColors.Window;
		}

		private bool ShouldSerializeBackColor()
		{
			if (this.ReadOnly)
				return this.BackColor != SystemColors.Control;
			else
				return this.BackColor != SystemColors.Window;
		}

		/// <summary>
		/// This property is not relevant for this class.
		/// </summary>
		[Browsable(false)]
		public override Image BackgroundImage
		{
			get { return base.BackgroundImage; }
			set { base.BackgroundImage = value; }
		}

		/// <summary>
		/// This property is not relevant for this class.
		/// </summary>
		[Browsable(false)]
		public override ImageLayout BackgroundImageLayout
		{
			get { return base.BackgroundImageLayout; }
			set { base.BackgroundImageLayout = value; }
		}

		/// <summary>
		/// Gets or sets the border type of the textbox.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="System.Windows.Forms.BorderStyle.Fixed3D"/>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The border type of the text box.")]
		[DefaultValue(BorderStyle.Fixed3D)]
		public virtual BorderStyle BorderStyle
		{
			get { return _textBox.BorderStyle; }
			set
			{
				if (_textBox.BorderStyle != value)
				{
					_textBox.BorderStyle = value;
					this.OnBorderStyleChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the cursor that is displayed when the mouse pointer is over the control.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="Cursors.IBeam"/>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The cursor that is displayed when the mouse pointer is over the control.")]
		public override Cursor Cursor
		{
			get { return _textBox.Cursor; }
			set
			{
				if ((_textBox.Cursor == null && value != null) || (_textBox.Cursor != null && !_textBox.Cursor.Equals(value)))
				{
					_textBox.Cursor = value;
					this.OnCursorChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Resets the <see cref="Cursor"/> property to its default value.
		/// </summary>
		public override void ResetCursor()
		{
			this.Cursor = Cursors.IBeam;
		}

		private bool ShouldSerializeCursor()
		{
			return this.Cursor == null || !this.Cursor.Equals(Cursors.IBeam);
		}

		/// <summary>
		/// Gets or sets the foreground color for the control.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="SystemColors.WindowText"/>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The foreground color for the control.")]
		public override Color ForeColor
		{
			get { return _textBox.ForeColor; }
			set { _textBox.ForeColor = value; }
		}

		/// <summary>
		/// Resets the <see cref="ForeColor"/> property to its default value.
		/// </summary>
		public override void ResetForeColor()
		{
			this.ForeColor = SystemColors.WindowText;
		}

		private bool ShouldSerializeForeColor()
		{
			return this.ForeColor != SystemColors.WindowText;
		}

		/// <summary>
		/// Gets or sets the separator used to delimit individual modifiers and the pressed key in the textbox display.
		/// </summary>
		/// <remarks>
		/// The default is &quot;+&quot;.
		/// </remarks>
		[Category("Appearance")]
		[Description("The separator used to delimit individual modifiers and the pressed key in the textbox display.")]
		[DefaultValue("+")]
		[Localizable(true)]
		public virtual string KeySeparator
		{
			get { return _keySeparator; }
			set
			{
				if (_keySeparator != value)
				{
					_keySeparator = value;
					this.OnKeySeparatorChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the key stroke value of the control.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="XKeys.None"/>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The key stroke value of the control.")]
		[DefaultValue(XKeys.None)]
		public virtual XKeys KeyStroke
		{
			get { return _keyStroke; }
			set
			{
				if (_keyStroke != value && IsValidKeyStroke(value))
				{
					_keyStroke = value;
					this.OnKeyStrokeChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Resets the <see cref="KeyStroke"/> property to its default value.
		/// </summary>
		public virtual void ResetKeyStroke()
		{
			this.KeyStroke = XKeys.None;
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the key stroke value is read-only.
		/// </summary>
		/// <remarks>
		/// The default is False.
		/// </remarks>
		[Category("Behavior")]
		[Description("Whether or not the key stroke value is read-only.")]
		[DefaultValue(false)]
		public virtual bool ReadOnly
		{
			get { return _textBox.ReadOnly; }
			set
			{
				if (_textBox.ReadOnly != value)
				{
					_textBox.ReadOnly = value;
					this.OnReadOnlyChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not a button should be shown to allow clearing the key stroke value.
		/// </summary>
		/// <remarks>
		/// The default is False.
		/// </remarks>
		[Category("Behavior")]
		[Description("Whether or not a button should be shown to allow clearing the key stroke value.")]
		[DefaultValue(false)]
		public virtual bool ShowClearButton
		{
			get { return _clearButton.Visible; }
			set
			{
				if (_clearButton.Visible != value)
				{
					_clearButton.Visible = value;
					this.OnShowClearButton(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets the current text displayed in the textbox representing the key stroke value.
		/// </summary>
		/// <remarks>
		/// This value may change if the user is in the process of entering a key stroke.
		/// Setting this property has no effect.
		/// </remarks>
		[ReadOnly(true)]
		[Browsable(false)]
		[DefaultValue("")]
		public override string Text
		{
			get { return base.Text; }
			set { }
		}

		/// <summary>
		/// Gets or sets a value indicating how text is aligned in the textbox.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="HorizontalAlignment.Center"/>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The alignment of text in the textbox.")]
		[DefaultValue(HorizontalAlignment.Center)]
		[Localizable(true)]
		public virtual HorizontalAlignment TextAlign
		{
			get { return _textBox.TextAlign; }
			set
			{
				if (_textBox.TextAlign != value)
				{
					_textBox.TextAlign = value;
					this.OnTextAlignChanged(EventArgs.Empty);
				}
			}
		}

		#endregion

		#region Designer Events

		/// <summary>
		/// Occurs when the <see cref="BorderStyle"/> property value changes.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the BorderStyle property value changes.")]
		public event EventHandler BorderStyleChanged
		{
			add { _borderStyleChanged += value; }
			remove { _borderStyleChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="KeySeparator"/> property value changes.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the KeySeparator property value changes.")]
		public event EventHandler KeySeparatorChanged
		{
			add { _keySeparatorChanged += value; }
			remove { _keySeparatorChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="KeyStroke"/> property value changes.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the KeyStroke property value changes.")]
		public event EventHandler KeyStrokeChanged
		{
			add { _keyStrokeChanged += value; }
			remove { _keyStrokeChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="ReadOnly"/> property value changes.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the ReadOnly property value changes.")]
		public event EventHandler ReadOnlyChanged
		{
			add { _readOnlyChanged += value; }
			remove { _readOnlyChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="ShowClearButton"/> property value changes.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the ShowClearButton property value changes.")]
		public event EventHandler ShowClearButtonChanged
		{
			add { _showClearButtonChanged += value; }
			remove { _showClearButtonChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="TextAlign"/> property value changes.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the TextAlign property value changes.")]
		public event EventHandler TextAlignChanged
		{
			add { _textAlignChanged += value; }
			remove { _textAlignChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="KeyStrokeCaptureBox"/> needs to validate whether or not a given key combination is a valid key stroke.
		/// </summary>
		[Category("Behavior")]
		[Description("Occurs when the control needs to validate whether or not a given key combination is a valid key stroke.")]
		public event ValidateKeyStrokeEventHandler ValidateKeyStroke
		{
			add { _validateKeyStrokeEventHandler += value; }
			remove { _validateKeyStrokeEventHandler -= value; }
		}

		#endregion

		#region Virtual Event Handlers

		/// <summary>
		/// Raises the <see cref="BorderStyleChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnBorderStyleChanged(EventArgs e)
		{
			if (_borderStyleChanged != null)
				_borderStyleChanged.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="Control.ContextMenuChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnContextMenuChanged(EventArgs e)
		{
			_textBox.ContextMenu = this.ContextMenu;
			base.OnContextMenuChanged(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.ContextMenuStripChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnContextMenuStripChanged(EventArgs e)
		{
			_textBox.ContextMenuStrip = this.ContextMenuStrip;
			base.OnContextMenuStripChanged(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.FontChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnFontChanged(EventArgs e)
		{
			_textBox.Font = this.Font;
			base.OnFontChanged(e);
		}

		/// <summary>
		/// Raises the <see cref="ReadOnlyChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
			if (!this.ShouldSerializeBackColor())
				this.ResetBackColor();
			this.ClearKeyInputState();

			if (_readOnlyChanged != null)
				_readOnlyChanged.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="ShowClearButtonChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnShowClearButton(EventArgs e)
		{
			if (_showClearButtonChanged != null)
				_showClearButtonChanged.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="KeySeparatorChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnKeySeparatorChanged(EventArgs e)
		{
			this.UpdateText();

			if (_keySeparatorChanged != null)
				_keySeparatorChanged.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="KeyStrokeChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnKeyStrokeChanged(EventArgs e)
		{
			this.UpdateText();
			_clearButton.ShowGreyed = this.KeyStroke == XKeys.None;

			if (_keyStrokeChanged != null)
				_keyStrokeChanged.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="Control.SizeChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnSizeChanged(EventArgs e)
		{
			_textBox.Size = this.Size;

			this.UpdateClearButtonBounds();

			base.OnSizeChanged(e);
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			this.UpdateClearButtonBounds();

			base.OnRightToLeftChanged(e);
		}

		/// <summary>
		/// Raises the <see cref="TextAlignChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnTextAlignChanged(EventArgs e)
		{
			if (_textAlignChanged != null)
				_textAlignChanged.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="ValidateKeyStroke"/> event.
		/// </summary>
		/// <param name="e">A <see cref="ValidateKeyStrokeEventArgs"/> that contains the event data.</param>
		protected virtual void OnValidateKeyStroke(ValidateKeyStrokeEventArgs e)
		{
			XKeys key = GetKeyPressed(e.KeyStroke);
			e.IsValid = !(key == XKeys.ControlKey || key == XKeys.AltKey || key == XKeys.ShiftKey);

			if (_validateKeyStrokeEventHandler != null)
				_validateKeyStrokeEventHandler.Invoke(this, e);
		}

		#endregion

		#region Key Stroke Helpers

		/// <summary>
		/// Tests whether or not a given key stroke is modified by one or more of the modifier keys
		/// (<see cref="XKeys.Control"/>, <see cref="XKeys.Alt"/> and <see cref="XKeys.Shift"/>).
		/// </summary>
		/// <param name="keyStroke">The key stroke to be tested.</param>
		/// <returns>True if the key stroke is modified; False otherwise.</returns>
		protected static bool IsKeyPressModified(XKeys keyStroke)
		{
			return (keyStroke & XKeys.Modifiers) != 0;
		}

		/// <summary>
		/// Tests whether or not a given key stroke is modified by the <see cref="XKeys.Control"/> modifier key.
		/// </summary>
		/// <param name="keyStroke">The key stroke to be tested.</param>
		/// <returns>True if the key stroke is modified by <see cref="XKeys.Control"/>; False otherwise.</returns>
		protected static bool IsKeyPressModifiedByControl(XKeys keyStroke)
		{
			return (keyStroke & XKeys.Control) == XKeys.Control;
		}

		/// <summary>
		/// Tests whether or not a given key stroke is modified by the <see cref="XKeys.Alt"/> modifier key.
		/// </summary>
		/// <param name="keyStroke">The key stroke to be tested.</param>
		/// <returns>True if the key stroke is modified by <see cref="XKeys.Alt"/>; False otherwise.</returns>
		protected static bool IsKeyPressModifiedByAlt(XKeys keyStroke)
		{
			return (keyStroke & XKeys.Alt) == XKeys.Alt;
		}

		/// <summary>
		/// Tests whether or not a given key stroke is modified by the <see cref="XKeys.Shift"/> modifier key.
		/// </summary>
		/// <param name="keyStroke">The key stroke to be tested.</param>
		/// <returns>True if the key stroke is modified by <see cref="XKeys.Shift"/>; False otherwise.</returns>
		protected static bool IsKeyPressModifiedByShift(XKeys keyStroke)
		{
			return (keyStroke & XKeys.Shift) == XKeys.Shift;
		}

		/// <summary>
		/// Gets the combination of modifier flags in the given key stroke.
		/// </summary>
		/// <param name="keyStroke">The key stroke from which the modifiers are to be extracted.</param>
		/// <returns>A bitwise combination of <see cref="XKeys.Control"/>, <see cref="XKeys.Alt"/> and <see cref="XKeys.Shift"/>.</returns>
		protected static XKeys GetKeyPressModifiers(XKeys keyStroke)
		{
			return (keyStroke & XKeys.Modifiers);
		}

		/// <summary>
		/// Gets the individual key that was pressed in the given key stroke without any modifier flags.
		/// </summary>
		/// <param name="keyStroke">The key stroke from which the pressed key is to be extracted.</param>
		/// <returns>A single value from the <see cref="XKeys"/> enumeration excluding
		/// <see cref="XKeys.Control"/>, <see cref="XKeys.Alt"/> and <see cref="XKeys.Shift"/> (i.e. not a bitwise combination).</returns>
		protected static XKeys GetKeyPressed(XKeys keyStroke)
		{
			return (keyStroke & ~XKeys.Modifiers);
		}

		/// <summary>
		/// Gets the individual key that was pressed in the given key stroke without any modifier flags.
		/// </summary>
		/// <param name="keyStroke">The key stroke from which the pressed key is to be extracted.</param>
		/// <param name="control">True if the <see cref="XKeys.Control"/> modifier was pressed; False otherwise.</param>
		/// <param name="alt">True if the <see cref="XKeys.Alt"/> modifier was pressed; False otherwise.</param>
		/// <param name="shift">True if the <see cref="XKeys.Shift"/> modifier was pressed; False otherwise.</param>
		/// <returns>A single value from the <see cref="XKeys"/> enumeration excluding
		/// <see cref="XKeys.Control"/>, <see cref="XKeys.Alt"/> and <see cref="XKeys.Shift"/> (i.e. not a bitwise combination).</returns>
		protected static XKeys GetKeyPressed(XKeys keyStroke, out bool control, out bool alt, out bool shift)
		{
			control = IsKeyPressModifiedByControl(keyStroke);
			alt = IsKeyPressModifiedByAlt(keyStroke);
			shift = IsKeyPressModifiedByShift(keyStroke);
			return GetKeyPressed(keyStroke);
		}

		#endregion

		/// <summary>
		/// Gets the preferred height for a <see cref="KeyStrokeCaptureBox"/> control.
		/// </summary>
		[Browsable(false)]
		public int PreferredHeight
		{
			get { return _textBox.PreferredHeight; }
		}

		/// <summary>
		/// Retrieves the size of a rectangular area into which a control can be fitted.
		/// </summary>
		/// <returns>
		/// An ordered pair of type <see cref="Size"/> representing the width and height of a rectangle.
		/// </returns>
		/// <param name="proposedSize">The custom-sized area for a control.</param>
		public override Size GetPreferredSize(Size proposedSize)
		{
			return _textBox.GetPreferredSize(proposedSize);
		}

		/// <summary>
		/// Clears the key stroke value from this control.
		/// </summary>
		public void Clear()
		{
			this.KeyStroke = XKeys.None;
		}

		/// <summary>
		/// Performs the work of setting the specified bounds of this control.
		/// </summary>
		/// <param name="x">The new <see cref="Control.Left"/> property value of the control.</param>
		/// <param name="y">The new <see cref="Control.Top"/> property value of the control.</param>
		/// <param name="width">The new <see cref="Control.Width"/> property value of the control.</param>
		/// <param name="height">The new <see cref="Control.Height"/> property value of the control.</param>
		/// <param name="specified">A bitwise combination of the <see cref="BoundsSpecified"/> values.</param>
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			// Set a fixed height for the control.
			base.SetBoundsCore(x, y, width, this.PreferredHeight, specified);
		}

		/// <summary>
		/// Tests whether or not a given key combination is a valid key stroke by invoking the <see cref="ValidateKeyStroke"/> event.
		/// </summary>
		/// <param name="keyStroke">The key combination to be tested.</param>
		/// <returns>True if the key combination is a valid key stroke; False otherwise.</returns>
		protected bool IsValidKeyStroke(XKeys keyStroke)
		{
			ValidateKeyStrokeEventArgs e = new ValidateKeyStrokeEventArgs(keyStroke);
			this.OnValidateKeyStroke(e);
			return e.IsValid;
		}

		/// <summary>
		/// Formats a key stroke as a human-readable string.
		/// </summary>
		/// <remarks>
		/// The default implementation separates modifiers from the key pressed,
		/// translates each modifier/key individually using <see cref="GetKeyName"/>, 
		/// and combines them together using <see cref="KeySeparator"/>.
		/// A trailing <see cref="KeySeparator"/> is shown if this particular key stroke
		/// is not valid as determined by <see cref="IsValidKeyStroke"/>.
		/// A key stroke of <see cref="XKeys.None"/> is formatted as an empty string.
		/// </remarks>
		/// <param name="keyStroke">The key stroke to be formatted.</param>
		/// <returns>A human-readable string representing the key stroke.</returns>
		protected virtual string FormatKeyStroke(XKeys keyStroke)
		{
			if (keyStroke == 0)
				return string.Empty;

			bool control, alt, shift;
			bool valid = IsValidKeyStroke(keyStroke);
			XKeys key = GetKeyPressed(keyStroke, out control, out alt, out shift);

			List<string> keys = new List<string>(4);
			if (control)
				keys.Add(GetKeyName(XKeys.Control));
			if (alt)
				keys.Add(GetKeyName(XKeys.Alt));
			if (shift)
				keys.Add(GetKeyName(XKeys.Shift));

			// display a trailing '+' if the keystroke is invalid
			keys.Add(valid ? GetKeyName(key) : string.Empty);

			return string.Join(_keySeparator ?? string.Empty, keys.ToArray());
		}

		/// <summary>
		/// Maps a single key value to an appropriate key name.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default implementation uses an <see cref="XKeysConverter"/>.
		/// </para>
		/// </remarks>
		/// <param name="key">A single value from the <see cref="XKeys"/> enumeration.</param>
		/// <returns>The key name for the given key.</returns>
		protected virtual string GetKeyName(XKeys key)
		{
			return TypeDescriptor.GetConverter(typeof (XKeys)).ConvertToString(key);
		}

		/// <summary>
		/// Causes the <see cref="KeyStrokeCaptureBox"/> to update the displayed <see cref="Text"/>.
		/// </summary>
		public void UpdateText()
		{
			this.UpdateText(this.KeyStroke);
		}

		private void UpdateText(XKeys keyStroke)
		{
			base.Text = _textBox.Text = this.FormatKeyStroke(keyStroke);
			_textBox.SelectionStart = _textBox.TextLength;
		}

		private void ClearKeyInputState()
		{
			_currentKeys.Clear();
			_keyStrokeAccepted = false;
		}

		private void UpdateClearButtonBounds()
		{
			int wi = _textBox.ClientSize.Height;
			_clearButton.Size = new Size(wi, wi);
			_clearButton.Location = this.PointToClient(_textBox.PointToScreen(new Point(this.RightToLeft == RightToLeft.Yes ? _textBox.ClientRectangle.Left : _textBox.ClientRectangle.Right - wi, _textBox.ClientRectangle.Top)));
		}

		private void HandleTextBoxKeyDown(object sender, KeyEventArgs e)
		{
			XKeys keyData = Convert(e.KeyData);
			e.Handled = true;
			e.SuppressKeyPress = true;

			// if control is read-only, ignore any user input
			if (this.ReadOnly)
				return;

			if (!_keyStrokeAccepted)
			{
				this.UpdateText(keyData);
			}

			XKeys key = GetKeyPressed(keyData);
			if (key != XKeys.None && !_currentKeys.Contains(key))
				_currentKeys.Add(key);
		}

		private void HandleTextBoxKeyUp(object sender, KeyEventArgs e)
		{
			XKeys keyData = Convert(e.KeyData);
			e.Handled = true;
			e.SuppressKeyPress = true;

			// if control is read-only, ignore any user input
			if (this.ReadOnly)
				return;

			if (!_keyStrokeAccepted)
			{
				if (this.IsValidKeyStroke(keyData))
				{
					_keyStrokeAccepted = true;
					this.KeyStroke = keyData;
				}
				else
				{
					// technically, we should update anyway, but doing this avoids an extra call to format
					this.UpdateText(keyData);
				}
			}

			XKeys key = GetKeyPressed(keyData);
			if (key != XKeys.None)
				_currentKeys.Remove(key);

			if (_currentKeys.Count == 0)
			{
				this.ClearKeyInputState();
				this.UpdateText();
			}
		}

		private void HandleTextBoxKeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
		}

		private void HandleTextBoxPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			e.IsInputKey = true;
		}

		private void HandleTextBoxFocusChanged(object sender, EventArgs e)
		{
			this.ClearKeyInputState();
			this.UpdateText();
		}

		private void HandleClearButtonClick(object sender, EventArgs e)
		{
			this.Clear();
		}

		private static XKeys Convert(Keys keys)
		{
			return (XKeys) keys;
		}

		/// <summary>
		/// This hack enables select all text on focus functionality.
		/// </summary>
		/// <seealso cref="http://stackoverflow.com/questions/97459/automatically-select-all-text-on-focus-in-winforms-textbox"/>
		private class XTextBox : TextBox
		{
			private bool _alreadyFocused;

			protected override void OnLeave(EventArgs e)
			{
				base.OnLeave(e);
				_alreadyFocused = false;
			}

			protected override void OnGotFocus(EventArgs e)
			{
				base.OnGotFocus(e);
				if (MouseButtons == MouseButtons.None)
				{
					_alreadyFocused = true;
					base.SelectAll();
				}
			}

			protected override void OnMouseUp(MouseEventArgs e)
			{
				base.OnMouseUp(e);
				if (!_alreadyFocused && base.SelectionLength == 0)
				{
					_alreadyFocused = true;
					base.SelectAll();
				}
			}
		}

		private class ClearButton : Control
		{
			private bool _showGreyed = true;

			/// <summary>
			/// Cannot use <see cref="Control.Enabled"/> because otherwise we lose cursor control.
			/// </summary>
			public bool ShowGreyed
			{
				get { return _showGreyed; }
				set
				{
					if (_showGreyed != value)
					{
						_showGreyed = value;
						this.Invalidate();
					}
				}
			}

			protected override void OnClick(EventArgs e)
			{
				if (!this.ShowGreyed)
					base.OnClick(e);
			}

			protected override void OnMouseEnter(EventArgs e)
			{
				base.OnMouseEnter(e);
				this.Invalidate();
			}

			protected override void OnMouseLeave(EventArgs e)
			{
				base.OnMouseLeave(e);
				this.Invalidate();
			}

			protected override void OnMouseDown(MouseEventArgs e)
			{
				base.OnMouseDown(e);
				this.Invalidate();
			}

			protected override void OnMouseUp(MouseEventArgs e)
			{
				base.OnMouseUp(e);
				this.Invalidate();
			}

			private bool IsMouseOver(Rectangle bounds)
			{
				return bounds.Contains(this.PointToClient(Cursor.Position));
			}

			private static bool IsLeftMouseButtonPressed()
			{
				return (MouseButtons & MouseButtons.Left) == MouseButtons.Left;
			}

			protected override void OnPaint(PaintEventArgs e)
			{
				if (VisualStyleRenderer.IsSupported
				    && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ExplorerBar.HeaderClose.Normal)
				    && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ExplorerBar.HeaderClose.Hot)
				    && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ExplorerBar.HeaderClose.Pressed))
				{
					VisualStyleElement element;
					if (!this.ShowGreyed && IsMouseOver(this.ClientRectangle))
						element = IsLeftMouseButtonPressed() ? VisualStyleElement.ExplorerBar.HeaderClose.Pressed : VisualStyleElement.ExplorerBar.HeaderClose.Hot;
					else
						element = VisualStyleElement.ExplorerBar.HeaderClose.Normal;

					VisualStyleRenderer renderer = new VisualStyleRenderer(element);
					Size size = renderer.GetPartSize(e.Graphics, this.ClientRectangle, ThemeSizeType.True);
					using (Bitmap bmp = new Bitmap(size.Width, size.Height))
					{
						using (Graphics g = Graphics.FromImage(bmp))
						{
							renderer.DrawBackground(g, this.ClientRectangle);
						}
						e.Graphics.DrawImage(bmp, this.ClientRectangle);
					}
				}
				else
				{
					const int lineWidth = 2;

					if (this.ShowGreyed)
						ControlPaint.DrawButton(e.Graphics, this.ClientRectangle, ButtonState.Inactive);
					else if (IsMouseOver(this.ClientRectangle) && IsLeftMouseButtonPressed())
						ControlPaint.DrawButton(e.Graphics, this.ClientRectangle, ButtonState.Pushed);
					else
						ControlPaint.DrawButton(e.Graphics, this.ClientRectangle, ButtonState.Normal);

					using (Pen p = new Pen(this.ShowGreyed ? SystemBrushes.GrayText : SystemBrushes.ControlText, lineWidth))
					{
						Rectangle crossBounds = this.ClientRectangle;
						crossBounds.Location += new Size(2*lineWidth, 2*lineWidth);
						crossBounds.Size -= new Size(4*lineWidth + 1, 4*lineWidth + 1);
						e.Graphics.DrawLine(p, crossBounds.Left, crossBounds.Top, crossBounds.Right, crossBounds.Bottom);
						e.Graphics.DrawLine(p, crossBounds.Left, crossBounds.Bottom, crossBounds.Right, crossBounds.Top);
					}
				}
			}
		}
	}

	/// <summary>
	/// Represents the method that will handle the <see cref="KeyStrokeCaptureBox.ValidateKeyStroke"/> event.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="ValidateKeyStrokeEventArgs"/> containing the data for the event.</param>
	public delegate void ValidateKeyStrokeEventHandler(object sender, ValidateKeyStrokeEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="KeyStrokeCaptureBox.ValidateKeyStroke"/> event.
	/// </summary>
	public class ValidateKeyStrokeEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the key stroke which is to be validated.
		/// </summary>
		public readonly XKeys KeyStroke;

		/// <summary>
		/// Gets or sets a value indicating whether or not the value of <see cref="KeyStroke"/> is valid.
		/// </summary>
		/// <remarks>
		/// Code that sets this property should typically check if another handler has already
		/// determined the key stroke to be invalid by logically ANDing the desired value
		/// with the original value of <see cref="IsValid"/>.
		/// </remarks>
		public bool IsValid { get; set; }

		/// <summary>
		/// Initializes a new instance of <see cref="ValidateKeyStrokeEventArgs"/>.
		/// </summary>
		/// <param name="keyStroke">The key stroke which is to be validated.</param>
		public ValidateKeyStrokeEventArgs(XKeys keyStroke)
		{
			this.KeyStroke = keyStroke;
			this.IsValid = true;
		}
	}
}
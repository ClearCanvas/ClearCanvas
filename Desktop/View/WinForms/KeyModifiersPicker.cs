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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// A control that allows the user to pick a combination of key modifiers.
	/// </summary>
	[DefaultEvent("KeyModifiersChanged")]
	[DefaultProperty("KeyModifiers")]
	public class KeyModifiersPicker : Control
	{
		private event EventHandler _appearanceChanged;
		private event EventHandler _checkAlignChanged;
		private event EventHandler _keyModifiersChanged;
		private event EventHandler _textAlignChanged;

		private readonly CheckBox _checkBoxCtrl = new CheckBox();
		private readonly CheckBox _checkBoxAlt = new CheckBox();
		private readonly CheckBox _checkBoxShift = new CheckBox();

		private XKeys _keyModifiers = XKeys.None;
		private bool _updatingCheckBoxChecked = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyModifiersPicker"/> control.
		/// </summary>
		public KeyModifiersPicker()
		{
			TypeConverter keysConverter = TypeDescriptor.GetConverter(typeof (XKeys));

			this.SuspendLayout();
			try
			{
				_checkBoxCtrl.Appearance = Appearance.Normal;
				_checkBoxCtrl.AutoCheck = true;
				_checkBoxCtrl.BackColor = Color.Transparent;
				_checkBoxCtrl.CheckAlign = ContentAlignment.MiddleLeft;
				_checkBoxCtrl.ContextMenu = base.ContextMenu;
				_checkBoxCtrl.ContextMenuStrip = base.ContextMenuStrip;
				_checkBoxCtrl.Dock = DockStyle.Left;
				_checkBoxCtrl.Font = base.Font;
				_checkBoxCtrl.TabIndex = 0;
				_checkBoxCtrl.Text = keysConverter.ConvertToString(XKeys.Control);
				_checkBoxCtrl.TextAlign = ContentAlignment.MiddleLeft;
				_checkBoxCtrl.CheckedChanged += HandleCheckBoxCheckedChanged;

				_checkBoxAlt.Appearance = Appearance.Normal;
				_checkBoxAlt.AutoCheck = true;
				_checkBoxAlt.BackColor = Color.Transparent;
				_checkBoxAlt.CheckAlign = ContentAlignment.MiddleLeft;
				_checkBoxAlt.ContextMenu = base.ContextMenu;
				_checkBoxAlt.ContextMenuStrip = base.ContextMenuStrip;
				_checkBoxAlt.Dock = DockStyle.Left;
				_checkBoxAlt.Font = base.Font;
				_checkBoxAlt.TabIndex = 1;
				_checkBoxAlt.Text = keysConverter.ConvertToString(XKeys.Alt);
				_checkBoxAlt.TextAlign = ContentAlignment.MiddleLeft;
				_checkBoxAlt.CheckedChanged += HandleCheckBoxCheckedChanged;

				_checkBoxShift.Appearance = Appearance.Normal;
				_checkBoxShift.AutoCheck = true;
				_checkBoxShift.BackColor = Color.Transparent;
				_checkBoxShift.CheckAlign = ContentAlignment.MiddleLeft;
				_checkBoxShift.ContextMenu = base.ContextMenu;
				_checkBoxShift.ContextMenuStrip = base.ContextMenuStrip;
				_checkBoxShift.Dock = DockStyle.Fill;
				_checkBoxShift.Font = base.Font;
				_checkBoxShift.TabIndex = 2;
				_checkBoxShift.Text = keysConverter.ConvertToString(XKeys.Shift);
				_checkBoxShift.TextAlign = ContentAlignment.MiddleLeft;
				_checkBoxShift.CheckedChanged += HandleCheckBoxCheckedChanged;

				this.SetStyle(ControlStyles.Selectable, false);
				this.Controls.Add(_checkBoxShift);
				this.Controls.Add(_checkBoxAlt);
				this.Controls.Add(_checkBoxCtrl);
			}
			finally
			{
				this.ResumeLayout(true);
			}
		}

		#region Designer Properties

		/// <summary>
		/// Gets or sets a value indicating the appearance of the checkboxes in the control.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="System.Windows.Forms.Appearance.Normal"/>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The alignment of the checkbox in the control.")]
		[DefaultValue(Appearance.Normal)]
		public virtual Appearance Appearance
		{
			get { return _checkBoxCtrl.Appearance; }
			set
			{
				if (_checkBoxCtrl.Appearance != value)
				{
					_checkBoxCtrl.Appearance = _checkBoxAlt.Appearance = _checkBoxShift.Appearance = value;
					this.OnAppearanceChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating how the checkbox is aligned in the control.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="ContentAlignment.MiddleLeft"/>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The alignment of the checkbox in the control.")]
		[DefaultValue(ContentAlignment.MiddleLeft)]
		[Localizable(true)]
		public virtual ContentAlignment CheckAlign
		{
			get { return _checkBoxCtrl.CheckAlign; }
			set
			{
				if (_checkBoxCtrl.CheckAlign != value)
				{
					_checkBoxCtrl.CheckAlign = _checkBoxAlt.CheckAlign = _checkBoxShift.CheckAlign = value;
					this.OnCheckAlignChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the key modifiers value of the control.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="XKeys.None"/>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The key modifiers value of the control.")]
		[DefaultValue(XKeys.None)]
		[TypeConverter(typeof (KeyModifiersFlagEnumPropertiesConverter))]
		public virtual XKeys KeyModifiers
		{
			get { return _keyModifiers; }
			set
			{
				value = value & XKeys.Modifiers;
				if (_keyModifiers != value)
				{
					_keyModifiers = value;
					this.OnKeyModifiersChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// This property is not relevant for this class.
		/// </summary>
		[Browsable(false)]
		public new bool TabStop
		{
			get { return base.TabStop; }
			set { base.TabStop = value; }
		}

		/// <summary>
		/// This property is not relevant for this class.
		/// </summary>
		[Browsable(false)]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating how text is aligned in the control.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="ContentAlignment.MiddleLeft"/>.
		/// </remarks>
		[Category("Appearance")]
		[Description("The alignment of text in the control.")]
		[DefaultValue(ContentAlignment.MiddleLeft)]
		[Localizable(true)]
		public virtual ContentAlignment TextAlign
		{
			get { return _checkBoxCtrl.TextAlign; }
			set
			{
				if (_checkBoxCtrl.TextAlign != value)
				{
					_checkBoxCtrl.TextAlign = _checkBoxAlt.TextAlign = _checkBoxShift.TextAlign = value;
					this.OnTextAlignChanged(EventArgs.Empty);
				}
			}
		}

		#endregion

		#region Designer Events

		/// <summary>
		/// Occurs when the <see cref="Appearance"/> property value changes.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the Appearance property value changes.")]
		public event EventHandler AppearanceChanged
		{
			add { _appearanceChanged += value; }
			remove { _appearanceChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="CheckAlign"/> property value changes.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the CheckAlign property value changes.")]
		public event EventHandler CheckAlignChanged
		{
			add { _checkAlignChanged += value; }
			remove { _checkAlignChanged -= value; }
		}

		/// <summary>
		/// Occurs when the <see cref="KeyModifiers"/> property value changes.
		/// </summary>
		[Category("Property Changed")]
		[Description("Occurs when the KeyModifiers property value changes.")]
		public event EventHandler KeyModifiersChanged
		{
			add { _keyModifiersChanged += value; }
			remove { _keyModifiersChanged -= value; }
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

		#endregion

		#region Virtual Event Handlers

		/// <summary>
		/// Raises the <see cref="AppearanceChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnAppearanceChanged(EventArgs e)
		{
			if (_appearanceChanged != null)
				_appearanceChanged.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="CheckAlignChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnCheckAlignChanged(EventArgs e)
		{
			if (_checkAlignChanged != null)
				_checkAlignChanged.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="Control.ContextMenuChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnContextMenuChanged(EventArgs e)
		{
			_checkBoxCtrl.ContextMenu = _checkBoxAlt.ContextMenu = _checkBoxShift.ContextMenu = this.ContextMenu;
			base.OnContextMenuChanged(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.ContextMenuStripChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnContextMenuStripChanged(EventArgs e)
		{
			_checkBoxCtrl.ContextMenuStrip = _checkBoxAlt.ContextMenuStrip = _checkBoxShift.ContextMenuStrip = this.ContextMenuStrip;
			base.OnContextMenuStripChanged(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.Enabled"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnEnabledChanged(EventArgs e)
		{
			_checkBoxCtrl.Enabled = _checkBoxAlt.Enabled = _checkBoxShift.Enabled = this.Enabled;
			base.OnEnabledChanged(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.FontChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnFontChanged(EventArgs e)
		{
			_checkBoxCtrl.Font = _checkBoxAlt.Font = _checkBoxShift.Font = this.Font;
			base.OnFontChanged(e);
		}

		/// <summary>
		/// Raises the <see cref="KeyModifiersChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnKeyModifiersChanged(EventArgs e)
		{
			_updatingCheckBoxChecked = true;
			try
			{
				_checkBoxCtrl.Checked = (_keyModifiers & XKeys.Control) == XKeys.Control;
				_checkBoxAlt.Checked = (_keyModifiers & XKeys.Alt) == XKeys.Alt;
				_checkBoxShift.Checked = (_keyModifiers & XKeys.Shift) == XKeys.Shift;
			}
			finally
			{
				_updatingCheckBoxChecked = false;
			}

			if (_keyModifiersChanged != null)
				_keyModifiersChanged.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="Control.SizeChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnSizeChanged(EventArgs e)
		{
			Size size = this.Size;
			size.Width /= 3;
			_checkBoxCtrl.Size = _checkBoxAlt.Size = _checkBoxShift.Size = size;
			base.OnSizeChanged(e);
		}

		/// <summary>
		/// Raises the <see cref="Control.RightToLeftChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected override void OnRightToLeftChanged(EventArgs e)
		{
			// _checkBoxShift always gets fill to take advantage of any extra roundoff size - plus, it's last anyway
			_checkBoxCtrl.Dock = _checkBoxAlt.Dock = (this.RightToLeft == RightToLeft.Yes ? DockStyle.Right : DockStyle.Left);
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

		#endregion

		#region Custom TypeConverter for KeyModifiers Property

		/// <summary>
		/// Expands the <see cref="KeyModifiers"/> property into individual flags.
		/// </summary>
		private class KeyModifiersFlagEnumPropertiesConverter : EnumConverter
		{
			private readonly PropertyDescriptorCollection _properties;

			public KeyModifiersFlagEnumPropertiesConverter()
				: base(typeof (XKeys))
			{
				_properties = new PropertyDescriptorCollection(new PropertyDescriptor[] {});
				_properties.Add(new KeyModifiersFlagPropertyDescriptor(XKeys.Control));
				_properties.Add(new KeyModifiersFlagPropertyDescriptor(XKeys.Alt));
				_properties.Add(new KeyModifiersFlagPropertyDescriptor(XKeys.Shift));
			}

			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				return false;
			}

			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				// our usage of this converter is a special case and too trivial to be worth implementing the
				// attribute filtering mechanism (which doesn't make a lot of sense anyway according to the MSDN docs).
				return _properties;
			}

			private class KeyModifiersFlagPropertyDescriptor : SimplePropertyDescriptor
			{
				private static readonly FieldInfo _boxedEnumValueField;
				private readonly XKeys _modifier;

				static KeyModifiersFlagPropertyDescriptor()
				{
					var arr = typeof (XKeys).FindMembers
						(MemberTypes.Field, BindingFlags.Public | BindingFlags.Instance, Type.FilterAttribute, FieldAttributes.RTSpecialName | FieldAttributes.SpecialName);
					if (arr.Length == 1)
						_boxedEnumValueField = (FieldInfo) arr[0];
				}

				public KeyModifiersFlagPropertyDescriptor(XKeys modifier)
					: base(typeof (XKeys), Enum.GetName(typeof (XKeys), modifier), typeof (bool))
				{
					_modifier = modifier;
				}

				public override string Category
				{
					get { return "Flags"; }
				}

				public override string Description
				{
					get { return string.Format("Value indicating whether or not the {0} flag is set.", Enum.GetName(typeof (XKeys), _modifier)); }
				}

				public override bool CanResetValue(object component)
				{
					return false;
				}

				public override bool ShouldSerializeValue(object component)
				{
					// serializable IIF the flag is set
					return (bool) this.GetValue(component);
				}

				public override object GetValue(object component)
				{
					// getting the value only - no need to worry about boxing
					return ((XKeys) component & _modifier) == _modifier;
				}

				public override void SetValue(object component, object value)
				{
					if (_boxedEnumValueField != null)
						_boxedEnumValueField.SetValue(component, ((XKeys) component) & ~_modifier | ((bool) value ? _modifier : 0));
				}
			}
		}

		#endregion

		private void HandleCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			if (!_updatingCheckBoxChecked)
			{
				XKeys keyModifiers = XKeys.None;
				if (_checkBoxCtrl.Checked)
					keyModifiers |= XKeys.Control;
				if (_checkBoxAlt.Checked)
					keyModifiers |= XKeys.Alt;
				if (_checkBoxShift.Checked)
					keyModifiers |= XKeys.Shift;
				this.KeyModifiers = keyModifiers;
			}
		}
	}
}
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

#region Additional permission to link with DotNetMagic

// Additional permission under GNU GPL version 3 section 7
// 
// If you modify this Program, or any covered work, by linking or combining it
// with DotNetMagic (or a modified version of that library), containing parts
// covered by the terms of the Crownwood Software DotNetMagic license, the
// licensors of this Program grant you additional permission to convey the
// resulting work.

#endregion

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Represents a section header title bar GUI element.
	/// </summary>
	public class TitleBar : Control
	{
		private event EventHandler _textAlignmentChanged;
		private readonly Crownwood.DotNetMagic.Controls.TitleBar _titleBar;

		/// <summary>
		/// Initializes a new instance of <see cref="TitleBar"/>.
		/// </summary>
		public TitleBar()
		{
			SuspendLayout();

			_titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			_titleBar.Dock = DockStyle.Fill;
			_titleBar.Text = string.Empty;
			_titleBar.MouseOverColor = Color.Empty;
			_titleBar.Style = VisualStyle.IDE2005;
			_titleBar.BackColor = Application.CurrentUITheme.Colors.StandardColorDark;
			_titleBar.ForeColor = Color.White;
			_titleBar.GradientActiveColor = Application.CurrentUITheme.Colors.StandardColorDark;
			_titleBar.GradientColoring = GradientColoring.LightBackToGradientColor;
			_titleBar.GradientDirection = GradientDirection.TopToBottom;

			Controls.Add(_titleBar);
			ResumeLayout(false);
			PerformLayout();

			Application.CurrentUIThemeChanged += HandleApplicationCurrentUIThemeChanged;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				Application.CurrentUIThemeChanged -= HandleApplicationCurrentUIThemeChanged;
			base.Dispose(disposing);
		}

		private void HandleApplicationCurrentUIThemeChanged(object sender, EventArgs e)
		{
			_titleBar.BackColor = _titleBar.GradientActiveColor = Application.CurrentUITheme.Colors.StandardColorDark;
		}

		/// <summary>
		/// Gets or sets the text of the title bar.
		/// </summary>
		[DefaultValue(@"")]
		[Localizable(true)]
		public override string Text
		{
			get { return _titleBar.Text; }
			set
			{
				if (_titleBar.Text == value) return;
				_titleBar.Text = value;
				OnTextChanged(new EventArgs());
			}
		}

		/// <summary>
		/// Resets the value of <see cref="Text"/>.
		/// </summary>
		public override void ResetText()
		{
			Text = string.Empty;
		}

		/// <summary>
		/// Gets or sets the alignment of the text in the title bar.
		/// </summary>
		public virtual StringAlignment TextAlignment
		{
			get { return _titleBar.TextAlignment; }
			set
			{
				if (_titleBar.TextAlignment == value) return;
				_titleBar.TextAlignment = value;
				OnTextAlignmentChanged(new EventArgs());
			}
		}

		/// <summary>
		/// Fired when the value of <see cref="TextAlignment"/> has changed.
		/// </summary>
		public event EventHandler TextAlignmentChanged
		{
			add { _textAlignmentChanged += value; }
			remove { _textAlignmentChanged -= value; }
		}

		/// <summary>
		/// Raises the <see cref="TextAlignmentChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnTextAlignmentChanged(EventArgs e)
		{
			EventsHelper.Fire(_textAlignmentChanged, this, e);
		}

		/// <summary>
		/// Resets the value of <see cref="TextAlignment"/>.
		/// </summary>
		public virtual void ResetTextAlignment()
		{
			_titleBar.ResetTextAlignment();
		}
	}
}
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
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Form used by the <see cref="DialogBoxView"/> class.
	/// </summary>
	/// <remarks>
	/// This class may be subclassed.
	/// </remarks>
	public partial class DialogBoxForm : DotNetMagicForm
	{
		private readonly Control _content;

		internal DialogBoxForm(string title, Control content, Size exactSize, DialogSizeHint sizeHint)
			: this(title, content, exactSize, sizeHint, false) {}

		internal DialogBoxForm(string title, Control content, Size exactSize, DialogSizeHint sizeHint, bool allowResize)
		{
			InitializeComponent();
			Text = title;

			_content = content;

			// important - if we do not set a minimum size, the full content may not be displayed
			_content.MinimumSize = _content.Size;
			_content.Dock = DockStyle.Fill;

			// adjust size of client area
			this.ClientSize = exactSize != Size.Empty ? exactSize : SizeHintHelper.TranslateHint(sizeHint, _content.Size);

			if (allowResize)
			{
				FormBorderStyle = FormBorderStyle.Sizable;
				MinimumSize = base.SizeFromClientSize(_content.Size);
			}

			_contentPanel.Controls.Add(_content);

			// Resize the dialog if size of the underlying content changed
			_content.SizeChanged += OnContentSizeChanged;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dialogBox"></param>
		/// <param name="content"></param>
		public DialogBoxForm(DialogBox dialogBox, Control content)
			: this(dialogBox.Title, content, dialogBox.Size, dialogBox.SizeHint, dialogBox.AllowUserResize)
		{
		}

		internal void DelayedClose(DialogBoxAction action)
		{
			BeginInvoke(new MethodInvoker(() => EndDialog(action)));
		}

		private void OnContentSizeChanged(object sender, EventArgs e)
		{
			if (ClientSize != _content.Size)
				ClientSize = _content.Size;
		}

		private void EndDialog(DialogBoxAction action)
		{
			// close the form
			switch (action)
			{
				case DialogBoxAction.Cancel:
					DialogResult = DialogResult.Cancel;
					break;
				case DialogBoxAction.Ok:
					DialogResult = DialogResult.OK;
					break;
			}
		}

	}
}
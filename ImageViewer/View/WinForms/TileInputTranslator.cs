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

using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	internal sealed class TileInputTranslator
	{
		private TileControl _tileControl;

		private Keys[] _consumeKeyStrokes = 
						{	Keys.ControlKey,
							Keys.LControlKey,
							Keys.RControlKey,
							Keys.ShiftKey,
							Keys.LShiftKey,
							Keys.RShiftKey,
							Keys.Menu,
                            Keys.LMenu,
                            Keys.RMenu
						};

		public TileInputTranslator(TileControl tileControl)
		{
			_tileControl = tileControl;
		}

		private Keys Modifiers
		{
			get { return System.Windows.Forms.Control.ModifierKeys; }
		}

		private Point MousePositionScreen
		{
			get { return System.Windows.Forms.Control.MousePosition; }
		}

		private Point MousePositionClient
		{
			get { return _tileControl.PointToClient(this.MousePositionScreen); }
		}

		private MouseButtons MouseButtons
		{
			get { return System.Windows.Forms.Control.MouseButtons; }
		}

		private bool Control
		{
			get { return (this.Modifiers & Keys.Control) == Keys.Control; }
		}

		private bool Alt
		{
			get { return (this.Modifiers & Keys.Alt) == Keys.Alt; }
		}

		private bool Shift
		{
			get { return (this.Modifiers & Keys.Shift) == Keys.Shift; }
		}

		private bool ConsumeKeyStroke(Keys keyCode)
		{
			foreach (Keys keyStroke in _consumeKeyStrokes)
			{
				if (keyCode == keyStroke)
					return true;
			}

			return false;
		}

		public object OnLostFocus()
		{
			return new LostFocusMessage();
		}

		public object OnMouseLeave()
		{
			return new MouseLeaveMessage();
		}

		public object OnMouseMove(MouseEventArgs e)
		{
			return new TrackMousePositionMessage(e.Location);
		}

		public object OnMouseDown(MouseEventArgs e)
		{
			return new MouseButtonMessage(e.Location, (XMouseButtons)e.Button, MouseButtonMessage.ButtonActions.Down, (uint)e.Clicks, this.Control, this.Alt, this.Shift);
		}

		public object OnMouseUp(MouseEventArgs e)
		{
			return new MouseButtonMessage(e.Location, (XMouseButtons)e.Button, MouseButtonMessage.ButtonActions.Up, 0, this.Control, this.Alt, this.Shift);
		}

		public object OnMouseWheel(MouseEventArgs e)
		{
			return new MouseWheelMessage(e.Delta, this.Control, this.Alt, this.Shift);
		}

		public object OnKeyDown(KeyEventArgs e)
		{
			if (ConsumeKeyStroke(e.KeyCode))
				return null;

			return new KeyboardButtonMessage((XKeys)e.KeyData, KeyboardButtonMessage.ButtonActions.Down);
		}

		public object OnKeyUp(KeyEventArgs e)
		{
			if (ConsumeKeyStroke(e.KeyCode))
				return null;

			return new KeyboardButtonMessage((XKeys)e.KeyData, KeyboardButtonMessage.ButtonActions.Up);
		}

		public object PostProcessMessage(Message msg, bool alreadyHandled)
		{
			if (msg.Msg == 0x100 && alreadyHandled)
			{
				Keys keyData = (Keys)msg.WParam;
				if (!ConsumeKeyStroke(keyData))
				{
					//when a keystroke gets handled by a control other than the tile, we release the capture.
					return new ReleaseCaptureMessage();
				}
			}

			return null;
		}

		public object PreProcessMessage(Message msg)
		{
			if (msg.Msg == 0x100)
			{
				Keys keyData = (Keys)msg.WParam;
				if (!ConsumeKeyStroke(keyData))
					return new KeyboardButtonDownPreview((XKeys)msg.WParam);
			}

			return null;
		}
	}
}

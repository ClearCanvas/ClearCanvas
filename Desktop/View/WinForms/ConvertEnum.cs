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
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	public static class ConvertEnum
	{
		public static ModifierFlags GetModifierFlags(DragEventArgs dragEventArgs)
		{
			DragEventArgsKeyState keyState = (DragEventArgsKeyState) dragEventArgs.KeyState;
			ModifierFlags modifiers = ModifierFlags.None;
			if ((keyState & DragEventArgsKeyState.Shift) == DragEventArgsKeyState.Shift)
				modifiers |= ModifierFlags.Shift;
			if ((keyState & DragEventArgsKeyState.Ctrl) == DragEventArgsKeyState.Ctrl)
				modifiers |= ModifierFlags.Control;
			if ((keyState & DragEventArgsKeyState.Alt) == DragEventArgsKeyState.Alt)
				modifiers |= ModifierFlags.Alt;
			return modifiers;
		}

		public static DragDropOption GetDragDropAction(DragDropEffects dragDropEffects)
		{
			DragDropOption action = DragDropOption.None;
			if ((dragDropEffects & DragDropEffects.Move) == DragDropEffects.Move)
				action |= DragDropOption.Move;
			if ((dragDropEffects & DragDropEffects.Copy) == DragDropEffects.Copy)
				action |= DragDropOption.Copy;
			return action;
		}

		public static DragDropEffects GetDragDropEffects(DragDropOption dragDropAction)
		{
			DragDropEffects effects = DragDropEffects.None;
			if ((dragDropAction & DragDropOption.Move) == DragDropOption.Move)
				effects |= DragDropEffects.Move;
			if ((dragDropAction & DragDropOption.Copy) == DragDropOption.Copy)
				effects |= DragDropEffects.Copy;
			return effects;
		}

		[Flags]
		private enum DragEventArgsKeyState
		{
			LeftButton = 1,
			RightButton = 2,
			Shift = 4,
			Ctrl = 8,
			MiddleButton = 16,
			Alt = 32
		}
	}
}
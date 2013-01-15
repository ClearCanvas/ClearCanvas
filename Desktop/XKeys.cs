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

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Enumeration of all the (potentially) available keys on a keyboard.
	/// </summary>
	/// <remarks>
	/// <see cref="XKeys"/> values are to be interpreted as a single key code in a bitwise combination
	/// with any number of modifiers (<see cref="Control"/>, <see cref="Alt"/> and <see cref="Shift"/>).
	/// The key code can be selected by using the <see cref="KeyCode"/> mask, and the modifiers can
	/// likewise be selected by using the <see cref="Modifiers"/> mask.
	/// </remarks>
	[Flags]
	[TypeConverter(typeof (XKeysConverter))]
	public enum XKeys
	{
		/// <summary>
		/// The A key.
		/// </summary>
		A = 65,

		/// <summary>
		/// The add key.
		/// </summary>
		Add = 107,

		/// <summary>
		/// The ALT modifier.
		/// </summary>
		Alt = 262144,

		/// <summary>
		/// The application key.
		/// </summary>
		Apps = 93,

		/// <summary>
		/// The ATTN key.
		/// </summary>
		Attn = 246,

		/// <summary>
		/// The B key.
		/// </summary>
		B = 66,

		/// <summary>
		/// The BACKSPACE key.
		/// </summary>
		Backspace = 8,

		/// <summary>
		/// The browser back key.
		/// </summary>
		BrowserBack = 166,

		/// <summary>
		/// The browser favorites key.
		/// </summary>
		BrowserFavorites = 171,

		/// <summary>
		/// The browser forward key.
		/// </summary>
		BrowserForward = 167,

		/// <summary>
		/// The browser home key.
		/// </summary>
		BrowserHome = 172,

		/// <summary>
		/// The browser refresh key.
		/// </summary>
		BrowserRefresh = 168,

		/// <summary>
		/// The browser search key.
		/// </summary>
		BrowserSearch = 170,

		/// <summary>
		/// The browser stop key.
		/// </summary>
		BrowserStop = 169,

		/// <summary>
		/// The C key.
		/// </summary>
		C = 67,

		/// <summary>
		/// The CANCEL key.
		/// </summary>
		Cancel = 3,

		/// <summary>
		/// The CAPS LOCK key.
		/// </summary>
		CapsLock = 20,

		/// <summary>
		/// The CLEAR key.
		/// </summary>
		Clear = 12,

		/// <summary>
		/// The CTRL modifier.
		/// </summary>
		Control = 131072,

		/// <summary>
		/// The CTRL key.
		/// </summary>
		ControlKey = 17,

		/// <summary>
		/// The CRSEL key.
		/// </summary>
		Crsel = 247,

		/// <summary>
		/// The D key.
		/// </summary>
		D = 68,

		/// <summary>
		/// The 0 key.
		/// </summary>
		Digit0 = 48,

		/// <summary>
		/// The 1 key.
		/// </summary>
		Digit1 = 49,

		/// <summary>
		/// The 2 key.
		/// </summary>
		Digit2 = 50,

		/// <summary>
		/// The 3 key.
		/// </summary>
		Digit3 = 51,

		/// <summary>
		/// The 4 key.
		/// </summary>
		Digit4 = 52,

		/// <summary>
		/// The 5 key.
		/// </summary>
		Digit5 = 53,

		/// <summary>
		/// The 6 key.
		/// </summary>
		Digit6 = 54,

		/// <summary>
		/// The 7 key.
		/// </summary>
		Digit7 = 55,

		/// <summary>
		/// The 8 key.
		/// </summary>
		Digit8 = 56,

		/// <summary>
		/// The 9 key.
		/// </summary>
		Digit9 = 57,

		/// <summary>
		/// The decimal key.
		/// </summary>
		Decimal = 110,

		/// <summary>
		/// The DELETE key.
		/// </summary>
		Delete = 46,

		/// <summary>
		/// The divide key.
		/// </summary>
		Divide = 111,

		/// <summary>
		/// The DOWN ARROW key.
		/// </summary>
		Down = 40,

		/// <summary>
		/// The E key.
		/// </summary>
		E = 69,

		/// <summary>
		/// The END key.
		/// </summary>
		End = 35,

		/// <summary>
		/// The ENTER key.
		/// </summary>
		Enter = 13,

		/// <summary>
		/// The ERASE EOF key.
		/// </summary>
		EraseEof = 249,

		/// <summary>
		/// The ESCAPE key.
		/// </summary>
		Escape = 27,

		/// <summary>
		/// The EXECUTE key.
		/// </summary>
		Execute = 43,

		/// <summary>
		/// The EXSEL key.
		/// </summary>
		Exsel = 248,

		/// <summary>
		/// The F key.
		/// </summary>
		F = 70,

		/// <summary>
		/// The F1 key.
		/// </summary>
		F1 = 112,

		/// <summary>
		/// The F2 key.
		/// </summary>
		F2 = 113,

		/// <summary>
		/// The F3 key.
		/// </summary>
		F3 = 114,

		/// <summary>
		/// The F4 key.
		/// </summary>
		F4 = 115,

		/// <summary>
		/// The F5 key.
		/// </summary>
		F5 = 116,

		/// <summary>
		/// The F6 key.
		/// </summary>
		F6 = 117,

		/// <summary>
		/// The F7 key.
		/// </summary>
		F7 = 118,

		/// <summary>
		/// The F8 key.
		/// </summary>
		F8 = 119,

		/// <summary>
		/// The F9 key.
		/// </summary>
		F9 = 120,

		/// <summary>
		/// The F10 key.
		/// </summary>
		F10 = 121,

		/// <summary>
		/// The F11 key.
		/// </summary>
		F11 = 122,

		/// <summary>
		/// The F12 key.
		/// </summary>
		F12 = 123,

		/// <summary>
		/// The F13 key.
		/// </summary>
		F13 = 124,

		/// <summary>
		/// The F14 key.
		/// </summary>
		F14 = 125,

		/// <summary>
		/// The F15 key.
		/// </summary>
		F15 = 126,

		/// <summary>
		/// The F16 key.
		/// </summary>
		F16 = 127,

		/// <summary>
		/// The F17 key.
		/// </summary>
		F17 = 128,

		/// <summary>
		/// The F18 key.
		/// </summary>
		F18 = 129,

		/// <summary>
		/// The F19 key.
		/// </summary>
		F19 = 130,

		/// <summary>
		/// The F20 key.
		/// </summary>
		F20 = 131,

		/// <summary>
		/// The F21 key.
		/// </summary>
		F21 = 132,

		/// <summary>
		/// The F22 key.
		/// </summary>
		F22 = 133,

		/// <summary>
		/// The F23 key.
		/// </summary>
		F23 = 134,

		/// <summary>
		/// The F24 key.
		/// </summary>
		F24 = 135,

		/// <summary>
		/// The G key.
		/// </summary>
		G = 71,

		/// <summary>
		/// The H key.
		/// </summary>
		H = 72,

		/// <summary>
		/// The HELP key.
		/// </summary>
		Help = 47,

		/// <summary>
		/// The HOME key.
		/// </summary>
		Home = 36,

		/// <summary>
		/// The I key.
		/// </summary>
		I = 73,

		/// <summary>
		/// The IME accept key.
		/// </summary>
		ImeAccept = 30,

		/// <summary>
		/// The IME convert key.
		/// </summary>
		ImeConvert = 28,

		/// <summary>
		/// The IME final mode key.
		/// </summary>
		ImeFinalMode = 24,

		/// <summary>
		/// The IME mode 1 key (Kana or Hangul mode)
		/// </summary>
		ImeMode1 = 21,

		/// <summary>
		/// The IME mode 3 key (Junja mode).
		/// </summary>
		ImeMode3 = 23,

		/// <summary>
		/// The IME mode 5 key (Kanji or Hanja mode).
		/// </summary>
		ImeMode5 = 25,

		/// <summary>
		/// The IME mode change key.
		/// </summary>
		ImeModeChange = 31,

		/// <summary>
		/// The IME non convert key.
		/// </summary>
		ImeNonConvert = 29,

		/// <summary>
		/// The INSERT key.
		/// </summary>
		Insert = 45,

		/// <summary>
		/// The J key.
		/// </summary>
		J = 74,

		/// <summary>
		/// The K key.
		/// </summary>
		K = 75,

		/// <summary>
		/// The key code bit mask.
		/// </summary>
		KeyCode = 65535,

		/// <summary>
		/// The L key.
		/// </summary>
		L = 76,

		/// <summary>
		/// The launch application 1 key.
		/// </summary>
		LaunchApplication1 = 182,

		/// <summary>
		/// The launch application 2 key.
		/// </summary>
		LaunchApplication2 = 183,

		/// <summary>
		/// The launch mail application key.
		/// </summary>
		LaunchMail = 180,

		/// <summary>
		/// The left mouse button (mouse button 1).
		/// </summary>
		LeftMouseButton = 1,

		/// <summary>
		/// The left CTRL key.
		/// </summary>
		LeftControlKey = 162,

		/// <summary>
		/// The LEFT ARROW key.
		/// </summary>
		Left = 37,

		/// <summary>
		/// The LINE FEED key.
		/// </summary>
		LineFeed = 10,

		/// <summary>
		/// The left SHIFT key.
		/// </summary>
		LeftShiftKey = 160,

		/// <summary>
		/// The left ALT key.
		/// </summary>
		LeftAltKey = 164,

		/// <summary>
		/// The left Windows key.
		/// </summary>
		LeftWinKey = 91,

		/// <summary>
		/// The M key.
		/// </summary>
		M = 77,

		/// <summary>
		/// The middle mouse button (mouse button 3).
		/// </summary>
		MiddleMouseButton = 4,

		/// <summary>
		/// The media player next track key.
		/// </summary>
		MediaNextTrack = 176,

		/// <summary>
		/// The media player play/pause key.
		/// </summary>
		MediaPlayPause = 179,

		/// <summary>
		/// The media player previous track key.
		/// </summary>
		MediaPreviousTrack = 177,

		/// <summary>
		/// The media player stop key.
		/// </summary>
		MediaStop = 178,

		/// <summary>
		/// The ALT key.
		/// </summary>
		AltKey = 18,

		/// <summary>
		/// The modifiers bit mask.
		/// </summary>
		Modifiers = -65536,

		/// <summary>
		/// The multiply key.
		/// </summary>
		Multiply = 106,

		/// <summary>
		/// The N key.
		/// </summary>
		N = 78,

		/// <summary>
		/// Represents no key (the empty value).
		/// </summary>
		None = 0,

		/// <summary>
		/// The NUM LOCK key.
		/// </summary>
		NumLock = 144,

		/// <summary>
		/// The 0 key on the number pad.
		/// </summary>
		NumPad0 = 96,

		/// <summary>
		/// The 1 key on the number pad.
		/// </summary>
		NumPad1 = 97,

		/// <summary>
		/// The 2 key on the number pad.
		/// </summary>
		NumPad2 = 98,

		/// <summary>
		/// The 3 key on the number pad.
		/// </summary>
		NumPad3 = 99,

		/// <summary>
		/// The 4 key on the number pad.
		/// </summary>
		NumPad4 = 100,

		/// <summary>
		/// The 5 key on the number pad.
		/// </summary>
		NumPad5 = 101,

		/// <summary>
		/// The 6 key on the number pad.
		/// </summary>
		NumPad6 = 102,

		/// <summary>
		/// The 7 key on the number pad.
		/// </summary>
		NumPad7 = 103,

		/// <summary>
		/// The 8 key on the number pad.
		/// </summary>
		NumPad8 = 104,

		/// <summary>
		/// The 9 key on the number pad.
		/// </summary>
		NumPad9 = 105,

		/// <summary>
		/// The O key.
		/// </summary>
		O = 79,

		/// <summary>
		/// The OEM 8 key (keycode 0xDF).
		/// </summary>
		Oem8 = 223,

		/// <summary>
		/// The OEM backslash key on a typical U.S. keyboard layout (OEM 102 key).
		/// </summary>
		OemBackslash = 226,

		/// <summary>
		/// The OEM clear key.
		/// </summary>
		OemClear = 254,

		/// <summary>
		/// The OEM close brackets key on a typical U.S. keyboard layout (OEM 6 key).
		/// </summary>
		OemCloseBrackets = 221,

		/// <summary>
		/// The OEM comma key.
		/// </summary>
		OemComma = 188,

		/// <summary>
		/// The OEM minus key.
		/// </summary>
		OemMinus = 189,

		/// <summary>
		/// The OEM open brackets key on a typical U.S. keyboard layout (OEM 4 key).
		/// </summary>
		OemOpenBrackets = 219,

		/// <summary>
		/// The OEM period key.
		/// </summary>
		OemPeriod = 190,

		/// <summary>
		/// The OEM pipe key on a typical U.S. keyboard layout (OEM 5 key).
		/// </summary>
		OemPipe = 220,

		/// <summary>
		/// The OEM plus key.
		/// </summary>
		OemPlus = 187,

		/// <summary>
		/// The OEM question mark key on a typical U.S. keyboard layout (OEM 2 key).
		/// </summary>
		OemQuestion = 191,

		/// <summary>
		/// The OEM quotes key on a typical U.S. keyboard layout (OEM 7 key).
		/// </summary>
		OemQuotes = 222,

		/// <summary>
		/// The OEM semicolon key on a typical U.S. keyboard layout (OEM 1 key).
		/// </summary>
		OemSemicolon = 186,

		/// <summary>
		/// The OEM tilde key on a typical U.S. keyboard layout (OEM 3 key).
		/// </summary>
		OemTilde = 192,

		/// <summary>
		/// The P key.
		/// </summary>
		P = 80,

		/// <summary>
		/// The PA1 key.
		/// </summary>
		Pa1 = 253,

		/// <summary>
		/// The PAGE DOWN key.
		/// </summary>
		PageDown = 34,

		/// <summary>
		/// The PAGE UP key.
		/// </summary>
		PageUp = 33,

		/// <summary>
		/// The PAUSE key.
		/// </summary>
		Pause = 19,

		/// <summary>
		/// The PLAY key.
		/// </summary>
		Play = 250,

		/// <summary>
		/// The PRINT key.
		/// </summary>
		Print = 42,

		/// <summary>
		/// The PRINT SCREEN key.
		/// </summary>
		PrintScreen = 44,

		/// <summary>
		/// The PROCESS KEY key.
		/// </summary>
		ProcessKey = 229,

		/// <summary>
		/// The Q key.
		/// </summary>
		Q = 81,

		/// <summary>
		/// The R key.
		/// </summary>
		R = 82,

		/// <summary>
		/// The right mouse button (mouse button 2).
		/// </summary>
		RightMouseButton = 2,

		/// <summary>
		/// The right CTRL key.
		/// </summary>
		RightControlKey = 163,

		/// <summary>
		/// The RIGHT ARROW key.
		/// </summary>
		Right = 39,

		/// <summary>
		/// The right ALT key.
		/// </summary>
		RightAltKey = 165,

		/// <summary>
		/// The right SHIFT key.
		/// </summary>
		RightShiftKey = 161,

		/// <summary>
		/// The right Windows key.
		/// </summary>
		RightWinKey = 92,

		/// <summary>
		/// The S key.
		/// </summary>
		S = 83,
		
		/// <summary>
		/// The SCROLL LOCK key.
		/// </summary>
		ScrollLock = 145,

		/// <summary>
		/// The SELECT key.
		/// </summary>
		Select = 41,

		/// <summary>
		/// The select media key.
		/// </summary>
		SelectMedia = 181,

		/// <summary>
		/// The separator key.
		/// </summary>
		Separator = 108,

		/// <summary>
		/// The SHIFT modifier.
		/// </summary>
		Shift = 65536,

		/// <summary>
		/// The SHIFT key.
		/// </summary>
		ShiftKey = 16,

		/// <summary>
		/// The SPACEBAR key.
		/// </summary>
		Space = 32,

		/// <summary>
		/// The subtract key.
		/// </summary>
		Subtract = 109,

		/// <summary>
		/// The T key.
		/// </summary>
		T = 84,

		/// <summary>
		/// The TAB key.
		/// </summary>
		Tab = 9,

		/// <summary>
		/// The U key.
		/// </summary>
		U = 85,

		/// <summary>
		/// The UP ARROW key.
		/// </summary>
		Up = 38,

		/// <summary>
		/// The V key.
		/// </summary>
		V = 86,

		/// <summary>
		/// The volume down key.
		/// </summary>
		VolumeDown = 174,

		/// <summary>
		/// The volume mute key.
		/// </summary>
		VolumeMute = 173,

		/// <summary>
		/// The volume up key.
		/// </summary>
		VolumeUp = 175,

		/// <summary>
		/// The W key.
		/// </summary>
		W = 87,

		/// <summary>
		/// The X key.
		/// </summary>
		X = 88,

		/// <summary>
		/// The first X mouse button (mouse button 4).
		/// </summary>
		XMouseButton1 = 5,

		/// <summary>
		/// The second X mouse button (mouse button 5).
		/// </summary>
		XMouseButton2 = 6,

		/// <summary>
		/// The Y key.
		/// </summary>
		Y = 89,

		/// <summary>
		/// The Z key.
		/// </summary>
		Z = 90,

		/// <summary>
		/// The ZOOM key.
		/// </summary>
		Zoom = 251
	}
}
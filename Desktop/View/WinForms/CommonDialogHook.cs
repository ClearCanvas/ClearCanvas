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
using System.Runtime.InteropServices;
using System.Text;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Wrapper for a native application hook that allows limited customization of dialog boxes (class #32770).
	/// </summary>
	internal sealed class CommonDialogHook : IDisposable
	{
		private readonly IDictionary<int, string> _buttonCaptions = new Dictionary<int, string> {{1, SR.ButtonOk}, {2, SR.ButtonCancel}, {3, SR.ButtonAbort}, {4, SR.ButtonRetry}, {5, SR.ButtonIgnore}, {6, SR.ButtonYes}, {7, SR.ButtonNo}};

		private CallWndRetProc _hookCallback;
		private IntPtr _hHook;

		/// <summary>
		/// Initializes a new instance of <see cref="CommonDialogHook"/>. Don't forget to <see cref="Dispose"/> it!
		/// </summary>
		public CommonDialogHook()
		{
			const int WH_CALLWNDPROCRET = 12;

#pragma warning disable 618,612
			// we have to use the unmanaged thread ID, and it should remain stable for the short lifetime of this hook anyway
			var unmanagedThreadId = AppDomain.GetCurrentThreadId();
#pragma warning restore 618,612

			_hookCallback = new CallWndRetProc(WndProcRetCallback); // must be written this way to ensure the delegate instance does not go out of scope early
			_hHook = SetWindowsHookEx(WH_CALLWNDPROCRET, _hookCallback, IntPtr.Zero, unmanagedThreadId); // install WndProcRet hook
		}

		~CommonDialogHook()
		{
			PerformDispose();
		}

		public void Dispose()
		{
			PerformDispose();
			GC.SuppressFinalize(this);
		}

		private void PerformDispose()
		{
			if (_hHook != IntPtr.Zero)
			{
				// uninstall hook
				UnhookWindowsHookEx(_hHook);
				_hHook = IntPtr.Zero;
			}

			_hookCallback = null;
		}

		/// <summary>
		/// Mapping of control IDs to captions for buttons on dialog boxes.
		/// </summary>
		public IDictionary<int, string> ButtonCaptions
		{
			get { return _buttonCaptions; }
		}

		/// <summary>
		/// Implementation of <see cref="CallWndRetProc"/>
		/// </summary>
		private IntPtr WndProcRetCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			const int WM_INITDIALOG = 0x110;
			const string CLASS_DIALOGBOX = "#32770";

			if (nCode >= 0)
			{
				var wndProcRetArgs = (CWPRETSTRUCT) Marshal.PtrToStructure(lParam, typeof (CWPRETSTRUCT));
				if (wndProcRetArgs.message == WM_INITDIALOG) // check that a dialog window has finished initializing
				{
					var className = new StringBuilder(10);
					GetClassName(wndProcRetArgs.hWnd, className, className.Capacity);
					if (className.ToString() == CLASS_DIALOGBOX) // check that the window class is the dialog box
					{
						// must be written this way to ensure the delegate instance does not go out of scope early
						var callback = new EnumWindowsProc(EnumChildWindowsCallback);
						EnumChildWindows(wndProcRetArgs.hWnd, callback, IntPtr.Zero); // enumerate over child windows
					}
				}
			}
			return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
		}

		/// <summary>
		/// Implementation of <see cref="EnumWindowsProc"/>
		/// </summary>
		private bool EnumChildWindowsCallback(IntPtr hWnd, IntPtr lParam)
		{
			const string CLASS_BUTTON = "Button";

			var className = new StringBuilder(10);
			GetClassName(hWnd, className, className.Capacity);
			if (className.ToString() == CLASS_BUTTON) // check that the window class is the button
			{
				// try to get a new button caption based on dialog control ID
				var controlId = GetDlgCtrlID(hWnd);
				var caption = _buttonCaptions.ContainsKey(controlId) ? _buttonCaptions[controlId] : string.Empty;
				if (!string.IsNullOrEmpty(caption))
					SetWindowText(hWnd, caption);
			}
			return true;
		}

		#region P/Invoke Declarations

		/// <summary>
		/// Retrieves the name of the class to which the specified window belongs.
		/// </summary>
		/// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
		/// <param name="lpClassName">The class name string.</param>
		/// <param name="nMaxCount">The length of the lpClassName buffer, in characters. The buffer must be large enough to include the terminating null character; otherwise, the class name string is truncated to nMaxCount-1 characters.</param>
		/// <returns>If the function succeeds, the return value is the number of characters copied to the buffer, not including the terminating null character. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms633582.aspx"/>
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		/// <summary>
		/// Changes the text of the specified window's title bar (if it has one). If the specified window is a control, the text of the control is changed. However, SetWindowText cannot change the text of a control in another application.
		/// </summary>
		/// <param name="hWnd">A handle to the window or control whose text is to be changed.</param>
		/// <param name="lpString">The new title or control text.</param>
		/// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms633546.aspx"/>
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern bool SetWindowText(IntPtr hWnd, String lpString);

		// ReSharper disable InconsistentNaming

		/// <summary>
		/// Retrieves the identifier of the specified control. 
		/// </summary>
		/// <param name="hWndCtl">A handle to the control. </param>
		/// <returns>If the function succeeds, the return value is the identifier of the control. If the function fails, the return value is zero. An invalid value for the hwndCtl parameter, for example, will cause the function to fail. To get extended error information, call GetLastError.</returns>
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms645478.aspx"/>
		[DllImport("user32.dll", SetLastError = true)]
		private static extern int GetDlgCtrlID(IntPtr hWndCtl);

		// ReSharper restore InconsistentNaming

		/// <summary>
		/// Enumerates the child windows that belong to the specified parent window by passing the handle to each child window, in turn, to an application-defined callback function. EnumChildWindows continues until the last child window is enumerated or the callback function returns FALSE.
		/// </summary>
		/// <param name="hWndParent">A handle to the parent window whose child windows are to be enumerated. If this parameter is NULL, this function is equivalent to EnumWindows. </param>
		/// <param name="lpEnumFunc">A pointer to an application-defined callback function. For more information, see EnumChildProc. </param>
		/// <param name="lParam">An application-defined value to be passed to the callback function. </param>
		/// <returns>The return value is not used.</returns>
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms633494.aspx"/>
		[DllImport("user32.dll")]
		[return : MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

		/// <summary>
		/// Installs an application-defined hook procedure into a hook chain. You would install a hook procedure to monitor the system for certain types of events. These events are associated either with a specific thread or with all threads in the same desktop as the calling thread. 
		/// </summary>
		/// <param name="hookType">The type of hook procedure to be installed. This parameter can be one of the following values. </param>
		/// <param name="lpfn">A pointer to the hook procedure. If the dwThreadId parameter is zero or specifies the identifier of a thread created by a different process, the lpfn parameter must point to a hook procedure in a DLL. Otherwise, lpfn can point to a hook procedure in the code associated with the current process. </param>
		/// <param name="hMod">A handle to the DLL containing the hook procedure pointed to by the lpfn parameter. The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by the current process and if the hook procedure is within the code associated with the current process. </param>
		/// <param name="dwThreadId">The identifier of the thread with which the hook procedure is to be associated. If this parameter is zero, the hook procedure is associated with all existing threads running in the same desktop as the calling thread. </param>
		/// <returns>If the function succeeds, the return value is the handle to the hook procedure. If the function fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms644990.aspx"/>
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		private static extern IntPtr SetWindowsHookEx(int hookType, CallWndRetProc lpfn, IntPtr hMod, int dwThreadId);

		/// <summary>
		/// Passes the hook information to the next hook procedure in the current hook chain. A hook procedure can call this function either before or after processing the hook information. 
		/// </summary>
		/// <param name="hHook">This parameter is ignored. </param>
		/// <param name="nCode">The hook code passed to the current hook procedure. The next hook procedure uses this code to determine how to process the hook information.</param>
		/// <param name="wParam">The wParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
		/// <param name="lParam">The lParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
		/// <returns>This value is returned by the next hook procedure in the chain. The current hook procedure must also return this value. The meaning of the return value depends on the hook type. For more information, see the descriptions of the individual hook procedures.</returns>
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms644974.aspx"/>
		[DllImport("user32.dll")]
		private static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);

		/// <summary>
		/// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function. 
		/// </summary>
		/// <param name="hHook">A handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx. </param>
		/// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms644993.aspx"/>
		[DllImport("user32.dll", SetLastError = true)]
		[return : MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hHook);

		/// <summary>
		/// An application-defined callback function used with the EnumWindows or EnumDesktopWindows function. It receives top-level window handles. The WNDENUMPROC type defines a pointer to this callback function. EnumWindowsProc is a placeholder for the application-defined function name. 
		/// </summary>
		/// <param name="hWnd">A handle to a top-level window. </param>
		/// <param name="lParam">The application-defined value given in EnumWindows or EnumDesktopWindows. </param>
		/// <returns>To continue enumeration, the callback function must return TRUE; to stop enumeration, it must return FALSE. </returns>
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms633498.aspx"/>
		private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

		/// <summary>
		/// An application-defined or library-defined callback function used with the SetWindowsHookEx function. The system calls this function after the SendMessage function is called. The hook procedure can examine the message; it cannot modify it. The HOOKPROC type defines a pointer to this callback function. CallWndRetProc is a placeholder for the application-defined or library-defined function name.
		/// </summary>
		/// <param name="nCode">Specifies whether the hook procedure must process the message. If nCode is HC_ACTION, the hook procedure must process the message. If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and should return the value returned by CallNextHookEx. </param>
		/// <param name="wParam">Specifies whether the message is sent by the current process. If the message is sent by the current process, it is nonzero; otherwise, it is NULL. </param>
		/// <param name="lParam">A pointer to a CWPRETSTRUCT structure that contains details about the message. </param>
		/// <returns>If nCode is less than zero, the hook procedure must return the value returned by CallNextHookEx. If nCode is greater than or equal to zero, it is highly recommended that you call CallNextHookEx and return the value it returns; otherwise, other applications that have installed WH_CALLWNDPROCRET hooks will not receive hook notifications and may behave incorrectly as a result. If the hook procedure does not call CallNextHookEx, the return value should be zero. </returns>
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms644976%28v=VS.85%29.aspx"/>
		private delegate IntPtr CallWndRetProc(int nCode, IntPtr wParam, IntPtr lParam);

		// ReSharper disable InconsistentNaming

		/// <summary>
		/// Defines the message parameters passed to a WH_CALLWNDPROCRET hook procedure, CallWndRetProc. 
		/// </summary>
		/// <see cref="http://msdn.microsoft.com/en-us/library/ms644963.aspx"/>
		[StructLayout(LayoutKind.Sequential)]
		private struct CWPRETSTRUCT
		{
			/// <summary>
			/// The return value of the window procedure that processed the message specified by the message value. 
			/// </summary>
			public IntPtr lResult;

			/// <summary>
			/// Additional information about the message. The exact meaning depends on the message value. 
			/// </summary>
			public IntPtr lParam;

			/// <summary>
			/// Additional information about the message. The exact meaning depends on the message value. 
			/// </summary>
			public IntPtr wParam;

			/// <summary>
			/// The message. 
			/// </summary>
			public uint message;

			/// <summary>
			/// A handle to the window that processed the message specified by the message value. 
			/// </summary>
			public IntPtr hWnd;
		}

		// ReSharper restore InconsistentNaming

		#endregion
	}
}
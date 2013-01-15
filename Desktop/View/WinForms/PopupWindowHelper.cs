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

#region Inline Attributions
// The source code contained in this file is based on an original work
// from http://vbaccelerator.com/home/NET/Code/Controls/Popup_Windows/Popup_Windows/article.asp
// the license of which is reproduced below:
//
//
// vbAccelerator Software License
// Version 1.0
// 
// Copyright (c) 2002 vbAccelerator.com
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//  The end-user documentation included with the redistribution, if any, must include the following acknowledgment:
// "This product includes software developed by vbAccelerator (http://vbaccelerator.com/)."
// Alternately, this acknowledgment may appear in the software itself, if and wherever such third-party acknowledgments normally appear.
// 
// The names "vbAccelerator" and "vbAccelerator.com" must not be used to endorse or promote products derived from this software without prior written permission. For written permission, please contact vbAccelerator through steve@vbaccelerator.com.
// Products derived from this software may not be called "vbAccelerator", nor may "vbAccelerator" appear in their name, without prior written permission of vbAccelerator.
// THIS SOFTWARE IS PROVIDED "AS IS" AND ANY EXPRESSED OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL 
// VBACCELERATOR OR ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, 
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY 
// OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vbAccelerator.Components.Controls
{

	#region Event Argument Classes
	/// <summary>
	/// Contains event information for a <see cref="PopupClosed"/> event.
	/// </summary>
	public class PopupClosedEventArgs : EventArgs
	{
		/// <summary>
		/// The popup form.
		/// </summary>
		private Form popup = null;

		/// <summary>
		/// Gets the popup form which is being closed.
		/// </summary>
		public Form Popup
		{
			get
			{
				return this.popup;
			}
		}

		/// <summary>
		/// Constructs a new instance of this class for the specified
		/// popup form.
		/// </summary>
		/// <param name="popup">Popup Form which is being closed.</param>
		public PopupClosedEventArgs(Form popup)
		{
			this.popup = popup;
		}
	}

	/// <summary>
	/// Arguments to a <see cref="PopupCancelEvent"/>.  Provides a
	/// reference to the popup form that is to be closed and 
	/// allows the operation to be cancelled.
	/// </summary>
	public class PopupCancelEventArgs : EventArgs
	{
		/// <summary>
		/// Whether to cancel the operation
		/// </summary>
		private bool cancel = false;
		/// <summary>
		/// Mouse down location
		/// </summary>
		private Point location;
		/// <summary>
		/// Popup form.
		/// </summary>
		private Form popup = null;

		/// <summary>
		/// Constructs a new instance of this class.
		/// </summary>
		/// <param name="popup">The popup form</param>
		/// <param name="location">The mouse location, if any, where the
		/// mouse event that would cancel the popup occured.</param>
		public PopupCancelEventArgs(Form popup, Point location)
		{
			this.popup = popup;
			this.location = location;
			this.cancel = false;
		}

		/// <summary>
		/// Gets the popup form
		/// </summary>
		public Form Popup
		{
			get
			{
				return this.popup;
			}
		}

		/// <summary>
		/// Gets the location that the mouse down which would cancel this 
		/// popup occurred
		/// </summary>
		public Point CursorLocation
		{
			get
			{
				return this.location;
			}
		}

		/// <summary>
		/// Gets/sets whether to cancel closing the form. Set to
		/// <c>true</c> to prevent the popup from being closed.
		/// </summary>
		public bool Cancel
		{
			get
			{
				return this.cancel;
			}
			set
			{
				this.cancel = value;
			}
		}

	}
	#endregion

	#region Delegates
	/// <summary>
	/// Represents the method which responds to a <see cref="PopupClosed"/> event.
	/// </summary>
	public delegate void PopupClosedEventHandler(object sender, PopupClosedEventArgs e);

	/// <summary>
	/// Represents the method which responds to a <see cref="PopupCancel"/> event.
	/// </summary>
	public delegate void PopupCancelEventHandler(object sender, PopupCancelEventArgs e);
	#endregion

	#region PopupWindowHelper
	/// <summary>
	/// A class to assist in creating popup windows like Combo Box drop-downs and Menus.
	/// This class includes functionality to keep the title bar of the popup owner form
	/// active whilst the popup is displayed, and to automatically cancel the popup
	/// whenever the user clicks outside the popup window or shifts focus to another 
	/// application.
	/// </summary>
	public class PopupWindowHelper : NativeWindow
	{
		#region Unmanaged Code
		[DllImport("user32", CharSet = CharSet.Auto)]
		private extern static int SendMessage(IntPtr handle, int msg, int wParam, IntPtr lParam);

		[DllImport("user32", CharSet = CharSet.Auto)]
		private extern static int PostMessage(IntPtr handle, int msg, int wParam, IntPtr lParam);

		private const int WM_ACTIVATE = 0x006;
		private const int WM_ACTIVATEAPP = 0x01C;
		private const int WM_NCACTIVATE = 0x086;

		[DllImport("user32")]
		private extern static void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

		private const int KEYEVENTF_KEYUP       = 0x0002;
		#endregion

		#region Member Variables
		/// <summary>
		/// Event Handler to detect when the popup window is closed
		/// </summary>
		private EventHandler popClosedHandler = null;
		/// <summary>
		/// Message filter to detect mouse clicks anywhere in the application
		/// whilst the popup window is being displayed.
		/// </summary>
		private PopupWindowHelperMessageFilter filter = null;
		/// <summary>
		/// The popup form that is being shown.
		/// </summary>
		private Form popup = null;
		/// <summary>
		/// The owner of the popup form that is being shown:
		/// </summary>
		private Form owner = null;
		/// <summary>
		/// Whether the popup is showing or not.
		/// </summary>
		private bool popupShowing = false;
		/// <summary>
		/// Whether the popup has been cancelled, notified by PopupCancel,
		/// rather than closed.
		/// </summary>
		private bool skipClose = false;
		#endregion

		/// <summary>
		/// Raised when the popup form is closed.
		/// </summary>
		public event PopupClosedEventHandler PopupClosed;
		/// <summary>
		/// Raised when the Popup Window is about to be cancelled.  The
		/// <see cref="PopupCancelEventArgs.Cancel"/> property can be
		/// set to <c>true</c> to prevent the form from being cancelled.
		/// </summary>
		public event PopupCancelEventHandler PopupCancel;

		/// <summary>
		/// Shows the specified Form as a popup window, keeping the
		/// Owner's title bar active and preparing to cancel the popup
		/// should the user click anywhere outside the popup window.
		/// <para>Typical code to use this message is as follows:</para>
		/// <code>
		///    frmPopup popup = new frmPopup();
		///    Point location = this.PointToScreen(new Point(button1.Left, button1.Bottom));
		///    popupHelper.ShowPopup(this, popup, location);
		/// </code>
		/// <para>Put as much initialisation code as possible
		/// into the popup form's constructor, rather than the <see cref="System.Windows.Forms.Load"/>
		/// event as this will improve visual appearance.</para>
		/// </summary>
		/// <param name="owner">Main form which owns the popup</param>
		/// <param name="popup">Window to show as a popup</param>
		/// <param name="location">Location relative to the screen to show the popup at.</param>
		public void ShowPopup(Form owner, Form popup, Point location)
		{

			this.owner = owner;
			this.popup = popup;

			// Start checking for the popup being cancelled
			Application.AddMessageFilter(filter);

			// Set the location of the popup form:
			popup.StartPosition = FormStartPosition.Manual;
			popup.Location = location; 
			// Make it owned by the window that's displaying it:
			owner.AddOwnedForm(popup);			
			// Respond to the Closed event in case the popup
			// is closed by its own internal means
			popClosedHandler = new EventHandler(popup_Closed);
			popup.Closed += popClosedHandler;

			// Show the popup:
			this.popupShowing = true;
			popup.Show();
			popup.Activate();
			
			// A little bit of fun.  We've shown the popup,
			// but because we've kept the main window's
			// title bar in focus the tab sequence isn't quite
			// right.  This can be fixed by sending a tab,
			// but that on its own would shift focus to the
			// second control in the form.  So send a tab,
			// followed by a reverse-tab.

			// Send a Tab command:
			keybd_event((byte) Keys.Tab, 0, 0, 0);
			keybd_event((byte) Keys.Tab, 0, KEYEVENTF_KEYUP, 0);

			// Send a reverse Tab command:
			keybd_event((byte) Keys.ShiftKey, 0, 0, 0);
			keybd_event((byte) Keys.Tab, 0, 0, 0);
			keybd_event((byte) Keys.Tab, 0, KEYEVENTF_KEYUP, 0);
			keybd_event((byte) Keys.ShiftKey, 0, KEYEVENTF_KEYUP, 0);


			// Start filtering for mouse clicks outside the popup
			filter.Popup = popup;
					
		}

		/// <summary>
		/// Responds to the <see cref="System.Windows.Forms.Form.Closed"/>
		/// event from the popup form.
		/// </summary>
		/// <param name="sender">Popup form that has been closed.</param>
		/// <param name="e">Not used.</param>
		private void popup_Closed(object sender, EventArgs e)
		{
			ClosePopup();
		}

		/// <summary>
		/// Subclasses the owning form's existing Window Procedure to enables the 
		/// title bar to remain active when a popup is show, and to detect if
		/// the user clicks onto another application whilst the popup is visible.
		/// </summary>
		/// <param name="m">Window Procedure Message</param>
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			if (this.popupShowing)
			{
				// check for WM_ACTIVATE and WM_NCACTIVATE
				if (m.Msg == WM_NCACTIVATE)
				{
					// Check if the title bar will made inactive:
					if (((int) m.WParam) == 0)
					{
						// If so reactivate it.
						SendMessage(this.Handle, WM_NCACTIVATE, 1, IntPtr.Zero);
						
						// Note it's no good to try and consume this message;
						// if you try to do that you'll end up with windows
						// that don't respond.
					}

				}
				else if (m.Msg == WM_ACTIVATEAPP)
				{
					// Check if the application is being deactivated.
					if ((int)m.WParam == 0)
					{
						// It is so cancel the popup:
						ClosePopup();
						// And put the title bar into the inactive state:
						PostMessage(this.Handle, WM_NCACTIVATE, 0, IntPtr.Zero);
					}
				}
			}
		}

		/// <summary>
		/// Called when the popup is being hidden.
		/// </summary>
		public void ClosePopup()
		{
			if (this.popupShowing)
			{
				if (!skipClose)
				{
					// Raise event to owner
					OnPopupClosed(new PopupClosedEventArgs(this.popup));					
				}
				skipClose = false;

				// Make sure the popup is closed and we've cleaned
				// up:
				this.owner.RemoveOwnedForm(this.popup);
				this.popupShowing = false;
				this.popup.Closed -= popClosedHandler;
				this.popClosedHandler = null;
				this.popup.Close();
				// No longer need to filter for clicks outside the
				// popup.
				Application.RemoveMessageFilter(filter);

				// If we did something from the popup which shifted
				// focus to a new form, like showing another popup
				// or dialog, then Windows won't know how to bring
				// the original owner back to the foreground, so
				// force it here:
				this.owner.Activate();
				
				// Null out references for GC
				this.popup = null;
				this.owner = null;
								
			}
		}

		/// <summary>
		/// Raises the <see cref="PopupClosed"/> event.
		/// </summary>
		/// <param name="e"><see cref="PopupClosedEventArgs"/> describing the
		/// popup form that is being closed.</param>
		protected virtual void OnPopupClosed(PopupClosedEventArgs e)
		{
			if (this.PopupClosed != null)
			{
				this.PopupClosed(this, e);
			}
		}

		private void popup_Cancel(object sender, PopupCancelEventArgs e)
		{
			OnPopupCancel(e);
		}

		/// <summary>
		/// Raises the <see cref="PopupCancel"/> event.
		/// </summary>
		/// <param name="e"><see cref="PopupCancelEventArgs"/> describing the
		/// popup form that about to be cancelled.</param>
		protected virtual void OnPopupCancel(PopupCancelEventArgs e)
		{
			if (this.PopupCancel != null)
			{
				this.PopupCancel(this, e);
				if (!e.Cancel)
				{
					skipClose = true;
				}
			}
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks>Use the <see cref="System.Windows.Forms.NativeWindow.AssignHandle"/>
		/// method to attach this class to the form you want to show popups from.</remarks>
		public PopupWindowHelper()
		{
			filter = new PopupWindowHelperMessageFilter(this);
			filter.PopupCancel += new PopupCancelEventHandler(popup_Cancel);
		}


	}
	#endregion

	#region PopupWindowHelperMessageFilter
	/// <summary>
	/// A Message Loop filter which detect mouse events whilst the popup form is shown
	/// and notifies the owning <see cref="PopupWindowHelper"/> class when a mouse
	/// click outside the popup occurs.
	/// </summary>
	public class PopupWindowHelperMessageFilter : IMessageFilter
	{
		private const int WM_LBUTTONDOWN = 0x201;
		private const int WM_RBUTTONDOWN = 0x204;
		private const int WM_MBUTTONDOWN = 0x207;
		private const int WM_NCLBUTTONDOWN = 0x0A1;
		private const int WM_NCRBUTTONDOWN = 0x0A4;
		private const int WM_NCMBUTTONDOWN = 0x0A7;

		/// <summary>
		/// Raised when the Popup Window is about to be cancelled.  The
		/// <see cref="PopupCancelEventArgs.Cancel"/> property can be
		/// set to <c>true</c> to prevent the form from being cancelled.
		/// </summary>
		public event PopupCancelEventHandler PopupCancel;
		
		/// <summary>
		/// The popup form
		/// </summary>
		private Form popup = null;
		/// <summary>
		/// The owning <see cref="PopupWindowHelper"/> object.
		/// </summary>
		private PopupWindowHelper owner = null;

		/// <summary>
		/// Constructs a new instance of this class and sets the owning
		/// object.
		/// </summary>
		/// <param name="owner">The <see cref="PopupWindowHelper"/> object
		/// which owns this class.</param>
		public PopupWindowHelperMessageFilter(PopupWindowHelper owner)
		{
			this.owner = owner;
		}

		/// <summary>
		/// Gets/sets the popup form which is being displayed.
		/// </summary>
		public Form Popup
		{
			get
			{
				return this.popup;
			}
			set
			{
				this.popup = value;
			}
		}

		/// <summary>
		/// Checks the message loop for mouse messages whilst the popup
		/// window is displayed.  If one is detected the position is
		/// checked to see if it is outside the form, and the owner
		/// is notified if so.
		/// </summary>
		/// <param name="m">Windows Message about to be processed by the
		/// message loop</param>
		/// <returns><c>true</c> to filter the message, <c>false</c> otherwise.
		/// This implementation always returns <c>false</c>.</returns>
		public bool PreFilterMessage(ref Message m)
		{
			if (this.popup != null)
			{
				switch (m.Msg)
				{				
					case WM_LBUTTONDOWN:
					case WM_RBUTTONDOWN:
					case WM_MBUTTONDOWN:
					case WM_NCLBUTTONDOWN:
					case WM_NCRBUTTONDOWN:
					case WM_NCMBUTTONDOWN:
						OnMouseDown();
						break;
				}
			}
			return false;
		}

		/// <summary>
		/// Checks the mouse location and calls the OnCancelPopup method
		/// if the mouse is outside the popup form.		
		/// </summary>
		private void OnMouseDown()
		{
			// Get the cursor location
			Point cursorPos = Cursor.Position;
			// Check if it is within the popup form
			if (!popup.Bounds.Contains(cursorPos))
			{
				// If not, then call to see if it should be closed
				OnCancelPopup(new PopupCancelEventArgs(popup, cursorPos));
			}
		}

		/// <summary>
		/// Raises the <see cref="PopupCancel"/> event.
		/// </summary>
		/// <param name="e">The <see cref="PopupCancelEventArgs"/> associated 
		/// with the cancel event.</param>
		protected virtual void OnCancelPopup(PopupCancelEventArgs e)
		{
			if (this.PopupCancel != null)
			{
				this.PopupCancel(this, e);
			}
			if (!e.Cancel)
			{
				owner.ClosePopup();
				// Clear reference for GC
				popup = null;
			}
		}


	}
	#endregion

}

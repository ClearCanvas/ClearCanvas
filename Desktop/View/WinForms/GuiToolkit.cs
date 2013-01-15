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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// WinForms implementation of <see cref="IGuiToolkit"/>. 
    /// </summary>
    [ExtensionOf(typeof(GuiToolkitExtensionPoint))]
    public class GuiToolkit : IGuiToolkit
    {
        private event EventHandler _started;
    	private Thread _guiThread;

        /// <summary>
        /// No-args constructor required by extension point framework.
        /// </summary>
        public GuiToolkit()
        {
            if (!Platform.IsWin32Platform)
                throw new NotSupportedException();
        }

        #region IGuiToolkit Members

        /// <summary>
        /// Gets the toolkit ID.
        /// </summary>
        /// <value>Always returns <see cref="ClearCanvas.Common.GuiToolkitID.WinForms"/>.</value>
        public string ToolkitID
        {
            get { return GuiToolkitID.WinForms; }
        }

        /// <summary>
        /// Occurs after the message loop has started.
        /// </summary>
        public event EventHandler Started
        {
            add { _started += value; }
            remove { _started -= value; }
        }

        /// <summary>
        /// Initializes WinForms and starts the message loop.  Blocks until <see cref="Terminate"/> is called.
        /// </summary>
        public void Run()
        {
        	_guiThread = Thread.CurrentThread;

        	Application.CurrentUICultureChanged += Application_CurrentUICultureChanged;

            // this must be called before any GUI objects are created - otherwise we get problems with icons not showing up
            System.Windows.Forms.Application.EnableVisualStyles();

            // create a timer to raise the Started event from the message pump
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 100;
            timer.Tick += delegate(object sender, EventArgs args)
            {
                // immediately disable the timer so we don't get a second Tick
                timer.Dispose();
                EventsHelper.Fire(_started, this, EventArgs.Empty);
            };
            timer.Enabled = true;

            // start the message pump
            System.Windows.Forms.Application.Run();
        }

        /// <summary>
        /// Terminates the message loop.
        /// </summary>
        public void Terminate()
        {
            System.Windows.Forms.Application.Exit();

			Application.CurrentUICultureChanged -= Application_CurrentUICultureChanged;
        }

    	private void Application_CurrentUICultureChanged(object sender, EventArgs e)
    	{
			// update the culture on the main GUI thread
    		_guiThread.CurrentUICulture = Application.CurrentUICulture;
    	}

        #endregion
    }
}

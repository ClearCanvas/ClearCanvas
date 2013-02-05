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

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    /// <summary>
    /// A generic modal popup dialog box that is automatically confirmed after a configurable timeout.
    /// </summary>
    /// <remarks>
    /// The <see cref="TimedDialog"/> control is derived from <see cref="ModalDialog"/>.  As such it provides basic 
    /// Show and Hide methods.  The content of the dialog box can be specified at design time or at run time
    /// through the <see cref="Message"/> property. The title bar can also be customized at run-time or design
    /// time by changing the <see cref="Title"/> property. 
    /// The default appearance of the title bar has a <see cref="Title"/> and an "X" button for closing.
    /// <para>
    /// The TimedDialog will automatically be dismissed after a configurable <see cref="Timeout"/> (in milliseconds).
    /// An optional Ok button can also be added to the dialog and is configured by <see cref="ShowOkButton"/>.
    /// When the dialog has timed out or the ok button has been pressed, the <see cref="Confirmed"/> event is
    /// called.
    /// </para> 
    /// 
    /// <example>
    /// The following example illustrate how to define a dialogbox without an Ok button.
    /// 
    /// aspx code:
    /// 
    /// <%@ Register Src="TimedDialog.ascx" TagName="TimedDialog" TagPrefix="clearcanvas" %>
    /// <clearcanvas:TimedDialog ID="TimedDialog" runat="server" Timeout="5000" ShowOkButton="false" />
    /// 
    /// C#:
    /// 
    ///  protected void Page_Load(object sender, EventArgs e)
    ///  {
    ///       TimedDialog.Message =
    ///                string.Format("The following studies have been placed in the WorkQueue to transfer:<BR/>");
    ///       TimedDialog.Show();
    ///  }
    /// 
    ///  protected void Button_Click(object sender, EventArgs e)
    ///  {
    ///        // do something...
    ///        TimedDialog.Close();
    ///  }
    /// </example>
    /// </remarks>
    public partial class TimedDialog : System.Web.UI.UserControl
    {
        #region Private Members
        private string _message;
        private string _title;
        private int _timeout;
        private bool _showOkButton = true;
        #endregion Private Members

        #region Public Properties

        /// <summary>
        /// Sets/Gets the associated data with the action.
        /// </summary>
        public object Data
        {
            set
            {
                ViewState[ "_Data"] = value;
            }
            get
            {
                return ViewState["_Data"];
            }
        }

        /// <summary>
        /// Sets/Gets the a background css 
        /// </summary>
        public string BackgroundCSS
        {
            set { ModalDialog.BackgroundCSS = value; }
            get { return ModalDialog.BackgroundCSS; }
        }


        /// <summary>
        /// Sets/Gets the messsage being displayed
        /// </summary>
        public string Message
        {
            set
            {
                _message = value;
                MessageLabel.Text = _message;
            }
            get
            {
                return _message;
            }
        }

        /// <summary>
        /// Sets/Gets the title of the dialog box.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                ModalDialog.Title = value;
            }
        }

        public int Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        public bool ShowOkButton
        {
            get { return _showOkButton; }
            set { _showOkButton = value; }
        }

        #endregion Public Properties

        #region Events


        /// <summary>
        /// Defines the event handler for <seealso cref="Confirmed"/> event.
        /// </summary>
        /// <param name="data"></param>
        public delegate void ConfirmedEventHandler(object data);

        /// <summary>
        /// Occurs when users click on "OK"
        /// </summary>
        public event ConfirmedEventHandler Confirmed;


        #endregion Events

        #region Protected Methods

        protected override void OnPreRender(EventArgs e)
        {
            OKButton.Visible = ShowOkButton;
          
            base.OnPreRender(e);
        }   

        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Confirmed != null)
                Confirmed(Data);

            Close();
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Dismisses the confirmation box.
        /// </summary>
        public void Close()
        {
            ModalDialog.Hide();
            TimerTimeout.Enabled = false;
        }

        /// <summary>
        /// Displays the confirmation message box
        /// </summary>
        public void Show()
        {
            ModalDialog.Show();
            TimerTimeout.Interval = Timeout;
            TimerTimeout.Enabled = true;            
        }

        #endregion Public Methods

        protected void TimerTimout_Tick(object sender, EventArgs e)
        {
            if (ModalDialog.State == ModalDialog.ShowState.Show)
                OKButton_Click(sender, e);
        }
    }
}
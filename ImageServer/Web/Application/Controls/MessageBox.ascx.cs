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
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    /// <summary>
    /// A dialog box that displays a message on the screen and waits for users to accept or reject.
    /// </summary>
    /// <remark>
    /// <para>
    /// The confirmation dialog box presents users with different buttons depending on the <see cref="MessageType"/>. For eg,
    /// if  <see cref="MessageType"/> is set to <see cref="MessageTypeEnum.YESNO"/>, "Yes" and "No" buttons will be displayed
    /// below the <see cref="Message"/>. If <see cref="MessageType"/> is set to <see cref="MessageTypeEnum.INFORMATION"/>, only
    /// "OK" button will be displayed.
    /// </para>
    /// <para>
    /// Unless explicitly set by the applications, the <see cref="Title"/> of the dialog box will be set based on the <see cref="MessageType"/>
    /// </para>
    /// <para>
    /// Applications should implement an event handler for the <see cref="Confirmed"/> event which is fired 
    /// when users press "Yes" (or equivalent buttons). The dialog box will close automatically when users press on the buttons.
    /// </para>
    /// </remark>
    public partial class MessageBox : System.Web.UI.UserControl
    {
        /// <summary>
        /// Types of confirmation message.
        /// </summary>
        public enum MessageTypeEnum
        {
            YESNO,  // displays "Yes" and "No" buttons
            OKCANCEL, // displays "OK" and "Cancel" buttons
            INFORMATION, // displays "OK" button.
            ERROR,      // displays "OK" button
            NONE            // do not display any button
        };

        #region Private Members

        private string _messageStyle;

    	#endregion Private Members

        #region Public Properties

        /// <summary>
        /// Sets/Gets the associated data with the action.
        /// </summary>
        public object Data
        {
            set
            {
                ViewState["Data"] = value;
            }
            get
            {
                return ViewState["Data"];
            }
        }

        /// <summary>
        /// Sets/Gets the type of message being displayed.
        /// </summary>
        public MessageTypeEnum MessageType
        {
            set { 
                ViewState["MsgType"] = value;
            }
            get
            {
                if (ViewState["MsgType"] == null)
                    return MessageTypeEnum.NONE;
                else
                    return (MessageTypeEnum) ViewState["MsgType"];
            }
        }

        /// <summary>
        /// Sets/Gets the a background css 
        /// </summary>
        public string BackgroundCSS
        {
            set {  ModalDialog.BackgroundCSS = value; }
            get { return ModalDialog.BackgroundCSS; }
        }


        /// <summary>
        /// Sets/Gets the messsage being displayed
        /// </summary>
        public string Message
        {
            set
            {
                ViewState["Message"] = value;
            }
            get
            {
                if (ViewState["Message"] == null)
                    return String.Empty;
                else
                    return (string)ViewState["Message"];
            }
        }

        public string WarningMessage{get; set;}

        public string MessageStyle
        {
            set { _messageStyle = value; }
            get { return _messageStyle; }
        }

        /// <summary>
        /// Sets/Gets the title of the dialog box.
        /// </summary>
        public string Title
        {
            set
            {
                ViewState["Title"] = value;
            }
            get
            {
                if (ViewState["Title"] == null)
                    return String.Empty;
                else
                    return (string)ViewState["Title"];
            }
        }

        #endregion Public Properties

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="Confirmed"/> event.
        /// </summary>
        /// <param name="data"></param>
        public delegate void ConfirmedEventHandler(object data);

        /// <summary>
        /// Occurs when users click on "Yes" or "OK"
        /// </summary>
        public event ConfirmedEventHandler Confirmed;

        /// <summary>
        /// Defines the event handler for <seealso cref="Cancel"/> event.
        /// </summary>
        public delegate void CancelEventHandler();

        /// <summary>
        /// Occurs when users click on "No" or "Cancel"
        /// </summary>
        public event CancelEventHandler Cancel;

        public delegate void OnShowEventHandler();
        public event OnShowEventHandler OnShow;

        public delegate void OnHideEventHandler();
        public event OnHideEventHandler OnHide;

        #endregion Events

        #region Protected Methods

        protected override void OnPreRender(EventArgs e)
        {
            YesButton.Visible = false;
            NoButton.Visible = false;
            OKButton.Visible = false;
            CancelButton.Visible = false;

            
            switch (MessageType)
            {
                case MessageTypeEnum.ERROR:
                    OKButton.Visible = true;
                    if (String.IsNullOrEmpty(Title))
                        Title = SR.ConfirmDialogError;
                    break;

                case MessageTypeEnum.INFORMATION:
                    OKButton.Visible = true;
                    break;

                case MessageTypeEnum.OKCANCEL:
                    OKButton.Visible = true;
                    CancelButton.Visible = true;
                    if (String.IsNullOrEmpty(Title))
                        Title = SR.ConfirmDialogDefault;
                    break;

                case MessageTypeEnum.YESNO:
                    YesButton.Visible = true;
                    NoButton.Visible = true;
                    if (String.IsNullOrEmpty(Title))
                        Title = SR.ConfirmDialogDefault;
                    break;

                default:
                    break;
            }

            MessageLabel.Text = Message;
            MessageLabel.Attributes["style"] = MessageStyle;
            ModalDialog.Title = Title;

            if (!string.IsNullOrEmpty(WarningMessage))
            {
                WarningMessageLabel.Text = WarningMessage;
                WarningMessageLabel.Visible = true;
            }
            else
                WarningMessageLabel.Visible = false;

            base.OnPreRender(e);
        }

        protected void YesButton_Click(object sender, EventArgs e)
        {
            OKButton_Click(sender, e);
        }


        protected void NoButton_Click(object sender, EventArgs e)
        {
            CancelButton_Click(sender, e);
        }

        protected void OKButton_Click(object sender, EventArgs e)
        {
            if (Confirmed != null)
                Confirmed(Data);

            Close();
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            if (Cancel != null)
                Cancel();
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
            if (OnHide != null) OnHide();

        }

        /// <summary>
        /// Displays the confirmation message box
        /// </summary>
        public void Show()
        {
            ModalDialog.Show();
            if (OnShow != null) OnShow();
        }

        #endregion Public Methods

    }
}
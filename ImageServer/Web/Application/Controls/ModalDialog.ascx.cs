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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    /// <summary>
    /// A generic modal popup dialog box.
    /// </summary>
    /// <remarks>
    /// A <see cref="ModalDialog"/> control provide all basic functionalities of a modal dialog box such as <see cref="Show"/> and <see cref="Hide"/>.
    /// The content of the dialog box can be specified at design time using the <see cref="ContentTemplate"/>. The title bar can also be customized by 
    /// changing <see cref="TitleBarTemplate"/>. The default appearance of the title bar has a <see cref="Title"/> and an "X" button for closing.
    /// <para>
    /// Note <see cref="ModalDialog"/> doesn't fire any events. Customized dialog box control should implement event handlers according to its requirement.
    /// </para> 
    /// 
    /// <example>
    /// The following example illustrate how to define a dialogbox with a "OK" button.
    /// 
    /// aspx code:
    /// 
    /// <%@ Register Src="ModalDialog.ascx" TagName="ModalDialog" TagPrefix="clearcanvas" %>
    /// 
    /// <clearcanvas:ModalDialog ID="ModalDialog1" runat="server" Title="Please Press the button">
    /// <ContentTemplate>
    ///      <asp:Button ID="YesButton" runat="server" OnClick="Button_Click" Text="Click Me" />
    /// </ContentTemplate>
    /// </clearcanvas:ModalDialog>
    /// 
    /// 
    /// C#:
    /// 
    ///  protected void Page_Load(object sender, EventArgs e)
    ///  {
    ///     ModalDialog1.Show();
    ///  }
    /// 
    ///  protected void Button_Click(object sender, EventArgs e)
    ///  {
    ///        // do something...
    ///        ModalDialog1.Close();
    ///  }
    /// 
    /// 
    /// 
    /// </example>
    ///
    /// Note that the ModalDialog control is declared globally in the Web.Config file, and is not
    /// required to be declared locally in an ASPX page. See Web.Config for information about the proper
    /// declaration of the tagPrefix.
    /// 
    /// </remarks>
    public partial class ModalDialog : UserControl
    {
        /// <summary>
        /// State enumeration
        /// </summary>
        public enum ShowState
        {
            Hide,
            Show
        }

        /// <summary>
        /// Template container classes
        /// </summary>
        [ParseChildren(true)]
        public class DialogTitleBarContainer : Panel, INamingContainer { }

        [ParseChildren(true)]
        public class DialogContentContainer : Panel, INamingContainer { }

        #region Private members
        private Dictionary<string, Control> _ctrlIDCache = new Dictionary<string, Control>();
        private Unit _width;
        private string _title;
        private string _titleBarCSS;
        private string _dialogContentCSS;
        private string _backgroundCSS = "DefaultModalDialogBackground";
        private bool _dropShadow = false;
        private bool _showCloseBox = false;
        private ITemplate _titleBarTemplate = null;
        private ITemplate _contentTemplate = null;
        private readonly DialogContentContainer _contentPanelContainer = new DialogContentContainer();
        private readonly DialogTitleBarContainer _titleBarPanelContainer = new DialogTitleBarContainer();
        #endregion Private members

        #region Events

        /// <summary>
        /// Defines the event handler for <seealso cref="OnShow"/> event.
        /// </summary>
        public delegate void ShowEventHandler();

        /// <summary>
        /// Occurs when modal dialog is made visible
        /// </summary>
        public event ShowEventHandler OnShow;

        /// <summary>
        /// Defines the event handler for <seealso cref="OnHide"/> event.
        /// </summary>
        public delegate void HideEventHandler();

        /// <summary>
        /// Occurs when modal dialog is hidden
        /// </summary>
        public event HideEventHandler OnHide;

        #endregion Events

        #region Public Properties
        /// <summary>
        /// The customized titlebar template
        /// </summary>
        [TemplateContainer(typeof(DialogTitleBarContainer))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate TitleBarTemplate
        {
            get
            {
                return _titleBarTemplate;
            }
            set
            {
                _titleBarTemplate = value;
            }
        }


        /// <summary>
        /// The content template
        /// </summary>
        [TemplateContainer(typeof(DialogContentContainer))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate ContentTemplate
        {
            get
            {
                return _contentTemplate;
            }
            set
            {
                _contentTemplate = value;
            }
        }


        /// <summary>
        /// Sets/Gets the title of the dialog box
        /// </summary>
        public string Title
        {
            get {
                return _title;
            }
            set
            {
                _title = value;
                if (TitleLabel!=null)
                    TitleLabel.Text = String.IsNullOrEmpty(value) ? " " : value;
                
                
            }
        }

        /// <summary>
        /// Sets/Gets the size of the dialog box.
        /// </summary>
        public Unit Width
        {
            get { return _width; }
            set
            {
                _width = value;
            }
        }

        /// <summary>
        /// Sets/Gets the CSS for the title bar
        /// </summary>
        public string TitleBarCSS
        {
            get { return _titleBarCSS; }
            set
            {
                _titleBarCSS = value;

            }
        }

        /// <summary>
        /// Gets/Sets the CSS for the dialog box content
        /// </summary>
        public string ContentCSS
        {
            get { return _dialogContentCSS; }
            set
            {
                _dialogContentCSS = value;

            }
        }

        /// <summary>
        /// Gets/Sets a value to indicate whether or not to drop shadow
        /// </summary>
        public bool DropShadow
        {
            get { return _dropShadow; }
            set
            {
                _dropShadow = value;
            }
        }

        /// <summary>
        /// Sets/Gets the CSS for the background area
        /// </summary>
        public string BackgroundCSS
        {
            get { return _backgroundCSS; }
            set
            {
                _backgroundCSS = value;

            }
        }

        /// <summary>
        ///  Gets/Sets the current state of the dialog box.
        /// </summary>
        /// <remarks>
        /// The dialog box will popup automatically when the value of <see cref="State"/> is <see cref="ShowState.Show"/>
        /// </remarks>
        public ShowState State
        {
            get
            {
                if (ViewState["_State"] != null)
                    return (ShowState)ViewState["_State"];
                else
                    return ShowState.Hide;
            }
            set { ViewState["_State"] = value; }
           
        }

        /// <summary>
        /// Gets the container control that contains the content of the dialog box
        /// </summary>
        /// <remarks>
        /// The content control contains everything defined in <see cref="ContentTemplate"/>
        /// </remarks>
        public DialogContentContainer ContentPanelContainer
        {
            get { return _contentPanelContainer; }
        }

        /// <summary>
        /// Gets the container control that contains the content of the title bar
        /// </summary>
        /// <remarks>
        /// The title bar container contains everything defined in <see cref="TitleBarTemplate"/>
        /// </remarks>
        public DialogTitleBarContainer TitleBarPanelContainer
        {
            get { return _titleBarPanelContainer; }
        }

        public bool ShowCloseBox
        {
            get { return _showCloseBox; }
            set { _showCloseBox = value; }
        }

        public string PopupExtenderID
        {
            get { return ModalPopupExtender.ClientID; }
        }

        #endregion Public Properties

        #region Protected Methods
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DialogContainer.Width = Width;
            if (!String.IsNullOrEmpty(_title))
                TitleLabel.Text = Title;

            DialogContainer.Width = Width;

            if (DialogContainer.Width!=Unit.Empty)
            {
                // this will make sure the dialog box 
                //DialogSizeTable.Width = Unit.Percentage(100.0);
            }

            bool useCustomizeTitleBar = _titleBarTemplate != null;

            if (useCustomizeTitleBar)
            {
                CustomizedTitleBarPanel.Visible = true;
                DefaultTitlePanel.Visible = false;
                _titleBarTemplate.InstantiateIn(TitleBarPanelContainer);
                TitlePanelPlaceHolder.Controls.Add(TitleBarPanelContainer);

                if (!String.IsNullOrEmpty(TitleBarCSS))
                    CustomizedTitleBarPanel.CssClass = TitleBarCSS;

            }
            else
            {
                DefaultTitlePanel.Visible = true;
                CustomizedTitleBarPanel.Visible = false;

                if (!String.IsNullOrEmpty(TitleBarCSS))
                    DefaultTitlePanel.CssClass = TitleBarCSS;
            }

            if (_contentTemplate != null)
            {
                _contentTemplate.InstantiateIn(ContentPanelContainer);
                ContentPlaceHolder.Controls.Add(ContentPanelContainer);

                if (!String.IsNullOrEmpty(ContentCSS))
                    ContentPanel.CssClass = ContentCSS;
            }

            ModalPopupExtender.BackgroundCssClass = BackgroundCSS;
            ModalPopupExtender.DropShadow = DropShadow;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (State == ShowState.Show)
                Show();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (String.IsNullOrEmpty(Title))
                Title = "&nbsp;";
        }

        protected Control FindControlHelper(string id)
        {
            Control ctrl = null;
            if (_ctrlIDCache.ContainsKey(id))
            {
                ctrl = _ctrlIDCache[id];
            }
            else
            {
                ctrl = base.FindControl(id);
                Control nc = NamingContainer;
                while ((ctrl == null) && (nc != null))
                {
                    ctrl = nc.FindControl(id);
                    nc = nc.NamingContainer;
                }

                // search inside the template containers
                if (ctrl == null)
                    ctrl = ContentPanelContainer.FindControl(id);

                if (ctrl == null)
                    ctrl = TitleBarPanelContainer.FindControl(id);


                if (null != ctrl)
                {
                    _ctrlIDCache[id] = ctrl;
                }
            }
            return ctrl;
        }

        protected void CloseButton_Click(object sender, ImageClickEventArgs e)
        {
            Hide();
        }

        #endregion Protected Methods

        #region Public Methods
        public override Control FindControl(string id)
        {
            return FindControlHelper(id);
        }

        /// <summary>
        /// Displays the dialog on the screen and wait for response from the users
        /// </summary>
        public void Show()
        {
            if (OnShow != null) OnShow();
            
            ModalPopupExtender.Show();
            
            // need to refresh to ensure that dialog is visible.
            RefreshUI();

            State = ShowState.Show;
        }

        public void RefreshUI()
        {
            ModalDialogUpdatePanel.Update();
            
        }

        /// <summary>
        /// Closes the dialog box
        /// </summary>
        public void Hide()
        {
            if (OnHide != null) OnHide();
            
            ModalPopupExtender.Hide();
            RefreshUI();
            State = ShowState.Hide;
        }

        #endregion Public Methods
    }
}
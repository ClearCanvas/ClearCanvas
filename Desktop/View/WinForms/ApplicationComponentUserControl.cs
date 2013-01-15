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
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Base class for user controls that are created by an Application Component View.
    /// </summary>
    public partial class ApplicationComponentUserControl : CustomUserControl
    {
        // N.B. do not make this class abstract, no matter how tempting it may look. You will break the VS Forms designer.

        /// <summary>
        /// Constructor required for Designer support.  Do not use this constructor in application code.
        /// </summary>
        public ApplicationComponentUserControl()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="component"></param>
        public ApplicationComponentUserControl(IApplicationComponent component)
        {
            InitializeComponent();

            _errorProvider.DataSource = component;
            component.ValidationVisibleChanged += ValidationVisibleChangedEventHandler;

            if (component is ApplicationComponent)
            {
                ActionModelNode menuModel = ((ApplicationComponent)component).MetaContextMenuModel;
                if (menuModel != null)
                {
                    ToolStripBuilder.BuildMenu(_contextMenu.Items, menuModel.ChildNodes);
                }
            }
        }

        /// <summary>
        /// Gets the default <see cref="System.Windows.Forms.ErrorProvider"/> for this user control
        /// </summary>
        public ErrorProvider ErrorProvider
        {
            get { return _errorProvider; }
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			// for some reason, the error provider doesn't automatically query the component
			// for validation errors on the initial page load, so we need to force this
			// (note: it can be null when running in the designer, so need to check this)
			if(_errorProvider != null)
			{
				_errorProvider.UpdateBinding();
			}
		}

        private void ValidationVisibleChangedEventHandler(object sender, EventArgs e)
        {
            _errorProvider.UpdateBinding();
        }
    }
}

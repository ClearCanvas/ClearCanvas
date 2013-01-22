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

#region Additional permission to link with DotNetMagic

// Additional permission under GNU GPL version 3 section 7
// 
// If you modify this Program, or any covered work, by linking or combining it
// with DotNetMagic (or a modified version of that library), containing parts
// covered by the terms of the Crownwood Software DotNetMagic license, the
// licensors of this Program grant you additional permission to convey the
// resulting work.

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class TabComponentContainerControl : UserControl
	{
		TabComponentContainer _component;

		public TabComponentContainerControl(TabComponentContainer component)
		{
			Platform.CheckForNullReference(component, "component");
			
			InitializeComponent();
			_component = component;

			_tabControl.ControlLeftOffset = 3;
			_tabControl.ControlTopOffset = 3;
			_tabControl.ControlRightOffset = 3;
			_tabControl.ControlBottomOffset = 3;

			foreach (TabPage page in _component.Pages)
			{
				Crownwood.DotNetMagic.Controls.TabPage tabPageUI = new Crownwood.DotNetMagic.Controls.TabPage(page.Name);
				tabPageUI.Tag = page;

				_tabControl.TabPages.Add(tabPageUI);
			}

			_tabControl.SelectionChanged += new Crownwood.DotNetMagic.Controls.SelectTabHandler(OnControlSelectionChanged);
			_component.CurrentPageChanged += new EventHandler(OnComponentCurrentPageChanged);

            ShowPage(_component.CurrentPage);
		}

		private void OnComponentCurrentPageChanged(object sender, EventArgs e)
		{
            ShowPage(_component.CurrentPage);
		}

        private void OnControlSelectionChanged(Crownwood.DotNetMagic.Controls.TabControl sender, Crownwood.DotNetMagic.Controls.TabPage oldPage, Crownwood.DotNetMagic.Controls.TabPage newPage)
		{
			TabPage tabPage = newPage.Tag as TabPage;
			_component.CurrentPage = tabPage;
		}

        private void ShowPage(TabPage page)
        {
            // find the tab corresponding to the current page
            Crownwood.DotNetMagic.Controls.TabPage tab = CollectionUtils.SelectFirst<Crownwood.DotNetMagic.Controls.TabPage>(_tabControl.TabPages,
                delegate(Crownwood.DotNetMagic.Controls.TabPage tp) { return tp.Tag == page; });

            // if the tab's control was not yet created, create it now
            if (tab.Control == null)
            {
                tab.Control = (Control)_component.GetPageView(page).GuiElement;
            }

            // ensure the correct tab is selected (in case the current page was changed programatically)
            tab.Selected = true;
        }

    }
}

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

using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class TabGroupComponentContainerControl : UserControl
    {
        TabGroupComponentContainer _component;

        public TabGroupComponentContainerControl(TabGroupComponentContainer component)
        {
            InitializeComponent();
            _component = component;

            CreateTabGroups();
        }

        private void CreateTabGroups()
        {
            _tabbedGroupsControl.PageChanged += new Crownwood.DotNetMagic.Controls.TabbedGroups.PageChangeHandler(OnControlPageChanged);

            _tabbedGroupsControl.RootDirection = _component.LayoutDirection == LayoutDirection.Vertical ?
                Crownwood.DotNetMagic.Common.LayoutDirection.Vertical :
                Crownwood.DotNetMagic.Common.LayoutDirection.Horizontal;

            foreach (TabGroup tabGroup in _component.TabGroups)
            {
                Crownwood.DotNetMagic.Controls.TabGroupLeaf tgl = _tabbedGroupsControl.RootSequence.AddNewLeaf() as Crownwood.DotNetMagic.Controls.TabGroupLeaf;

                foreach (TabPage page in tabGroup.Component.Pages)
                {
                    Crownwood.DotNetMagic.Controls.TabPage tabPageUI = new Crownwood.DotNetMagic.Controls.TabPage(page.Name);
                    tabPageUI.Tag = page;
                    tgl.TabPages.Add(tabPageUI);
                }
            }

            // The weight can only be set after each leaf is created
            // Ask control to reposition children according to new spacing
            for (int i = 0; i < _component.TabGroups.Count; i++)
            {
                Crownwood.DotNetMagic.Controls.TabGroupLeaf tgl = _tabbedGroupsControl.RootSequence[i] as Crownwood.DotNetMagic.Controls.TabGroupLeaf;
                tgl.Space = (decimal)(_component.TabGroups[i].Weight * 100);
            }

            _tabbedGroupsControl.RootSequence.Reposition();
        }

        private void OnControlPageChanged(Crownwood.DotNetMagic.Controls.TabbedGroups tg, Crownwood.DotNetMagic.Controls.TabPage selectedPage)
        {
            if (selectedPage != null)
            {
                TabPage tabPage = selectedPage.Tag as TabPage;

                if (tabPage.Component.IsStarted == false)
                    tabPage.Component.Start();

                if (selectedPage.Control == null)
                {
                    TabGroup tabGroup = _component.GetTabGroup(tabPage);
                    selectedPage.Control = (Control)tabGroup.Component.GetPageView(tabPage).GuiElement;
                }
            }
        }

        private void OnTabControlCreated(Crownwood.DotNetMagic.Controls.TabbedGroups tg, Crownwood.DotNetMagic.Controls.TabControl tc)
        {
            // Place a thin border between edge of the tab control and inside contents
            tc.ControlTopOffset = 3;
            tc.ControlBottomOffset = 3;
            tc.ControlLeftOffset = 3;
            tc.ControlRightOffset = 3;
            tc.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
        }
    }
}

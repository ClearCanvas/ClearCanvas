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
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
    /// Provides a Windows Forms user-interface for <see cref="StackTabComponentContainer"/>
    /// </summary>
	public partial class StackTabComponentContainerControl : CustomUserControl
	{
		private VisualStyle _activeStyle = VisualStyle.Office2007Blue;
		private VisualStyle _inactiveStyle = VisualStyle.Office2007Black;

		private readonly StackTabComponentContainer _component;

		/// <summary>
		/// Constructor for Designer
		/// </summary>
		public StackTabComponentContainerControl()
		{
			InitializeComponent();
		}

		public StackTabComponentContainerControl(StackTabComponentContainer component)
		{
			InitializeComponent();
			_component = component;

			_stackTabControl.RootDirection = Crownwood.DotNetMagic.Common.LayoutDirection.Vertical;
			// Set the sizing spaces between leaves
			_stackTabControl.ResizeBarVector = _component.StackStyle == StackStyle.ShowMultiple ? 1 : 0;
			_stackTabControl.PageChanged += OnControlPageChanged;

			CreateStackTabs();

			_component.Pages.ItemAdded += OnComponentPageAdded;
			_component.Pages.ItemRemoved += OnComponentPageRemoved;
			_component.CurrentPageChanged += OnComponentCurrentPageChanged;

		}

		#region Design-time Properties

		[DefaultValue(VisualStyle.Office2007Blue)]
    	public VisualStyle ActiveStyle
    	{
			get { return _activeStyle; }
			set { _activeStyle = value; }
    	}

		[DefaultValue(VisualStyle.Office2007Black)]
		public VisualStyle InactiveStyle
		{
			get { return _inactiveStyle; }
			set { _inactiveStyle = value; }
		}

		#endregion

		#region Event Handlers

		private void OnTabButtonClick(object sender, EventArgs e)
		{
			Crownwood.DotNetMagic.Controls.TitleBar titleBar = (Crownwood.DotNetMagic.Controls.TitleBar)sender;
			TabGroupLeaf tgl = GetLeafForTitleBar(titleBar);
			ToggleLeaf(tgl);
		}

		private void OnTabTitleClicked(object sender, EventArgs e)
		{
			Crownwood.DotNetMagic.Controls.TitleBar titleBar = (Crownwood.DotNetMagic.Controls.TitleBar) sender;
			TabGroupLeaf selectedLeaf = GetLeafForTitleBar(titleBar);

			if (IsLeafExpanded(selectedLeaf))
				ChangeComponentPageFromLeaf(selectedLeaf);
		}

		private void OnTabTitleDoubleClicked(object sender, EventArgs e)
		{
			Crownwood.DotNetMagic.Controls.TitleBar titleBar = (Crownwood.DotNetMagic.Controls.TitleBar) sender;
			TabGroupLeaf tgl = GetLeafForTitleBar(titleBar);
			ToggleLeaf(tgl);
		}

		private void OnControlPageChanged(TabbedGroups tg, Crownwood.DotNetMagic.Controls.TabPage selectedPage)
		{
			if (_component.StackStyle == StackStyle.ShowMultiple)
			{
				if (selectedPage != null)
					ChangeComponentPageFromLeaf(tg.LeafForPage(selectedPage));
			}
		}

		private void OnComponentPageAdded(object sender, ListEventArgs<StackTabPage> e)
		{
			CreateStackTab(e.Item);
		}

		private void OnComponentPageRemoved(object sender, ListEventArgs<StackTabPage> e)
		{
			RemoveStackTab(e.Item);
		}

		private void OnComponentCurrentPageChanged(object sender, EventArgs e)
		{
			if (_component.CurrentPage != null)
			{
				TabGroupLeaf tgl = GetLeafForStackPage(_component.CurrentPage);
				HighlightLeaf(tgl, true);
			}
		}

		#endregion

		#region Private Helpers

		private void CreateStackTabs()
		{
			foreach (StackTabPage page in _component.Pages)
			{
				CreateStackTab(page);
			}
		}

		private void RecalculateTabSizes()
		{
			// The space of each leaf can only be set after each leaf is created
			// Open up only the first leaf and close all others
			for (int i = 0; i < _stackTabControl.RootSequence.Count; i++)
			{
				TabGroupLeaf tgl = (TabGroupLeaf) _stackTabControl.RootSequence[i];

				if (_component.StackStyle == StackStyle.ShowMultiple && _component.OpenAllTabsInitially)
				{
					OpenLeaf(tgl, (decimal)100 / _stackTabControl.RootSequence.Count);
				}
				else
				{
					if (tgl == _stackTabControl.RootSequence[0])
					{
						OpenLeaf(tgl, 100);
						HighlightLeaf(tgl, true);
					}
					else
						CloseLeaf(tgl);
				}
			}

			// Reflect spacing changes immediately
			_stackTabControl.RootSequence.Reposition();
		}

		private void CreateStackTab(StackTabPage page)
		{
			StackTab stackTab = new StackTab(page, DockStyle.Top);

			if (_component.StackStyle == StackStyle.ShowMultiple)
			{
				stackTab.TitleBar.ActAsButton = ActAsButton.JustArrow;
				stackTab.TitleBar.ArrowButton = ArrowButton.DownArrow;
			}
			else
			{
				stackTab.TitleBar.ActAsButton = ActAsButton.WholeControl;
				stackTab.TitleBar.ArrowButton = ArrowButton.None;
			}

			stackTab.ButtonClicked += OnTabButtonClick;
			stackTab.TitleClicked += OnTabTitleClicked;
			stackTab.TitleDoubleClicked += OnTabTitleDoubleClicked;

			TabGroupLeaf tgl = _stackTabControl.RootSequence.AddNewLeaf();
			tgl.MinimumSize = stackTab.MinimumRequestedSize;

			// Prevent user from resizing
			tgl.ResizeBarLock = _component.StackStyle == StackStyle.ShowMultiple ? false : true;

			Crownwood.DotNetMagic.Controls.TabPage tabPageUI = new Crownwood.DotNetMagic.Controls.TabPage(page.Name, stackTab);
			tabPageUI.Tag = page;
			tgl.TabPages.Add(tabPageUI);

			RecalculateTabSizes();
		}

		private void RemoveStackTab(StackTabPage page)
		{
			for (int i = 0; i < _stackTabControl.RootSequence.Count; i++)
			{
				TabGroupLeaf tgl = (TabGroupLeaf)_stackTabControl.RootSequence[i];
				Crownwood.DotNetMagic.Controls.TabPage tabPageUI = CollectionUtils.FirstElement<Crownwood.DotNetMagic.Controls.TabPage>(tgl.TabPages);
				if(tabPageUI.Tag == page)
				{
					// remove the tab page
					// note that this automatically removes the parent leaf, since it is the only tab
					tgl.TabPages.Remove(tabPageUI);
					break;
				}
			}

			RecalculateTabSizes();
		}

		private void OpenLeaf(TabGroupLeaf tgl, decimal space)
		{
			if (tgl == null)
				return;

			// Only open the leaf visually, it does not set a leaf active
			Crownwood.DotNetMagic.Controls.TabPage tabPageUI = tgl.TabPages[0];
			StackTabPage page = (StackTabPage) tabPageUI.Tag;
			StackTab stackTab = (StackTab) tabPageUI.Control;

			if (page.Component.IsStarted == false)
				page.Component.Start();

			if ( stackTab.ApplicationComponentControl == null)
			{
				stackTab.ApplicationComponentControl = (Control)_component.GetPageView(page).GuiElement;
				stackTab.ApplicationComponentControl.Dock = DockStyle.Fill;
			}

			SetArrowState(stackTab.TitleBar, true);

			tgl.Space = space;
		}

		private void CloseLeaf(TabGroupLeaf tgl)
		{
			if (tgl == null)
				return;

			SetArrowState(GetTitleBarForPage(tgl.TabPages[0]), false);
			tgl.Space = 0;
			HighlightLeaf(tgl, false);
		}

		private void ToggleLeaf(TabGroupLeaf tgl)
		{
			ExpandCollapseLeaf(tgl);
			if (IsLeafExpanded(tgl))
			{
				ChangeComponentPageFromLeaf(tgl);
			}
			else
			{
				TabGroupLeaf firstExpandedLeaf = GetFirstExpandedLeaf();

				if (firstExpandedLeaf != null)
					ChangeComponentPageFromLeaf(firstExpandedLeaf);
			}
		}

		private void ExpandCollapseLeaf(TabGroupLeaf selectedLeaf)
		{
			if (selectedLeaf == null)
				return;

			if (_component.StackStyle == StackStyle.ShowMultiple)
			{
				// Remember which title bar sent message
				List<TabGroupLeaf> openedLeaves = new List<TabGroupLeaf>();
				TabGroupLeaf tgl = _stackTabControl.FirstLeaf();
				while (tgl != null)
				{
					// Remember which leaf is opened
					if (IsLeafExpanded(tgl))
						openedLeaves.Add(tgl);

					HighlightLeaf(tgl, false);
					tgl = _stackTabControl.NextLeaf(tgl);
				}

				// Add to openedLeaf because we want to open this
				if (IsLeafExpanded(selectedLeaf))
				{
					openedLeaves.Remove(selectedLeaf);
					CloseLeaf(selectedLeaf);
				}
				else
				{
					openedLeaves.Add(selectedLeaf);
				}

				// Open each leaf with evenly distributed space
				Decimal leafSpace = openedLeaves.Count == 0 ? 100 : (decimal)100 / openedLeaves.Count;
				Decimal spaceRemained = 100;
				foreach (TabGroupLeaf leaf in openedLeaves)
				{
					OpenLeaf(leaf, leafSpace);
					spaceRemained -= leafSpace;
				}

				if (openedLeaves.Count == 0)
				{
					// We need at least one leaf open, so keep the current leaf open if it was the only one opened
					OpenLeaf(selectedLeaf, spaceRemained);
				}
				else
				{
					openedLeaves[0].Space += spaceRemained;
				}
			}
			else
			{
				TabGroupLeaf tgl = _stackTabControl.FirstLeaf();
				while (tgl != null)
				{
					// Is the source of the click?
					if (tgl != selectedLeaf)
						CloseLeaf(tgl);

					HighlightLeaf(tgl, false);
					tgl = _stackTabControl.NextLeaf(tgl);
				}

				OpenLeaf(selectedLeaf, 100);
			}

			// Reflect changes immediately
			_stackTabControl.RootSequence.Reposition();
		}

		private static bool IsLeafExpanded(TabGroupLeaf tgl)
		{
			return tgl.Space > 0;
		}

		private static void SetArrowState(Crownwood.DotNetMagic.Controls.TitleBar titleBar, bool open)
		{
			switch (titleBar.ArrowButton)
			{
				case ArrowButton.UpArrow:
				case ArrowButton.DownArrow:
					titleBar.ArrowButton = open ? ArrowButton.UpArrow : ArrowButton.DownArrow;
					break;
				case ArrowButton.LeftArrow:
				case ArrowButton.RightArrow:
					titleBar.ArrowButton = open ? ArrowButton.LeftArrow : ArrowButton.RightArrow;
					break;
				case ArrowButton.Pinned:
				case ArrowButton.Unpinned:
					titleBar.ArrowButton = open ? ArrowButton.Unpinned : ArrowButton.Pinned;
					break;
				default:
					break;
			}
		}

		private static Crownwood.DotNetMagic.Controls.TitleBar GetTitleBarForPage(Crownwood.DotNetMagic.Controls.TabPage page)
		{
			return ((StackTab) page.Control).TitleBar;
		}

		private TabGroupLeaf GetLeafForTitleBar(Crownwood.DotNetMagic.Controls.TitleBar titleBar)
		{
			TabGroupLeaf tgl = _stackTabControl.FirstLeaf();
			while (tgl != null)
			{
				// Extract the StackTabTitleBar instance from page
				Crownwood.DotNetMagic.Controls.TitleBar tb = GetTitleBarForPage(tgl.TabPages[0]);
				if (tb == titleBar)
					break;

				tgl = _stackTabControl.NextLeaf(tgl);
			}

			return tgl;
		}

		private TabGroupLeaf GetLeafForStackPage(StackTabPage page)
		{
			TabGroupLeaf tgl = _stackTabControl.FirstLeaf();
			while (tgl != null)
			{
				StackTabPage p = (StackTabPage)tgl.TabPages[0].Tag;

				if (p == page)
					break;

				tgl = _stackTabControl.NextLeaf(tgl);
			}

			return tgl;
		}

		private TabGroupLeaf GetFirstExpandedLeaf()
		{
			TabGroupLeaf tgl = _stackTabControl.FirstLeaf();
			while (tgl != null)
			{
				if (IsLeafExpanded(tgl))
					break;

				tgl = _stackTabControl.NextLeaf(tgl);
			}

			return tgl;
		}

		#endregion

		private void ChangeComponentPageFromLeaf(TabGroupLeaf tgl)
		{
			StackTabPage page = (StackTabPage) tgl.TabPages[0].Tag;
			if (_component.CurrentPage != page)
			{
				TabGroupLeaf previousLeaf = GetLeafForStackPage(_component.CurrentPage);
				HighlightLeaf(previousLeaf, false);

				if (_component.CurrentPage != page)
					_component.CurrentPage = page;
			}

			HighlightLeaf(tgl, true);
		}

		private void HighlightLeaf(TabGroupLeaf tgl, bool highlight)
		{
			Crownwood.DotNetMagic.Controls.TabPage tabPageUI = tgl.TabPages[0];
			StackTab stackTab = (StackTab) tabPageUI.Control;
			
			if (highlight)
			{
				stackTab.TitleBar.Style = _activeStyle;
				_stackTabControl.ActiveLeaf = tgl;

				stackTab.Select();
				stackTab.Focus();
			}
			else
			{
				stackTab.TitleBar.Style = _inactiveStyle;
			}
		}
	}
}

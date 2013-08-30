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
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	public partial class SelectOverlaysControl : UserControl
	{
		public SelectOverlaysControl(SelectOverlaysAction action, Action close)
		{
			InitializeComponent();

			SuspendLayout();

			_applyToAll.Enabled = action.Enabled;

			_close.Click += (sender, args) =>
			                	{
			                		action.Apply();
			                		close();
			                	};
			_applyToAll.Click += (sender, args) =>
			                     	{
			                     		action.ApplyEverywhere();
			                     		close();
			                     	};

			foreach (var overlayItem in action.Items)
			{
				var item = overlayItem;
				var check = new IconCheckBox
				            	{
				            		Checked = overlayItem.IsSelected,
				            		CheckEnabled = action.Enabled,
				            		Text = item.ResourceResolver.LocalizeString(item.DisplayName)
				            	};

				if (item.IconSet != null)
				{
					var icon = item.IconSet.CreateIcon(IconSize.Small, action.ResourceResolver);
					check.Image = icon;
				}

				check.CheckedChanged += (sender, args) =>
				                        	{
				                        		item.IsSelected = check.Checked;
				                        		action.Apply();
				                        	};

				_overlaysPanel.Controls.Add(check);
			}

			ResumeLayout();
		}
	}
}
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
using System.Text;

//using ClearCanvas.Workstation.Model;
//using ClearCanvas.ImageViewer.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using Gtk;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
    public class GtkMenuBuilder
    {
        public static void BuildMenu(MenuShell menu, ActionModelNode node)
        {
            if (node.PathSegment != null)
            {
				
                MenuItem menuItem;
                if (node.Action != null)
                {
                    // this is a leaf node (terminal menu item)
                    menuItem = new ActiveMenuItem((IClickAction)node.Action);
                }
                else
                {
                    // this menu item has a sub menu
					string menuText = node.PathSegment.LocalizedText.Replace('&', '_');
					menuItem = new MenuItem(menuText);
                    menuItem.Submenu = new Menu();
                }

                menu.Append(menuItem);
                menu = (MenuShell)menuItem.Submenu;
            }

            foreach (ActionModelNode child in node.ChildNodes)
            {
                BuildMenu(menu, child);
            }
        }
    }
}

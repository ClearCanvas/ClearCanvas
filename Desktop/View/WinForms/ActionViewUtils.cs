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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
	internal static class ActionViewUtils
	{
		/// <summary>
		/// Sets the tooltip text on the specified item, from the specified action.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="action"></param>
		internal static void SetTooltipText(ToolStripItem item, IAction action)
		{
			var actionTooltip = action.Tooltip;
			if (string.IsNullOrEmpty(actionTooltip))
				actionTooltip = (action.Label ?? string.Empty).Replace("&", "");

			var clickAction = action as IClickAction;

			if (clickAction == null || clickAction.KeyStroke == XKeys.None)
			{
				item.ToolTipText = actionTooltip;
				return;
			}

			var keyCode = clickAction.KeyStroke & XKeys.KeyCode;

			var builder = new StringBuilder();
			builder.Append(actionTooltip);

			if (keyCode != XKeys.None)
			{
				if (builder.Length > 0)
					builder.AppendLine();
				builder.AppendFormat("{0}: ", SR.LabelKeyboardShortcut);
				builder.Append(XKeysConverter.Format(clickAction.KeyStroke));
			}

			item.ToolTipText = builder.ToString();
		}

		/// <summary>
		/// Sets the icon on the specified item, from the specified action.
		/// </summary>
		/// <param name="item"></param>
		/// <param name="action"></param>
		/// <param name="iconSize"></param>
		internal static void SetIcon(ToolStripItem item, IAction action, IconSize iconSize)
		{
			if (action.IconSet != null && action.ResourceResolver != null)
			{
				try
				{
					var oldImage = item.Image;

					item.Image = action.IconSet.CreateIcon(iconSize, action.ResourceResolver);
					if (oldImage != null)
						oldImage.Dispose();

					item.Invalidate();
				}
				catch (Exception e)
				{
					// the icon was either null or not found - log some helpful message
					Platform.Log(LogLevel.Error, e);
				}
			}
		}
	}
}

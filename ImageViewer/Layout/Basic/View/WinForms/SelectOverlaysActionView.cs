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

using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	[ExtensionOf(typeof (SelectOverlaysActionViewExtensionPoint))]
	public class SelectOverlaysActionView : WinFormsActionView
	{
		private ToolStripControlHost _toolStripControlHost;

		#region Overrides of WinFormsView

		public override object GuiElement
		{
			get
			{
				if (_toolStripControlHost == null)
				{
					var action = (SelectOverlaysAction) base.Context.Action;
					var control = new SelectOverlaysControl(action, CloseToolstrip) {BackColor = Color.Transparent};
					//Seems we have to put the control inside a panel in order for it to be sized properly.
					var panel = new Panel
					            	{
					            		AutoSize = false,
					            		BackColor = Color.Transparent,
					            		Padding = new Padding(0, 0, 0, 0),
					            		Margin = new Padding(0)
					            	};

					panel.Controls.Add(control);
					control.Location = new Point(0, 0);
					panel.Size = panel.MinimumSize = panel.PreferredSize;

					return _toolStripControlHost = new ToolStripControlHost(panel)
					                               	{
					                               		AutoSize = false,
					                               		ControlAlign = ContentAlignment.TopLeft,
					                               		Margin = new Padding(0),
					                               		Padding = new Padding(0),
					                               		Size = panel.Size
					                               	};
				}

				return _toolStripControlHost;
			}
		}

		private void CloseToolstrip()
		{
			_toolStripControlHost.PerformClick();
		}

		#endregion
	}
}
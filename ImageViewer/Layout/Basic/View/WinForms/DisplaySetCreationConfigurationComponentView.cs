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

using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms view onto <see cref="DisplaySetCreationConfigurationComponent"/>.
	/// </summary>
	[ExtensionOf(typeof (DisplaySetCreationConfigurationComponentViewExtensionPoint))]
	public class DisplaySetCreationConfigurationComponentView : WinFormsView, IApplicationComponentView
	{
		private DisplaySetCreationConfigurationComponent _component;
		private Panel _control;

		#region IApplicationComponentView Members

		/// <summary>
		/// Called by the host to assign this view to a component.
		/// </summary>
		public void SetComponent(IApplicationComponent component)
		{
			_component = (DisplaySetCreationConfigurationComponent) component;
		}

		#endregion

		/// <summary>
		/// Gets the underlying GUI component for this view.
		/// </summary>
		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					// this control is pretty full already, and some of the localizations of this page are pretty wordy, so we're inserting an autoscroll pane here
					_control = new Panel {AutoScroll = true};
					_control.Controls.Add(new DisplaySetCreationConfigurationComponentControl(_component));
				}
				return _control;
			}
		}
	}
}
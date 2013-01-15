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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.View.WinForms
{
	[ExtensionOf(typeof (StudyComposerItemEditorComponentViewExtensionPoint))]
	public class StudyComposerItemPropertiesComponentView : WinFormsView, IApplicationComponentView
	{
		private StudyComposerItemEditorComponent _component;
		private StudyComposerItemEditorComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (StudyComposerItemEditorComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new StudyComposerItemEditorComponentControl(_component);
				}
				return _control;
			}
		}
	}
}
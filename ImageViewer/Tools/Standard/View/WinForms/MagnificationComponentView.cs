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
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	[ExtensionOf(typeof(MagnificationViewExtensionPoint))]
    public class MagnificationComponentView : WinFormsView, IMagnificationView
    {
        private MagnificationForm _form;

		public override object GuiElement
        {
            get { throw new InvalidOperationException("Not valid for this view type."); }
        }

		#region IMagnificationView Members

        public void Open(IPresentationImage image, Point locationTile, RenderMagnifiedImage render)
		{
            _form = new MagnificationForm((PresentationImage)image, locationTile, render);
			_form.Show();
		}

		public void Close()
		{
			if (_form != null)
			{
				_form.Dispose();
				_form = null;
			}
		}

		public void UpdateMouseLocation(Point location)
		{
			if (_form == null)
				throw new InvalidOperationException("Open must be called before UpdateMouseInformation");

			_form.UpdateMousePosition(location);
		}

		#endregion
    }
}

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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// WinForms implementation of the <see cref="LayoutComponentViewExtensionPoint"/> extension point.
    /// The actual user-interface is implemented by <see cref="LayoutControl"/>.
    /// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(LayoutComponentViewExtensionPoint))]
    public class LayoutComponentView : WinFormsView, IApplicationComponentView
    {
        private Control _control;
        private LayoutComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public LayoutComponentView()
        {
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (LayoutComponent)component;
        }

        #endregion

        /// <summary>
        /// Overridden to return an instance of <see cref="LayoutControl"/>
        /// </summary>
        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new LayoutControl(_component);
                }
                return _control;
            }
        }
    }
}

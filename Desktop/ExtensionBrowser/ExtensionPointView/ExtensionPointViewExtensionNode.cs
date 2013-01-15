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

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.ExtensionBrowser.ExtensionPointView
{
    public class ExtensionPointViewExtensionNode : BrowserNode
    {
        private ExtensionInfo _ext;

        internal ExtensionPointViewExtensionNode(ExtensionInfo ext)
        {
            _ext = ext;
        }

        protected override void CreateChildNodes()
        {
            // no child nodes
        }

        public override string DisplayName
        {
            get { return GetDefaultDisplayName(_ext); }
        }

        public override string Details
        {
            get { return GetDefaultDetails(_ext); }
        }

        public override bool Enabled
        {
            get { return _ext.Enabled; }
        }
    }
}

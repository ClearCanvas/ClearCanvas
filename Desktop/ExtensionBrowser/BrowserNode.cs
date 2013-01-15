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

namespace ClearCanvas.Desktop.ExtensionBrowser
{
    public abstract class BrowserNode : IBrowserNode
    {
        private List<IBrowserNode> _childNodes;


        protected abstract void CreateChildNodes();

        protected void AddChild(IBrowserNode child)
        {
            _childNodes.Add(child);
        }

        protected string GetDefaultDisplayName(IBrowsable browsableObject)
        {
            // for now, return the formal name of the object
            // in future, might use some logic to determine whether to use the formal
            // name or the friendly name
            return browsableObject.FormalName;
        }

        protected string GetDefaultDetails(IBrowsable browsableObject)
        {
            // for now, return the description of the object
            // although a given node class may wish to provide other details
            return browsableObject.Description == null ? "" : browsableObject.Description;
        }

        #region IBrowserNode Members

        public List<IBrowserNode> ChildNodes
        {
            get
            {
                if (_childNodes == null)
                {
                    _childNodes = new List<IBrowserNode>();
                    CreateChildNodes();
                }
                return _childNodes;
            }
        }

        public virtual bool Enabled
        {
            get { return true; }
        }

        public abstract string DisplayName
        {
            get;
        }

        public abstract string Details
        {
            get;
        }

        #endregion
    }
}

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

using ClearCanvas.Desktop.ExtensionBrowser.PluginView;
using ClearCanvas.Desktop.ExtensionBrowser.ExtensionPointView;

namespace ClearCanvas.Desktop.ExtensionBrowser
{
    /// <summary>
    /// Defines an extension point for views onto the <see cref="ExtensionBrowserComponent"/>
    /// </summary>
    [ExtensionPoint()]
	public sealed class ExtensionBrowserComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// Component that displays the set of installed extensions in a tree view.
    /// </summary>
    [AssociateView(typeof(ExtensionBrowserComponentViewExtensionPoint))]
    public class ExtensionBrowserComponent : ApplicationComponent
    {
        private PluginViewRootNode _pluginViewTree;
        private ExtensionPointViewRootNode _extPtViewTree;


        public override void Start()
        {
            base.Start();

            // create the browser trees here - this will not incur any real cost,
            // because each level of the browser tree does not load it's children until requested
            _pluginViewTree = new PluginViewRootNode();
            _extPtViewTree = new ExtensionPointViewRootNode();
        }

        /// <summary>
        /// Gets a tree model that represents a "plugin-centric" view of the extensions
        /// </summary>
        public PluginViewRootNode PluginTree
        {
            get { return _pluginViewTree; }
        }

        /// <summary>
        /// Gets a tree model that represents an "extension point centric" view of the extensions
        /// </summary>
        public ExtensionPointViewRootNode ExtensionPointTree
        {
            get { return _extPtViewTree; }
        }
    }
}

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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Automation
{
    [ExtensionPoint]
    [Obsolete("Deprecated due to quirks with generic extension points that may cause some extensions to be missed.")]
    public sealed class AutomationExtensionPoint<T> : ExtensionPoint<T>
    {
    }

    public interface IAutomationContext
    {
        IImageBox SelectedImageBox { get; }
        ITile SelectedTile { get; }
    }

    public class AutomationContext : IAutomationContext
    {
        public static IAutomationContext Current { get; internal set; }

        private readonly ImageViewerComponent _viewer;

        internal AutomationContext(ImageViewerComponent viewer)
        {
            _viewer = viewer;
        }

        #region IContext Members

        public IWorkspaceLayout WorkspaceLayoutService { get { return _viewer.GetAutomationService<IWorkspaceLayout>(); } }
        public IDisplaySetLayout DisplaySetLayoutService { get { return _viewer.GetAutomationService<IDisplaySetLayout>(); } }
        public IStack StackService { get { return _viewer.GetAutomationService<IStack>(); } }

        public IImageBox SelectedImageBox { get { return _viewer.SelectedImageBox; } }
        public ITile SelectedTile { get { return _viewer.SelectedTile; } }

        #endregion
    }
}

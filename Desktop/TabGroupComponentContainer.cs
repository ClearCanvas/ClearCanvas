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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
	/// Defines an extension point for views onto the <see cref="TabGroupComponentContainer"/>
    /// </summary>
	public sealed class TabbedGroupsComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	/// <summary>
	/// An enumeration describing the available layout directions of the <see cref="TabGroupComponentContainer"/>.
	/// </summary>
    public enum LayoutDirection
    {
		/// <summary>
		/// The layout should be horizontal.
		/// </summary>
        Horizontal = 0,

		/// <summary>
		/// The layout should be vertical.
		/// </summary>
        Vertical = 1
    }

	/// <summary>
	/// An application component that serves as a container for other components, hosted in <see cref="TabGroup"/>s.
	/// </summary>
    [AssociateView(typeof(TabbedGroupsComponentContainerViewExtensionPoint))]
    public class TabGroupComponentContainer : ApplicationComponentContainer
    {
		/// <summary>
		/// A host for <see cref="TabGroup"/>s.
		/// </summary>
        private class TabGroupHost : ContainedComponentHost
        {
            internal TabGroupHost(
                TabGroupComponentContainer owner, TabGroup tabGroup)
                : base(owner, tabGroup.Component)
            {
            }
        }

        private readonly List<TabGroup> _tabGroups;
        private readonly LayoutDirection _layoutDirection;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TabGroupComponentContainer(LayoutDirection layoutDirection)
        {
            _tabGroups = new List<TabGroup>();
            _layoutDirection = layoutDirection;
        }

		/// <summary>
		/// Adds a <see cref="TabGroup"/> to the container.
		/// </summary>
        public void AddTabGroup(TabGroup tg)
        {
            //if (tg != null && tg.ComponentHost != null && tg.ComponentHost.IsStarted)
            //    throw new InvalidOperationException(SR.ExceptionCannotSetTabGroupAfterContainerStarted);

            tg.ComponentHost = new TabGroupHost(this, tg);
            _tabGroups.Add(tg);
        }

		/// <summary>
		/// Gets a list of the <see cref="TabGroup"/>s in the container.
		/// </summary>
		public IList<TabGroup> TabGroups
        {
            get { return _tabGroups.AsReadOnly(); }
        }

		/// <summary>
		/// Gets the <see cref="LayoutDirection"/> of the container.
		/// </summary>
        public LayoutDirection LayoutDirection
        {
            get { return _layoutDirection; }
        }

		/// <summary>
		/// Gets the <see cref="TabGroup"/> that owns a particular <see cref="TabPage"/>.
		/// </summary>
        public TabGroup GetTabGroup(TabPage page)
        {
            foreach (TabGroup tg in _tabGroups)
            {
                if (CollectionUtils.Contains(tg.Component.Pages,
                    delegate(TabPage tp) { return tp == page; }))
                {
                    return tg;
                }
            }

            return null;
        }

        #region ApplicationComponent overrides

		/// <summary>
		/// Starts this component and all the contained <see cref="TabGroup"/>s.
		/// </summary>
        public override void Start()
        {
            base.Start();

            foreach (TabGroup tabGroup in _tabGroups)
            {
                tabGroup.ComponentHost.StartComponent();
            }
        }

		/// <summary>
		/// Stops this component and all the contained <see cref="TabGroup"/>s.
		/// </summary>
		public override void Stop()
        {
            foreach (TabGroup tabGroup in _tabGroups)
            {
                tabGroup.ComponentHost.StopComponent();
            }

            base.Stop();
        }

		/// <summary>
		/// Unless overridden, returns the union of all actions in the contained <see cref="TabGroup"/>s.
		/// </summary>
        public override IActionSet ExportedActions
        {
            get
            {
                IActionSet exportedActionSet = new ActionSet(); ;

                // export the actions from all subcomponents
                foreach (TabGroup tabGroup in _tabGroups)
                {
                    exportedActionSet.Union(tabGroup.Component.ExportedActions);
                }

                return exportedActionSet;
            }
        }

        #endregion

        #region ApplicationComponentContainer overrides

		/// <summary>
		/// Enumerates all the <see cref="IApplicationComponent"/>s hosted in the contained <see cref="TabGroup"/>s.
		/// </summary>
        public override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get 
            {
                List<IApplicationComponent> components = new List<IApplicationComponent>();
                foreach (TabGroup tabGroup in _tabGroups)
                {
                    components.AddRange(tabGroup.Component.ContainedComponents);
                }
                return components;
            }
        }

		/// <summary>
		/// Enumerates all the <see cref="IApplicationComponent"/>s hosted 
		/// in the contained <see cref="TabGroup"/>s that are currently visible.
		/// </summary>
		public override IEnumerable<IApplicationComponent> VisibleComponents
        {
            get 
            {
                List<IApplicationComponent> components = new List<IApplicationComponent>();
                foreach (TabGroup tabGroup in _tabGroups)
                {
                    components.AddRange(tabGroup.Component.VisibleComponents);
                }
                return components;
            }
        }

		/// <summary>
		/// Does nothing, since all contained <see cref="IApplicationComponent" />s are already visible.
		/// </summary>
		public override void EnsureVisible(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted components are started by default
        }

		/// <summary>
		/// Does nothing, since all contained <see cref="IApplicationComponent" />s have already been started.
		/// </summary>
		public override void EnsureStarted(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted components are visible by default
        }

        #endregion

    }
}

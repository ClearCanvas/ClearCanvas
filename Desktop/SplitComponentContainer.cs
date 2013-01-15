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
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
	/// Defines an extension point for views onto the <see cref="SplitComponentContainer"/>.
    /// </summary>
	public sealed class SplitComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	/// <summary>
	/// Specifies the orientation of the <see cref="SplitComponentContainer"/>.
	/// </summary>
	public enum SplitOrientation
	{
		/// <summary>
		/// The <see cref="SplitComponentContainer"/> should be split horizontally.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// The <see cref="SplitComponentContainer"/> should be split vertically.
		/// </summary>
		Vertical = 1
	}

	/// <summary>
	/// A component container for hosting two <see cref="IApplicationComponent"/>s
	/// separated by a splitter.
	/// </summary>
    [AssociateView(typeof(SplitComponentContainerViewExtensionPoint))]
    public class SplitComponentContainer : ApplicationComponentContainer
    {
		/// <summary>
		/// A host for a <see cref="SplitPane"/>.
		/// </summary>        
        private class SplitPaneHost : ContainedComponentHost
        {
            internal SplitPaneHost(SplitComponentContainer owner,
                SplitPane pane)
                : base(owner, pane.Component)
            {
            }

			/// <summary>
			/// Contained components will use the comand history provided by the host that 
			/// owns the container.
			/// </summary>
			public override CommandHistory CommandHistory
			{
				get
				{
					return OwnerHost.CommandHistory;
				}
			}
		}

		private SplitPane _pane1;
		private SplitPane _pane2;
		private SplitOrientation _splitOrientation;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SplitComponentContainer(
			SplitPane pane1, 
			SplitPane pane2, 
			SplitOrientation splitOrientation)
        {
			this.Pane1 = pane1;
			this.Pane2 = pane2;

			_splitOrientation = splitOrientation;
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        public SplitComponentContainer(SplitOrientation splitOrientation)
        {
            _splitOrientation = splitOrientation;
        }

		/// <summary>
		/// Gets or sets the first <see cref="SplitPane"/>.
		/// </summary>
		public SplitPane Pane1
		{
			get { return _pane1; }
            set
            {
                if(_pane1 != null && _pane1.ComponentHost != null && _pane1.ComponentHost.IsStarted)
					throw new InvalidOperationException(SR.ExceptionCannotSetPaneAfterContainerStarted);

                _pane1 = value;
                _pane1.ComponentHost = new SplitPaneHost(this, _pane1);
            }
		}

		/// <summary>
		/// Gets or sets the second <see cref="SplitPane"/>.
		/// </summary>
		public SplitPane Pane2
		{
			get { return _pane2; }
            set
            {
                if (_pane2 != null && _pane2.ComponentHost != null && _pane2.ComponentHost.IsStarted)
					throw new InvalidOperationException(SR.ExceptionCannotSetPaneAfterContainerStarted);

                _pane2 = value;
                _pane2.ComponentHost = new SplitPaneHost(this, _pane2);
            }
        }

		/// <summary>
		/// Gets the <see cref="SplitOrientation"/> of the container.
		/// </summary>
		public SplitOrientation SplitOrientation
		{
			get { return _splitOrientation; }
        }

        #region ApplicationComponent overrides

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		///  <remarks>
		/// <para>
		/// Calls <see cref="ApplicationComponent.Start"/> on both of the <see cref="SplitPane"/>s.
		/// </para>
		/// <para>
		/// Override this method to implement custom initialization logic.  Overrides must be sure to call the base implementation.
		/// </para>
		/// </remarks>
		public override void Start()
        {
			base.Start();

			_pane1.ComponentHost.StartComponent();
            _pane2.ComponentHost.StartComponent();
        }

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Calls <see cref="ApplicationComponent.Stop"/> on both of the <see cref="SplitPane"/>s.
		/// </para>
		/// <para>
		/// Override this method to implement custom termination logic.  Overrides must be sure to call the base implementation.
		/// </para>
		/// </remarks>
		public override void Stop()
        {
            _pane1.ComponentHost.StopComponent();
            _pane2.ComponentHost.StopComponent();

            base.Stop();
        }

		/// <summary>
		/// Returns the set of actions that the component wishes to export to the desktop.
		/// </summary>
		/// <remarks>
		/// The <see cref="IActionSet"/> returned by this method is the union of the 
		/// exported actions from the two <see cref="SplitPane"/>s.
		/// </remarks>
		public override IActionSet ExportedActions
        {
            get
            {
                // export the actions from both subcomponents
                return _pane1.Component.ExportedActions.Union(_pane2.Component.ExportedActions);
            }
        }

        #endregion

        #region ApplicationComponentContainer overrides

		/// <summary>
		/// Gets an enumeration of the contained components.
		/// </summary>
		/// <remarks>
		/// Simply returns both <see cref="SplitPane"/>s.
		/// </remarks>
		public override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get { return new IApplicationComponent[] { _pane1.Component, _pane2.Component }; }
        }

		/// <summary>
		/// Gets an enumeration of the components that are currently visible.
		/// </summary>
		/// <remarks>
		/// Simply returns both <see cref="SplitPane"/>s, since they are always visible.
		/// </remarks>
		public override IEnumerable<IApplicationComponent> VisibleComponents
        {
            get { return this.ContainedComponents; }
        }

		/// <summary>
		/// Ensures that the specified component is visible.
		/// </summary>
		/// <remarks>
		/// Does nothing because both <see cref="SplitPane"/>s are already visible.
		/// </remarks>
		public override void EnsureVisible(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted components are started by default
        }

		/// <summary>
		/// Ensures that the specified component has been started.
		/// </summary>
		/// <remarks>
		/// Does nothing because both <see cref="SplitPane"/>s are already started.
		/// </remarks>
		public override void EnsureStarted(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted components are visible by default
        }

        #endregion
    }
}

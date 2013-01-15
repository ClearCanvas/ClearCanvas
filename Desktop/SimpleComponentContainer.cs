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

namespace ClearCanvas.Desktop
{
    /// <summary>
	/// Defines an extension point for views onto the <see cref="SimpleComponentContainer"/>.
    /// </summary>
	public sealed class SimpleComponentContainerViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

	/// <summary>
	/// A simple container class for hosting <see cref="IApplicationComponent"/>s
	/// that provides Ok and Cancel buttons.
	/// </summary>
    [AssociateView(typeof(SimpleComponentContainerViewExtensionPoint))]
    public class SimpleComponentContainer : ApplicationComponentContainer
    {
        private class HostImpl : ContainedComponentHost
        {
            internal HostImpl(
                SimpleComponentContainer owner,
                IApplicationComponent component)
                : base(owner, component)
            {
            }

            #region ApplicationComponentHost overrides

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

			/// <summary>
            /// Gets or sets the title displayed in the user-interface.
            /// </summary>
            public override string Title
            {
                set { OwnerHost.Title = value; }
            }

            #endregion
        }


		private readonly IApplicationComponent _component;
        private readonly HostImpl _componentHost;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SimpleComponentContainer(IApplicationComponent component)
		{
			_component = component;
            _componentHost = new HostImpl(this, _component);
		}

		/// <summary>
		/// The host object for the contained <see cref="IApplicationComponent"/>.
		/// </summary>
		public ApplicationComponentHost ComponentHost
        {
            get { return _componentHost; }
        }

        #region ApplicationComponent overrides

		/// <summary>
		/// Starts this component and the <see cref="ComponentHost"/>.
		/// </summary>
		///  <remarks>
		/// Override this method to implement custom initialization logic.  Overrides must be sure to call the base implementation.
		/// </remarks>
		public override void Start()
        {
			base.Start();

			_componentHost.StartComponent();
        }

		/// <summary>
		/// Stops this component and the <see cref="ComponentHost"/>.
		/// </summary>
		/// <remarks>
		/// Override this method to implement custom termination logic.  Overrides must be sure to call the base implementation.
		/// </remarks>
		public override void Stop()
        {
            _componentHost.StopComponent();

            base.Stop();
        }

		/// <summary>
		/// Gets a value indicating whether there are any data validation errors.
		/// </summary>
		public override bool HasValidationErrors
		{
			get { return _componentHost.Component.HasValidationErrors || base.HasValidationErrors; }
		}

		/// <summary>
		/// Sets the <see cref="ApplicationComponent.ValidationVisible"/> property and raises the 
		/// <see cref="ApplicationComponent.ValidationVisibleChanged"/> event.
		/// </summary>
		public override void ShowValidation(bool show)
		{
			base.ShowValidation(show);
			_componentHost.Component.ShowValidation(show);
		}

        #endregion

        #region ApplicationComponentContainer overrides

		/// <summary>
		/// Gets an enumeration of the contained components.
		/// </summary>
		public override IEnumerable<IApplicationComponent> ContainedComponents
        {
            get { return new IApplicationComponent[] { _componentHost.Component }; }
        }

		/// <summary>
		/// Gets an enumeration of the components that are currently visible.
		/// </summary>
		public override IEnumerable<IApplicationComponent> VisibleComponents
        {
            get { return this.ContainedComponents; }
        }

		/// <summary>
		/// Does nothing, since the hosted component is started by default.
		/// </summary>
		public override void EnsureStarted(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted component is started by default
        }

		/// <summary>
		/// Does nothing, since the hosted component is visible by default.
		/// </summary>
		public override void EnsureVisible(IApplicationComponent component)
        {
            if (!this.IsStarted)
                throw new InvalidOperationException(SR.ExceptionContainerNeverStarted);

            // nothing to do, since the hosted component is visible by default
        }

        #endregion

        #region Presentation Model

		/// <summary>
		/// Called by the view to indicate the user dismissed the dialog with "Ok"; the <see cref="ApplicationComponent.ExitCode"/>
		/// is set to <see cref="ApplicationComponentExitCode.Accepted"/>.
		/// </summary>
		public void OK()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}
			base.ExitCode = ApplicationComponentExitCode.Accepted;
			base.Host.Exit();
		}

		/// <summary>
		/// Called by the view to indicate the user dismissed the dialog with "Cancel"; the <see cref="ApplicationComponent.ExitCode"/>
		/// is set to <see cref="ApplicationComponentExitCode.None"/>.
		/// </summary>
		public void Cancel()
		{
			base.ExitCode = ApplicationComponentExitCode.None;
            base.Host.Exit();
        }

        #endregion
    }
}

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

using System.Collections.Generic;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract base class for application components that act as containers for other application components.
    /// </summary>
    public abstract class ApplicationComponentContainer : ApplicationComponent, IApplicationComponentContainer
	{
		#region ContainedComponentHost class

		/// <summary>
        /// Defines an application component host appropriate for the components the 
        /// <see cref="ApplicationComponentContainer"/> will contain.  The host overrides delegate to
        /// the host of the parent container.
        /// </summary>
        protected class ContainedComponentHost : ApplicationComponentHost
        {
            private readonly ApplicationComponentContainer _owner;

            /// <summary>
            /// Contruct the contained sub host with the <see cref="ApplicationComponentContainer"/>
            /// owner that will provide access to the real host.  The contained component is passed
            /// to the base <see cref="ApplicationComponentHost"/>.
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="component"></param>
            public ContainedComponentHost(
                ApplicationComponentContainer owner,
                IApplicationComponent component)
                : base(component)
            {
                Platform.CheckForNullReference(owner, "owner");

                _owner = owner;
            }

            /// <summary>
            /// Gets the associated desktop window.
            /// </summary>
            public override DesktopWindow DesktopWindow
            {
                get { return OwnerHost.DesktopWindow; }
            }

            /// <summary>
            /// Gets the title displayed in the user-interface.
            /// </summary>
            /// <remarks>
            /// The title generally cannot be set.  This behavior is inherited from the 
            /// base.
            /// </remarks>
            /// <exception cref="NotSupportedException">The host does not support setting the title.</exception>
            public override string Title
            {
                get { return OwnerHost.Title; }
            }

            /// <summary>
            /// Provide access to the owning host in case subclasses need to override host behavior not 
            /// already handled by this class.
            /// </summary>
            protected IApplicationComponentHost OwnerHost
            {
                get { return _owner.Host; }
            }
		}

		#endregion

		private IApplicationComponentContainerValidationStrategy _validationStrategy;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ApplicationComponentContainer()
        {
            _validationStrategy = new NoComponentsValidationStrategy();
        }

        /// <summary>
        /// Gets or sets the validation strategy that determines how this container responds
        /// to validation requests.
        /// </summary>
        public IApplicationComponentContainerValidationStrategy ValidationStrategy
        {
            get { return _validationStrategy; }
            set { _validationStrategy = value; }
        }

        /// <summary>
        /// Gets a value indicating whether there are any data validation errors.
        /// </summary>
        /// <remarks>
        /// The default implementation of this property delegates to the <see cref="ValidationStrategy"/> object.
        /// Invoking this property may cause any unstarted components in the container to be started,
        /// which means that it may throw exceptions.
        /// </remarks>
        public override bool HasValidationErrors
        {
            get
            {
                return _validationStrategy.HasValidationErrors(this);
            }
        }

        /// <summary>
		/// Sets the <see cref="ApplicationComponent.ValidationVisible"/> property and raises the 
		/// <see cref="ApplicationComponent.ValidationVisibleChanged"/> event.
        /// </summary>
        /// <remarks>
        /// The default implementation of this property delegates to the <see cref="ValidationStrategy"/> object.
        /// Invoking this property may cause any unstarted components in the container to be started,
        /// which means that it may throw exceptions.
        /// </remarks>
        /// <param name="show"></param>
        public override void ShowValidation(bool show)
        {
            _validationStrategy.ShowValidation(this, show);
        }

        #region IApplicationComponentContainer Members

        /// <summary>
        /// Gets an enumeration of the contained components.
        /// </summary>
        public abstract IEnumerable<IApplicationComponent> ContainedComponents { get; }

        /// <summary>
        /// Gets an enumeration of the components that are currently visible.
        /// </summary>
        public abstract IEnumerable<IApplicationComponent> VisibleComponents { get; }

        /// <summary>
        /// Ensures that the specified component is visible.
        /// </summary>
        public abstract void EnsureVisible(IApplicationComponent component);

        /// <summary>
        /// Ensures that the specified component has been started.
        /// </summary>
        public abstract void EnsureStarted(IApplicationComponent component);

        #endregion
    }
}

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

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an interface that must be implemented by application component containers (components
    /// that host other components).
    /// </summary>
    public interface IApplicationComponentContainer
    {
        /// <summary>
        /// Gets all contained components.
        /// </summary>
        IEnumerable<IApplicationComponent> ContainedComponents { get; }

        /// <summary>
        /// Get the contained components that are currently visible to the user.
        /// </summary>
        IEnumerable<IApplicationComponent> VisibleComponents { get; }

        /// <summary>
        /// Ensures that the specified component is made visible to the user.
        /// </summary>
        void EnsureVisible(IApplicationComponent component);

        /// <summary>
        /// Ensures that the specified component has been started. 
        /// </summary>
        /// <remarks>
        /// A container may choose not to start components until they are actually displayed for the first time.
        /// This method ensures that a component is started regardless of whether it has ever been displayed.
        /// This is necessary, for instance, if the component is to be validated as part of validating the container.
        /// </remarks>
        void EnsureStarted(IApplicationComponent component);
    }
}

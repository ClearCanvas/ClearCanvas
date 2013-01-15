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

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Defines the interface to provide custom validation strategies for application components.
    /// </summary>
    public interface IApplicationComponentContainerValidationStrategy
    {
        /// <summary>
        /// Determines whether the specified container has validation errors, according to this strategy.
        /// </summary>
        bool HasValidationErrors(IApplicationComponentContainer container);

        /// <summary>
        /// Displays validation errors for the specified container to the user, according to the logic
        /// encapsulated in this strategy.
        /// </summary>
        void ShowValidation(IApplicationComponentContainer container, bool show);
    }
}

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
    /// Defines the interface to a validation rule that is applied to a <see cref="IApplicationComponent"/>.
    /// </summary>
    /// <remarks>
	/// The <see cref="PropertyName"/> property specifies a property of the application component
	/// that the rule applies to.  Any validation error message will be displayed next to the GUI object
	/// that is bound to this property.
    /// </remarks>
    public interface IValidationRule
    {
        /// <summary>
        /// Gets the name of the property on the application component that this rule applies to.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Obtains the current result of evaluating this rule based on the current state of the application component.
        /// </summary>
        ValidationResult GetResult(IApplicationComponent component);
    }
}

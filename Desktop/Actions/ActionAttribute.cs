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

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Abstract base class for the set of attributes that are used to specify actions declaratively.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public abstract class ActionAttribute : Attribute
    {
        private readonly string _actionID;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">A logical action identifier.</param>
        protected ActionAttribute(string actionID)
        {
            _actionID = actionID;
        }

        /// <summary>
        /// Returns the logical action ID qualified by the type name of the specified target object.
        /// </summary>
        /// <param name="target">The object whose type should be used to qualify the action ID.</param>
        public string QualifiedActionID(object target)
        {
            // create a fully qualified action ID
            return string.Format("{0}:{1}", target.GetType().FullName, _actionID);
        }

		/// <summary>
		/// Applies this attribute to an <see cref="IAction"/> instance, via the specified <see cref="IActionBuildingContext"/>.
		/// </summary>
		public abstract void Apply(IActionBuildingContext builder);
    }
}

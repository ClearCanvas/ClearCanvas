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

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Indicates to the framework that an action used to have one or more different IDs
    /// and should replace the old entries in the action model configuration.
    /// </summary>
    public class ActionFormerlyAttribute : ActionDecoratorAttribute
    {
        private readonly string[] _formerActionIds;

        /// <summary>
        /// Attribute constructor.
        /// </summary>
        /// <param name="actionID">The id of the action.</param>
        /// <param name="formerActionIds">One or more fully qualified "former" action IDs.</param>
        public ActionFormerlyAttribute(string actionID, params string[] formerActionIds) : base(actionID)
        {
            _formerActionIds = formerActionIds;
        }

        public override void Apply(IActionBuildingContext builder)
        {
            foreach (var formerActionId in _formerActionIds)
                builder.Action.FormerActionIDs.Add(formerActionId);
        }
    }
}

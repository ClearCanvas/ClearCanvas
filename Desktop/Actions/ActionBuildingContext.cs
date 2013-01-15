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

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IActionBuildingContext"/>.
    /// </summary>
    internal class ActionBuildingContext : IActionBuildingContext
    {
        private readonly string _actionID;
        private readonly object _actionTarget;
        private readonly ResourceResolver _resolver;
        private Action _action;

        public ActionBuildingContext(string actionID, object actionTarget)
        {
            _actionID = actionID;
            _actionTarget = actionTarget;

            _resolver = new ActionResourceResolver(_actionTarget);
        }

        public string ActionID
        {
            get { return _actionID; }
        }

        public Action Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public IResourceResolver ResourceResolver
        {
            get { return _resolver; }
        }

        public object ActionTarget
        {
            get { return _actionTarget; }
        }
    }
}

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
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// A helper class for rendering an action model as HTML.
    /// </summary>
    public class HtmlActionModelRenderer
    {
        /// <summary>
        /// Searches <paramref name="actionNode"/> and returns the action (represented as HTML) whose label matches
        /// <paramref name="labelSearch"/>.
        /// </summary>
        /// <param name="actionNode">The node to be searched.</param>
        /// <param name="labelSearch">The label to match on.</param>
        /// <param name="actionLabel">The new label to be applied to the action in the returned HTML.</param>
        /// <returns>The found action represented as HTML, otherwise an empty string.</returns>
        public string GetHTML(ActionModelNode actionNode, string labelSearch, string actionLabel)
        {
            IAction[] actions = actionNode.GetActionsInOrder();
            if (actions.Length == 0)
                return "";

            // find the action corresponding to the action label, if exist
            foreach (var action in actions)
            {
                if (action.Label == labelSearch)
                    return GetHTML(action.Path.LocalizedPath, actionLabel);
            }

            return "";
        }

        private static string GetHTML(string actionPath, string actionLabel)
        {
            return String.Format("<a href=\"action://{0}\">{1}</a>", actionPath, actionLabel);
        }
    }
}
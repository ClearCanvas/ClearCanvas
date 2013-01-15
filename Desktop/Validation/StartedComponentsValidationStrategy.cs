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

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Implements a validation strategy that considers only the contained nodes that have been visited.
    /// </summary>
	public class StartedComponentsValidationStrategy : IApplicationComponentContainerValidationStrategy
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public StartedComponentsValidationStrategy()
		{
		}

    	#region IApplicationComponentContainerValidationStrategy Members

    	/// <summary>
    	/// Determines whether the specified container has validation errors, according to this strategy.
    	/// </summary>
    	public bool HasValidationErrors(IApplicationComponentContainer container)
        {
            // true if any started component has validation errors
            return CollectionUtils.Contains<IApplicationComponent>(container.ContainedComponents,
                delegate(IApplicationComponent c)
                {
                    return c.IsStarted && c.HasValidationErrors;
                });
        }

    	/// <summary>
    	/// Displays validation errors for the specified container to the user, according to the logic
    	/// encapsulated in this strategy.
    	/// </summary>
    	public void ShowValidation(IApplicationComponentContainer container, bool show)
        {
            if (show)
            {
                // propagate to each started component
                foreach (IApplicationComponent c in container.ContainedComponents)
                {
                    if(c.IsStarted)
                        c.ShowValidation(show);
                }

                bool visibleComponentHasErrors = CollectionUtils.Contains<IApplicationComponent>(container.VisibleComponents,
                    delegate(IApplicationComponent c) { return c.HasValidationErrors; });

                // if there are no errors on a visible component, find the first component with errors and ensure it is visible
                if (!visibleComponentHasErrors)
                {
                    IApplicationComponent firstComponentWithErrors = CollectionUtils.SelectFirst<IApplicationComponent>(
                        container.ContainedComponents,
                        delegate(IApplicationComponent c) { return c.IsStarted && c.HasValidationErrors; });

                    if (firstComponentWithErrors != null)
                        container.EnsureVisible(firstComponentWithErrors);
                }
            }
            else
            {
                // propagate to each started component
                foreach (IApplicationComponent c in container.ContainedComponents)
                {
                    if (c.IsStarted)
                        c.ShowValidation(show);
                }
            }

        }

        #endregion
    }
}

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
    /// Implements a validation strategy that considers all contained nodes, regardless of whether the user
    /// has visited them or not.
    /// </summary>
    public class AllComponentsValidationStrategy : IApplicationComponentContainerValidationStrategy
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		public AllComponentsValidationStrategy()
		{
		}

    	#region IApplicationComponentContainerValidationStrategy Members

    	/// <summary>
    	/// Determines whether the specified container has validation errors, according to this strategy.
    	/// </summary>
    	public bool HasValidationErrors(IApplicationComponentContainer container)
        {
            // true if any contained component has validation errors
            return CollectionUtils.Contains<IApplicationComponent>(container.ContainedComponents,
                delegate(IApplicationComponent c)
                {
                    container.EnsureStarted(c);
                    return c.HasValidationErrors;
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
                // propagate to each component, starting the component if not already started
                foreach (IApplicationComponent c in container.ContainedComponents)
                {
                    container.EnsureStarted(c);
                    c.ShowValidation(show);
                }

                bool visibleComponentHasErrors = CollectionUtils.Contains(container.VisibleComponents,
                    delegate(IApplicationComponent c) { return c.HasValidationErrors; });

                // if there are no errors on a visible component, find the first component with errors and ensure it is visible
                if (!visibleComponentHasErrors)
                {
                    IApplicationComponent firstComponentWithErrors = CollectionUtils.SelectFirst(
                        container.ContainedComponents,
                        delegate(IApplicationComponent c) { return c.HasValidationErrors; });

                    if (firstComponentWithErrors != null)
                    {
                        container.EnsureVisible(firstComponentWithErrors);

                        // bug #1644 : call ShowValidation after this component is already visible,
                        // to ensure that the error icons actually show up
                        firstComponentWithErrors.ShowValidation(show);
                    }
                }
            }
            else
            {
                // propagate to each started component
                foreach (IApplicationComponent c in container.ContainedComponents)
                {
                    if(c.IsStarted)
                        c.ShowValidation(show);
                }
            }
            
        }

        #endregion
    }
}

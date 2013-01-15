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
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Abstract base class for tools that operate in a <see cref="IWorkflowItemToolContext{TItem}"/> based context.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	/// <typeparam name="TContext"></typeparam>
    public abstract class WorkflowItemTool<TItem, TContext> : Tool<TContext>, IDropHandler<TItem>
        where TItem : WorklistItemSummaryBase
		where TContext : IWorkflowItemToolContext<TItem>
    {
        private readonly string _operationName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="operationName">Specifies the name of the workflow operation that this tool invokes.</param>
        protected WorkflowItemTool(string operationName)
        {
            _operationName = operationName;
        }

		/// <summary>
		/// Gets a value indicating whether this tool is currently enabled.
		/// </summary>
		/// <remarks>
		/// The default implmentation is based on the workflow operation enablement. Subclasses may
		/// override this property to modify this behaviour.
		/// </remarks>
        public virtual bool Enabled
        {
            get
            {
                return this.Context.SelectedItems.Count == 1 &&
					this.Context.GetOperationEnablement(_operationName);
            }
        }

		/// <summary>
		/// Occurs when the value of <see cref="Enabled"/> changes.
		/// </summary>
        public event EventHandler EnabledChanged
        {
            add { this.Context.SelectionChanged += value; }
            remove { this.Context.SelectionChanged -= value; }
        }

		/// <summary>
		/// Invokes the workflow operation.
		/// </summary>
        public void Apply()
        {
            TItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
			try
			{
				bool success = Execute(item);
				if (success)
				{
					this.Context.InvalidateSelectedFolder();
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		#region Protected API

		/// <summary>
		/// Gets the name of the workflow operation that this tool invokes.
		/// </summary>
        protected string OperationName
        {
            get { return _operationName; }
        }

		/// <summary>
		/// Called to invoke the workflow operation.
		/// </summary>
		/// <remarks>
		/// The method should return true if the operation was completed, in order to invalidate the currently selected folder.
		/// A return value of false implies that either the user cancelled, or the operation failed.  
		/// </remarks>
		/// <param name="item"></param>
		/// <returns></returns>
        protected abstract bool Execute(TItem item);

		#endregion

		#region IDropHandler<TItem> Members

		/// <summary>
		/// Asks the handler if it can accept the specified items.  This value is used to provide visual feedback
		/// to the user to indicate that a drop is possible.
		/// </summary>
		public virtual bool CanAcceptDrop(ICollection<TItem> items)
        {
            return this.Context.GetOperationEnablement(this.OperationName);
        }

		/// <summary>
		/// Asks the handler to process the specified items, and returns true if the items were successfully processed.
		/// </summary>
		public virtual bool ProcessDrop(ICollection<TItem> items)
        {
            TItem item = CollectionUtils.FirstElement(items);
			try
			{
				bool success = Execute(item);
				if (success)
				{
					// This invalidates the selected folder so that any updates effected by the tools Execute method are reflected.
					// Additionally, if the Execute method opens an ApplicationComponent, updates effected by the component's Start method
					// are reflected.  Changes effected by an ApplicationComponent after the Start method are not reflected in the invalidated
					// folder.  In this case, the selected folder should be invalidated again when the component closes.
					this.Context.InvalidateSelectedFolder();
					return true;
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
			return false;
		}

        #endregion
    }
}

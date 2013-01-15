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

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public interface IPerformingWorkflowItemToolContext : IWorkflowItemToolContext<ModalityWorklistItemSummary>
    {
    }

    public interface IPerformingWorkflowFolderToolContext : IWorkflowFolderToolContext
    {
    }

	public abstract class PerformingWorkflowFolderSystemBase<TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint>
		: WorklistFolderSystem<ModalityWorklistItemSummary, TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint, IModalityWorkflowService>
		where TFolderExtensionPoint : ExtensionPoint<IWorklistFolder>, new()
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
    {
		class PerformingWorkflowItemToolContext : WorkflowItemToolContext, IPerformingWorkflowItemToolContext
        {
            public PerformingWorkflowItemToolContext(WorkflowFolderSystem owner)
				:base(owner)
            {
            }
        }

        class PerformingWorkflowFolderToolContext : WorkflowFolderToolContext, IPerformingWorkflowFolderToolContext
        {
            public PerformingWorkflowFolderToolContext(WorkflowFolderSystem owner)
				:base(owner)
            {
            }
        }


        protected PerformingWorkflowFolderSystemBase(string title)
            : base(title)
		{
        }

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new PerformingWorkflowFolderToolContext(this);
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new PerformingWorkflowItemToolContext(this);
		}
    }
}

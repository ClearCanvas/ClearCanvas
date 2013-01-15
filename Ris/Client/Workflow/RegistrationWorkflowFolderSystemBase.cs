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
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
    public interface IRegistrationWorkflowItemToolContext : IWorkflowItemToolContext<RegistrationWorklistItemSummary>
    {
    }

    public interface IRegistrationWorkflowFolderToolContext : IWorkflowFolderToolContext
    {
    }

	public abstract class RegistrationWorkflowFolderSystemBase<TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint>
		: WorklistFolderSystem<RegistrationWorklistItemSummary, TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint, IRegistrationWorkflowService>
		where TFolderExtensionPoint : ExtensionPoint<IWorklistFolder>, new()
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
	{
		class RegistrationWorkflowItemToolContext : WorkflowItemToolContext, IRegistrationWorkflowItemToolContext
        {
            public RegistrationWorkflowItemToolContext(WorkflowFolderSystem owner)
				:base(owner)
            {
            }
        }

		class RegistrationWorkflowFolderToolContext : WorkflowFolderToolContext, IRegistrationWorkflowFolderToolContext
        {
            public RegistrationWorkflowFolderToolContext(WorkflowFolderSystem owner)
				:base(owner)
            {
            }
        }


        protected RegistrationWorkflowFolderSystemBase(string title)
            : base(title)
        {
        }

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new RegistrationWorkflowFolderToolContext(this);
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new RegistrationWorkflowItemToolContext(this);
		}
    }
}
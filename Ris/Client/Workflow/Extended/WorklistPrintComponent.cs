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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	/// <summary>
	/// Extension point for views onto <see cref="WorklistPrintComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class WorklistPrintComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// WorklistPrintComponent class.
	/// </summary>
	[AssociateView(typeof(WorklistPrintComponentViewExtensionPoint))]
	public class WorklistPrintComponent : ApplicationComponent
	{
		private WorklistPrintViewComponent _worklistPrintPreviewComponent;
		private ChildComponentHost _worklistPrintPreviewComponentHost;

		private readonly WorklistPrintViewComponent.PrintContext _printContext;

		public WorklistPrintComponent(string folderSystemName, string folderName, string folderDescription, int totalCount, List<object> items)
		{
			_printContext = new WorklistPrintViewComponent.PrintContext(folderSystemName, folderName, folderDescription, totalCount, items);
		}

		public override void Start()
		{
			_worklistPrintPreviewComponent = new WorklistPrintViewComponent(_printContext);
			_worklistPrintPreviewComponentHost = new ChildComponentHost(this.Host, _worklistPrintPreviewComponent);
			_worklistPrintPreviewComponentHost.StartComponent();

			base.Start();
		}

		public ApplicationComponentHost WorklistPrintPreviewComponentHost
		{
			get { return _worklistPrintPreviewComponentHost; }
		}

		public void Print()
		{
			if (DialogBoxAction.No == this.Host.DesktopWindow.ShowMessageBox(SR.MessagePrintWorklist, MessageBoxActions.YesNo))
				return;

			// print the rendered document
			_worklistPrintPreviewComponent.PrintDocument();

			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		public void Close()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}
	}
}

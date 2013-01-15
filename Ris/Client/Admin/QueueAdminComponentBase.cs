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
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Admin
{
	public interface IQueueItemToolContext<TQueueItemDetail> : IToolContext
	{
		IDesktopWindow DesktopWindow { get; }
		ClickHandlerDelegate DefaultAction { get; set; }

		TQueueItemDetail SelectedQueueItem { get; }
		event EventHandler SelectedQueueItemChanged;

		void Refresh();
	}

	public abstract class QueueAdminComponentBase<TQueueItemSummary, TQueueItemDetail> : ApplicationComponent
		where TQueueItemSummary : DataContractBase
		where TQueueItemDetail : DataContractBase
	{
		protected class QueueItemPreviewComponent : DHtmlComponent
		{
			private readonly QueueAdminComponentBase<TQueueItemSummary, TQueueItemDetail> _owner;

			public QueueItemPreviewComponent(QueueAdminComponentBase<TQueueItemSummary, TQueueItemDetail> owner)
			{
				_owner = owner;
			}

			public override void Start()
			{
				_owner.SelectedHL7QueueItemChanged += Refresh;
				SetUrl(_owner.PreviewPageUrl);
				base.Start();
			}

			public override void Stop()
			{
				_owner.SelectedHL7QueueItemChanged -= Refresh;
				base.Stop();
			}

			private void Refresh(object sender, EventArgs e)
			{
				NotifyAllPropertiesChanged();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _owner.SelectedWorkQueueItem;
			}
		}

		protected class QueueItemToolContext : ToolContext, IQueueItemToolContext<TQueueItemDetail>
		{
			private readonly QueueAdminComponentBase<TQueueItemSummary, TQueueItemDetail> _component;

			public QueueItemToolContext(QueueAdminComponentBase<TQueueItemSummary, TQueueItemDetail> component)
			{
				_component = component;
			}

			#region IHL7QueueToolContext Members

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public ClickHandlerDelegate DefaultAction
			{
				get { return _component._defaultAction; }
				set { _component._defaultAction = value; }
			}

			public TQueueItemDetail SelectedQueueItem
			{
				get { return _component.SelectedWorkQueueItem; }
			}

			public event EventHandler SelectedQueueItemChanged
			{
				add { _component.SelectedHL7QueueItemChanged += value; }
				remove { _component.SelectedHL7QueueItemChanged -= value; }
			}

			public void Refresh()
			{
				_component.Refresh();
			}

			#endregion
		}

		class DummyItem
		{
			private readonly string _displayString;

			public DummyItem(string displayString)
			{
				_displayString = displayString;
			}

			public override string ToString()
			{
				return _displayString;
			}
		}

		private IPagingController<TQueueItemSummary> _pagingController;
		private PagingActionModel<TQueueItemSummary> _pagingActionHandler;

		private TQueueItemDetail _selectedWorkQueueItem;
		private event EventHandler _selectedWorkQueueItemChanged;

		private readonly Table<TQueueItemSummary> _queue;
		private readonly ToolSet _toolSet;
		private ClickHandlerDelegate _defaultAction;

		private DHtmlComponent _previewComponent;
		private ApplicationComponentHost _previewComponentHost;

		private static readonly object _nullFilterItem = new DummyItem(SR.DummyItemNone);
		private static readonly int _pageSize = 50;

		protected QueueAdminComponentBase(Table<TQueueItemSummary> queue, IExtensionPoint extensionPoint)
		{
			_queue = queue;
			_toolSet = new ToolSet(extensionPoint, new QueueItemToolContext(this));
		}

		public abstract string PreviewPageUrl { get; }
		public abstract void InitialiseFormData();
		public abstract void Refresh();
		public abstract IList<TQueueItemSummary> GetSummaryItemsPage(int firstRow, int maxRows);
		public abstract TQueueItemDetail GetQueueItemDetail(TQueueItemSummary queueItemSummary);

		public override void Start()
		{
			InitialisePaging();
			InitialiseFormData();

			_previewComponent = new QueueItemPreviewComponent(this);
			_previewComponentHost = new ChildComponentHost(this.Host, _previewComponent);
			_previewComponentHost.StartComponent();

			base.Start();
		}

		public override void Stop()
		{
			if (_previewComponentHost != null)
			{
				_previewComponentHost.StopComponent();
				_previewComponentHost = null;
			}

			base.Stop();
		}

		private void InitialisePaging()
		{
			_pagingController = new PagingController<TQueueItemSummary>(
				_pageSize, 
				delegate(int firstrow, int maxrows, Action<IList<TQueueItemSummary>> resulthandler)
				{
					_queue.Items.Clear();

					IList<TQueueItemSummary> hl7QueueItems = null;
					try
					{
						hl7QueueItems = GetSummaryItemsPage(firstrow, maxrows);
					}
					catch (Exception e)
					{
						ExceptionHandler.Report(e, Host.DesktopWindow);
					}

					resulthandler(hl7QueueItems);
				});
			_pagingController.PageChanged += PagingControllerPageChangedEventHandler;

			_pagingActionHandler = new PagingActionModel<TQueueItemSummary>(_pagingController, Host.DesktopWindow);
		}


		private void PagingControllerPageChangedEventHandler(object sender, PageChangedEventArgs<TQueueItemSummary> e)
		{
			_queue.Items.AddRange(e.Items);
		}

		protected void GetFirstPage()
		{
			_pagingController.GetFirst();
		}

		public object NullFilterItem
		{
			get { return _nullFilterItem; }
		}

		public Table<TQueueItemSummary> Queue
		{
			get { return _queue; }
		}

		public ApplicationComponentHost PreviewComponentHost
		{
			get { return _previewComponentHost; }
		}

		public ActionModelNode MenuModel
		{
			get
			{
				return ActionModelRoot.CreateModel(this.GetType().FullName, "workqueue-contextmenu", _toolSet.Actions);
			}
		}

		public ActionModelNode ToolbarModel
		{
			get
			{
				ActionModelNode node = ActionModelRoot.CreateModel(this.GetType().FullName, "workqueue-toolbar", _toolSet.Actions);
				node.Merge(_pagingActionHandler);
				return node;
			}
		}

		public void SetSelectedItem(ISelection selection)
		{
			var selectedSummaryTableItem = selection.Item as TQueueItemSummary;

			_selectedWorkQueueItem = selectedSummaryTableItem == null 
				? null 
				: GetQueueItemDetail(selectedSummaryTableItem);

			EventsHelper.Fire(_selectedWorkQueueItemChanged, this, EventArgs.Empty);
		}

		#region QueueItemToolContext Helpers

		public TQueueItemDetail SelectedWorkQueueItem
		{
			get { return _selectedWorkQueueItem; }
		}

		public event EventHandler SelectedHL7QueueItemChanged
		{
			add { _selectedWorkQueueItemChanged += value; }
			remove { _selectedWorkQueueItemChanged -= value; }
		}

		public override IActionSet ExportedActions
		{
			get { return _toolSet.Actions; }
		}

		#endregion
	}
}
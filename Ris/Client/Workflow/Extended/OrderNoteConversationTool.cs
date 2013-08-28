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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Extended.Common.OrderNotes;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	/// <summary>
	/// Base class for tools which open an <see cref="OrderNoteConversationComponent"/>
	/// </summary>
	/// <typeparam name="TSummaryItem">A <see cref="DataContractBase"/> that can provide an OrderRef and an appropriate value for the component's title</typeparam>
	/// <typeparam name="TToolContext">Must be <see cref="IWorkflowItemToolContext{TSummaryItem}"/></typeparam>
	public abstract class OrderNoteConversationToolBase<TSummaryItem, TToolContext> : Tool<TToolContext> 
		where TSummaryItem : DataContractBase
		where TToolContext : IWorkflowItemToolContext<TSummaryItem>
	{
		/// <summary>
		/// A title for the <see cref="OrderNoteConversationComponent"/> from the derived class' <see cref="TSummaryItem"/>
		/// </summary>
		protected abstract string TitleContextDescription { get; }

		/// <summary>
		/// An <see cref="EntityRef"/> for an Order from the derived class' <see cref="TSummaryItem"/>.
		/// </summary>
		protected abstract EntityRef OrderRef { get; }

		/// <summary>
		/// Specifies an enumeration of <see cref="OrderNoteCategory"/> which will be displayed in the <see cref="OrderNoteConversationComponent"/>
		/// </summary>
		protected abstract string[] OrderNoteCategories { get; }

		/// <summary>
		/// The first <see cref="TSummaryItem"/> in the current <see cref="TToolContext"/>
		/// </summary>
		protected TSummaryItem SummaryItem
		{
			get
			{
				var context = (IWorkflowItemToolContext<TSummaryItem>)this.ContextBase;
				return CollectionUtils.FirstElement(context.SelectedItems);
			}
		}

		public virtual bool Enabled
		{
			get
			{
				var items = ((IWorkflowItemToolContext<TSummaryItem>) this.ContextBase).SelectedItems;
				return items != null && items.Count == 1;
			}
		}

		public event EventHandler EnabledChanged
		{
			add { ((IWorkflowItemToolContext<TSummaryItem>)this.ContextBase).SelectionChanged += value; }
			remove { ((IWorkflowItemToolContext<TSummaryItem>)this.ContextBase).SelectionChanged -= value; }
		}

		/// <summary>
		/// Opens an <see cref="OrderNoteConversationComponent"/>
		/// </summary>
		public void Open()
		{
			if(this.OrderRef == null)
				return;

			try
			{
				var component = new OrderNoteConversationComponent(this.OrderRef, this.OrderNoteCategories, this.TemplatesXml, this.SoftKeysXml);
				var dialog = ApplicationComponent.LaunchAsWorkspaceDialog(this.Context.DesktopWindow,
					component,
					GetTitle());
				dialog.Closed += delegate { OnDialogClosed(component.ExitCode); };
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		protected virtual void OnDialogClosed(ApplicationComponentExitCode exitCode)
		{
		}

		protected virtual string TemplatesXml
		{
			get { return null; }
		}

		protected virtual string SoftKeysXml
		{
			get { return null; }
		}

		private string GetTitle()
		{
			var sb = new StringBuilder();

			sb.Append(
				CollectionUtils.Reduce(
					this.OrderNoteCategories, 
					"",
					delegate(string categoryKey, string memo)
					{
						var category = OrderNoteCategory.FromKey(categoryKey);
						return memo + (category != null ? category.DisplayValue : "");
					}));
			sb.Append(this.TitleContextDescription);

			return sb.ToString();
		}
	}

	/// <summary>
	/// Tool which opens an <see cref="OrderNoteConversationComponent"/> from an <see cref="IOrderNoteboxItemToolContext"/>
	/// </summary>
	[MenuAction("pd", "folderexplorer-items-contextmenu/Open Conversation", "Open")]
	[ButtonAction("pd", "folderexplorer-items-toolbar/Open Conversation", "Open")]
	[Tooltip("pd", "Review/reply to the selected note.")]
	[EnabledStateObserver("pd", "Enabled", "EnabledChanged")]
	[IconSet("pd", "Icons.OrderNoteConversationSmall.png", "Icons.OrderNoteConversationMedium.png", "Icons.OrderNoteConversationLarge.png")]
	[ExtensionOf(typeof(OrderNoteboxItemToolExtensionPoint))]
	public class OrderNoteConversationTool : OrderNoteConversationToolBase<OrderNoteboxItemSummary, IOrderNoteboxItemToolContext>
	{
        public override void Initialize()
        {
            base.Initialize();

			this.Context.RegisterDoubleClickHandler(
                (IClickAction) CollectionUtils.SelectFirst(this.Actions, a => a is IClickAction && a.ActionID.EndsWith("pd")));
        }

		protected override EntityRef OrderRef
		{
			get { return this.SummaryItem.OrderRef; }
		}

		protected override string TitleContextDescription
		{
			get
			{
				return string.Format(SR.FormatTitleContextDescriptionOrderNoteConversation,
					PersonNameFormat.Format(this.SummaryItem.PatientName),
					MrnFormat.Format(this.SummaryItem.Mrn),
					AccessionFormat.Format(this.SummaryItem.AccessionNumber));
			}
		}

		protected override string[] OrderNoteCategories
		{
			get { return new [] { this.SummaryItem.Category }; }
		}

		protected override void OnDialogClosed(ApplicationComponentExitCode exitCode)
		{
			// invalidate the sent items folder in case any notes were posted
			this.Context.InvalidateFolders(typeof(SentItemsFolder));
			this.Context.InvalidateFolders(typeof(PersonalInboxFolder));
			this.Context.InvalidateFolders(typeof(GroupInboxFolder));

			base.OnDialogClosed(exitCode);
		}
	}
}
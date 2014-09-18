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
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="AttachedDocumentPreviewComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class AttachedDocumentPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[ExtensionPoint]
	public class AttachedDocumentToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface IAttachedDocumentToolContext : IToolContext
	{
		AttachmentSite Site { get; }
		event EventHandler SelectedAttachmentChanged;
		AttachmentSummary SelectedAttachment { get; }
		AttachedDocumentSummary SelectedDocument { get; }

		void AddAttachment(AttachedDocumentSummary document, EnumValueInfo category);
		void RemoveSelectedAttachment();
		void OpenSelectedAttachment();
		bool IsReadonly { get; }

		IDesktopWindow DesktopWindow { get; }
	}

	public enum AttachmentSite
	{
		Patient,
		Order
	}

	/// <summary>
	/// AttachedDocumentPreviewComponent class
	/// </summary>
	[AssociateView(typeof (AttachedDocumentPreviewComponentViewExtensionPoint))]
	public class AttachedDocumentPreviewComponent : ApplicationComponent
	{
		private class AttachedDocumentToolContext : ToolContext, IAttachedDocumentToolContext
		{
			private readonly AttachedDocumentPreviewComponent _component;

			internal AttachedDocumentToolContext(AttachedDocumentPreviewComponent component)
			{
				_component = component;
			}

			#region IAttachedDocumentToolContext Members

			public AttachmentSite Site
			{
				get { return _component._site; }
			}

			public event EventHandler SelectedAttachmentChanged
			{
				add { _component.SelectedDocumentChanged += value; }
				remove { _component.SelectedDocumentChanged -= value; }
			}

			public AttachmentSummary SelectedAttachment
			{
				get { return _component.SelectedAttachment; }
			}

			public AttachedDocumentSummary SelectedDocument
			{
				get { return _component.SelectedDocument; }
			}

			public void AddAttachment(AttachedDocumentSummary document, EnumValueInfo category)
			{
				_component.AddAttachment(document, category);
			}

			public void RemoveSelectedAttachment()
			{
				_component.RemoveSelectedAttachment();
			}

			public void OpenSelectedAttachment()
			{
				_component.OpenAttachment();
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public bool IsReadonly
			{
				get { return _component.Readonly; }
			}

			#endregion
		}

		// Summary component members
		private AttachmentSite _site;
		private AttachmentSummary _selectedAttachment;
		private AttachmentSummary _initialSelection;
		private event EventHandler _selectedDocumentChanged;

		private readonly AttachmentSummaryTable _patientAttachmentTable;

		private ToolSet _toolSet;
		private readonly bool _readonly;

		private EntityRef _patientProfileRef;
		private EntityRef _orderRef;

		/// <summary>
		/// Constructor to show/hide the summary section
		/// </summary>
		/// <param name="readonly">True to show the summary toolbar, false to hide it</param>
		/// <param name="site">Set the component attachment mode</param>
		public AttachedDocumentPreviewComponent(bool @readonly, AttachmentSite site)
		{
			_readonly = @readonly;
			_site = site;

			_patientAttachmentTable = new AttachmentSummaryTable();
		}

		public override void Start()
		{
			_toolSet = new ToolSet(new AttachedDocumentToolExtensionPoint(), new AttachedDocumentToolContext(this));

			if (_site == AttachmentSite.Patient)
				LoadPatientAttachments();
			else
				LoadOrderAttachments();

			base.Start();
		}

		public override void Stop()
		{
			_toolSet.Dispose();

			base.Stop();
		}

		#region Events

		public event EventHandler SelectedDocumentChanged
		{
			add { _selectedDocumentChanged += value; }
			remove { _selectedDocumentChanged -= value; }
		}

		#endregion

		#region Presentation Models

		public bool Readonly
		{
			get { return _readonly; }
		}

		/// <summary>
		/// Gets and sets the patient owner.
		/// </summary>
		public EntityRef PatientProfileRef
		{
			get { return _patientProfileRef; }
			set
			{
				if (_patientProfileRef == value)
					return;

				_site = AttachmentSite.Patient;
				_patientProfileRef = value;

				if (this.IsStarted)
					LoadPatientAttachments();
			}
		}

		/// <summary>
		/// Gets and sets the order owner.
		/// </summary>
		public EntityRef OrderRef
		{
			get { return _orderRef; }
			set
			{
				if (_orderRef == value)
					return;

				_site = AttachmentSite.Order;
				_orderRef = value;
				LoadOrderAttachments();
			}
		}

		public IList<AttachmentSummary> Attachments
		{
			get { return _patientAttachmentTable.Items; }
			set
			{
				_patientAttachmentTable.Items.Clear();
				_patientAttachmentTable.Items.AddRange(value);
			}
		}

		public ITable AttachmentTable
		{
			get { return _patientAttachmentTable; }
		}

		public ActionModelRoot AttachmentActionModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, "attached-document-items", _toolSet.Actions); }
		}

		public override IActionSet ExportedActions
		{
			get { return _toolSet.Actions; }
		}

		public ISelection Selection
		{
			get { return new Selection(_selectedAttachment); }
			set
			{
				var newSelection = (AttachmentSummary) value.Item;
				if (_selectedAttachment != newSelection)
				{
					_selectedAttachment = newSelection;
					NotifyPropertyChanged("Selection");
					EventsHelper.Fire(_selectedDocumentChanged, this, EventArgs.Empty);
				}
			}
		}

		public void DoubleClickedSelectedAttachment()
		{
			OpenAttachment();
		}

		public void OnControlLoad()
		{
			if (_initialSelection != null)
				this.Selection = new Selection(_initialSelection);
		}

		public void SetInitialSelection(AttachmentSummary attachmentSummary)
		{
			_initialSelection = attachmentSummary;
		}

		#endregion

		private AttachmentSummary SelectedAttachment
		{
			get { return _selectedAttachment; }
		}

		private AttachedDocumentSummary SelectedDocument
		{
			get { return SelectedAttachment == null ? null : SelectedAttachment.Document; }
		}

		public void AddAttachment(AttachedDocumentSummary document, EnumValueInfo category)
		{
			var attachment = new AttachmentSummary(category, null, Platform.Time, document);
			this.AttachmentTable.Items.Add(attachment);
			this.Modified = true;
		}

		private void RemoveSelectedAttachment()
		{
			if (_selectedAttachment == null)
				return;

			this.AttachmentTable.Items.Remove(_selectedAttachment);
			this.Modified = true;
		}

		private void OpenAttachment()
		{
			var document = this.SelectedDocument;
			if (document == null)
				return;

			try
			{
				BlockingOperation.Run(() =>
				                      {
					                      var localUri = AttachedDocument.DownloadFile(document);
					                      Process.Start(localUri);
				                      });
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.ExceptionFailedToDisplayDocument, Host.DesktopWindow);
			}
		}

		private void LoadPatientAttachments()
		{
			if (_patientProfileRef == null)
				return;

			Async.Request(
				this,
				(IBrowsePatientDataService service) =>
				{
					var request = new GetDataRequest
					              {
						              GetPatientProfileDetailRequest = new GetPatientProfileDetailRequest
						                                               {
							                                               PatientProfileRef = _patientProfileRef,
							                                               IncludeAttachments = true
						                                               }
					              };
					return service.GetData(request);
				},
				response =>
				{
					this.Attachments = response.GetPatientProfileDetailResponse.PatientProfile.Attachments;
					if (this.Attachments.Count > 0)
						this.SetInitialSelection(this.Attachments[0]);
				});
		}

		private void LoadOrderAttachments()
		{
			if (_orderRef == null)
				return;

			Async.Request(
				this,
				(IBrowsePatientDataService service) =>
				{
					var request = new GetDataRequest
					              {
						              GetOrderDetailRequest = new GetOrderDetailRequest
						                                      {
							                                      OrderRef = _orderRef,
							                                      IncludeAttachments = true
						                                      }
					              };
					return service.GetData(request);
				},
				response =>
				{
					this.Attachments = response.GetOrderDetailResponse.Order.Attachments;
					if (this.Attachments.Count > 0)
						this.SetInitialSelection(this.Attachments[0]);
				});
		}
	}
}
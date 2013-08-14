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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public interface IReportEditorComponent : IApplicationComponent
	{
		ICannedTextLookupHandler CannedTextLookupHandler { get; }
		bool PreviewVisible { get; }
		ApplicationComponentHost ReportPreviewHost { get; }
		string EditorText { get; set; }
	}

	public abstract class ReportEditorComponentBase<TReportEditorContext, TCloseReason> : ApplicationComponent, IReportEditorComponent
		where TReportEditorContext : IReportEditorContextBase<TCloseReason>
	{

		#region PreviewComponent

		/// <summary>
		/// Provides the preview of existing report parts.
		/// </summary>
		public class PreviewComponent : DHtmlComponent
		{
			private readonly ReportEditorComponentBase<TReportEditorContext, TCloseReason> _owner;

			public PreviewComponent(ReportEditorComponentBase<TReportEditorContext, TCloseReason> owner)
			{
				_owner = owner;
			}

			public void Refresh()
			{
				NotifyAllPropertiesChanged();
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _owner._context.Report;
			}
		}

		#endregion

		private readonly PreviewComponent _previewComponent;
		private ChildComponentHost _previewHost;

		private ReportContent _reportContent;

		private ICannedTextLookupHandler _cannedTextLookupHandler;

		private readonly TReportEditorContext _context;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected ReportEditorComponentBase(TReportEditorContext context)
		{
			_context = context;
			_previewComponent = new PreviewComponent(this);
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			_cannedTextLookupHandler = new CannedTextLookupHandler(this.Host.DesktopWindow);

			LoadOrCreateReportContent();

			_previewComponent.SetUrl(this.PreviewUrl);
			_previewHost = new ChildComponentHost(this.Host, _previewComponent);
			_previewHost.StartComponent();

			_context.WorklistItemChanged += WorklistItemChangedEventHandler;

			base.Start();
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			if (_previewHost != null)
			{
				_previewHost.StopComponent();
				_previewHost = null;
			}

			base.Stop();
		}

		private void WorklistItemChangedEventHandler(object sender, EventArgs e)
		{
			this.Modified = false;	// clear modified flag

			if (_context.WorklistItem != null)
			{
				LoadOrCreateReportContent();
			}
			else
			{
				_reportContent = null;
			}
			_previewComponent.Refresh();
			NotifyPropertyChanged("EditorText");
			NotifyPropertyChanged("PreviewVisible");
		}

		protected virtual void LoadOrCreateReportContent()
		{
			if (!string.IsNullOrEmpty(_context.ReportContent))
			{
				// editing an existing report - just deserialize the content
				_reportContent = JsmlSerializer.Deserialize<ReportContent>(_context.ReportContent);
			}
			else
			{
				// create a new ReportContent object
				_reportContent = new ReportContent(null);

				// HACK: update the active ReportPart object with the structured report
				// (this is solely for the benefit of the Preview component, it does not have any affect on what is ultimately saved)
				if (_context.Report != null)
				{
					var activePart = _context.Report.GetPart(_context.ActiveReportPartIndex);
					activePart.Content = _reportContent.ToJsml();
				}
			}

			// Reset component validation since we just reloaded the report content
			ShowValidation(false);
		}

		#region ITranscriptionEditor Members

		public IApplicationComponent GetComponent()
		{
			return this;
		}

		public virtual bool Save(TCloseReason reason)
		{
			_context.ReportContent = _reportContent.ToJsml();
			return true;
		}

		#endregion

		#region Presentation Model

		public ICannedTextLookupHandler CannedTextLookupHandler
		{
			get { return _cannedTextLookupHandler; }
		}

		public virtual bool PreviewVisible
		{
			get { return _context.IsAddendum; }
		}

		public ApplicationComponentHost ReportPreviewHost
		{
			get { return _previewHost; }
		}

		[ValidateNotNull]
		public string EditorText
		{
			get { return _reportContent != null ? _reportContent.ReportText : ""; }
			set
			{
				if (!Equals(value, _reportContent.ReportText))
				{
					_reportContent.ReportText = value;
					this.Modified = true;
				}
			}
		}

		#endregion

		protected abstract string PreviewUrl { get; }

		protected TReportEditorContext Context
		{
			get { return _context; }
		}

		private bool IsAddendum
		{
			get { return _context.ActiveReportPartIndex > 0; }
		}
	}
}
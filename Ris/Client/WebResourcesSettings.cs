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

using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Provides application settings for all core RIS web content URLss
	/// </summary>
	/// <remarks>
	/// This code is adapted from the Visual Studio generated template code;  the generated code has been removed from the project.  Additional 
	/// settings need to be manually added to this class.
	/// </remarks>
	[SettingsGroupDescription("Configures web content URLs.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	public sealed class WebResourcesSettings : global::System.Configuration.ApplicationSettingsBase
	{
		private static WebResourcesSettings defaultInstance = ((WebResourcesSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new WebResourcesSettings())));

		public WebResourcesSettings()
		{
		}

		public static WebResourcesSettings Default
		{
			get
			{
				return defaultInstance;
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("http://localhost/RIS")]
		[global::System.Configuration.SettingsDescription("Provides base URL for HtmlApplicationComponent web resources.  URL should specify protocol (i.e. http://server, file:///C:/directory, etc.)")]
		public string BaseUrl
		{
			get
			{
				return ((string)(this["BaseUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("banner.htm")]
		public string BannerPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BannerPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("downtime-form-template.htm")]
		public string DowntimeFormTemplatePageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["DowntimeFormTemplatePageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("PatientProfileDiff.html")]
		public string PatientReconciliationPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["PatientReconciliationPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("work-queue-preview.htm")]
		public string WorkQueuePreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["WorkQueuePreviewPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("order-note-preview.htm")]
		public string OrderNotePreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["OrderNotePreviewPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("worklist-summary.htm")]
		public string WorklistSummaryPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["WorklistSummaryPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("worklist-print-preview.htm")]
		public string WorklistPrintPreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["WorklistPrintPreviewPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("attached-document-preview.htm")]
		public string AttachedDocumentPreviewUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["AttachedDocumentPreviewUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("external-practitioner-overview.htm")]
		public string ExternalPractitionerOverviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ExternalPractitionerOverviewPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("merged-order-detail.htm")]
		public string MergedOrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["MergedOrderDetailPageUrl"]));
			}
		}

		#region Home Page Preview settings

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/registration.htm")]
		public string RegistrationFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["RegistrationFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/booking.htm")]
		public string BookingFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BookingFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/performing.htm")]
		public string PerformingFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["PerformingFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/protocolling.htm")]
		public string ProtocollingFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ProtocollingFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/reporting.htm")]
		public string ReportingFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ReportingFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/transcription.htm")]
		public string TranscriptionFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["TranscriptionFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/emergency.htm")]
		public string EmergencyFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["EmergencyFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/order-notes.htm")]
		public string OrderNoteboxFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["OrderNoteboxFolderSystemUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("preview/external-practitioner.htm")]
		public string ExternalPractitionerFolderSystemUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ExternalPractitionerFolderSystemUrl"]));
			}
		}

		#endregion

		#region Patient Biography settings

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("biography/allergy-detail.htm")]
		public string BiographyAllergyDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BiographyAllergyDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("biography/order-detail.htm")]
		public string BiographyOrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BiographyOrderDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("biography/visit-detail.htm")]
		public string BiographyVisitDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BiographyVisitDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("biography/patient-profile-detail.htm")]
		public string BiographyPatientProfilePageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BiographyPatientProfilePageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("biography/report-detail.htm")]
		public string BiographyReportDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["BiographyReportDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("biography/workflow-history.htm")]
		public string WorkflowHistoryPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["WorkflowHistoryPageUrl"]));
			}
		}

		#endregion


		#region Reporting Settings

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/order-detail.htm")]
		public string ReportingOrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ReportingOrderDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/default-report-editor.htm")]
		public string DefaultReportEditorPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["DefaultReportEditorPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/default-addendum-editor.htm")]
		public string DefaultAddendumEditorPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["DefaultAddendumEditorPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/report-preview.htm")]
		public string ReportPreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ReportPreviewPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/prior-report-preview.htm")]
		public string PriorReportPreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["PriorReportPreviewPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("reporting/print-report-preview.htm")]
		public string PrintReportPreviewUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["PrintReportPreviewUrl"]));
			}
		}

		#endregion

		#region Protocolling Settings

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("protocolling/order-detail.htm")]
		public string ProtocollingOrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["ProtocollingOrderDetailPageUrl"]));
			}
		}

		#endregion

		#region Performing Settings

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("performing/order-detail.htm")]
		public string PerformingOrderDetailPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["PerformingOrderDetailPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("performing/order-additional-info.htm")]
		public string OrderAdditionalInfoPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["OrderAdditionalInfoPageUrl"]));
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("forms/performing/mpps.htm")]
		public string DetailsPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["DetailsPageUrl"]));
			}
		}

		#endregion

		#region Transcription Settings

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("transcription/report-preview.htm")]
		public string TranscriptionPreviewPageUrl
		{
			get
			{
				return WebResourceAbsoluteUrlHelper.FromRelative((string)(this["TranscriptionPreviewPageUrl"]));
			}
		}
		#endregion
	}

	public static class WebResourceAbsoluteUrlHelper
	{
		private static readonly char[] _slash = new[] { '/' };

		public static string FromRelative(string relativeUrl)
		{
			return WebResourcesSettings.Default.BaseUrl.TrimEnd(_slash) + '/' + relativeUrl.TrimStart(_slash);
		}
	}
}

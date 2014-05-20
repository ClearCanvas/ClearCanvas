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
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	[ExtensionPoint]
	public class DHtmlComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	/// <summary>
	/// Base class for components that display an HTML page.
	/// </summary>
	[AssociateView(typeof(DHtmlComponentViewExtensionPoint))]
	public class DHtmlComponent : ApplicationComponent
	{

		#region DHTML script callback class

		/// <summary>
		/// The script callback is an object that is made available to the web browser so that
		/// the javascript code can invoke methods on the host.
		/// </summary>
		[ComVisible(true)]  // must be COM-visible
		public class DHtmlScriptCallback
		{
			private readonly DHtmlComponent _component;
			private readonly HtmlActionModelRenderer _actionModelRenderer;

			public DHtmlScriptCallback(DHtmlComponent component)
			{
				_component = component;
				_actionModelRenderer = new HtmlActionModelRenderer();
			}

			/// <summary>
			/// Handle uncaught script errors from the browser.
			/// </summary>
			/// <param name="message"></param>
			/// <param name="url"></param>
			/// <param name="lineNumber"></param>
			public void OnScriptError(string message, string url, int lineNumber)
			{
				Platform.Log(LogLevel.Error, "WebBrowser JScript Error: {0} ({1} line {2})", message, url, lineNumber);
			}

			/// <summary>
			/// Surrogate for the browser's window.alert method.
			/// </summary>
			/// <param name="message"></param>
			public void Alert(string message)
			{
				_component.Host.ShowMessageBox(message, MessageBoxActions.Ok);
			}

			/// <summary>
			/// Surrogate for the browser's window.confirm method.
			/// </summary>
			/// <param name="message"></param>
			/// <param name="type"></param>
			/// <returns></returns>
			public bool Confirm(string message, string type)
			{
				if (string.IsNullOrEmpty(type))
					type = "okcancel";
				type = type.ToLower();

				if (type == MessageBoxActions.OkCancel.ToString().ToLower())
				{
					return _component.Host.ShowMessageBox(message, MessageBoxActions.OkCancel) == DialogBoxAction.Ok;
				}

				if (type == MessageBoxActions.YesNo.ToString().ToLower())
				{
					return _component.Host.ShowMessageBox(message, MessageBoxActions.YesNo) == DialogBoxAction.Yes;
				}

				// invalid type parameter
				throw new NotSupportedException("Type must be YesNo or OkCancel");
			}

			/// <summary>
			/// Gets or sets a value indicating whether content has been modified by the user.
			/// </summary>
			public bool Modified
			{
				get { return _component.Modified; }
				set { _component.Modified = value; }
			}

			/// <summary>
			/// Gets the date format string.
			/// </summary>
			public string DateFormat
			{
				get { return Format.DateFormat; }
			}

			/// <summary>
			/// Gets the time format string.
			/// </summary>
			public string TimeFormat
			{
				get { return Format.TimeFormat; }
			}

			/// <summary>
			/// Gets the date-time format string.
			/// </summary>
			public string DateTimeFormat
			{
				get { return Format.DateTimeFormat; }
			}

			/// <summary>
			/// Formats the specified date object according to the <see cref="DateFormat"/> value.
			/// </summary>
			/// <param name="isoDateString"></param>
			/// <returns></returns>
			public string FormatDate(string isoDateString)
			{
				var dt = DateTimeUtils.ParseISO(isoDateString);
				return dt == null ? "" : Format.Date(dt);
			}

			/// <summary>
			/// Formats the specified date object according to the <see cref="TimeFormat"/> value.
			/// </summary>
			/// <param name="isoDateString"></param>
			/// <returns></returns>
			public string FormatTime(string isoDateString)
			{
				var dt = DateTimeUtils.ParseISO(isoDateString);
				return dt == null ? "" : Format.Time(dt);
			}

			/// <summary>
			/// Formats the specified date object according to the <see cref="DateTimeFormat"/> value.
			/// </summary>
			/// <param name="isoDateString"></param>
			/// <returns></returns>
			public string FormatDateTime(string isoDateString)
			{
				var dt = DateTimeUtils.ParseISO(isoDateString);
				return dt == null ? "" : Format.DateTime(dt);
			}

			public string FormatRelativeTime(string isoTimeSpanString, string resolution)
			{
				int res;
				int.TryParse(resolution, out res);

				TimeSpan timeSpan;
				TimeSpan.TryParse(isoTimeSpanString, out timeSpan);

				if (res == 1440)
				{
					return new RelativeTimeInDays(timeSpan.Days).ToString();
				}

				var hours = timeSpan.Days * 24 + timeSpan.Hours;
				return new RelativeTimeInHours(hours).ToString();
			}

			public string FormatDescriptiveTime(string isoDateString)
			{
				if (isoDateString == null)
					return "";

				var input = DateTimeUtils.ParseISO(isoDateString);

				return Format.Date(input, true);
			}

			/// <summary>
			/// Formats the specified address (must be a JSML encoded <see cref="AddressDetail"/> object).
			/// </summary>
			/// <param name="jsml"></param>
			/// <returns></returns>
			public string FormatAddress(string jsml)
			{
				var addressDetail = JsmlSerializer.Deserialize<AddressDetail>(jsml);
				return addressDetail == null ? "" : AddressFormat.Format(addressDetail);
			}

			/// <summary>
			/// Formats the specified healthcard (must be a JSML encoded <see cref="HealthcardDetail"/> object).
			/// </summary>
			/// <param name="jsml"></param>
			/// <returns></returns>
			public string FormatHealthcard(string jsml)
			{
				var healthcardDetail = JsmlSerializer.Deserialize<HealthcardDetail>(jsml);
				return healthcardDetail == null ? "" : HealthcardFormat.Format(healthcardDetail);
			}

			/// <summary>
			/// Formats the specified MRN (must be a JSML encoded <see cref="CompositeIdentifierDetail"/> object).
			/// </summary>
			/// <param name="jsml"></param>
			/// <returns></returns>
			public string FormatMrn(string jsml)
			{
				var mrnDetail = JsmlSerializer.Deserialize<CompositeIdentifierDetail>(jsml);
				return mrnDetail == null ? "" : MrnFormat.Format(mrnDetail);
			}

			/// <summary>
			/// Formats the specified Visit # (must be a JSML encoded <see cref="CompositeIdentifierDetail"/> object).
			/// </summary>
			/// <param name="jsml"></param>
			/// <returns></returns>
			public string FormatVisitNumber(string jsml)
			{
				var visitNumberDetail = JsmlSerializer.Deserialize<CompositeIdentifierDetail>(jsml);
				return visitNumberDetail == null ? "" : VisitNumberFormat.Format(visitNumberDetail);
			}

			/// <summary>
			/// Formats the specified Accession #, which must be an unformatted accession number string.
			/// </summary>
			/// <param name="accessionString"></param>
			/// <returns></returns>
			public string FormatAccessionNumber(string accessionString)
			{
				return AccessionFormat.Format(accessionString);
			}

			/// <summary>
			/// Formats the specified Person Name (must be a JSML encoded <see cref="PersonNameDetail"/> object).
			/// </summary>
			/// <param name="jsml"></param>
			/// <returns></returns>
			public string FormatPersonName(string jsml)
			{
				var nameDetail = JsmlSerializer.Deserialize<PersonNameDetail>(jsml);
				return nameDetail == null ? "" : PersonNameFormat.Format(nameDetail);
			}

			/// <summary>
			/// Formats the specified Staff (must be a JSML encoded <see cref="StaffSummary"/> or <see cref="StaffDetail"/> object).
			/// </summary>
			/// <param name="jsml"></param>
			/// <returns></returns>
			public string FormatStaffNameAndRole(string jsml)
			{
				try
				{
					var staffSummary = JsmlSerializer.Deserialize<StaffSummary>(jsml);
					return staffSummary == null ? "" : StaffNameAndRoleFormat.Format(staffSummary);
				}
				catch (InvalidCastException)
				{
					var staffDetail = JsmlSerializer.Deserialize<StaffDetail>(jsml);
					return staffDetail == null ? "" : StaffNameAndRoleFormat.Format(staffDetail);
				}
			}

			/// <summary>
			/// Formats the specified Procedure (must be a JSML encoded <see cref="ProcedureSummary"/> or <see cref="ProcedureDetail"/> object).
			/// </summary>
			/// <param name="jsml"></param>
			/// <returns></returns>
			public string FormatProcedureName(string jsml)
			{
				try
				{
					var procedureSummary = JsmlSerializer.Deserialize<ProcedureSummary>(jsml);
					return procedureSummary == null ? "" : ProcedureFormat.Format(procedureSummary);
				}
				catch (InvalidCastException)
				{
					var procedureDetail = JsmlSerializer.Deserialize<ProcedureDetail>(jsml);
					return procedureDetail == null ? "" : ProcedureFormat.Format(procedureDetail);
				}
			}

			/// <summary>
			/// Formats the specified Telephone number (must be a JSML encoded <see cref="TelephoneDetail"/> object).
			/// </summary>
			/// <param name="jsml"></param>
			/// <returns></returns>
			public string FormatTelephone(string jsml)
			{
				var telephoneDetail = JsmlSerializer.Deserialize<TelephoneDetail>(jsml);
				return telephoneDetail == null ? "" : TelephoneFormat.Format(telephoneDetail);
			}

			/// <summary>
			/// Formats the specified Telephone number (must be a non-formatted 10-digit string).
			/// </summary>
			/// <param name="telephone"></param>
			/// <returns></returns>
			public string FormatStringTelephone(string telephone)
			{
				return TelephoneFormat.Format(telephone, TextFieldMasks.TelephoneNumberFullMask);
			}

			public string FormatOrderListItemProcedureName(string jsml)
			{
				var orderListItem = JsmlSerializer.Deserialize<OrderListItem>(jsml);
				return orderListItem == null ? "" : ProcedureFormat.Format(orderListItem);
			}

			/// <summary>
			/// Gets a proxy object for the specified service contract, which the script can use to call service operations.
			/// </summary>
			/// <param name="serviceContractName"></param>
			/// <returns></returns>
			public JsmlServiceProxy GetServiceProxy(string serviceContractName)
			{
				var proxy = new JsmlServiceProxy(serviceContractName, WebServicesSettings.Default.UseJsmlShimService);

				// subscribe to the proxy's async completion event, in order to route callbacks back to the script
				proxy.AsyncInvocationCompleted += _component.AsyncInvocationCompletedEventHandler;
				proxy.AsyncInvocationError += _component.AsyncInvocationErrorEventHandler;
				return proxy;
			}

			/// <summary>
			/// Not used.
			/// </summary>
			/// <param name="labelSearch"></param>
			/// <param name="actionLabel"></param>
			/// <returns></returns>
			public string GetActionHtml(string labelSearch, string actionLabel)
			{
				return _actionModelRenderer.GetHTML(_component.GetActionModel(), labelSearch, actionLabel);
			}

			/// <summary>
			/// Gets the URL of the specified attached document (must be a JSML encoded <see cref="AttachedDocumentSummary"/> object).
			/// </summary>
			/// <param name="jsml"></param>
			/// <returns></returns>
			public string GetAttachedDocumentUrl(string jsml)
			{
				var attachedDocumentSummary = JsmlSerializer.Deserialize<AttachedDocumentSummary>(jsml);
				return AttachedDocument.DownloadFile(attachedDocumentSummary);
			}

			/// <summary>
			/// Attempts to resolve the specified search string to a staff (returns a JSML encoded <see cref="StaffSummary"/>.
			/// </summary>
			/// <param name="search"></param>
			/// <returns></returns>
			public string ResolveStaffName(string search)
			{
				StaffSummary staff;
				var lookupHandler = new StaffLookupHandler(_component.Host.DesktopWindow);
				var resolved = lookupHandler.ResolveName(search, out staff);

				// bug #2896: the name may "resolve" to nothing, so we still need to check if staff actually has a value 
				if (!resolved || staff == null)
				{
					resolved = lookupHandler.ResolveNameInteractive(search, out staff);
				}
				return resolved ? JsmlSerializer.Serialize(staff, "staff") : null;
			}

			/// <summary>
			/// Attempts to resolve the specified search string to a staff (returns a JSML encoded <see cref="StaffSummary"/>.
			/// </summary>
			/// <param name="search"></param>
			/// <param name="jsmlStaffTypesFilter">JSML encoded string list of staff types codes to filter with.</param>
			/// <returns></returns>
			public string ResolveFilteredStaffName(string search, string jsmlStaffTypesFilter)
			{
				StaffSummary staff;
				var filter = JsmlSerializer.Deserialize<List<String>>(jsmlStaffTypesFilter);
				var lookupHandler = new StaffLookupHandler(_component.Host.DesktopWindow, filter.ToArray());
				var resolved = lookupHandler.ResolveName(search, out staff);

				// bug #2896: the name may "resolve" to nothing, so we still need to check if staff actually has a value 
				if (!resolved || staff == null)
				{
					resolved = lookupHandler.ResolveNameInteractive(search, out staff);
				}
				return resolved ? JsmlSerializer.Serialize(staff, "staff") : null;
			}

			/// <summary>
			/// Opens a pop up providing details on the specified practitioner
			/// (must be a JSML encoded <see cref="ExternalPractitionerSummary"/> object).
			/// </summary>
			/// <param name="jsml"></param>
			/// <returns></returns>
			public void OpenPractitionerDetail(string jsml)
			{
				var practitionerSummary = JsmlSerializer.Deserialize<ExternalPractitionerSummary>(jsml);

				LaunchAsDialog(
					_component.Host.DesktopWindow,
					new ExternalPractitionerOverviewComponent() { PractitionerSummary = practitionerSummary },
					SR.TitleExternalPractitioner + " - " + PersonNameFormat.Format(practitionerSummary.Name));
			}

			/// <summary>
			/// Gets the healthcard context object (JSML encoded).
			/// </summary>
			/// <returns></returns>
			public string GetHealthcareContext()
			{
				return JsmlSerializer.Serialize(_component.GetHealthcareContext(), "context");
			}

			/// <summary>
			/// Gets the specified tag.
			/// </summary>
			/// <param name="tag"></param>
			/// <returns></returns>
			public string GetTag(string tag)
			{
				return _component.GetTag(tag);
			}

			/// <summary>
			/// Sets the specified tag.
			/// </summary>
			/// <param name="tag"></param>
			/// <param name="data"></param>
			public void SetTag(string tag, string data)
			{
				_component.SetTag(tag, data);
			}

			/// <summary>
			/// Notifies the component that the script has finished executing.
			/// </summary>
			public void OnScriptCompleted()
			{
				_component.OnScriptCompleted();
			}

			/// <summary>
			/// Gets a JSML-encoded FacilitySummary containing details of the client's current working facility
			/// </summary>
			/// <returns></returns>
			public string GetWorkingFacility()
			{
				return JsmlSerializer.Serialize(LoginSession.Current.WorkingFacility, "facility");
			}

			/// <summary>
			/// Gets the configured base URL for web resources.
			/// </summary>
			/// <returns></returns>
			public string WebResourcesBaseUrl()
			{
				return WebResourcesSettings.Default.BaseUrl;
			}
		}

		#endregion


		private DHtmlScriptCallback _scriptCallback;
		private Uri _htmlPageUrl;

		private event EventHandler _dataSaving;
		private event EventHandler _printDocumentRequested;
		private event EventHandler _scriptCompleted;
		private event EventHandler<AsyncInvocationCompletedEventArgs> _asyncInvocationCompleted;
		private event EventHandler<AsyncInvocationErrorEventArgs> _asyncInvocationError;


		/// <summary>
		/// Constructor
		/// </summary>
		public DHtmlComponent()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="url"></param>
		public DHtmlComponent(string url)
		{
			SetUrl(url);
		}

		#region Public API

		/// <summary>
		/// Sets the URL of the page to display, causing the browser to navigate to or refresh this page.
		/// </summary>
		/// <param name="url"></param>
		public void SetUrl(string url)
		{
			this.HtmlPageUrl = string.IsNullOrEmpty(url)
				? null
				: new Uri(string.Format("{0}?{1}", url, UrlQueryString.Build(new {lang = InstalledLocales.Instance.Selected.Culture})));
		}

		/// <summary>
		/// Asks this component to save any data that the user may have changed.
		/// </summary>
		public virtual void SaveData()
		{
			if (_htmlPageUrl != null)
				EventsHelper.Fire(_dataSaving, this, EventArgs.Empty);
		}

		/// <summary>
		/// Occurs when the document has finished rendering, including scripts.
		/// </summary>
		/// <remarks>
		/// This would typically be fired after the document's body onload is finished.
		/// </remarks>
		public event EventHandler ScriptCompleted
		{
			add { _scriptCompleted += value; }
			remove { _scriptCompleted -= value; }
		}

		/// <summary>
		/// Print the component's current document.
		/// </summary>
		/// <remarks>
		/// This method should be called from the body of an EventHandler attached to the <see cref="ScriptCompleted"/> 
		/// to ensure all data is loaded in the document.
		/// </remarks>
		public void PrintDocument()
		{
			EventsHelper.Fire(_printDocumentRequested, this, EventArgs.Empty);
		}

		#endregion

		#region PresentationModel

		/// <summary>
		/// Gets a value indicating whether scrollbars should be displayed.
		/// </summary>
		public virtual bool ScrollBarsEnabled
		{
			get { return true; }
		}

		public Uri HtmlPageUrl
		{
			get { return _htmlPageUrl; }
			private set
			{
				// Do not assume same url implies page should not be reloaded
				_htmlPageUrl = value;
				NotifyAllPropertiesChanged();
			}
		}

		public DHtmlScriptCallback ScriptObject
		{
			get
			{
				if (_scriptCallback == null)
				{
					_scriptCallback = CreateScriptCallback();
				}
				return _scriptCallback;
			}
		}

		public void InvokeAction(string path)
		{
			var embeddedActionModel = GetActionModel();
			if (embeddedActionModel == null) 
				return;

			// need to find the action in the model that matches the uri path
			// TODO clean this up - this is a bit of hack right now
			var uriPath = new ActionPath(path, null);
			foreach (var child in embeddedActionModel.ChildNodes)
			{
				if (child is ActionNode)
				{
					var actionNode = (ActionNode)child;
					if (actionNode.Action.Path.LastSegment.ResourceKey == uriPath.LastSegment.ResourceKey)
					{
						((IClickAction)actionNode.Action).Click();
						break;
					}
				}
			}
		}

		/// <summary>
		/// Notifies view that the component is requesting that any data modified by the user be saved.
		/// </summary>
		public event EventHandler DataSaving
		{
			add { _dataSaving += value; }
			remove { _dataSaving -= value; }
		}

		/// <summary>
		/// Notifies the view that the component is requesting the document be printed.
		/// </summary>
		public event EventHandler PrintDocumentRequested
		{
			add { _printDocumentRequested += value; }
			remove { _printDocumentRequested += value; }
		}

		/// <summary>
		/// Notifies the view that an asynchronous service operation has completed.
		/// </summary>
		public event EventHandler<AsyncInvocationCompletedEventArgs> AsyncInvocationCompleted
		{
			add { _asyncInvocationCompleted += value; }
			remove { _asyncInvocationCompleted -= value; }
		}

		/// <summary>
		/// Notifies the view that an asynchronous service operation has resulted in an error.
		/// </summary>
		public event EventHandler<AsyncInvocationErrorEventArgs> AsyncInvocationError
		{
			add { _asyncInvocationError += value; }
			remove { _asyncInvocationError -= value; }
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Not used.
		/// </summary>
		/// <returns></returns>
		protected virtual ActionModelNode GetActionModel()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// Gets the context object that is provided to the script.
		/// </summary>
		/// <returns></returns>
		protected virtual DataContractBase GetHealthcareContext()
		{
			throw new NotSupportedException("Healthcare context not supported by this component.");
		}

		/// <summary>
		/// Gets the value associated with the specified tag.
		/// </summary>
		/// <remarks>
		/// The default implementation of this method retrieves tags from the dictionary returned by the
		/// <see cref="TagData"/> property.
		/// In most cases this method should not be overridden - override the <see cref="TagData"/> property
		/// instead.  The only reason to override this method is to do special processing of a given tag
		/// (for example, to define a special tag that is not stored in the dictionary).
		/// </remarks>
		/// <param name="tag"></param>
		/// <returns></returns>
		protected virtual string GetTag(string tag)
		{
			// if component doesn't support tag data, just do the most lenient thing and return null
			// we could throw an exception, but that seems counter to the spirit of javascript
			if (this.TagData == null)
				return null;

			string value;
			this.TagData.TryGetValue(tag, out value);

			return value;
		}

		/// <summary>
		/// Gets the value associated with the specified tag.
		/// </summary>
		/// <remarks>
		/// The default implementation of this method stores tags in the dictionary returned by
		/// the <see cref="TagData"/> property.
		/// In most cases this method should not be overridden - override the <see cref="TagData"/> property
		/// instead.  The only reason to override this method is to do special processing of a given tag
		/// (for example, to define a special tag that is not stored in the dictionary).
		/// </remarks>
		/// <param name="tag"></param>
		/// <param name="data"></param>
		protected virtual void SetTag(string tag, string data)
		{
			// in this case, throwing an exception is probably warranted because there is no point
			// letting the page believe that it is successfully storing tags when in fact it isn't
			if (this.TagData == null)
				throw new NotSupportedException("This component does not support storage of tags.");

			this.TagData[tag] = data;
		}

		/// <summary>
		/// Gets the dictionary used for default storage of tag data.
		/// </summary>
		/// <remarks>
		/// The default implementations of <see cref="GetTag"/> and <see cref="SetTag"/> use the dictionary
		/// returned by this property to store tag data.  The default implementation of this property
		/// returns an empty dictionary.  Therefore this property must be overridden
		/// to support tag storage.  Alternatively, the <see cref="GetTag"/> and <see cref="SetTag"/> methods
		/// may be overridden directly, but in most cases this is not necessary.
		/// </remarks>
		protected virtual IDictionary<string, string> TagData
		{
			get
			{
				return new Dictionary<string, string>();
			}
		}

		/// <summary>
		/// Factory method to create script callback.  Override to provide custom implementation.
		/// </summary>
		/// <returns></returns>
		protected virtual DHtmlScriptCallback CreateScriptCallback()
		{
			return new DHtmlScriptCallback(this);
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Allows the <see cref="ScriptObject"/> to raise the <see cref="ScriptCompleted"/> event.
		/// </summary>
		private void OnScriptCompleted()
		{
			EventsHelper.Fire(_scriptCompleted, this, EventArgs.Empty);
		}

		private void AsyncInvocationCompletedEventHandler(object sender, AsyncInvocationCompletedEventArgs e)
		{
			// forward the event on to the view layer
			EventsHelper.Fire(_asyncInvocationCompleted, sender, e);
		}

		public void AsyncInvocationErrorEventHandler(object sender, AsyncInvocationErrorEventArgs e)
		{
			Platform.Log(LogLevel.Error, e);

			// forward the event on to the view layer
			EventsHelper.Fire(_asyncInvocationError, sender, e);
		}

		#endregion
	}
}

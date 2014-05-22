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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	public partial class DHtmlComponentControl : ApplicationComponentUserControl
	{
		private readonly DHtmlComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public DHtmlComponentControl(DHtmlComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

#if DEBUG
			// in dev, show script error dialogs and browser context menu
			_webBrowser.ScriptErrorsSuppressed = false;
			_webBrowser.IsWebBrowserContextMenuEnabled = true;
#else
			// in production, suppress script error dialogs and browser context menu
			_webBrowser.ScriptErrorsSuppressed = true;
			_webBrowser.IsWebBrowserContextMenuEnabled = false;
#endif

			_component.AllPropertiesChanged += AllPropertiesChangedEventHandler;

			//_webBrowser.DataBindings.Add("Url", _component, "HtmlPageUrl", true, DataSourceUpdateMode.OnPropertyChanged);
			_webBrowser.ObjectForScripting = _component.ScriptObject;
			_webBrowser.Navigating += NavigatingEventHandler;
			_webBrowser.ScrollBarsEnabled = _component.ScrollBarsEnabled;
			if (_component.HtmlPageUrl != null)
			{
				_webBrowser.Navigate(_component.HtmlPageUrl);
			}


			_component.Validation.Add(new ValidationRule("DUMMY_PROPERTY",
				delegate
				{
					var result = _webBrowser.Document != null ? _webBrowser.Document.InvokeScript("hasValidationErrors") : null;

					// if result == null, the hasValidationErrors method is not implemented by the page
					// in this case, assume there are no errors
					var hasErrors = (result == null) ? false : (bool)result;
					return new ValidationResult(!hasErrors, "");
				}));

			_component.ValidationVisibleChanged += _component_ValidationVisibleChanged;
			_component.DataSaving += _component_DataSaving;

			_component.PrintDocumentRequested += _component_PrintDocument;
			_component.AsyncInvocationCompleted += _component_AsyncInvocationCompleted;
			_component.AsyncInvocationError += _component_AsyncInvocationError;

			_webBrowser.Disposed += delegate
			{
				_component.ValidationVisibleChanged -= _component_ValidationVisibleChanged;
				_component.DataSaving -= _component_DataSaving;
				_component.PrintDocumentRequested -= _component_PrintDocument;
				_component.AsyncInvocationCompleted -= _component_AsyncInvocationCompleted;
				_component.AsyncInvocationError -= _component_AsyncInvocationError;
			};
		}

		private void _component_PrintDocument(object sender, EventArgs e)
		{
			_webBrowser.Print();
		}

		private void _component_DataSaving(object sender, EventArgs e)
		{
			if (_webBrowser.Document != null)
			{
				_webBrowser.Document.InvokeScript("saveData", new object[] { _component.ValidationVisible });
			}
		}

		private void _component_ValidationVisibleChanged(object sender, EventArgs e)
		{
			if (_webBrowser.Document != null)
			{
				_webBrowser.Document.InvokeScript("showValidation", new object[] { _component.ValidationVisible });
			}
		}

		private void _component_AsyncInvocationCompleted(object sender, AsyncInvocationCompletedEventArgs e)
		{
			if (_webBrowser.Document != null)
			{
				_webBrowser.Document.InvokeScript("__asyncInvocationCompleted", new object[] { e.InvocationId, e.Response });
			}
		}

		private void _component_AsyncInvocationError(object sender, AsyncInvocationErrorEventArgs e)
		{
			if (_webBrowser.Document != null)
			{
				_webBrowser.Document.InvokeScript("__asyncInvocationError", new object[] { e.InvocationId, e.Error.Message });
			}
		}

		private void AllPropertiesChangedEventHandler(object sender, EventArgs e)
		{
			// navigate to the new URI
			// Bug #2845 even if it is the same URI, we want to navigate rather than refresh, so that scroll position is reset to top

			try
			{
				_webBrowser.Navigate(_component.HtmlPageUrl);
			}
			catch (Exception ex)
			{
				// Later versions of IE (>= 9 ?) will sometimes throw an exception if we attempt to navigate while it has a dialog up
				// (e.g. in the event of a script error)
				// If we don't catch it, the entire workstation crashes.
				Platform.Log(LogLevel.Error, ex, "An exception was thrown by the WebBrowser control");
			}
		}

		private void NavigatingEventHandler(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
		{
			if (e.Url.OriginalString.StartsWith("action:"))
			{
				e.Cancel = true;    // cancel the webbrowser navigation

				_component.InvokeAction(e.Url.LocalPath);
			}
		}
	}
}

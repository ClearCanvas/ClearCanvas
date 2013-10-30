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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Dicom.Iod.ContextGroups;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ExtensionPoint]
	public class KeyImageInformationEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	[AssociateView(typeof(KeyImageInformationEditorComponentViewExtensionPoint))]
	public class KeyImageInformationEditorComponent : ApplicationComponent
	{
		private DateTime _datetime;
		private string _description;
		private string _seriesDescription;
		private string _seriesNumber;
		private KeyObjectSelectionDocumentTitle _docTitle;

		private KeyImageInformationEditorComponent()
		{
		}

		public DateTime DateTime
		{
			get { return _datetime; }
			protected set
			{
				if (_datetime != value)
				{
					_datetime = value;
					NotifyPropertyChanged("DateTime");
				}
			}
		}

		public KeyObjectSelectionDocumentTitle DocumentTitle
		{
			get { return _docTitle; }
			set
			{
				if (_docTitle != value)
				{
					_docTitle = value;
					NotifyPropertyChanged("DocumentTitle");
				}
			}
		}

		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					NotifyPropertyChanged("Description");
				}
			}
		}

		[ValidateLength(1, 64, Message = "MessageInvalidSeriesDescription")]
		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set
			{
				if (_seriesDescription != value)
				{
					_seriesDescription = value;
					NotifyPropertyChanged("SeriesDescription");
				}
			}
		}

		public string SeriesNumber
		{
			get { return _seriesNumber; }
			set
			{
				if (_seriesNumber != value)
				{
					_seriesNumber = value;
					NotifyPropertyChanged("SeriesNumber");
				}
			}
		}

		public static IEnumerable<KeyObjectSelectionDocumentTitle> StandardDocumentTitles
		{
			get { return KeyObjectSelectionDocumentTitleContextGroup.Values; }
		}

		public void Accept()
		{
			ExitCode = ApplicationComponentExitCode.Accepted;
			this.Host.Exit();
		}

		public void Cancel()
		{
			ExitCode = ApplicationComponentExitCode.None;
			this.Host.Exit();
		}

		internal static void Launch(IDesktopWindow desktopWindow)
		{
			var info = KeyImageClipboard.GetKeyImageClipboard(desktopWindow);
			if (info == null)
				throw new ArgumentException("There is no valid key image data available for the given window.", "desktopWindow");

			KeyImageInformationEditorComponent component = new KeyImageInformationEditorComponent();
			component.Description = info.Description;
			component.DocumentTitle = info.DocumentTitle;
			component.SeriesDescription = info.SeriesDescription;

			ApplicationComponentExitCode exitCode = LaunchAsDialog(desktopWindow, component, SR.TitleEditKeyImageInformation);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				info.Description = component.Description;
				info.DocumentTitle = component.DocumentTitle;
				info.SeriesDescription = component.SeriesDescription;
			}
		}

		[ValidationMethodFor("SeriesNumber")]
		private ValidationResult ValidateSeriesNumber()
		{
			int value;
			if (String.IsNullOrEmpty(SeriesNumber) || !int.TryParse(SeriesNumber, out value))
				return new ValidationResult(false, "MessageInvalidSeriesNumber");
			else
				return new ValidationResult(true, "");
		}
	}
}
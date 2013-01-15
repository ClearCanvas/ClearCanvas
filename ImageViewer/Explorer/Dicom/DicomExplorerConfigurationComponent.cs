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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint]
	public sealed class DicomExplorerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (DicomExplorerConfigurationComponentViewExtensionPoint))]
	public class DicomExplorerConfigurationComponent : ConfigurationApplicationComponent
	{
		private SearchResultColumnOptionCollection _resultColumns = new SearchResultColumnOptionCollection();
		private bool _selectDefaultServerOnStartup = false;

		protected SearchResultColumnOptionCollection ResultColumns
		{
			get { return _resultColumns; }
		}

		public bool SelectDefaultServerOnStartup
		{
			get { return _selectDefaultServerOnStartup; }
			set
			{
				if (SelectDefaultServerOnStartup == value)
					return;

				_selectDefaultServerOnStartup = value;
				NotifyPropertyChanged("SelectDefaultServerOnStartup");
				Modified = true;
			}
		}

		public bool ShowPhoneticIdeographicNames
		{
			get { return ShowPhoneticName || ShowIdeographicName; }
			set
			{
				if (ShowPhoneticIdeographicNames == value)
					return;

				ShowPhoneticName = ShowIdeographicName = value;
				Modified = true;
			}
		}

		public bool ShowPhoneticName
		{
			get { return _resultColumns[StudyTable.ColumnNamePhoneticName].Visible; }
			set
			{
				if (ShowPhoneticName == value)
					return;

				_resultColumns[StudyTable.ColumnNamePhoneticName].Visible = value;
				NotifyPropertyChanged("ShowPhoneticName");
				NotifyPropertyChanged("ShowPhoneticIdeographicNames");
				Modified = true;
			}
		}

		public bool ShowIdeographicName
		{
            get { return _resultColumns[StudyTable.ColumnNameIdeographicName].Visible; }
			set
			{
				if (ShowIdeographicName == value)
					return;

                _resultColumns[StudyTable.ColumnNameIdeographicName].Visible = value;
				NotifyPropertyChanged("ShowIdeographicName");
				NotifyPropertyChanged("ShowPhoneticIdeographicNames");
				Modified = true;
			}
		}

		public bool ShowNumberOfImagesInStudy
		{
            get { return _resultColumns[StudyTable.ColumnNameNumberOfInstances].Visible; }
			set
			{
				if (ShowNumberOfImagesInStudy == value)
					return;

                _resultColumns[StudyTable.ColumnNameNumberOfInstances].Visible = value;
				NotifyPropertyChanged("ShowNumberOfImagesInStudy");
				Modified = true;
			}
		}

		public override void Start()
		{
			_selectDefaultServerOnStartup = DicomExplorerConfigurationSettings.Default.SelectDefaultServerOnStartup;
			_resultColumns = new SearchResultColumnOptionCollection(DicomExplorerConfigurationSettings.Default.ResultColumns);
			base.Start();
		}

		public override void Save()
		{
			DicomExplorerConfigurationSettings.Default.SelectDefaultServerOnStartup = SelectDefaultServerOnStartup;
			DicomExplorerConfigurationSettings.Default.ResultColumns = _resultColumns;
			DicomExplorerConfigurationSettings.Default.Save();
		}
	}
}
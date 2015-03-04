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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Dicom.Iod;

namespace ClearCanvas.Dicom.Utilities.Xml.Study
{
	/// <summary>
	/// Represents an <see cref="ISeries"/> whose main source of data is a <see cref="StudyXml"/> document.
	/// </summary>
	//JR: made this class public so that Specifications using Jscript.NET can operate on it (JScript.NET can't see members on internal classes)
	public class Series : ISeries
	{
		private class SopInstanceCollection : ISopInstanceCollection
		{
			private readonly Series _owner;

			public SopInstanceCollection(Series owner)
			{
				_owner = owner;
			}

			public IEnumerator<ISopInstance> GetEnumerator()
			{
				return Xml.Select(_owner.GetSopInstance).GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public int Count
			{
				get { return Xml.NumberOfSeriesRelatedInstances; }
			}

			public bool Contains(string sopInstanceUid)
			{
				return Xml[sopInstanceUid] != null;
			}

			public ISopInstance Get(string sopInstanceUid)
			{
				var seriesXml = Xml[sopInstanceUid];
				if (seriesXml == null)
					throw new ArgumentException("Invalid value for series instance UID.");

				return _owner.GetSopInstance(seriesXml);
			}

			public ISopInstance this[string sopInstanceUid]
			{
				get { return Get(sopInstanceUid); }
			}

			public bool TryGet(string sopInstanceUid, out ISopInstance series)
			{
				var seriesXml = Xml[sopInstanceUid];
				if (seriesXml != null)
				{
					series = _owner.GetSopInstance(seriesXml);
					return true;
				}
				series = null;
				return false;
			}

			private SeriesXml Xml
			{
				get { return _owner._xml; }
			}
		}


		private readonly SeriesXml _xml;
		private readonly Study _parentStudy;
		private readonly IDicomFileLoader _dicomFileLoader;
		private readonly ISopInstance _firstSopInstance;
		private readonly SopInstanceCollection _sopInstanceCollection;

		internal Series(SeriesXml xml, Study parent, IDicomFileLoader dicomFileLoader)
		{
			_xml = xml;
			_parentStudy = parent;
			_dicomFileLoader = dicomFileLoader;
			_sopInstanceCollection = new SopInstanceCollection(this);
			_firstSopInstance = _sopInstanceCollection.FirstOrDefault();
		}

		#region ISeries Members

		public IStudy ParentStudy
		{
			get { return _parentStudy; }
		}

		public ISopInstanceCollection SopInstances
		{
			get { return _sopInstanceCollection; }
		}

		public string StationName
		{
			get { return FirstSopInstance.GetAttribute(DicomTags.StationName).ToString(); }
		}

		public string Manufacturer
		{
			get { return FirstSopInstance.GetAttribute(DicomTags.Manufacturer).ToString(); }
		}

		public string ManufacturersModelName
		{
			get { return FirstSopInstance.GetAttribute(DicomTags.ManufacturersModelName).ToString(); }
		}

		public string InstitutionName
		{
			get { return FirstSopInstance.GetAttribute(DicomTags.InstitutionName).ToString(); }
		}

		public string InstitutionAddress
		{
			get { return FirstSopInstance.GetAttribute(DicomTags.InstitutionAddress).ToString(); }
		}

		public string InstitutionalDepartmentName
		{
			get { return FirstSopInstance.GetAttribute(DicomTags.InstitutionalDepartmentName).ToString(); }
		}

		#endregion

		#region ISeriesData Members

		public string StudyInstanceUid
		{
			get { return _parentStudy.StudyInstanceUid; }
		}

		public string SeriesInstanceUid
		{
			get { return _xml.SeriesInstanceUid; }
		}

		public string Modality
		{
			get { return FirstSopInstance.GetAttribute(DicomTags.Modality).ToString(); }
		}

		public string SeriesDescription
		{
			get { return FirstSopInstance.GetAttribute(DicomTags.SeriesDescription).ToString(); }
		}

		public int SeriesNumber
		{
			get { return FirstSopInstance.GetAttribute(DicomTags.SeriesNumber).GetInt32(0, 0); }
		}

		public int? NumberOfSeriesRelatedInstances
		{
			get { return _xml.NumberOfSeriesRelatedInstances; }
		}

		#endregion

		private ISopInstance FirstSopInstance
		{
			get
			{
				if (_firstSopInstance == null)
					throw new InvalidOperationException("Series contains no SOPs.");

				return _firstSopInstance;
			}
		}

		private SopInstance GetSopInstance(InstanceXml xml)
		{
			return new SopInstance(xml, this, _dicomFileLoader);
		}
	}
}

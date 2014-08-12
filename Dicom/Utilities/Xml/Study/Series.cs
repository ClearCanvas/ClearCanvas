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
		private readonly SeriesXml _xml;
		private readonly Study _parentStudy;
		private readonly IDicomFileLoader _dicomFileLoader;
		private readonly SopInstance _firstSopInstance;

		internal Series(SeriesXml xml, Study parent, IDicomFileLoader dicomFileLoader)
		{
			_xml = xml;
			_parentStudy = parent;
			_dicomFileLoader = dicomFileLoader;
			_firstSopInstance = GetSopInstance(_xml.First());
		}

		#region ISeries Members

		public IStudy ParentStudy
		{
			get { return _parentStudy; }
		}

		public int SopInstanceCount
		{
			get { return _xml.NumberOfSeriesRelatedInstances; }
		}

		public ISopInstance FirstSopInstance
		{
			get { return _firstSopInstance; }
		}

		public ISopInstance GetSopInstance(string sopInstanceUid)
		{
			var instanceXml = _xml[sopInstanceUid];
			return instanceXml == null ? null : GetSopInstance(instanceXml);
		}

		public IEnumerable<ISopInstance> EnumerateSopInstances()
		{
			return _xml.Select(GetSopInstance);
		}

		public string StationName
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.StationName).ToString(); }
		}

		public string Manufacturer
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.Manufacturer).ToString(); }
		}

		public string ManufacturersModelName
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.ManufacturersModelName).ToString(); }
		}

		public string InstitutionName
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.InstitutionName).ToString(); }
		}

		public string InstitutionAddress
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.InstitutionAddress).ToString(); }
		}

		public string InstitutionalDepartmentName
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.InstitutionalDepartmentName).ToString(); }
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
			get { return _firstSopInstance.GetAttribute(DicomTags.Modality).ToString(); }
		}

		public string SeriesDescription
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.SeriesDescription).ToString(); }
		}

		public int SeriesNumber
		{
			get { return _firstSopInstance.GetAttribute(DicomTags.SeriesNumber).GetInt32(0, 0); }
		}

		public int? NumberOfSeriesRelatedInstances
		{
			get { return _xml.NumberOfSeriesRelatedInstances; }
		}

		#endregion

		private SopInstance GetSopInstance(InstanceXml xml)
		{
			return new SopInstance(xml, this, _dicomFileLoader);
		}
	}
}

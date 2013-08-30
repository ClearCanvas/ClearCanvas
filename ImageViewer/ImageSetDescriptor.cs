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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Common;
using System.Text;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Definition of an <see cref="IImageSetDescriptor"/> whose contents are based on
	/// a DICOM Study.
	/// </summary>
	public interface IDicomImageSetDescriptor : IImageSetDescriptor
	{
		/// <summary>
		/// Gets the <see cref="IStudyRootStudyIdentifier"/> for the DICOM study from which
		/// the <see cref="IImageSet"/> was created.
		/// </summary>
		IStudyRootStudyIdentifier SourceStudy { get; }
        
        IDicomServiceNode Server { get; }

        Exception LoadStudyError { get; }

		// TODO CR (Aug 13): turn this into a single status enum property. make it flags even, if really needed
        bool IsOffline { get; }
        bool IsNearline { get; }
        bool IsInUse { get; }
        bool IsNotLoadable { get; }
    }

	/// <summary>
	/// Implementation of <see cref="IDicomImageSetDescriptor"/>.
	/// </summary>
	public class DicomImageSetDescriptor : ImageSetDescriptor, IDicomImageSetDescriptor
	{
		private string _name;
		private string _patientInfo;
		private string _uid;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DicomImageSetDescriptor(IStudyRootStudyIdentifier sourceStudy)
            : this(sourceStudy, null, null)
		{
        }

	    /// <summary>
        /// Constructor.
        /// </summary>
        public DicomImageSetDescriptor(IStudyRootStudyIdentifier sourceStudy, IDicomServiceNode server, Exception loadStudyError)
		{
            Platform.CheckForNullReference(sourceStudy, "sourceStudy");
            SourceStudy = sourceStudy;
            Server = server ?? sourceStudy.FindServer(true);

	        LoadStudyError = loadStudyError;
            IsOffline = loadStudyError is OfflineLoadStudyException;
            IsNearline = loadStudyError is NearlineLoadStudyException;
            IsInUse = loadStudyError is InUseLoadStudyException;
            IsNotLoadable = loadStudyError is StudyLoaderNotFoundException;
		}

	    #region IDicomImageSetDescriptor Members

        /// <summary>
		/// Gets the <see cref="IStudyRootStudyIdentifier"/> for the DICOM study from which
		/// the <see cref="IImageSet"/> was created.
		/// </summary>
		public IStudyRootStudyIdentifier SourceStudy { get; private set; }
	    public IDicomServiceNode Server { get; private set; }
	    public Exception LoadStudyError { get; private set; }

        public bool IsOffline { get; private set; }
        public bool IsNearline { get; private set; }
        public bool IsInUse { get; private set; }
        public bool IsNotLoadable { get; private set; }

        #endregion

        /// <summary>
		/// Gets the descriptive name of the <see cref="IImageSet"/>.
		/// </summary>
		public override string Name
		{
			get
			{
				if (_name == null)
					_name = GetName() ?? "";
				return _name;
			}
			set { throw new InvalidOperationException("The Name property cannot be set publicly."); }
		}

		/// <summary>
		/// Gets a description of the patient whose images are contained in the <see cref="IImageSet"/>.
		/// </summary>
		public override string PatientInfo
		{
			get
			{
				if (_patientInfo == null)
					_patientInfo = GetPatientInfo() ?? "";
				return _patientInfo;
			}
			set { throw new InvalidOperationException("The PatientInfo property cannot be set publicly."); }
		}

		/// <summary>
		/// Gets a unique identifier for the <see cref="IImageSet"/>.
		/// </summary>
		public override string Uid
		{
			get
			{
				if (_uid == null)
					_uid = GetUid() ?? "";
				return _uid;
			}
			set { throw new InvalidOperationException("The Uid property cannot be set publicly."); }
		}

	    /// <summary>
		/// Gets the descriptive name of the <see cref="IImageSet"/>.
		/// </summary>
		protected virtual string GetName()
		{
			DateTime studyDate;
			DateParser.Parse(SourceStudy.StudyDate, out studyDate);
			DateTime studyTime;
            TimeParser.Parse(SourceStudy.StudyTime, out studyTime);

            string modalitiesInStudy = StringUtilities.Combine(SourceStudy.ModalitiesInStudy, ", ");

			var nameBuilder = new StringBuilder();
			nameBuilder.AppendFormat("{0} {1}", studyDate.ToString(Format.DateFormat), 
												studyTime.ToString(Format.TimeFormat));

            if (!String.IsNullOrEmpty(SourceStudy.AccessionNumber))
                nameBuilder.AppendFormat(", A#: {0}", SourceStudy.AccessionNumber);

            nameBuilder.AppendFormat(", [{0}] {1}", modalitiesInStudy ?? "", SourceStudy.StudyDescription);

            if (LoadStudyError != null)
            {
                string serverName;
                if (Server == null)
                    serverName = SR.LabelUnknownServer;
                else
                    serverName = Server.Name;

                nameBuilder.Insert(0, String.Format("({0}) ", serverName));
            }

	        return nameBuilder.ToString();
		}

		/// <summary>
		/// Gets a description of the patient whose images are contained in the <see cref="IImageSet"/>.
		/// </summary>
		protected virtual string GetPatientInfo()
		{
            return String.Format("{0} \u00B7 {1}", new PersonName(SourceStudy.PatientsName).FormattedName, SourceStudy.PatientId);
		}

		/// <summary>
		/// Gets a unique identifier for the <see cref="IImageSet"/>.
		/// </summary>
		protected virtual string GetUid()
		{
            return SourceStudy.StudyInstanceUid;
		}
	}

	/// <summary>
	/// Default implementation of <see cref="IImageSetDescriptor"/>.
	/// </summary>
	public class BasicImageSetDescriptor : ImageSetDescriptor
	{
		private string _name;
		private string _patientInfo;
		private string _uid;

		/// <summary>
		/// Constructor.
		/// </summary>
		public BasicImageSetDescriptor()
		{}

		/// <summary>
		/// Gets the descriptive name of the <see cref="IImageSet"/>.
		/// </summary>
		public override string Name
		{
			get { return _name ?? ""; }
			set { _name = value; }
		}

		/// <summary>
		/// Gets a description of the patient whose images are contained in the <see cref="IImageSet"/>.
		/// </summary>
		public override string PatientInfo
		{
			get { return _patientInfo ?? ""; }
			set { _patientInfo = value; }
		}

		/// <summary>
		/// Gets a unique identifier for the <see cref="IImageSet"/>.
		/// </summary>
		public override string Uid
		{
			get { return _uid ?? ""; }
			set { _uid = value; }
		}
	}

	/// <summary>
	/// Definition of an object that describes the contents of an <see cref="IDisplaySet"/>.
	/// </summary>
	public interface IImageSetDescriptor
	{
		/// <summary>
		/// Gets the descriptive name of the <see cref="IImageSet"/>.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets a description of the patient whose images are contained in the <see cref="IImageSet"/>.
		/// </summary>
		string PatientInfo { get; }

		/// <summary>
		/// Gets a unique identifier for the <see cref="IImageSet"/>.
		/// </summary>
		string Uid { get; }
	}

	/// <summary>
	/// Abstract base implementation of <see cref="IImageSetDescriptor"/>.
	/// </summary>
	public abstract class ImageSetDescriptor : IImageSetDescriptor
	{
		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected ImageSetDescriptor()
		{
		}

		#region IImageSetDescriptor Members

		/// <summary>
		/// Gets the descriptive name of the <see cref="IImageSet"/>.
		/// </summary>
		public abstract string Name { get; set; }

		/// <summary>
		/// Gets a description of the patient whose images are contained in the <see cref="IImageSet"/>.
		/// </summary>
		public abstract string PatientInfo { get; set; }

		/// <summary>
		/// Gets a unique identifier for the <see cref="IImageSet"/>.
		/// </summary>
		public abstract string Uid { get; set; }

		#endregion

		/// <summary>
		/// Gets a text description of this <see cref="IImageSetDescriptor"/>.
		/// </summary>
		public override string ToString()
		{
			return StringUtilities.Combine(new string[] { PatientInfo, Name, Uid }, " | ", true);
		}
	}
}

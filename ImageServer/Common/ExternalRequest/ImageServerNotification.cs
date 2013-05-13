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

using System.Runtime.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageServer.Common.ExternalRequest
{
    [DataContract(Namespace = ImageServerExternalRequestNamespace.Value)]
    [ImageServerNotificationType("0AF481B3-BD83-4E68-9577-E2C952F50C14")]
    public class ImageServerNotification
    {
    }

    [DataContract(Namespace = ImageServerExternalRequestNamespace.Value)]
    [ImageServerNotificationType("6A8B5F4C-4F88-4B9E-A421-7C510C63F0E1")]
    public class NotificationPatient : PatientRootPatientIdentifier
    {
        public NotificationPatient()
        { }

        public NotificationPatient(IPatientData p)
            : base(p)
        { }

        public NotificationPatient(DicomAttributeCollection c)
            : base(c)
        { }
    }

    [DataContract(Namespace = ImageServerExternalRequestNamespace.Value)]
    [ImageServerNotificationType("AE83CF35-2DF9-479C-9F24-F6F91607C4A0")]
    public class NotificationStudy : StudyIdentifier
    {
        public NotificationStudy()
        { }

        public NotificationStudy(IStudyData s)
            : base(s)
        { }

        public NotificationStudy(DicomAttributeCollection c)
            : base(c)
        {
            string modality = c[DicomTags.Modality].ToString();
            ModalitiesInStudy = new[] { modality };
        }
    }

    [DataContract(Namespace = ImageServerExternalRequestNamespace.Value)]
    [ImageServerNotificationType("3D4BC1CE-9D4E-454F-9D70-10F4E6EF0E59")]
    public class NotificationSeries : SeriesIdentifier
    {
        public NotificationSeries()
        { }

        public NotificationSeries(ISeriesData s)
            : base(s)
        { }

        public NotificationSeries(DicomAttributeCollection c)
            : base(c)
        {    
        }
    }

    [DataContract(Namespace = ImageServerExternalRequestNamespace.Value)]
    [ImageServerNotificationType("EBDB2829-8233-4F71-961E-24F2259BDE08")]
    public class NotificationImage : ImageIdentifier
    {
        public NotificationImage()
        { }

        public NotificationImage(DicomAttributeCollection c)
            : base(c)
        {
        }
    }
}

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

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    /// <summary>
    /// Model object behind the <see cref="SeriesDetailsPanel"/>
    /// </summary>
    public class SeriesDetails
    {
        private string _seriesInstanceUid;
        private string _modality;
        private string _seriesNumber;
        private string _seriesDescription;
        private int _numberOfSeriesRelatedInstances;
        private string _performedDate;
        private string _performedTime;
        private string _sourceApplicationEntityTitle;

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public string Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }

        public string SeriesNumber
        {
            get { return _seriesNumber; }
            set { _seriesNumber = value; }
        }

        public string SeriesDescription
        {
            get { return _seriesDescription; }
            set { _seriesDescription = value; }
        }

        public int NumberOfSeriesRelatedInstances
        {
            get { return _numberOfSeriesRelatedInstances; }
            set { _numberOfSeriesRelatedInstances = value; }
        }

        public string PerformedDate
        {
            get { return _performedDate; }
            set { _performedDate = value; }
        }

        public string SourceApplicationEntityTitle
        {
            get { return _sourceApplicationEntityTitle; }
            set { _sourceApplicationEntityTitle = value; }
        }

        public string PerformedTime
        {
            get { return _performedTime; }
            set { _performedTime = value; }
        }
    }
}

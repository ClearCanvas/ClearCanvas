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
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.SeriesDetails
{
    static public class SeriesDetailsAssembler
    {
        /// <summary>
        /// Returns an instance of <see cref="SeriesDetails"/> for a <see cref="Series"/>.
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        /// <remark>
        /// 
        /// </remark>
        static public SeriesDetails CreateSeriesDetails(Model.Series series)
        {
            SeriesDetails details = new SeriesDetails();

            details.Modality = series.Modality;
            details.NumberOfSeriesRelatedInstances = series.NumberOfSeriesRelatedInstances;
            details.PerformedDate = series.PerformedProcedureStepStartDate;
            details.PerformedTime = series.PerformedProcedureStepStartTime;
            details.SeriesDescription = series.SeriesDescription;
            details.SeriesInstanceUid = series.SeriesInstanceUid;
            details.SeriesNumber = series.SeriesNumber;
            details.SourceApplicationEntityTitle = series.SourceApplicationEntityTitle;

            return details;
        }
    }
}

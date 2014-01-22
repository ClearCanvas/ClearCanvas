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
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy
{
    internal class DeleteSeriesFromDBCommand : ServerDatabaseCommand
    {
        private readonly StudyStorageLocation _location;
        private readonly Series _series;

        public DeleteSeriesFromDBCommand(StudyStorageLocation location, Series series)
            : base(String.Format("Delete Series In DB {0}", series.SeriesInstanceUid))
        {
            _location = location;
            _series = series;
        }

        public Series Series
        {
            get { return _series; }
        }


        protected override void OnExecute(CommandProcessor theProcessor, ClearCanvas.Enterprise.Core.IUpdateContext updateContext)
        {
            IDeleteSeries broker = updateContext.GetBroker<IDeleteSeries>();
            DeleteSeriesParameters criteria = new DeleteSeriesParameters();
            criteria.StudyStorageKey = _location.Key;
            criteria.SeriesInstanceUid = _series.SeriesInstanceUid;
            if (!broker.Execute(criteria))
                throw new ApplicationException("Error occurred when calling DeleteSeries");
        }
    }

    internal class DeleteSeriesFromDBCommandEventArgs:EventArgs
    {
    }
}
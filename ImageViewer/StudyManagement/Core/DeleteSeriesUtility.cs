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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.StudyManagement.Core.Command;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
    /// <summary>
    /// Utility class for deleting a series.
    /// </summary>
    public class DeleteSeriesUtility
    {
        private StudyLocation _location;
        public List<string> SeriesInstanceUids { get; set; } 
        public int NumberOfSeriesRelatedInstances { get; set; }
        public StudyXml StudyXml { get; set; }

        public void Initialize(StudyLocation location, List<string> seriesInstanceUids )
        {
            _location = location;
            SeriesInstanceUids = seriesInstanceUids;

            StudyXml = _location.LoadStudyXml();

            NumberOfSeriesRelatedInstances = 0;

            foreach (string seriesInstanceUid in seriesInstanceUids)
            {
                foreach (SeriesXml seriesXml in StudyXml)
                {
                    if (seriesXml.SeriesInstanceUid.Equals(seriesInstanceUid))
                    {
                        NumberOfSeriesRelatedInstances += seriesXml.NumberOfSeriesRelatedInstances;
                    }
                }
            }
        }

        public bool DeletingAllSeries()
        {
            return NumberOfSeriesRelatedInstances == StudyXml.NumberOfStudyRelatedInstances;
        }

        public bool Process()
        {
            using (
                var processor =
                    new ViewerCommandProcessor("Deleting series from study: " + _location.Study.StudyInstanceUid))
            {
                try
                {
                    DicomAttributeCollection instance = null;
                    foreach (string seriesInstanceUid in SeriesInstanceUids)
                    {
                        foreach (SeriesXml seriesXml in StudyXml)
                        {
                            if (seriesXml.SeriesInstanceUid.Equals(seriesInstanceUid))
                            {
                                foreach (InstanceXml instanceXml in seriesXml)
                                {
                                    processor.AddCommand(
                                        new FileDeleteCommand(
                                            _location.GetSopInstancePath(seriesInstanceUid, instanceXml.SopInstanceUid),
                                            true));
                                }
                            }
                            else
                            {
                                // Save an instance we're keeping so we can update the study
                                if (instance == null)
                                    instance = CollectionUtils.FirstElement(seriesXml).Collection;
                            }
                        }
                        processor.AddCommand(new RemoveSeriesFromStudyXml(StudyXml, seriesInstanceUid));
                    }

                    processor.AddCommand(new SaveStudyXmlCommand(StudyXml, _location));

                    processor.AddCommand(new InsertOrUpdateStudyCommand(_location, StudyXml, InsertOrUpdateStudyCommand.UpdateReason.SopsDeleted));

                    // Do the actual processing
                    if (!processor.Execute())
                    {
                        Platform.Log(LogLevel.Error, "Failure deleting {0} series for Study: {1}",
                                     SeriesInstanceUids.Count, _location.Study.StudyInstanceUid);
                        throw new ApplicationException(
                            "Unexpected failure (" + processor.FailureReason + ") executing command for Study: " +
                            _location.Study.StudyInstanceUid, processor.FailureException);
                    }
                    Platform.Log(LogLevel.Info, "Deleted {0} Series for Study {1}", SeriesInstanceUids.Count,
                                 _location.Study.StudyInstanceUid);
                    return true;
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.",
                                 processor.Description);
                    processor.Rollback();
                    throw new ApplicationException("Unexpected exception when processing file.", e);
                }
            }
        }
    }
}

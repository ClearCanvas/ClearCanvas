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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Dicom.Tests;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.Tests
{
    public class TestBase : AbstractTest
    {
        protected Study CreateTestStudy1()
        {
            var studyUid = "1.2.3";
            var sops = base.SetupMRSeries(4, 5, studyUid);
            var xml = new StudyXml(studyUid);

            var seriesUids = new Dictionary<string, string>();
            var seriesModalities = new Dictionary<string, string>();
            var modalities = new[] { "MR", "MR", "SC", "KO" };

            foreach (var sop in sops)
            {
                //Make the UIDs constant.
                var seriesUid = sop[DicomTags.SeriesInstanceUid].ToString();
                if (!seriesUids.ContainsKey(seriesUid))
                {
                    seriesModalities[seriesUid] = modalities[seriesUids.Count];
                    seriesUids[seriesUid] = string.Format("1.2.3.{0}", seriesUids.Count + 1);
                }

                var modality = seriesModalities[seriesUid];
                seriesUid = seriesUids[seriesUid];

                sop[DicomTags.SeriesInstanceUid].SetString(0, seriesUid);
                sop[DicomTags.Modality].SetString(0, modality);

                var file = new DicomFile("", new DicomAttributeCollection(), sop);
                xml.AddFile(file);
            }

            var study = new Study();
            study.Update(xml);
            return study;
        }
    }
}

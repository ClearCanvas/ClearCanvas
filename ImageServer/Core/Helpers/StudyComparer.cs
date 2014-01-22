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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common.Helpers
{
    public class ComparisionDifference
    {
        public string Description;
        public DicomTag   DicomTag;
        public string ExpectValue;
        public string RealValue;

        public ComparisionDifference(uint tag, string expected, string actual)
        {
            DicomTag = DicomTagDictionary.GetDicomTag(tag);
            Description = DicomTag.Name;
            ExpectValue = expected;
            RealValue = actual;
        }
    }

    public class DifferenceCollection : List<ComparisionDifference>
    {
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (ComparisionDifference diff in this)
            {
                sb.AppendLine(String.Format("\t{0}: Expected='{1}'    Actual='{2}'", diff.Description, diff.ExpectValue, diff.RealValue));
            }
            return sb.ToString();
        }
    }

    public class StudyComparer
    {
        static void InternalCompare(uint tag, string expected, string actual, int maxLength, ICollection<ComparisionDifference> list)
        {
            // reduce the strings if they exceed the max lengths
            if (maxLength > 0)
            {
                DicomTag dicomTag = null;
                if (!string.IsNullOrEmpty(expected) && expected.Length > maxLength)
                {
                    dicomTag = DicomTagDictionary.GetDicomTag(tag);
                    Platform.Log(LogLevel.Warn, string.Format("'{0}' exceeds max length allowed for {1}. Only first {2} characters are used for comparison", expected, dicomTag.Name, maxLength));
                    expected = expected.Substring(0, maxLength);
                }

                if (!string.IsNullOrEmpty(actual) && actual.Length > maxLength)
                {
                    dicomTag = dicomTag ?? DicomTagDictionary.GetDicomTag(tag);
                    Platform.Log(LogLevel.Warn, string.Format("'{0}' exceeds max length allowed for {1}. Only first {2} characters are used for comparison", actual, dicomTag.Name, maxLength));
                    actual = actual.Substring(0, maxLength);
                }
            }
            

            if (!StringUtils.AreEqual(expected, actual, StringComparison.InvariantCultureIgnoreCase))
            {
                list.Add(new ComparisionDifference(tag, expected, actual));
            }
            
        }

        public DifferenceCollection Compare(DicomMessageBase message, Study study, StudyCompareOptions options)
        {
            DifferenceCollection list = new DifferenceCollection();

            if (options.MatchIssuerOfPatientId)
            {
                InternalCompare(DicomTags.IssuerOfPatientId, study.IssuerOfPatientId,
                                message.DataSet[DicomTags.IssuerOfPatientId].ToString(), 64, list);
            }

            if (options.MatchPatientId)
            {
                InternalCompare(DicomTags.PatientId, study.PatientId,
                                message.DataSet[DicomTags.PatientId].ToString(), 64, list);
            }


            if (options.MatchPatientsName)
            {
                InternalCompare(DicomTags.PatientsName, study.PatientsName,
                                message.DataSet[DicomTags.PatientsName].ToString(), 64, list);
            }

            if (options.MatchPatientsBirthDate)
            {
                InternalCompare(DicomTags.PatientsBirthDate, study.PatientsBirthDate,
                                message.DataSet[DicomTags.PatientsBirthDate].ToString(), 8, list);
            }

            if (options.MatchPatientsSex)
            {
                InternalCompare(DicomTags.PatientsSex, study.PatientsSex,
                                message.DataSet[DicomTags.PatientsSex].ToString(), 2, list);
            }


            if (options.MatchAccessionNumber)
            {
                InternalCompare(DicomTags.AccessionNumber, study.AccessionNumber,
                                message.DataSet[DicomTags.AccessionNumber].ToString(), 16, list);
            }

            return list;
        }

    }
}
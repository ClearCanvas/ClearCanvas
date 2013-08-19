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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Validation
{
    public class StudyIntegrityValidator
    {
        /// <summary>
        /// Validates the state of the study.
        /// </summary>
        /// <param name="context">Name of the application</param>
        /// <param name="studyStorage">The study to validate</param>
        /// <param name="modes">Specifying what validation to execute</param>
        public void ValidateStudyState(String context, StudyStorageLocation studyStorage, StudyIntegrityValidationModes modes)
        {
            Platform.CheckForNullReference(studyStorage, "studyStorage");
            if (modes == StudyIntegrityValidationModes.None)
                return;

            using (var scope = new ServerExecutionContext())
            {
                // Force a re-load, the study may have changed if a delete happened.
                Study study = Study.Find(scope.PersistenceContext, studyStorage.Key);

                if (study!=null)
                {
                    StudyXml studyXml = studyStorage.LoadStudyXml();

                    if (modes == StudyIntegrityValidationModes.Default ||
                        (modes & StudyIntegrityValidationModes.InstanceCount) == StudyIntegrityValidationModes.InstanceCount)
                    {
                        if (studyXml != null && studyXml.NumberOfStudyRelatedInstances != study.NumberOfStudyRelatedInstances)
                        {
                            var validationStudyInfo = new ValidationStudyInfo(study, studyStorage.ServerPartition);
                            
                            throw new StudyIntegrityValidationFailure(
                                ValidationErrors.InconsistentObjectCount, validationStudyInfo,
                                String.Format("Number of instances in database and xml do not match: {0} vs {1}.",
                                    study.NumberOfStudyRelatedInstances,
                                    studyXml.NumberOfStudyRelatedInstances
                                ));
                        }
                    }
                }
            }
        }
    }
}

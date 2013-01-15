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
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.ImageViewer.Common.WorkItem
{
    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("b2dcf1f6-6e1a-48cd-b807-b720811a6575")]
    [WorkItemKnownType]
    public abstract class WorkItemProgress : DataContractBase
    {
        public virtual string Status { get { return string.Empty; } }

        [DataMember(IsRequired = false)]
        public string StatusDetails { get; set; }

        public virtual Decimal PercentComplete {get { return new decimal(0.0); }}

        public virtual Decimal PercentFailed { get { return new decimal(0.0); } }

        [DataMember(IsRequired = true)]
        public bool IsCancelable { get; set; }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("d3b84be6-edc7-40e1-911f-f6ec30c2a128")]
    [WorkItemKnownType]
    public class ImportFilesProgress : WorkItemProgress
    {
        public ImportFilesProgress()
        {
            IsCancelable = true;
        }

        [DataMember(IsRequired = true)]
        public int TotalFilesToImport { get; set; }

        [DataMember(IsRequired = true)]
        public int NumberOfFilesImported { get; set; }
   
        [DataMember(IsRequired = true)]
        public int NumberOfImportFailures { get; set; }

        [DataMember(IsRequired = true)]
        public int PathsToImport { get; set; }

        [DataMember(IsRequired = true)]
        public int PathsImported { get; set; }

        /// <summary>
        /// When the work item completes, this value is set to either true or false to indicate
        /// whether or not all the files were enumerated. If false, the work item terminated prematurely.
        /// </summary>
        [DataMember(IsRequired = true)]
        public bool? CompletedEnumeration { get; set; }

        public int TotalImportsProcessed
        {
            get { return NumberOfFilesImported + NumberOfImportFailures; }
        }

        public override string Status
        {
            get
            {
                //If enumeration didn't complete, don't give the user a false impression of either
                //the total number of files to import, or the total number of failures.
                //If there were failures, the status will be "Failed", and that's good enough.
                if (CompletedEnumeration.HasValue && !CompletedEnumeration.Value)
                {
                    return string.Format(SR.ImportFilesProgress_StatusEnumerationIncomplete, NumberOfFilesImported);
                }

                //If the work item hasn't completed yet, or if it is complete and all files were enumerated (and possibly failed),
                //only then do we report all the numbers (#imported, total, #failures).
                return string.Format(SR.ImportFilesProgress_Status, NumberOfFilesImported, TotalFilesToImport, NumberOfImportFailures);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                // TODO (CR Jun 2012 - Med): Shouldn't this be based on files, not paths?
                // (SW) Paths was used because we don't know the total file count up front, and didn't want to enumerate the files before processing
                // them.  This was a work around to show real progress.
                if (PathsToImport > 0)
                    return (Decimal)PathsImported / PathsToImport;

                return new decimal(0.0);
            }
        }

        public override Decimal PercentFailed
        {
            get
            {
                if (NumberOfImportFailures > 0)
                    return (Decimal)NumberOfImportFailures / TotalImportsProcessed;

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("8C994CAB-630E-4a92-AA5B-7BF5D6095D6D")]
    [WorkItemKnownType]
    public class ProcessStudyProgress : WorkItemProgress
    {
        public ProcessStudyProgress()
        {
            IsCancelable = false;
        }

        [DataMember(IsRequired = true)]
        public int TotalFilesToProcess { get; set; }

        [DataMember(IsRequired = true)]
        public int NumberOfFilesProcessed { get; set; }
   
        [DataMember(IsRequired = true)]
        public int NumberOfProcessingFailures { get; set; }

        public int TotalFilesProcessed
        {
            get { return NumberOfFilesProcessed + NumberOfProcessingFailures; }
        }

        public override string Status
        {
            get
            {
                var error = string.Format(SR.StudyProcessProgress_Status, NumberOfFilesProcessed, TotalFilesToProcess, NumberOfProcessingFailures);           
                return error;
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (TotalFilesToProcess > 0)
                    return (Decimal)TotalFilesProcessed / TotalFilesToProcess;

                return new decimal(0.0);
            }
        }

        public override Decimal PercentFailed
        {
            get
            {
                if (NumberOfProcessingFailures > 0)
                    return (Decimal) NumberOfProcessingFailures/TotalFilesToProcess;

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("A1F64C74-FAE9-4a4c-A120-E82EE45EA21B")]
    [WorkItemKnownType]
    public class ReindexProgress : WorkItemProgress
    {
        public ReindexProgress()
        {
            IsCancelable = true;
        }

        [DataMember(IsRequired = true)]
        public int StudiesToProcess { get; set; }

        [DataMember(IsRequired = true)]
        public int StudiesProcessed { get; set; }

        [DataMember(IsRequired = true)]
        public int TotalStudyFolders { get; set; }

        [DataMember(IsRequired = true)]
        public int StudyFoldersProcessed { get; set; }

        [DataMember(IsRequired = true)]
        public int StudiesDeleted { get; set; }

        [DataMember(IsRequired = true)]
        public int StudiesFailed { get; set; }

        [DataMember(IsRequired = true)]
        public bool Complete { get; set; }

        public override string Status
        {
            get
            {
                if (StudiesDeleted == 0 && StudiesToProcess == 0 && StudyFoldersProcessed == 0 && TotalStudyFolders == 0 && StudiesProcessed == 0 && StudiesFailed == 0)
                {
                    return Complete ? SR.ReindexProgress_StatusNoStudies : string.Empty;
                }

                return string.Format(SR.ReindexProgress_Status, StudiesProcessed, StudiesToProcess,
                    StudyFoldersProcessed, TotalStudyFolders, StudiesDeleted, StudiesFailed);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (Complete && StudiesToProcess == 0)
                    return new decimal(100.0);

                if (StudiesToProcess > 0 || TotalStudyFolders > 0)
                    return (Decimal)(StudiesProcessed + StudyFoldersProcessed) / (StudiesToProcess+TotalStudyFolders);

                return new decimal(0.0);
            }
        }

        public override Decimal PercentFailed
        {
            get
            {
                if ((StudiesToProcess > 0 || TotalStudyFolders > 0) && StudiesFailed > 0)
                    return (Decimal)StudiesFailed / (StudiesToProcess + TotalStudyFolders);

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("D7040F82-8021-420E-B72B-0890053BE8C5")]
    [WorkItemKnownType]
    public class ReapplyRulesProgress : WorkItemProgress
    {
        public ReapplyRulesProgress()
        {
            IsCancelable = false;
        }

        [DataMember(IsRequired = true)]
        public int TotalStudiesToProcess { get; set; }

        [DataMember(IsRequired = true)]
        public int StudiesProcessed { get; set; }

        [DataMember(IsRequired = true)]
        public bool Complete { get; set; }

        public override string Status
        {
            get
            {
                if (TotalStudiesToProcess == 0 && StudiesProcessed == 0)
                {
                    return Complete ? SR.ReapplyRulesProgress_StatusNoStudies : string.Empty;
                }

                return string.Format(SR.ReapplyRulesProgress_Status, StudiesProcessed, TotalStudiesToProcess);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (Complete && TotalStudiesToProcess == 0)
                    return new decimal(100.0);

                if (TotalStudiesToProcess > 0)
                    return (Decimal)StudiesProcessed / TotalStudiesToProcess ;

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("68BCA074-F1F1-4870-8ABD-281B31E10B6A")]
    [WorkItemKnownType]
    public class DicomSendProgress : WorkItemProgress
    {
        public DicomSendProgress()
        {
            IsCancelable = true;
        }

        [DataMember(IsRequired = true)]
        public int TotalImagesToSend { get; set; }

        [DataMember(IsRequired = true)]
        public int WarningSubOperations { get; set; }

        [DataMember(IsRequired = true)]
        public int FailureSubOperations { get; set; }

        [DataMember(IsRequired = true)]
        public int SuccessSubOperations { get; set; }

        public int RemainingSubOperations
        {
            get { return TotalImagesToSend - (WarningSubOperations + FailureSubOperations + SuccessSubOperations); }
        }

        public override string Status
        {
            get
            {
                if (TotalImagesToSend == 0)
                    return string.Empty;

                return string.Format(SR.DicomSendProgress_Status,
                                     SuccessSubOperations + WarningSubOperations, FailureSubOperations,
                                     RemainingSubOperations);
            }
        }

        // TODO (CR Jun 2012): Again, can "complete" get confused with the status?
        public override Decimal PercentComplete
        {
            get
            {
                if (TotalImagesToSend > 0)
                    return (Decimal) (WarningSubOperations + FailureSubOperations + SuccessSubOperations)/TotalImagesToSend;

                return new decimal(0.0);
            }
        }

        public override Decimal PercentFailed
        {
            get
            {
                if (TotalImagesToSend > 0 && FailureSubOperations > 0)
                    return (Decimal)FailureSubOperations / TotalImagesToSend;

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("24DB4BC0-2759-468E-802B-07C54F91A68D")]
    [WorkItemKnownType]
    public class DicomRetrieveProgress : WorkItemProgress
    {
        public DicomRetrieveProgress()
        {
            IsCancelable = true;
        }

        [DataMember(IsRequired = true)]
        public int ImagesToRetrieve { get; set; }

        [DataMember(IsRequired = true)]
        public int WarningSubOperations { get; set; }

        [DataMember(IsRequired = true)]
        public int FailureSubOperations { get; set; }

        [DataMember(IsRequired = true)]
        public int SuccessSubOperations { get; set; }

        public int RemainingSubOperations
        {
            get { return ImagesToRetrieve - (WarningSubOperations + FailureSubOperations + SuccessSubOperations); }
        }

        public override string Status
        {
            get
            {
                if (ImagesToRetrieve == 0)
                    return string.Empty;

                return string.Format(SR.DicomRetrieveProgress_Status,
                                     SuccessSubOperations + WarningSubOperations, FailureSubOperations,
                                     RemainingSubOperations);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (ImagesToRetrieve > 0)
                    return (Decimal)(WarningSubOperations + FailureSubOperations + SuccessSubOperations) / ImagesToRetrieve;

                return new decimal(0.0);
            }
        }

        public override Decimal PercentFailed
        {
            get
            {
                if (ImagesToRetrieve > 0 && FailureSubOperations > 0)
                    return (Decimal)FailureSubOperations / ImagesToRetrieve;

                return new decimal(0.0);
            }
        }
    }

    [DataContract(Namespace = ImageViewerWorkItemNamespace.Value)]
    [WorkItemProgressDataContract("179B5CD7-8C67-44C1-8211-5B800FE069C4")]
    [WorkItemKnownType]
    public class DeleteProgress : WorkItemProgress
    {
        public DeleteProgress()
        {
            IsCancelable = true;
        }

        [DataMember(IsRequired = true)]
        public int TotalImagesToDelete { get; set; }

        [DataMember(IsRequired = true)]
        public int ImagesDeleted { get; set; }

        public override string Status
        {
            get
            {
                if (TotalImagesToDelete == 0)
                    return string.Empty;

                if (ImagesDeleted == 1)
                    return string.Format(SR.DeleteProgress_Status,ImagesDeleted);

                return string.Format(SR.DeleteProgressPlural_Status, ImagesDeleted);
            }
        }

        public override Decimal PercentComplete
        {
            get
            {
                if (TotalImagesToDelete > 0)
                    return (Decimal)(ImagesDeleted) / TotalImagesToDelete;

                return new decimal(0.0);
            }
        }
    }
}

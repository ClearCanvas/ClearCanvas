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
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.Auditing;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyLoaders.Local
{
    [ExtensionOf(typeof(ServiceNodeServiceProviderExtensionPoint))]
    internal class StudyLoaderServiceProvider : ServiceNodeServiceProvider
    {
        private bool IsLocalServiceNode
        {
            get
            {
                var dicomServiceNode = Context.ServiceNode as IDicomServiceNode;
                return dicomServiceNode != null && dicomServiceNode.IsLocal && StudyStore.IsSupported;
            }
        }

        public override bool IsSupported(System.Type type)
        {
            return type == typeof (IStudyLoader) && IsLocalServiceNode;
        }

        public override object GetService(System.Type type)
        {
            return IsSupported(type) ? new LocalStoreStudyLoader() : null;
        }
    }

    //TODO (Marmot):Move once IStudyLoader is moved to Common?

    [ExtensionOf(typeof(StudyLoaderExtensionPoint))]
    public class LocalStoreStudyLoader : StudyLoader
    {
        private IEnumerator<ISopInstance> _sops;

        public LocalStoreStudyLoader() : base("DICOM_LOCAL")
        {
            int? frameLookAhead = PreLoadingSettings.Default.FrameLookAheadCount;
            if (PreLoadingSettings.Default.LoadAllFrames)
                frameLookAhead = null;

            var coreStrategy = new SimpleCorePrefetchingStrategy(frame => frame.ParentImageSop.DataSource is LocalStoreSopDataSource);
            PrefetchingStrategy = new WeightedWindowPrefetchingStrategy(coreStrategy, "DICOM_LOCAL", "Simple prefetcing strategy for local images.")
                                      {
                                          Enabled = PreLoadingSettings.Default.Enabled,
                                          RetrievalThreadConcurrency = PreLoadingSettings.Default.Concurrency,
                                          FrameLookAheadCount = frameLookAhead,
                                          SelectedImageBoxWeight = PreLoadingSettings.Default.SelectedImageBoxWeight,
                                          UnselectedImageBoxWeight = PreLoadingSettings.Default.UnselectedImageBoxWeight,
                                          DecompressionThreadConcurrency = 0
                                      };
        }

        protected override int OnStart(StudyLoaderArgs studyLoaderArgs)
        {
            _sops = null;

            EventResult result = EventResult.Success;
            var loadedInstances = new AuditedInstances();
            try
            {
                using (var context = new DataAccessContext())
                {
                    if (!studyLoaderArgs.Options.IgnoreInUse)
                    {
                        var workItemStatusFilter = WorkItemStatusFilter.StatusIn(
                            WorkItemStatusEnum.Pending,
                            WorkItemStatusEnum.InProgress,
                            WorkItemStatusEnum.Idle,
                            WorkItemStatusEnum.Failed);

                        var updateItems = context.GetWorkItemBroker().GetWorkItems(
                            WorkItemConcurrency.StudyUpdate,
                            workItemStatusFilter,
                            studyLoaderArgs.StudyInstanceUid);

                        var deleteItems = context.GetWorkItemBroker().GetWorkItems(
                            WorkItemConcurrency.StudyDelete,
                            workItemStatusFilter,
                            studyLoaderArgs.StudyInstanceUid);

                        var updateTriggerItems = context.GetWorkItemBroker().GetWorkItems(
                            WorkItemConcurrency.StudyUpdateTrigger,
                            workItemStatusFilter,
                            studyLoaderArgs.StudyInstanceUid);

                        if (updateItems.Any() || deleteItems.Any() || updateTriggerItems.Any())
                        {
                            var message = string.Format("There are work items actively modifying the study with UID '{0}'.", studyLoaderArgs.StudyInstanceUid);
                            throw new InUseLoadStudyException(studyLoaderArgs.StudyInstanceUid, message);
                        }
                    }

                    IStudy study = context.GetStudyBroker().GetStudy(studyLoaderArgs.StudyInstanceUid);
                    if (study == null)
                    {
                        result = EventResult.MajorFailure;
                        loadedInstances.AddInstance(studyLoaderArgs.StudyInstanceUid);
                        throw new NotFoundLoadStudyException(studyLoaderArgs.StudyInstanceUid);
                    }
                    loadedInstances.AddInstance(study.PatientId, study.PatientsName, study.StudyInstanceUid);

                    _sops = study.GetSopInstances().GetEnumerator();
                    return study.NumberOfStudyRelatedInstances;
                }
            }
            finally
            {
                AuditHelper.LogOpenStudies(new[] { AuditHelper.LocalAETitle }, loadedInstances, EventSource.CurrentUser, result);
            }
        }

        protected override SopDataSource LoadNextSopDataSource()
        {
            if (_sops == null)
                return null;

            if (!_sops.MoveNext())
            {
                _sops = null;
                return null;
            }

            return new LocalStoreSopDataSource(_sops.Current);
        }
    }
}
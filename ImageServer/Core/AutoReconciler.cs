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
using System.IO;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Command;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Helpers;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Core.Reconcile.CreateStudy;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core
{
    /// <summary>
    /// Class implementing <see cref="IStudyPreProcessor"/> to update a DICOM image
    /// based on past reconciliation.
    /// </summary>
    public class AutoReconciler : BasePreprocessor, IStudyPreProcessor
    {
        #region Private Fields
        private readonly static ServerCache<ServerEntityKey, UidMapper> _uidMapCache = new ServerCache<ServerEntityKey, UidMapper>(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(15));

        private readonly string _contextID;

        #endregion

        
        #region Private Members

        #endregion

        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="AutoReconciler"/> to update
        /// a DICOM file according to the history.
        /// </summary>
        /// <param name="contextID">Context ID of the operation. Used to identify files belonging to the same group (usually from the same association).</param>
        /// <param name="storageLocation"></param>
        public AutoReconciler(String contextID, StudyStorageLocation storageLocation) 
            : base("AUTO-RECONCILE", storageLocation)
        {
            Platform.CheckForEmptyString(contextID, "contextID");
            _contextID = contextID;
        } 
        #endregion

        #region IStudyPreProcessor Members

        /// <summary>
        /// Processes the specified <see cref="DicomFile"/> object.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="TargetStudyInvalidStateException">Thrown when the target study is in invalid state and cannot be updated.</exception>
        public InstancePreProcessingResult Process(DicomFile file)
        {
            Platform.CheckForNullReference(file, "file");

            AutoReconcilerResult preProcessingResult = null;
            
            // Update the file based on the reconciliation in the past
            IList<StudyHistory> histories = FindReconcileHistories(StorageLocation, file);
            if (histories != null && histories.Count > 0)
            {
                preProcessingResult = ApplyHistories(file, histories);
            }

            if (preProcessingResult!=null)
            {
                StringBuilder log = new StringBuilder();
                log.AppendLine(String.Format("AUTO-RECONCILE: {0}. SOP {1}", preProcessingResult.Action, file.MediaStorageSopInstanceUid));
                foreach (UpdateItem change in preProcessingResult.Changes)
                {
                    if (change.NewValue != null && !change.NewValue.Equals(change.OriginalValue))
                    {
                        log.AppendLine(String.Format("\tSet {0}: {1} => {2}", change.Tag, change.OriginalValue, change.NewValue));    
                    }
                }
                Platform.Log(LogLevel.Info, log.ToString());
               
            }
            return preProcessingResult;
        }

        #endregion

        #region Private Methods

        private AutoReconcilerResult ApplyHistories(DicomFile file, IList<StudyHistory> histories)
        {
            Platform.CheckForNullReference(file, "file");
            Platform.CheckForNullReference(histories, "histories");

            AutoReconcilerResult preProcessingResult = null;

            StudyHistory lastHistory = histories[0];
            var parser = new StudyReconcileDescriptorParser();
            StudyReconcileDescriptor changeLog = parser.Parse(lastHistory.ChangeDescription);

            switch (changeLog.Action)
            {
                case StudyReconcileAction.CreateNewStudy:
                case StudyReconcileAction.Merge:
                    if (lastHistory.DestStudyStorageKey != null)
                        preProcessingResult = MergeImage(changeLog.Action, file, lastHistory);
                    break;

                case StudyReconcileAction.Discard:
                    preProcessingResult = new AutoReconcilerResult(StudyReconcileAction.Discard) { DiscardImage = true };
                    break;

                case StudyReconcileAction.ProcessAsIs:
                    if (lastHistory.DestStudyStorageKey != null)
                    {
                        preProcessingResult = ProcessImageAsIs(file, lastHistory);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
            return preProcessingResult;
        }

        private AutoReconcilerResult ProcessImageAsIs(DicomFile file, StudyHistory lastHistory)
        {
            StudyStorage destinationStudy = StudyStorage.Load(lastHistory.DestStudyStorageKey);
        	StudyStorageLocation destStudy;
            var preProcessingResult = new AutoReconcilerResult(StudyReconcileAction.ProcessAsIs);

			//Load the destination.  An exception will be thrown if any issues are encountered.
        	FilesystemMonitor.Instance.GetWritableStudyStorageLocation(destinationStudy.ServerPartitionKey,
        	                                                           destinationStudy.StudyInstanceUid, StudyRestore.True,
        	                                                           StudyCache.True, out destStudy);

            bool belongsToAnotherStudy = !destStudy.Equals(StorageLocation);

            EnsureStudyCanBeUpdated(destStudy);

            if (belongsToAnotherStudy)
            {
                preProcessingResult.Changes = new List<UpdateItem>
                                                  {
                                                      new UpdateItem(DicomTags.StudyInstanceUid, file.DataSet[DicomTags.StudyInstanceUid].ToString(), destStudy.StudyInstanceUid)
                                                  };

                file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(destStudy.StudyInstanceUid);
                var importContext = new SopInstanceImporterContext(
                    _contextID,
                    file.SourceApplicationEntityTitle, destStudy.ServerPartition);
                var importer = new SopInstanceImporter(importContext);
                DicomProcessingResult result = importer.Import(file);

                if (!result.Successful)
                {
                    throw new ApplicationException("Unable to import image to destination study");
                }
            }
            return preProcessingResult;
        }

        private AutoReconcilerResult MergeImage(StudyReconcileAction action, DicomFile file, StudyHistory lastHistory)
        {
            string originalSeriesUid = file.DataSet[DicomTags.SeriesInstanceUid].ToString();
            string originalSopUid = file.DataSet[DicomTags.SopInstanceUid].ToString();
            
            AutoReconcilerResult preProcessingResult = null;
            UidMapper uidMapper = null;
        	if (lastHistory.DestStudyStorageKey != null)
            {
                StudyStorage destinationStudy = StudyStorage.Load(lastHistory.DestStudyStorageKey);

				//Load the destination.  An exception will be thrown if any issues are encountered.
                StudyStorageLocation destStudy;
                FilesystemMonitor.Instance.GetWritableStudyStorageLocation(destinationStudy.ServerPartitionKey,
																		   destinationStudy.StudyInstanceUid, 
																		   StudyRestore.True, StudyCache.True, 
																		   out destStudy);

                EnsureStudyCanBeUpdated(destStudy);

                bool belongsToAnotherStudy = !destStudy.Equals(StorageLocation);

                var commandBuilder = new ImageUpdateCommandBuilder();
                IList<BaseImageLevelUpdateCommand> commands = commandBuilder.BuildCommands<StudyMatchingMap>(destStudy);
                if (belongsToAnotherStudy)
                {
                    Platform.Log(LogLevel.Info, "AUTO-RECONCILE: Move SOP {0} to Study {1}, A#: {2}, Patient {3}", originalSopUid, destStudy.StudyInstanceUid, destStudy.Study.AccessionNumber, destStudy.Study.PatientsName);

                    // Load the Uid Map, either from cache or from disk
                    if (!_uidMapCache.TryGetValue(destStudy.Key, out uidMapper))
                    {
                        var mapXml = new UidMapXml();
                        mapXml.Load(destStudy);
                        uidMapper = new UidMapper(mapXml);

                        _uidMapCache.Add(destStudy.Key, uidMapper);
                    }

                    try
                    {
                        commands.Add(GetUidMappingCommand(StorageLocation, destStudy, uidMapper, originalSopUid, originalSeriesUid));
                    }
                    catch (InstanceAlreadyExistsException ex)
                    {
                        Platform.Log(LogLevel.Info, "An instance already exists with the SOP Instance Uid {0}", ex.SopInstanceUid);
                        preProcessingResult = new AutoReconcilerResult(StudyReconcileAction.Discard) { DiscardImage = true };

                        return preProcessingResult;
                    }
                }


                preProcessingResult = new AutoReconcilerResult(action) { Changes = GetUpdateList(file, commands) };

                UpdateImage(file, commands);

                // First, must update the map
                if (uidMapper != null && uidMapper.Dirty)
                {
                    UpdateUidMap(destStudy, uidMapper);
                }

                if (belongsToAnotherStudy)
                {
                    var importContext = new SopInstanceImporterContext(_contextID, file.SourceApplicationEntityTitle, destStudy.ServerPartition);
                    var importer = new SopInstanceImporter(importContext);
                    DicomProcessingResult result = importer.Import(file);

                    if (!result.Successful)
                    {
                        throw new ApplicationException(result.ErrorMessage);
                    }
                }


            }
            return preProcessingResult;
        }

        private static void EnsureStudyCanBeUpdated(StudyStorageLocation destStudy)
        {
            string reason;
            if (!destStudy.CanUpdate(out reason))
            {
                throw new TargetStudyInvalidStateException(reason) { StudyInstanceUid = destStudy.StudyInstanceUid };
            }
        }

        private static void UpdateUidMap(StudyStorageLocation dest, UidMapper uidMapper)
        {
            using (var processor = new ServerCommandProcessor("Update UID Mapping Processor"))
            {
                processor.AddCommand(new SaveUidMapXmlCommand(dest, uidMapper));
                if (!processor.Execute())
                {
                    throw new ApplicationException(String.Format("AUTO-RECONCILE Failed: Unable to update uid mapping. Reason: {0}", processor.FailureReason), processor.FailureException);
                }
            }
        }

        private static void UpdateImage(DicomFile file, IEnumerable<BaseImageLevelUpdateCommand> commands)
        {
            using (var processor = new ServerCommandProcessor("Update Image According to History"))
            {
                foreach (BaseImageLevelUpdateCommand cmd in commands)
                {
                    cmd.File = file;
                    processor.AddCommand(cmd);
                }

                if (!processor.Execute())
                {
                    throw new ApplicationException(String.Format("AUTO-RECONCILE Failed: Unable to update image to match target study. Reason: {0}", processor.FailureReason), processor.FailureException);
                }
            }
        }

        private static List<UpdateItem> GetUpdateList(DicomFile file, IEnumerable<BaseImageLevelUpdateCommand> commands)
        {
            var updateList = new List<UpdateItem>();
            foreach (BaseImageLevelUpdateCommand cmd in commands)
            {
                if (cmd.UpdateEntry != null && cmd.UpdateEntry.TagPath != null && cmd.UpdateEntry.TagPath.Tag != null)
                {
                    var item = new UpdateItem(cmd, file);
                    updateList.Add(item);
                }
            }
            return updateList;
        }

        private static SeriesSopUpdateCommand GetUidMappingCommand(StudyStorageLocation sourceStudy, StudyStorageLocation targetStudy, UidMapper uidMapper, string originalSopUid, string originalSeriesUid)
        {
            SeriesSopUpdateCommand cmd;
            string newSeriesUid = uidMapper.FindNewSeriesUid(originalSeriesUid);
            string newSopUid = uidMapper.FindNewSopUid(originalSopUid);
            if (string.IsNullOrEmpty(newSeriesUid) == false)
            {
                // Series was assigned new uid
                if (string.IsNullOrEmpty(newSopUid))
                {
                    // this is new instance
                    newSopUid = DicomUid.GenerateUid().UID;
                    uidMapper.AddSop(originalSopUid, newSopUid);
                    cmd = new SeriesSopUpdateCommand(sourceStudy, targetStudy, uidMapper);

                }
                else
                {
                    Platform.Log(LogLevel.Info, "Map SOP UID {0} to {1}", originalSopUid, newSopUid);
                    // this is duplicate
                    if (File.Exists(targetStudy.GetSopInstancePath(newSeriesUid, newSopUid)))
                    {
                        throw new InstanceAlreadyExistsException("Instance already exists")
                                  {
                                      SeriesInstanceUid = newSeriesUid,
                                      SopInstanceUid = newSopUid
                                  };

                    }

                    cmd = new SeriesSopUpdateCommand(sourceStudy, targetStudy, uidMapper);
                }
            }
            else
            {
                // this is new series
                newSopUid = DicomUid.GenerateUid().UID;
                uidMapper.AddSeries(sourceStudy.StudyInstanceUid, targetStudy.StudyInstanceUid, originalSeriesUid, newSopUid);
                cmd = new SeriesSopUpdateCommand(sourceStudy, targetStudy, uidMapper);

                Platform.Log(LogLevel.Info, "Map SOP UID {0} to {1}", originalSopUid, newSopUid);
            }

            return cmd;
        }

        private static IList<StudyHistory> FindReconcileHistories(StudyStorageLocation storageLocation, DicomMessageBase file)
        {
            var fileDesc = new ImageSetDescriptor(file.DataSet);

            var studyHistoryList = new List<StudyHistory>(
                StudyHistoryHelper.FindStudyHistories(storageLocation.StudyStorage,
                                                new[] { StudyHistoryTypeEnum.StudyReconciled }));

            IList<StudyHistory> reconcileHistories = studyHistoryList.FindAll(
                delegate(StudyHistory item)
                    {
                        var desc = XmlUtils.Deserialize<ImageSetDescriptor>(item.StudyData.DocumentElement);
                        return desc.Equals(fileDesc);
                    });

            if (reconcileHistories.Count == 0)
            {
                // no history found in cache... reload the list and search again one more time
                studyHistoryList = new List<StudyHistory>(
                    StudyHistoryHelper.FindStudyHistories(storageLocation.StudyStorage,
                                                    new[] { StudyHistoryTypeEnum.StudyReconciled }));

                reconcileHistories = studyHistoryList.FindAll(
                    delegate(StudyHistory item)
                        {
                            var desc = XmlUtils.Deserialize<ImageSetDescriptor>(item.StudyData.DocumentElement);
                            return desc.Equals(fileDesc);
                        });

            }

            return reconcileHistories;
        }


        #endregion
    }

    class AutoReconcilerResult : InstancePreProcessingResult
    {
        #region Private Members
        private readonly StudyReconcileAction _action;
        private IList<UpdateItem> _changes = new List<UpdateItem>();
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        public AutoReconcilerResult(StudyReconcileAction action)
        {
            AutoReconciled = true;
            _action = action;
        }

        /// <summary>
        /// Gets or sets the list of changes made when <see cref="AutoReconciler"/> was executed on a DICOM file.
        /// </summary>
        public IList<UpdateItem> Changes
        {
            get { return _changes; }
            set { _changes = value; }
        }

        /// <summary>
        /// Gets other action that was used when <see cref="AutoReconciler"/> was executed on a DICOM file.
        /// </summary>
        public StudyReconcileAction Action
        {
            get { return _action; }

        }

    }
}
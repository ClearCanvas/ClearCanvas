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
using System.ComponentModel;
using System.IO;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemFileImporter
{
    /// <summary>
    /// Background worker thread to import dicom files from a directory.
    /// </summary>
    internal class DirectoryImporterBackgroundProcess : BackgroundWorker
    {
        #region Private Fields
        private DateTime _startTimeStamp = Platform.Time;
        private readonly DirectoryImporterParameters _parms;
        private readonly List<string> _skippedStudies = new List<String>();
        private SopInstanceImporter _importer;

        private EventHandler<SopImportedEventArgs> _sopImportedHandlers;
        private EventHandler _restoreTriggerHandlers; 
        
        #endregion

        #region Constructors
        public DirectoryImporterBackgroundProcess(DirectoryImporterParameters parms)
        {
            Platform.CheckForNullReference(parms, "parms");
            Platform.CheckMemberIsSet(parms.Directory, "parms.Directory");
            Platform.CheckMemberIsSet(parms.PartitionAE, "parms.PartitionAE");
            Platform.CheckMemberIsSet(parms.Filter, "parms.Filter");
            
            _parms = parms;
            WorkerSupportsCancellation = true;
        }
        #endregion

        #region Events


        public event EventHandler RestoreTriggered
        {
            add { _restoreTriggerHandlers+= value; }
            remove { _restoreTriggerHandlers -= value; }
        }

        public event EventHandler<SopImportedEventArgs> SopImported
        {
            add { _sopImportedHandlers += value; }
            remove { _sopImportedHandlers -= value; }
        }
        #endregion

        #region Protected Methods

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = String.Format("Import Files to {0} [{1}]", 
                    _parms.PartitionAE, Thread.CurrentThread.ManagedThreadId);
            
            if (_parms.Directory.Exists)
            {
                
                int counter = 0;
                Platform.Log(LogLevel.Debug, "Importing dicom files from {0}", _parms.Directory.FullName);
                FileProcessor.Process(_parms.Directory.FullName, _parms.Filter,
                                      delegate(string filePath, out bool cancel)
                                      {
                                          if (CancellationPending || counter >= _parms.MaxImages)
                                          {
                                              cancel = true;
                                              return;
                                          }

                                          if (ProcessFile(filePath)> 0)
                                          {

                                              counter++;
                                          }

                                          cancel = false;
                                          if (_parms.Delay > 0)
                                            Thread.Sleep(TimeSpan.FromSeconds(_parms.Delay));
                                          
                                      }, true);

                if (counter > 0)
                    Platform.Log(LogLevel.Info, "{0} files have been successfully imported from {1}.", counter, _parms.Directory.FullName);

                DirectoryInfo[] subDirs = _parms.Directory.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    DirectoryUtility.DeleteEmptySubDirectories(subDir.FullName, true);
                    DirectoryUtility.DeleteIfEmpty(subDir.FullName);
                }

                
            }

            base.OnDoWork(e);
        }

        private int ProcessFile(string filePath)
        {
            int importedSopCount = 0;
            bool isDicomFile = false;
            bool skipped = false;
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                DicomFile file;
                try
                {
                    file = new DicomFile(filePath);
                    file.Load();
                    isDicomFile = true;

                    string studyInstanceUid;
                    if (file.DataSet[DicomTags.StudyInstanceUid].TryGetString(0, out studyInstanceUid))
                    {
                        skipped = _skippedStudies.Contains(studyInstanceUid);
                        if (!skipped)
                        {
                            InitializeImporter();

                            try
                            {
                                DicomProcessingResult result = _importer.Import(file);
                                if (result.Successful)
                                {
                                    if (result.Duplicate)
                                    {
                                        // was imported but is duplicate
                                    }
                                    else
                                    {
                                        importedSopCount = 1;
                                        Platform.Log(LogLevel.Info, "Imported SOP {0} to {1}", result.SopInstanceUid, _parms.PartitionAE);
                                        ProgressChangedEventArgs progress = new ProgressChangedEventArgs(100, result.SopInstanceUid);

                                        // Fire the imported event.
                                        SopImportedEventArgs args = new SopImportedEventArgs
                                                                    	{
                                                                    		StudyInstanceUid = result.StudyInstanceUid,
                                                                    		SeriesInstanceUid = result.SeriesInstanceUid,
                                                                    		SopInstanceUid = result.SopInstanceUid
                                                                    	};
                                    	EventsHelper.Fire(_sopImportedHandlers, this, args);

                                        OnProgressChanged(progress);
                                    }
                                }
                                else
                                {
                                    if (result.DicomStatus == DicomStatuses.StorageStorageOutOfResources)
                                    {
                                        if (result.RestoreRequested)
                                            EventsHelper.Fire(_restoreTriggerHandlers, this, null);

                                        Platform.Log(LogLevel.Info, "Images for study {0} cannot be imported at this time because: {1}", result.StudyInstanceUid, result.ErrorMessage);
                                        _skippedStudies.Add(result.StudyInstanceUid);
                                        skipped = true;
                                    }
                                    else
                                    {
										Platform.Log(LogLevel.Warn, "Failed to import {0} to {1} : {2}", filePath, _parms.PartitionAE, result.ErrorMessage);
                                    }
                                }
                            }
                            catch (DicomDataException ex)
                            {
                                // skip to next file, this file will be deleted
                                Platform.Log(LogLevel.Warn, ex, "Failed to import {0} to {1}: {2}", filePath, _parms.PartitionAE, ex.Message);
                                skipped = true;
                            }
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Sop does not contains Study Instance Uid tag");
                    }
                }
                catch(Exception ex)
                {
                    Platform.Log(LogLevel.Error, ex);
                    
                }
                finally
                {
                    try
                    {
                        if (importedSopCount > 0)
                        {
                            DeleteFile(fileInfo);
                        }
                        else if (!isDicomFile)
                        {
                            DeleteFile(fileInfo);
                        }
                        else
                        {
                            //is dicom file but could not be imported.
                            if (!skipped)
                                DeleteFile(fileInfo);
                        }
                    }
                    catch(IOException ex)
                    {
                        Platform.Log(LogLevel.Error, ex, "Unable to delete file after it has been imported: {0}", fileInfo.FullName);
                        // Raise alert because this file is stuck in the incoming folder and becomes a duplicate when it is imported again later on.
                        // Depending on the duplicate policy, SIQ may be filled with many duplicate entries.
                        ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, "File Importer", -1, null, TimeSpan.Zero,
                                             "The following file has been imported but could not be removed : {0}.\nError: {1}",
                                             fileInfo.FullName, ex.Message);
                    }

                }
            }

            return importedSopCount;
        }

        private static void DeleteFile(FileInfo file)
        {
            Platform.CheckForNullReference(file, "file");

            if (file.Exists)
            {
                if (file.IsReadOnly)
                    file.IsReadOnly = false;

                file.Delete();
            }
        }

        private void InitializeImporter()
        {
            if (_importer==null)
            {
                SopInstanceImporterContext context = new SopInstanceImporterContext(
                                String.Format("{0}_{1}", _parms.PartitionAE, _startTimeStamp.ToString("yyyyMMddhhmmss")),
                                _parms.PartitionAE, _parms.PartitionAE);

                _importer = new SopInstanceImporter(context);
            }
           
        }

        #endregion
    }
}
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
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Enterprise.Command;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Rules;
using ClearCanvas.ImageServer.Services.ServiceLock.PartitionReapplyRules;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemStudyProcess
{
    /// <summary>
    /// Class for reprocessing the rules engine for studies on a filesystem.
    /// </summary>
	public class FilesystemStudyProcessItemProcessor : BaseReapplyRulesServiceLockItemProcessor, IServiceLockItemProcessor, ICancelable
    {
        #region Private Members
        private readonly FilesystemReprocessStatistics _stats = new FilesystemReprocessStatistics();
        private readonly Dictionary<ServerPartition, ServerRulesEngine> _engines = new Dictionary<ServerPartition, ServerRulesEngine>();
		private readonly Dictionary<ServerPartition, ServerRulesEngine> _postArchivalEngines = new Dictionary<ServerPartition, ServerRulesEngine>();
        private readonly Dictionary<ServerPartition, ServerRulesEngine> _dataAccessEngine = new Dictionary<ServerPartition, ServerRulesEngine>();
        #endregion

        #region Private Methods

	    /// <summary>
        /// Reprocess a file systems
        /// </summary>
        /// <param name="filesystem"></param>
        private void ReprocessFilesystem(Filesystem filesystem)
        {
            var filesystemDir = new DirectoryInfo(filesystem.FilesystemPath);

            foreach (DirectoryInfo partitionDir in filesystemDir.GetDirectories())
            {
                ServerPartition partition;
                if (GetServerPartition(partitionDir.Name, out partition) == false)
                {
					if (!partitionDir.Name.EndsWith("_Incoming") && !partitionDir.Name.Equals("temp")
					 && !partitionDir.Name.Equals("ApplicationLog") && !partitionDir.Name.Equals("AlertLog"))
	                    Platform.Log(LogLevel.Error, "Unknown partition folder '{0}' in filesystem: {1}", partitionDir.Name,
                                 filesystem.Description);
                    continue;
                }

                // Since we found a partition, we should find a rules engine too.
                ServerRulesEngine engine = _engines[partition];
            	ServerRulesEngine postArchivalEngine = _postArchivalEngines[partition];
                ServerRulesEngine dataAccessEngine = _dataAccessEngine[partition];

				foreach (DirectoryInfo dateDir in partitionDir.GetDirectories())
				{
					if (dateDir.FullName.EndsWith("Deleted")
						|| dateDir.FullName.EndsWith(ServerPlatform.ReconcileStorageFolder))
						continue;

					foreach (DirectoryInfo studyDir in dateDir.GetDirectories())
					{
						String studyInstanceUid = studyDir.Name;
						try
						{
							StudyStorageLocation location = LoadReadableStorageLocation(partition.GetKey(), studyInstanceUid);
							if (location == null)
							{
								foreach (DirectoryInfo seriesDir in studyDir.GetDirectories())
								{
									FileInfo[] sopInstanceFiles = seriesDir.GetFiles("*.dcm");

									DicomFile file = null;
									foreach (FileInfo sopFile in sopInstanceFiles)
									{
										if (!sopFile.FullName.EndsWith(ServerPlatform.DicomFileExtension))
											continue;

										try
										{
											file = new DicomFile(sopFile.FullName);
                                            file.Load(DicomTags.StudyId, DicomReadOptions.DoNotStorePixelDataInDataSet | DicomReadOptions.Default);
											break;
										}
										catch (Exception e)
										{
											Platform.Log(LogLevel.Warn, e, "Unexpected failure loading file: {0}.  Continuing to next file.",
											             sopFile.FullName);
											file = null;
										}
									}
									if (file != null)
									{
										studyInstanceUid = file.DataSet[DicomTags.StudyInstanceUid].ToString();
										break;
									}
								}

								location = LoadReadableStorageLocation(partition.GetKey(), studyInstanceUid);
								if (location == null)
									continue;
							}

                            ProcessStudy(partition, location, engine, postArchivalEngine, dataAccessEngine);
							_stats.NumStudies++;

							if (CancelPending) return;
						}
						catch (Exception e)
						{
							Platform.Log(LogLevel.Error, e,
							             "Unexpected error while processing study: {0} on partition {1}.", studyInstanceUid,
							             partition.Description);
						}
					}

					// Cleanup the directory, if its empty.
					DirectoryUtility.DeleteIfEmpty(dateDir.FullName);
				}
            }
        }

        /// <summary>
        /// Get a ServerPartition object from the list of loaded partitions.
        /// </summary>
        /// <param name="partitionFolderName">The folder for the partition.</param>
        /// <param name="partition">The ServerPartition object.</param>
        /// <returns>true on success.</returns>
        private bool GetServerPartition(string partitionFolderName, out ServerPartition partition)
        {
            foreach (ServerPartition part in _engines.Keys)
            {
                if (part.PartitionFolder == partitionFolderName)
                {
                    partition = part;
                    return true;
                }
            }

            partition = null;
            return false;
        }

        /// <summary>
        /// Load the <see cref="ServerRulesEngine"/> for each partition.
        /// </summary>
        private void LoadRulesEngine()
        {
			using (var context = new ServerExecutionContext())
			{
				var broker = context.ReadContext.GetBroker<IServerPartitionEntityBroker>();
				var criteria = new ServerPartitionSelectCriteria();
				IList<ServerPartition> partitions = broker.Find(criteria);

				foreach (ServerPartition partition in partitions)
				{
					var engine = new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyProcessed, partition.Key);
                    engine.Load();
                    _engines.Add(partition, engine);
					
					engine = new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyArchived, partition.Key);
                    engine.Load();
                    _postArchivalEngines.Add(partition, engine);

                    engine = new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyProcessed, partition.Key);
				    engine.AddIncludeType(ServerRuleTypeEnum.DataAccess);
                    engine.Load(); 
                    _dataAccessEngine.Add(partition, engine);
                    
				}
			}
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Process the <see cref="ServiceLock"/> item.
        /// </summary>
        /// <param name="item"></param>
        protected override void OnProcess(Model.ServiceLock item)
        {
        	LoadRulesEngine();

        	ServerFilesystemInfo info = FilesystemMonitor.Instance.GetFilesystemInfo(item.FilesystemKey);

        	_stats.StudyRate.Start();
        	_stats.Filesystem = info.Filesystem.Description;

        	Platform.Log(LogLevel.Info, "Starting reprocess of filesystem: {0}", info.Filesystem.Description);

        	ReprocessFilesystem(info.Filesystem);

        	Platform.Log(LogLevel.Info, "Completed reprocess of filesystem: {0}", info.Filesystem.Description);

        	_stats.StudyRate.SetData(_stats.NumStudies);
        	_stats.StudyRate.End();

        	StatisticsLogger.Log(LogLevel.Info, _stats);

        	item.ScheduledTime = item.ScheduledTime.AddDays(1);

			if (CancelPending)
			{
				Platform.Log(LogLevel.Info,
				             "Filesystem Reprocess of {0} has been canceled, rescheduling.  Entire filesystem will be reprocessed.",
				             info.Filesystem.Description);
				UnlockServiceLock(item, true, Platform.Time.AddMinutes(1));
			}
			else
	        	UnlockServiceLock(item, false, Platform.Time.AddDays(1));
        }

    	public new void Dispose()
        {
            base.Dispose();
        }
        #endregion
    }
}

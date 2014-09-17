-- WorkQueueTypeEnum inserts
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'StudyProcess','Process Study','Processing of a new incoming study.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'AutoRoute','Auto Route','DICOM Auto-route request.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'DeleteStudy','Delete Study','Automatic deletion of a Study.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'WebDeleteStudy','Web Delete Study','Manual study delete via the Web UI.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'WebMoveStudy','Web Move Study','Manual DICOM move of a study via the Web UI.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),105,'WebEditStudy','Web Edit Study','Manual study edit via the Web UI.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),106,'CleanupStudy','Cleanup Study','Cleanup all unprocessed or failed instances within a study.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),107,'CompressStudy','Compress Study','Compress a study.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),108,'MigrateStudy','Study Tier Migration','Migrate studies between tiers.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),109,'PurgeStudy','Purge Study','Purge archived study and place offline.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),110,'ReprocessStudy','Reprocess Study','Reprocess an entire study.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),111,'ReconcileStudy','Reconcile Study','Reconcile images.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),112,'ReconcileCleanup','Cleanup Failed Reconcile Study','Cleanup a failed Reconcile Study entry')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),113,'ReconcilePostProcess','Process Reconciled Images','Process reconciled images.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),114,'ProcessDuplicate','Process Duplicate','Process duplicate.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),115,'CleanupDuplicate','Cleanup Duplicate','Cleanup failed ProcessDuplicate entry.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),116,'ExternalEdit','External Edit','Edit request trigger by an external application.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),117,'StudyAutoRoute','Study Auto Route','DICOM Auto-route request of a Study.')
GO


--  WorkQueuePriorityEnum inserts
INSERT INTO [ImageServer].[dbo].WorkQueuePriorityEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES     (newid(),100,'Low','Low','Low priority')
GO

INSERT INTO [ImageServer].[dbo].WorkQueuePriorityEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES     (newid(),200,'Medium','Medium','Medium priority')
GO

INSERT INTO [ImageServer].[dbo].WorkQueuePriorityEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES     (newid(),300,'High','High','High priority')
GO

INSERT INTO [ImageServer].[dbo].WorkQueuePriorityEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES     (newid(),400,'Stat','Stat','Stat priority')
GO


-- WorkQueueTypeProperties inserts
  -- StudyProcess
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (100,300,1,1,3,5,60,60,120,30,3000,105,5,0,1)
GO
  -- AutoRoute
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (101,300,1,1,3,10,180,60,120,120,-1,101,0,1,0)
GO
  -- DeleteStudy
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (102,200,0,1,3,30,180,60,120,15,-1,102,6,0,1)
GO
  -- WebDeleteStudy
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (103,200,0,1,3,30,180,60,120,15,-1,103,6,0,1) 
GO

  -- WebEditStudy
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (105,300,1,1,3,30,180,60,120,15,-1,104,3,0,1)
GO

  -- WebMoveStudy
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (104,200,1,1,3,30,180,60,120,15,-1,101,0,1,0)
GO

  -- CleanupStudy
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (106,200,0,1,3,60,180,60,120,15,-1,109,2,0,1)
GO

  -- CompressStudy
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (107,200,1,1,2,30,180,60,120,120,300,110,2,0,1)
GO
  -- MigrateStudy
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (108,100,0,1,3,60,180,60,120,15,300,111,1,0,1)
GO
  -- PurgeStudy
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (109,200,0,1,3,60,180,60,120,15,300,106,1,0,1)
GO
  -- ReprocessStudy
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (110,200,1,1,3,60,180,60,120,15,300,112,10,0,1)
GO
  -- ReconcileStudy
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (111,200,1,1,3,60,180,60,120,15,300,107,4,0,1)
GO
  -- ReconcileCleanup
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (112,200,1,1,3,60,180,60,120,15,300,107,3,0,1)
GO
  -- ReconcilePostProcess
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (113,200,1,1,3,60,180,60,120,15,300,107,4,0,1)
GO
  -- ProcessDuplicate
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (114,200,1,1,3,60,180,60,120,120,300,107,4,0,1)
GO
  -- CleanupDuplicate
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (115,200,1,0,3,60,180,60,120,120,300,105,4,0,1)
GO
  -- ExternalEdit
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (116,300,0,1,1,30,180,60,120,240,-1,101,3,0,1)
GO
  -- StudyAutoRoute
INSERT INTO [ImageServer].[dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (117,300,1,1,3,10,180,60,120,15,-1,101,0,1,0)
GO

-- WorkQueueStatusEnum inserts
INSERT INTO [ImageServer].[dbo].[WorkQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'Idle','Idle','Waiting to expire or for more images')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),200,'Pending','Pending','Pending')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),201,'In Progress','In Progress','In Progress')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),202,'Completed','Completed','The Queue entry is completed.')
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),203,'Failed','Failed','The Queue entry has failed.')
GO


-- FilesystemTierEnum
INSERT INTO [ImageServer].[dbo].[FilesystemTierEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'Tier1','Tier 1','Filesystem Tier 1')
GO

INSERT INTO [ImageServer].[dbo].[FilesystemTierEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'Tier2','Tier 2','Filesystem Tier 2')
GO

INSERT INTO [ImageServer].[dbo].[FilesystemTierEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'Tier3','Tier 3','Filesystem Tier 3')
GO


-- ServerRuleTypeEnum inserts
INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'AutoRoute','Auto Routing','A DICOM auto-routing rule')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'StudyDelete','Study Delete','A rule to specify when to delete a study')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'Tier1Retention','Tier1 Retention','A rule to specify how long a study will be retained on Tier1')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'OnlineRetention','Online Retention','A rule to specify how long a study will be retained online')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'StudyCompress','Study Compress','A rule to specify when a study should be compressed')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),105,'SopCompress','SOP Compress','A rule to specify when a SOP Instance should be compressed (during initial processing)')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),106,'DataAccess','Data Access','A rule to specify the Authority Groups that have access to a study')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),107,'StudyQualityControl','Study Quality Control','A rule for quality control purposes when studies are received')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),108,'StudyAutoRoute','Study Auto Routing','A DICOM auto-routing rule for studies')
GO


-- ServerRuleApplyTimeEnum inserts
INSERT INTO [ImageServer].[dbo].[ServerRuleApplyTimeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'SopReceived','SOP Received','Apply rule when a SOP Instance has been received')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleApplyTimeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'SopProcessed','SOP Processed','Apply rule when a SOP Instance has been processed')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleApplyTimeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'SeriesProcessed','Series Processed','Apply rule when a Series is initially processed')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleApplyTimeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'StudyProcessed','Study Processed','Apply rule after a Study has been processed')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleApplyTimeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'StudyArchived','Study Archived','Apply rule after a Study is archived')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleApplyTimeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),105,'StudyRestored','Study Restored','Apply rule after a Study has been restored')
GO

INSERT INTO [ImageServer].[dbo].[ServerRuleApplyTimeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),106,'SopEdited','SOP Edited','Apply rule when a SOP Instance is edited')
GO


-- FilesystemQueueTypeEnum inserts
INSERT INTO [ImageServer].[dbo].[FilesystemQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'DeleteStudy','Delete Study','A record telling when a study is eligible for deletion.  The study will be completely removed from the system.')
GO
           
INSERT INTO [ImageServer].[dbo].[FilesystemQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'PurgeStudy','Purge Study','A record telling when a study can be purged from a filesystem.  Only archived studies can be purged.  The study will remain archived and can be restored.')
GO

INSERT INTO [ImageServer].[dbo].[FilesystemQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'TierMigrate','Tier Migrate','A record telling when a study is eligible to be migrated to a lower tier filesystem.')
GO

INSERT INTO [ImageServer].[dbo].[FilesystemQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'LosslessCompress','Lossless Compress','A record telling when a study is eligible for lossless compression and the type of compression to be performed on the study.')
GO

INSERT INTO [ImageServer].[dbo].[FilesystemQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'LossyCompress','Lossy Compress','A record telling when a study is eligible for lossy compression and the type of compression to be performed.')
GO


-- ServiceLockTypeEnum inserts
INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'FilesystemDelete', 'Filesystem Watermark Check', 'This services checks if a filesystem is above its high watermark.  If the filesystem is above the high watermark it migrates studies, deletes studies, and purges studies until the low watermark is reached.')																		 
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'FilesystemReinventory','Filesystem Reinventory','This service re-inventories the studies stored on a filesystem.  It scans the contents of the filesystem, and if a study is not already stored in the database, it will insert records to process the study into the WorkQueue.')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'FilesystemStudyProcess','Filesystem Reapply Rules','This service scans the contents of a filesystem and reapplies Study Processing rules to all studies on the filesystem that have not been archived.  Studies that have been archived will have Study Archived and Data Access rules applied.')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'FilesystemLosslessCompress','Filesystem Lossless Compress','This service checks for studies that are eligible to be lossless compressed on a filesystem.  It works independently from the watermarks configured for the filesystem and will insert records into the WorkQueue to compress the studies as soon as they are eligible.')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'FilesystemLossyCompress','Filesystem Lossy Compress','This service checks for studies that are eligible to be lossy compressed on a filesystem.  It works independently from the watermarks configured for the filesystem and will insert records into the WorkQueue to compress the studies as soon as they are eligible.')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),105,'FilesystemRebuildXml','Filesystem Rebuild Study XML','Rebuild the Study XML file for each study stored on the Filesystem')
GO
INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),200,'ArchiveApplicationLog','Archive Application Log','This service removes application log entries from the database and archives them in zip files to a filesystem.  When initially run, it selects a filesystem from the lowest filesystem tier configured on the system.')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),201,'PurgeAlerts','Purge Alerts','This service by default removes Alert records from the database after a configurable time.  If configured it can save the alerts in zip files on a filesystem.  When initially run, it selects a filesystem from the lowest filesystem tier configured on the system to archive to.')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),202,'ImportFiles','Import Dicom Files','This service periodically scans the filesystem for dicom files and imports them into the system.')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),300,'SyncDataAccess','Synchronize Data Access','This service periodically synchronizes the deletion status of Authority Groups on the Enterprise Services with Data Access granted to studies on the ImageServer.')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),301,'ExternalRequestProcess','Process External Requests','This service processes requests made to the ImageServer from external applications.')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),302,'ExternalNotificationProcess','Process External Notifications','This service processes notifications to send to external applications.')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),303,'PartitionOrderPurge','Partition Order Purge','This service purges orders not linked to studies on a partition.')
GO

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),401,'PartitionReapplyRules','Partition Reapply Rules','This service scans the contents of a partition and reapplies Study Processing rules to all studies on the partition that have not been archived.  Studies that have been archived will have Study Archived and Data Access rules applied.')
GO

-- ServiceLock Entries not associated with a Filesystem
INSERT INTO [ImageServer].[dbo].ServiceLock
	([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
VALUES (newid(),200,0,getdate(),null,1)

INSERT INTO [ImageServer].[dbo].ServiceLock
	([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
VALUES (newid(),201,0,getdate(),null,1)

INSERT INTO [ImageServer].[dbo].ServiceLock
	([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
VALUES (newid(),202,0,getdate(),null,1)


-- ServerSopClass inserts
INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.1.1', '12-lead ECG Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.1.3', 'Ambulatory ECG Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.11', 'Basic Text SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.4.1', 'Basic Voice Audio Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.4', 'Blending Softcopy Presentation State Storage SOP Class', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.3.1', 'Cardiac Electrophysiology Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.65', 'Chest CAD SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.2', 'Color Softcopy Presentation State Storage SOP Class', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.33', 'Comprehensive SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1', 'Computed Radiography Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.2', 'CT Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.3', 'Deformable Spatial Registration Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.3', 'Digital Intra-oral X-Ray Image Storage – For Presentation', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.3.1', 'Digital Intra-oral X-Ray Image Storage – For Processing', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.2', 'Digital Mammography X-Ray Image Storage – For Presentation', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.2.1', 'Digital Mammography X-Ray Image Storage – For Processing', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.1', 'Digital X-Ray Image Storage – For Presentation', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.1.1.1', 'Digital X-Ray Image Storage – For Processing', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.104.1', 'Encapsulated PDF Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.2.1', 'Enhanced CT Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.4.1', 'Enhanced MR Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.22', 'Enhanced SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.1.1', 'Enhanced XA Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.2.1', 'Enhanced XRF Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.1.2', 'General ECG Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.1', 'Grayscale Softcopy Presentation State Storage SOP Class', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.1.29', 'Hardcopy  Grayscale Image Storage SOP Class (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.1.30', 'Hardcopy Color Image Storage SOP Class (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9.2.1', 'Hemodynamic Waveform Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.59', 'Key Object Selection Document', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.50', 'Mammography CAD SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.4', 'MR Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.4.2', 'MR Spectroscopy Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.2', 'Multi-frame Grayscale Byte Secondary Capture Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.3', 'Multi-frame Grayscale Word Secondary Capture Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.1', 'Multi-frame Single Bit Secondary Capture Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7.4', 'Multi-frame 1 Color Secondary Capture Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.5', 'Nuclear Medicine Image  Storage (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.20', 'Nuclear Medicine Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.5.2', 'Ophthalmic Photography 16 Bit Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.5.1', 'Ophthalmic Photography 8 Bit Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.128', 'Positron Emission Tomography Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.40', 'Procedure Log Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11.3', 'Pseudo-Color Softcopy Presentation State Storage SOP Class', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66', 'Raw Data Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.67', 'Real World Value Mapping Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.4', 'RT Beams Treatment Record Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.6', 'RT Brachy Treatment Record Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.2', 'RT Dose Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.1', 'RT Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.9', 'RT Ion Beams Treatment Record Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.8', 'RT Ion Plan Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.5', 'RT Plan Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage], [ImplicitOnly])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.3', 'RT Structure Set Storage', 1, 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.481.7', 'RT Treatment Summary Record Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.7', 'Secondary Capture Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.4', 'Segmentation Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.2', 'Spatial Fiducials Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.66.1', 'Spatial Registration Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.9', 'Standalone Curve Storage (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.10', 'Standalone Modality LUT Storage (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.8', 'Standalone Overlay Storage (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.129', 'Standalone PET Curve Storage (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.11', 'Standalone VOI LUT Storage (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.5.3', 'Stereometric Relationship Storage', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.1.27', 'Stored Print Storage SOP Class (Retired)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.6.1', 'Ultrasound Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.6', 'Ultrasound Image Storage (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.3.1', 'Ultrasound Multi-frame Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.3', 'Ultrasound Multi-frame Image Storage (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.1.1', 'Video Endoscopic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.2.1', 'Video Microscopic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.4.1', 'Video Photographic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.1', 'VL Endoscopic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.2', 'VL Microscopic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.4', 'VL Photographic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.3', 'VL Slide-Coordinates Microscopic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.3', 'X-Ray Angiographic Bi-Plane Image Storage (Retired)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.1', 'X-Ray Angiographic Image Storage', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.88.67', 'X-Ray Radiation Dose SR', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.12.2', 'X-Ray Radiofluoroscopic Image Storage', 0)
GO
     
INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.13.1.1', 'X-Ray 3D Angiographic Image Storage', 0);
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.13.1.2', 'X-Ray 3D Craniofacial Image Storage', 0);
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.77.1.5.4', 'Ophthalmic Tomography Image Storage', 0);
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.104.2', 'Encapsulated CDA Storage', 1);
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.13.1.3', 'Breast Tomosynthesis Image Storage', 0);
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.6.2', 'Enhanced US Volume Storage', 0);
GO

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (newid(), '1.2.840.10008.5.1.4.1.1.130', 'Enhanced PET Storage', 0);
GO

-- ServerTransferSyntax inserts
INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.2', 'Explicit VR Big Endian', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.1', 'Explicit VR Little Endian', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2', 'Implicit VR Little Endian: Default Transfer Syntax for DICOM', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.4.91', 'JPEG 2000 Image Compression', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.4.90', 'JPEG 2000 Image Compression (Lossless Only)', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.4.50', 'JPEG Baseline (Process 1)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.4.51', 'JPEG Extended (Process 2 & 4)', 0)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.4.70', 'JPEG Lossless, non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1])', 1)
GO

INSERT INTO [ImageServer].[dbo].[ServerTransferSyntax] ([GUID],[Uid],[Description],[Lossless])
VALUES (newid(), '1.2.840.10008.1.2.5', 'RLE Lossless', 1)
GO


-- [StudyStatusEnum] inserts
INSERT INTO [ImageServer].[dbo].[StudyStatusEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),100,'Online','Online','Study is online')
GO

INSERT INTO [ImageServer].[dbo].[StudyStatusEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),101,'OnlineLossless','Online (Lossless)','Study is online and lossless compressed')
GO

INSERT INTO [ImageServer].[dbo].[StudyStatusEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),102,'OnlineLossy','Online (Lossy)','Study is online and lossy compressed')
GO

INSERT INTO [ImageServer].[dbo].[StudyStatusEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),103,'Nearline','Nearline','The study is nearline (in an automated library)')
GO


-- DuplicateSopPolicyEnum inserts
INSERT INTO [ImageServer].[dbo].DuplicateSopPolicyEnum([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),100,'SendSuccess','Send Success','Send a DICOM C-STORE-RSP success status when receiving a duplicate, but ignore the file.')
GO

INSERT INTO [ImageServer].[dbo].DuplicateSopPolicyEnum([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),101,'RejectDuplicates','Reject Duplicates','Send a DICOM C-STORE-RSP reject status when receiving a duplicate.')
GO

INSERT INTO [ImageServer].[dbo].DuplicateSopPolicyEnum([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),103,'CompareDuplicates','Compare Duplicates','Process duplicate objects received and compare them to originals flagging any differences as a failure.')
GO

INSERT INTO [ImageServer].[dbo].DuplicateSopPolicyEnum([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),104,'AcceptLatest','Accept Latest','Process duplicate objects received and always accecpt the latest file received.')
GO


-- ArchiveQueueStatusEnum inserts
INSERT INTO [ImageServer].[dbo].[ArchiveQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),100,'Pending','Pending','Pending')
GO

INSERT INTO [ImageServer].[dbo].[ArchiveQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),101,'In Progress','In Progress','In Progress')
GO

INSERT INTO [ImageServer].[dbo].[ArchiveQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),102,'Completed','Completed','The Queue entry is completed.')
GO

INSERT INTO [ImageServer].[dbo].[ArchiveQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),103,'Failed','Failed','The Queue entry has failed.')
GO


-- RestoreQueueStatusEnum inserts
INSERT INTO [ImageServer].[dbo].[RestoreQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),100,'Pending','Pending','Pending')
GO

INSERT INTO [ImageServer].[dbo].[RestoreQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),101,'In Progress','In Progress','In Progress')
GO

INSERT INTO [ImageServer].[dbo].[RestoreQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),102,'Completed','Completed','The Queue entry is completed.')
GO

INSERT INTO [ImageServer].[dbo].[RestoreQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),103,'Failed','Failed','The Queue entry has failed.')
GO

INSERT INTO [ImageServer].[dbo].[RestoreQueueStatusEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),104,'Restoring','Restoring','The Queue entry is waiting for the study to be restored by the archive.')
GO


-- ArchiveTypeEnum inserts
INSERT INTO [ImageServer].[dbo].[ArchiveTypeEnum] ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES (newid(),100,'HsmArchive','HSM Archive','Hierarchical storage management archive such as StorageTek QFS')
GO

-- AlertCategoryEnum inserts
INSERT INTO [ImageServer].[dbo].[AlertCategoryEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),1001,'System','System alert','System alert')
GO
INSERT INTO [ImageServer].[dbo].[AlertCategoryEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),1002,'Application','Application alert','Application alert')
GO
INSERT INTO [ImageServer].[dbo].[AlertCategoryEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),1003,'Security','Security alert','Security alert')
GO
INSERT INTO [ImageServer].[dbo].[AlertCategoryEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),1004,'User','User alert','User alert')
GO

-- AlertLevelEnum inserts
INSERT INTO [ImageServer].[dbo].[AlertLevelEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),1001,'Informational','Informational','Informational alert')
GO
INSERT INTO [ImageServer].[dbo].[AlertLevelEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),1002,'Warning','Warning','Warning alert')
GO
INSERT INTO [ImageServer].[dbo].[AlertLevelEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),1003,'Error','Error','Error alert')
GO
INSERT INTO [ImageServer].[dbo].[AlertLevelEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),1004,'Critical','Critical','Critical alert')
GO

-- QueueStudyStateEnum inserts
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),101,'Idle','Idle','The study currently does not have any queue entries.')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),102,'DeleteScheduled','Delete Scheduled','The study is scheduled for deletion')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),103,'WebDeleteScheduled','Web Delete Scheduled','The study or a series is scheduled for deletion.')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),104,'EditScheduled','Edit Scheduled','The study is scheduled for editing')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),105,'ProcessingScheduled','Processing','The study is being processed')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),106,'PurgeScheduled','Purge Scheduled','The study has been scheduled for purging')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),107,'ReconcileScheduled','Reconcile Scheduled','The study has been scheduled for reconciliation')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),108,'ReconcileRequired','Reconcile Required','The study needs to be reconciled')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),109,'CleanupScheduled','Cleanup Scheduled','A WorkQueue entry for the study needs to be cleaned up')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),110,'CompressScheduled','Compress Scheduled','The study is scheduled for compression')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),111,'MigrationScheduled','Migration Scheduled','The study is scheduled for migration to a new tier of storage')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),112,'ReprocessScheduled','Reprocess Scheduled','The study is scheduled for reprocessing')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),113,'RestoreScheduled','Restore Scheduled','The study is scheduled for restore')
GO
INSERT INTO [ImageServer].[dbo].[QueueStudyStateEnum]([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES(newid(),114,'ArchiveScheduled','Archive Scheduled','The study is scheduled for archiving')
GO

-- StudyIntegrityReasonEnum inserts
INSERT INTO [ImageServer].[dbo].[StudyIntegrityReasonEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'InconsistentData','Inconsistent Data','Images must be reconciled because of inconsistent data.')
GO

-- StudyIntegrityReasonEnum inserts
INSERT INTO [ImageServer].[dbo].[StudyIntegrityReasonEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'Duplicate','Duplicate','Duplicates were received and need to be reconciled.')
GO

-- StudyHistoryTypeEnum inserts
INSERT INTO [ImageServer].[dbo].[StudyHistoryTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'StudyReconciled','Study was reconciled','Demographics in the orginal images were modified to match against another study on the server.')
GO

INSERT INTO [ImageServer].[dbo].[StudyHistoryTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),200,'WebEdited','Web GUI Edited','Study was edited via the Web GUI')
GO

INSERT INTO [ImageServer].[dbo].[StudyHistoryTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),201,'Duplicate','Duplicate  Processsed','Duplicate was received and processed.')
GO

INSERT INTO [ImageServer].[dbo].[StudyHistoryTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),202,'Reprocessed','Study was reprocessed','Study was reprocessed.')
GO

INSERT INTO [ImageServer].[dbo].[StudyHistoryTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),203,'SeriesDeleted','One or more series was deleted','One or more series was deleted manually.')
GO
INSERT INTO [ImageServer].[dbo].[StudyHistoryTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),204,'ExternalEdit','External Edit','Study was edited via an external request.')
GO
INSERT INTO [ImageServer].[dbo].[StudyHistoryTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),205,'StudyCompress','Study Compress','Study was compressed.')
GO
INSERT INTO [ImageServer].[dbo].[StudyHistoryTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),206,'SopCompress','SOP Compress','Study was compressed by a SOP Compress rule.')
GO

-- Canned Text
INSERT INTO [ImageServer].[dbo].[CannedText]([GUID],[Label],[Category],[Text])
     VALUES(newid(), 'Corrupted study', 'DeleteStudyReason', 'Study is corrupted.')
GO

INSERT INTO [ImageServer].[dbo].[CannedText]([GUID],[Label],[Category],[Text])
     VALUES(newid(), 'Invalid data', 'DeleteStudyReason', 'Study contains some invalid data.')
GO
     
INSERT INTO [ImageServer].[dbo].[CannedText]([GUID],[Label],[Category],[Text])
     VALUES(newid(), 'Corrupted series', 'DeleteSeriesReason', 'Series is corrupted.')
GO
     
INSERT INTO [ImageServer].[dbo].[CannedText]([GUID],[Label],[Category],[Text])
     VALUES(newid(), 'Invalid series data', 'DeleteSeriesReason', 'Series contains some invalid data.')          
GO
     
INSERT INTO [ImageServer].[dbo].[CannedText]([GUID],[Label],[Category],[Text])
     VALUES(newid(), 'Data is incorrect', 'EditStudyReason', 'Data is incorrect.')
GO
     
INSERT INTO [ImageServer].[dbo].[CannedText]([GUID],[Label],[Category],[Text])
     VALUES(newid(), 'Data is missing', 'EditStudyReason', 'Data is missing.')               
GO
     
-- Device Types     
INSERT INTO [ImageServer].[dbo].[DeviceTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'Workstation','Workstation','Workstation')
GO
INSERT INTO [ImageServer].[dbo].[DeviceTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'Modality','Modality','Modality')
GO
INSERT INTO [ImageServer].[dbo].[DeviceTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'Server','Server','Server')
GO
INSERT INTO [ImageServer].[dbo].[DeviceTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'Broker','Broker','Broker')
GO
INSERT INTO [ImageServer].[dbo].[DeviceTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'PriorsServer','Priors Server','Server with Prior Studies for the Web Viewer')
GO
INSERT INTO [ImageServer].[dbo].[DeviceTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),105,'PrimaryPacs','Primary PACS','Primary PACS Server, the ImageServer will accept duplicate SOP Instances from this server')
GO

-- ServerPartitionTypeEnum
INSERT INTO [ImageServer].[dbo].[ServerPartitionTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'Standard','Standard','A standard ImageServer Partition')
GO
INSERT INTO [ImageServer].[dbo].[ServerPartitionTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'VFS','VFS','An ImageServer Virtual File System Partition')
GO

-- ExternalRequestQueueStatusEnum inserts
INSERT INTO [ImageServer].[dbo].[ExternalRequestQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),100,'Notification','Notification','The request is a permanent notification request')
GO
INSERT INTO [ImageServer].[dbo].[ExternalRequestQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'Authorization','Authorization','The request is a transient authorization request')
GO


INSERT INTO [ImageServer].[dbo].[ExternalRequestQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),200,'Pending','Pending','Pending')
GO

INSERT INTO [ImageServer].[dbo].[ExternalRequestQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),201,'In Progress','In Progress','In Progress')
GO

INSERT INTO [ImageServer].[dbo].[ExternalRequestQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),202,'Completed','Completed','The Queue entry is completed.')
GO

INSERT INTO [ImageServer].[dbo].[ExternalRequestQueueStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),203,'Failed','Failed','The Queue entry has failed.')
GO


-- OrderStatusEnum inserts
INSERT INTO [ImageServer].[dbo].[OrderStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),101,'New','New','New Order')
GO

INSERT INTO [ImageServer].[dbo].[OrderStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'Canceled','Canceled','Order Cancelled')
GO

INSERT INTO [ImageServer].[dbo].[OrderStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'Complete','Complete','Order Completed')
GO

INSERT INTO [ImageServer].[dbo].[OrderStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'InProcess','In Process','Order In Process')
GO




--  QCStatusEnum inserts
INSERT INTO [ImageServer].[dbo].QCStatusEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES     (newid(),100,'Processing','Processing','Processing')
GO

INSERT INTO [ImageServer].[dbo].QCStatusEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES     (newid(),200,'NA','N/A','Not Applicable')
GO

INSERT INTO [ImageServer].[dbo].QCStatusEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES     (newid(),300,'Passed','Passed','Passed')
GO

INSERT INTO [ImageServer].[dbo].QCStatusEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES     (newid(),400,'Failed','Failed','Failed')
GO

INSERT INTO [ImageServer].[dbo].QCStatusEnum
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES     (newid(),500,'Incomplete','Incomplete','Incomplete (Missing required scans)')
GO

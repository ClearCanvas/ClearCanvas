SET NUMERIC_ROUNDABORT OFF
GO
SET ANSI_PADDING, ANSI_WARNINGS, CONCAT_NULL_YIELDS_NULL, ARITHABORT, QUOTED_IDENTIFIER, ANSI_NULLS ON
GO
IF EXISTS (SELECT * FROM tempdb..sysobjects WHERE id=OBJECT_ID('tempdb..#tmpErrors')) DROP TABLE #tmpErrors
GO
CREATE TABLE #tmpErrors (Error int)
GO
SET XACT_ABORT ON
GO
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
GO
BEGIN TRANSACTION
GO
PRINT N'Dropping foreign keys from [dbo].[WorkQueue]'
GO
ALTER TABLE [dbo].[WorkQueue] DROP
CONSTRAINT [FK_WorkQueue_StudyHistory]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping foreign keys from [dbo].[StudyIntegrityQueueUid]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueueUid] DROP
CONSTRAINT [FK_StudyIntegrityQueueUid_StudyIntegrityQueue]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping foreign keys from [dbo].[RestoreQueue]'
GO
ALTER TABLE [dbo].[RestoreQueue] DROP
CONSTRAINT [FK_RestoreQueue_ArchiveStudyStorage]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping constraints from [dbo].[Alert]'
GO
ALTER TABLE [dbo].[Alert] DROP CONSTRAINT [PK_SystemAlert]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping constraints from [dbo].[ArchiveStudyStorage]'
GO
ALTER TABLE [dbo].[ArchiveStudyStorage] DROP CONSTRAINT [PK_StorageArchive]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping constraints from [dbo].[StudyHistory]'
GO
ALTER TABLE [dbo].[StudyHistory] DROP CONSTRAINT [PK_StudyHistory]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping constraints from [dbo].[StudyHistory]'
GO
ALTER TABLE [dbo].[StudyHistory] DROP CONSTRAINT [DF_StudyHistory_GUID]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping constraints from [dbo].[StudyIntegrityQueue]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueue] DROP CONSTRAINT [PK_StudyIntegrityQueue]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping constraints from [dbo].[StudyIntegrityQueue]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueue] DROP CONSTRAINT [DF_StudyIntegrityQueue_GUID]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping constraints from [dbo].[StudyIntegrityQueueUid]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueueUid] DROP CONSTRAINT [PK_StudyIntegrityQueueUid]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Dropping constraints from [dbo].[StudyIntegrityQueueUid]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueueUid] DROP CONSTRAINT [DF_StudyIntegrityQueueUid_GUID]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[StudyIntegrityQueue]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueue] ALTER COLUMN [GUID] [uniqueidentifier] NOT NULL

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
ALTER TABLE [dbo].[StudyIntegrityQueue] ALTER COLUMN [GUID] ADD ROWGUIDCOL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating primary key [PK_StudyIntegrityQueue] on [dbo].[StudyIntegrityQueue]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueue] ADD CONSTRAINT [PK_StudyIntegrityQueue] PRIMARY KEY CLUSTERED ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_StudyIntegrityQueue] on [dbo].[StudyIntegrityQueue]'
GO
CREATE NONCLUSTERED INDEX [IX_StudyIntegrityQueue] ON [dbo].[StudyIntegrityQueue] ([StudyStorageGUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[StudyIntegrityQueueUid]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueueUid] ALTER COLUMN [GUID] [uniqueidentifier] NOT NULL

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
ALTER TABLE [dbo].[StudyIntegrityQueueUid] ALTER COLUMN [GUID] ADD ROWGUIDCOL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_StudyIntegrityQueueUid] on [dbo].[StudyIntegrityQueueUid]'
GO
CREATE CLUSTERED INDEX [IX_StudyIntegrityQueueUid] ON [dbo].[StudyIntegrityQueueUid] ([StudyIntegrityQueueGUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating primary key [PK_StudyIntegrityQueueUid] on [dbo].[StudyIntegrityQueueUid]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueueUid] ADD CONSTRAINT [PK_StudyIntegrityQueueUid] PRIMARY KEY NONCLUSTERED ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[StudyHistory]'
GO
ALTER TABLE [dbo].[StudyHistory] ALTER COLUMN [GUID] [uniqueidentifier] NOT NULL

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
ALTER TABLE [dbo].[StudyHistory] ALTER COLUMN [GUID] ADD ROWGUIDCOL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_StudyHistory] on [dbo].[StudyHistory]'
GO
CREATE CLUSTERED INDEX [IX_StudyHistory] ON [dbo].[StudyHistory] ([StudyStorageGUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating primary key [PK_StudyHistory] on [dbo].[StudyHistory]'
GO
ALTER TABLE [dbo].[StudyHistory] ADD CONSTRAINT [PK_StudyHistory] PRIMARY KEY NONCLUSTERED ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Altering [dbo].[Alert]'
GO
ALTER TABLE [dbo].[Alert] ALTER COLUMN [GUID] [uniqueidentifier] NOT NULL

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
ALTER TABLE [dbo].[Alert] ALTER COLUMN [GUID] ADD ROWGUIDCOL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating primary key [PK_SystemAlert] on [dbo].[Alert]'
GO
ALTER TABLE [dbo].[Alert] ADD CONSTRAINT [PK_SystemAlert] PRIMARY KEY CLUSTERED ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IXC_ArchiveStudyStorage] on [dbo].[ArchiveStudyStorage]'
GO
CREATE CLUSTERED INDEX [IXC_ArchiveStudyStorage] ON [dbo].[ArchiveStudyStorage] ([StudyStorageGUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_ArchiveQueue] on [dbo].[ArchiveQueue]'
GO
CREATE NONCLUSTERED INDEX [IX_ArchiveQueue] ON [dbo].[ArchiveQueue] ([StudyStorageGUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating primary key [PK_StorageArchive] on [dbo].[ArchiveStudyStorage]'
GO
ALTER TABLE [dbo].[ArchiveStudyStorage] ADD CONSTRAINT [PK_StorageArchive] PRIMARY KEY NONCLUSTERED ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Creating index [IX_RestoreQueue] on [dbo].[RestoreQueue]'
GO
CREATE NONCLUSTERED INDEX [IX_RestoreQueue] ON [dbo].[RestoreQueue] ([StudyStorageGUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding constraints to [dbo].[Alert]'
GO
ALTER TABLE [dbo].[Alert] ADD CONSTRAINT [DF_Alert_GUID] DEFAULT (newid()) FOR [GUID]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding constraints to [dbo].[StudyHistory]'
GO
ALTER TABLE [dbo].[StudyHistory] ADD CONSTRAINT [DF_StudyHistory_GUID] DEFAULT (newid()) FOR [GUID]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding constraints to [dbo].[StudyIntegrityQueue]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueue] ADD CONSTRAINT [DF_StudyIntegrityQueue_GUID] DEFAULT (newid()) FOR [GUID]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding constraints to [dbo].[StudyIntegrityQueueUid]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueueUid] ADD CONSTRAINT [DF_StudyIntegrityQueueUid_GUID] DEFAULT (newid()) FOR [GUID]
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[WorkQueue]'
GO
ALTER TABLE [dbo].[WorkQueue] ADD
CONSTRAINT [FK_WorkQueue_StudyHistory] FOREIGN KEY ([StudyHistoryGUID]) REFERENCES [dbo].[StudyHistory] ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[StudyIntegrityQueueUid]'
GO
ALTER TABLE [dbo].[StudyIntegrityQueueUid] ADD
CONSTRAINT [FK_StudyIntegrityQueueUid_StudyIntegrityQueue] FOREIGN KEY ([StudyIntegrityQueueGUID]) REFERENCES [dbo].[StudyIntegrityQueue] ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
PRINT N'Adding foreign keys to [dbo].[RestoreQueue]'
GO
ALTER TABLE [dbo].[RestoreQueue] ADD
CONSTRAINT [FK_RestoreQueue_ArchiveStudyStorage] FOREIGN KEY ([ArchiveStudyStorageGUID]) REFERENCES [dbo].[ArchiveStudyStorage] ([GUID])
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END

GO
IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO

PRINT N'Inserting new row into [dbo].[ServiceLockTypeEnum]'
GO
INSERT INTO [dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),105,'FilesystemRebuildXml','Filesystem Rebuild Study XML','Rebuild the Study XML file for each study stored on the Filesystem')
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Inserting Filesystem Rebuild Study XML for existing filesystems'
GO
INSERT INTO [ImageServer].[dbo].ServiceLock
	([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime],[FilesystemGUID],[Enabled])
select newid(),105, 0, getdate(), GUID, 0 from Filesystem
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

IF @@TRANCOUNT>0 BEGIN
PRINT 'The database update succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The database update failed'
GO
DROP TABLE #tmpErrors
GO

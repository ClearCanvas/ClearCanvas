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

PRINT N'Adding new columns to Study table'
GO

ALTER TABLE dbo.Study ADD
	ResponsiblePerson nvarchar(64) NULL,
	ResponsibleOrganization nvarchar(64) NULL,
	QueryXml xml NULL
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding ServerPartitionDataAccess table'

/****** Object:  Table [dbo].[ServerPartitionDataAccess]    Script Date: 01/01/2012 23:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServerPartitionDataAccess](
	[GUID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_ServerPartitionDataAccess_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[DataAccessGroupGUID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ServerPartitionDataAccess] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_ServerPartitionDataAccess_DataAccessGroupGUID] ON [dbo].[ServerPartitionDataAccess] 
(
	[DataAccessGroupGUID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_ServerPartitionDataAccess_ServerPartitionGUID] ON [dbo].[ServerPartitionDataAccess] 
(
	[ServerPartitionGUID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]

ALTER TABLE [dbo].[ServerPartitionDataAccess]  WITH CHECK ADD  CONSTRAINT [FK_ServerPartitionDataAccess_DataAccessGroup] FOREIGN KEY([DataAccessGroupGUID])
REFERENCES [dbo].[DataAccessGroup] ([GUID])
GO

ALTER TABLE [dbo].[ServerPartitionDataAccess]  WITH CHECK ADD  CONSTRAINT [FK_ServerPartitionDataAccess_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding ExternalEdit WorkQueueTypeEnum'

INSERT INTO [dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),116,'ExternalEdit','External Edit','Edit request trigger by an external application.')
GO
  -- ExternalEdit
INSERT INTO [dbo].[WorkQueueTypeProperties]
           ([WorkQueueTypeEnum],[WorkQueuePriorityEnum],[MemoryLimited],[AlertFailedWorkQueue],
           [MaxFailureCount],[ProcessDelaySeconds],[FailureDelaySeconds],[DeleteDelaySeconds],
           [PostponeDelaySeconds],[ExpireDelaySeconds],[MaxBatchSize], [QueueStudyStateEnum], [QueueStudyStateOrder],
           [ReadLock],[WriteLock])
     VALUES
           (116,300,0,1,1,30,180,60,120,240,-1,101,3,0,1)
GO

PRINT N'Updating WebEditStudy priority to HIGH'
UPDATE [dbo].[WorkQueueTypeProperties] set WorkQueuePriorityEnum = 300 WHERE WorkQueueTypeEnum = 105
GO

PRINT N'Adding ExternalEdit [StudyHistoryTypeEnum]'
INSERT INTO [dbo].[StudyHistoryTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),204,'ExternalEdit','External Edit','Study was edited via an external request.')
GO
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating  [QueueStudyStateEnum] WebDeleteScheduled description'
UPDATE [dbo].[QueueStudyStateEnum] SET [LongDescription]='The study or a series is scheduled for deletion.' 
WHERE [Lookup] = 'WebDeleteScheduled'

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO


IF EXISTS (SELECT * FROM #tmpErrors) ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT>0 BEGIN
PRINT 'The database update succeeded'
COMMIT TRANSACTION
END
ELSE PRINT 'The database update failed'
GO
DROP TABLE #tmpErrors
GO

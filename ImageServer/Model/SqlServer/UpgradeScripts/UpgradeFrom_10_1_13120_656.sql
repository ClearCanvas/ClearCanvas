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

PRINT N'Create ServerPartitionTypeEnum table'
GO
CREATE TABLE dbo.ServerPartitionTypeEnum
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	Enum smallint NOT NULL,
	Lookup varchar(32) NOT NULL,
	Description nvarchar(32) NOT NULL,
	LongDescription nvarchar(512) NOT NULL
	)  ON STATIC
GO
ALTER TABLE dbo.ServerPartitionTypeEnum ADD CONSTRAINT
	DF_ServerPartitionTypeEnum_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.ServerPartitionTypeEnum ADD CONSTRAINT
	PK_ServerPartitionTypeEnum PRIMARY KEY CLUSTERED 
	(
	Enum
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON STATIC

GO

PRINT N'Inserting ServerPartitionTypeEnum values'
GO

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


IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding ServerPartition ServerPartitionTypeEnum column and foreign key reference'
GO

ALTER TABLE dbo.ServerPartition ADD
	ServerPartitionTypeEnum smallint NOT NULL CONSTRAINT DF_ServerPartition_ServerPartitionTypeEnum DEFAULT 100
GO
ALTER TABLE dbo.ServerPartition ADD CONSTRAINT
	FK_ServerPartition_ServerPartitionTypeEnum FOREIGN KEY
	(
	ServerPartitionTypeEnum
	) REFERENCES dbo.ServerPartitionTypeEnum
	(
	Enum
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding ServerPartitionAlternateAeTitle table'

GO
CREATE TABLE dbo.ServerPartitionAlternateAeTitle
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	ServerPartitionGUID uniqueidentifier NOT NULL,
	AeTitle varchar(16) NOT NULL,
	Port int NOT NULL,
	Enabled bit NOT NULL,
	AllowStorage bit NOT NULL,
	AllowKOPR bit NOT NULL,
	AllowRetrieve bit NOT NULL,
	AllowQuery bit NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.ServerPartitionAlternateAeTitle ADD CONSTRAINT
	DF_ServerPartitionAlternateAeTitle_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.ServerPartitionAlternateAeTitle ADD CONSTRAINT
	PK_ServerPartitionAlternateAeTitle PRIMARY KEY CLUSTERED 
	(
	GUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.ServerPartitionAlternateAeTitle ADD CONSTRAINT
	FK_ServerPartitionAlternateAeTitle_ServerPartition FOREIGN KEY
	(
	ServerPartitionGUID
	) REFERENCES dbo.ServerPartition
	(
	GUID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding ExternalRequestQueueStatusEnum and ExternalRequestQueue tables'

CREATE TABLE dbo.ExternalRequestQueueStatusEnum
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	Enum smallint NOT NULL,
	Lookup varchar(32) NOT NULL,
	Description nvarchar(32) NOT NULL,
	LongDescription nvarchar(512) NOT NULL
	)  ON STATIC
GO
ALTER TABLE dbo.ExternalRequestQueueStatusEnum ADD CONSTRAINT
	DF_ExternalRequestQueueStatusEnum_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.ExternalRequestQueueStatusEnum ADD CONSTRAINT
	PK_ExternalRequestQueueStatusEnum PRIMARY KEY CLUSTERED 
	(
	Enum
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON STATIC

GO

CREATE TABLE dbo.ExternalRequestQueue
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	ExternalRequestQueueStatusEnum smallint NULL,
	RequestType varchar(48) NOT NULL,
	OperationToken varchar(64) NULL,
	RequestId varchar(64) NULL,
	RequestXml xml NOT NULL,
	StateXml xml NULL,
	InsertTime datetime NOT NULL,
	DeletionTime datetime NULL,
	Revision int NOT NULL CONSTRAINT DF_ExternalRequestQueue_Revision DEFAULT 1
	)  ON QUEUES
	 TEXTIMAGE_ON QUEUES
GO
ALTER TABLE dbo.ExternalRequestQueue ADD CONSTRAINT
	DF_ExternalRequestQueue_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.ExternalRequestQueue ADD CONSTRAINT
	DF_ExternalRequestQueue_InsertTime DEFAULT getdate() FOR InsertTime
GO
ALTER TABLE dbo.ExternalRequestQueue ADD CONSTRAINT
	PK_ExternalRequestQueue PRIMARY KEY NONCLUSTERED 
	(
	GUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES

GO
CREATE NONCLUSTERED INDEX IX_ExternalRequestQueue_OperationToken ON dbo.ExternalRequestQueue
	(
	OperationToken
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO
CREATE NONCLUSTERED INDEX IX_ExternalRequestQueue_RequestId ON dbo.ExternalRequestQueue
(
	RequestId ASC
) WITH( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO
CREATE CLUSTERED INDEX IXC_ExternalRequestQueue_InsertTime ON dbo.ExternalRequestQueue
	(
	InsertTime DESC
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON QUEUES
GO
CREATE NONCLUSTERED INDEX IX_ExternalRequestQueue_RequestType ON dbo.ExternalRequestQueue
	(
	RequestType
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO
ALTER TABLE dbo.ExternalRequestQueue ADD CONSTRAINT
	FK_ExternalRequestQueue_ExternalRequestQueueStatusEnum FOREIGN KEY
	(
	ExternalRequestQueueStatusEnum
	) REFERENCES dbo.ExternalRequestQueueStatusEnum
	(
	Enum
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO


IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Inserting new  ExternalRequestQueueStatusEnum records'

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

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Create NotificationQueue table'

/****** Object:  Table [dbo].[NotificationQueue]    Script Date: 5/14/2013 6:17:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NotificationQueue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[NotificationQueue](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ExternalRequestQueueGUID] [uniqueidentifier] NOT NULL,
	[RestNotificationUrl] [nvarchar](128) NOT NULL,
	[NotificationXml] [xml] NOT NULL,
	[InsertTime] [datetime] NOT NULL,
	[LastTryTime] [datetime] NOT NULL,
	[Failed] [bit] NOT NULL,
	[RetryCount] [int] NOT NULL,
 CONSTRAINT [PK_NotificationQueue] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [INDEXES]
) ON [QUEUES] TEXTIMAGE_ON [QUEUES]
END
GO
/****** Object:  Index [IXC_NotificationQueue_InsertTime]    Script Date: 5/14/2013 6:17:07 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[NotificationQueue]') AND name = N'IXC_NotificationQueue_InsertTime')
CREATE CLUSTERED INDEX [IXC_NotificationQueue_InsertTime] ON [dbo].[NotificationQueue]
(
	[InsertTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [QUEUES]
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_NotificationQueue_GUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[NotificationQueue] ADD  CONSTRAINT [DF_NotificationQueue_GUID]  DEFAULT (newid()) FOR [GUID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_NotificationQueue_InsertTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[NotificationQueue] ADD  CONSTRAINT [DF_NotificationQueue_InsertTime]  DEFAULT (getdate()) FOR [InsertTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_NotificationQueue_LastTryTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[NotificationQueue] ADD  CONSTRAINT [DF_NotificationQueue_LastTryTime]  DEFAULT (getdate()) FOR [LastTryTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_NotificationQueue_Failed]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[NotificationQueue] ADD  CONSTRAINT [DF_NotificationQueue_Failed]  DEFAULT ((0)) FOR [Failed]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_NotificationQueue_RetryCount]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[NotificationQueue] ADD  CONSTRAINT [DF_NotificationQueue_RetryCount]  DEFAULT ((0)) FOR [RetryCount]
END

GO


IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Create new ServiceLockTypeEnum for External Request Processor table'

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


IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Add new ExternalRequestQueueGUID column to WorkQueue and add Foreign Key'

ALTER TABLE dbo.WorkQueue ADD
	ExternalRequestQueueGUID uniqueidentifier NULL
GO
ALTER TABLE dbo.WorkQueue ADD CONSTRAINT	FK_WorkQueue_ExternalRequestQueue FOREIGN KEY	(	ExternalRequestQueueGUID	) 
REFERENCES dbo.ExternalRequestQueue	(	GUID	) 

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Add new WorkQueueUidData column to WorkQueueUid table'

ALTER TABLE dbo.WorkQueueUid ADD
	WorkQueueUidData xml NULL

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Add AcceptLatest DuplicateSopPolicyEnum'

INSERT INTO [ImageServer].[dbo].DuplicateSopPolicyEnum([GUID],[Enum],[Lookup],[Description],[LongDescription])
VALUES(newid(),104,'AcceptLatest','Accept Latest','Process duplicate objects received and always accecpt the latest file received.')
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Add PrimaryPacs DeviceTypeEnum'

INSERT INTO [ImageServer].[dbo].[DeviceTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),105,'PrimaryPacs','Primary PACS','Primary PACS Server, the ImageServer will accept duplicate SOP Instances from this server')
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Add ServerPartitionGUID column to ServiceLock'

ALTER TABLE dbo.ServiceLock ADD
	ServerPartitionGUID uniqueidentifier NULL
GO
ALTER TABLE [dbo].[ServiceLock] WITH CHECK ADD CONSTRAINT [FK_ServiceLock_ServerPartitionGUID] FOREIGN KEY([ServerPartitionGUID]) 
REFERENCES [dbo].[ServerPartition] 	([GUID]) 
GO
ALTER TABLE [dbo].[ServiceLock]  CHECK CONSTRAINT [FK_ServiceLock_ServerPartitionGUID]
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Add PartitionReapplyRules ServiceLockTypeEnum'

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),401,'PartitionReapplyRules','Partition Reapply Rules','This service scans the contents of a partition and reapplies Study Processing rules to all studies on the partition that have not been archived.  Studies that have been archived will have Study Archived and Data Access rules applied.')
GO

DECLARE @ServerPartitionGUID uniqueidentifier
DECLARE partition_cursor CURSOR FOR SELECT GUID From ServerPartition
OPEN partition_cursor
FETCH NEXT FROM partition_cursor INTO @ServerPartitionGUID
WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO [ImageServer].[dbo].[ServiceLock]
			([GUID],[ServiceLockTypeEnum],[Lock],[ScheduledTime], [Enabled], [ServerPartitionGUID])
		VALUES (newid(), (select Enum from ServiceLockTypeEnum where Lookup='PartitionReapplyRules'), 0, getdate(), 0, @ServerPartitionGUID)
    
    FETCH NEXT FROM partition_cursor INTO @ServerPartitionGUID
END 
CLOSE partition_cursor;
DEALLOCATE partition_cursor;

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Add StudyQualityControl ServerRuleTypeEnum'

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),107,'StudyQualityControl','Study Quality Control','A rule for quality control purposes when studies are received')
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Add QCOutput to Study'

ALTER TABLE dbo.Study ADD
	QCOutput varchar(MAX) NULL
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

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

PRINT N'Create ProcedureCode, Staff, Order, and OrderStatusEnum table'
GO

/****** Object:  Table [dbo].[Order]    Script Date: 6/12/2014 12:59:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Order](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[OrderStatusEnum] [smallint] NOT NULL,
	[InsertTime] [datetime] NOT NULL,
	[UpdatedTime] [datetime] NOT NULL,
	[PatientsName] [nvarchar](64) NOT NULL,
	[PatientId] [nvarchar](64) NOT NULL,
	[IssuerOfPatientId] [nvarchar](64) NULL,
	[AccessionNumber] [varchar](64) NOT NULL,
	[ScheduledDateTime] [datetime] NOT NULL,
	[RequestedProcedureCodeGUID] [uniqueidentifier] NOT NULL,
	[EnteredByStaffGUID] [uniqueidentifier] NULL,
	[ReferringStaffGUID] [uniqueidentifier] NULL,
	[Priority] [varchar](2) NOT NULL,
	[PatientClass] [varchar](2) NULL,
	[ReasonForStudy] [nvarchar](199) NULL,
	[PointOfCare] [nvarchar](20) NULL,
	[Room] [nvarchar](20) NULL,
	[Bed] [nvarchar](20) NULL,
	[StudyInstanceUid] [varchar](64) NULL,
	[QCExpected] bit NOT NULL CONSTRAINT DF_Order_QC DEFAULT 0,
 CONSTRAINT [PK_Order] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderStatusEnum]    Script Date: 6/12/2014 12:59:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderStatusEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[OrderStatusEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_OrderStatusEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProcedureCode]    Script Date: 6/12/2014 12:59:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProcedureCode]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ProcedureCode](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[Identifier] [varchar](20) NOT NULL,
	[Text] [nvarchar](199) NULL,
	[CodingSystem] [varchar](20) NULL,
 CONSTRAINT [PK_Procedure] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Staff]    Script Date: 6/12/2014 12:59:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Staff]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Staff](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[Identifier] [nvarchar](15) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Staff] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Index [IXC_Order_ScheduledDateTime]    Script Date: 6/12/2014 12:59:57 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND name = N'IXC_Order_ScheduledDateTime')
CREATE CLUSTERED INDEX [IXC_Order_ScheduledDateTime] ON [dbo].[Order]
(
	[ServerPartitionGUID] ASC,
	[ScheduledDateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Order_AccessionNumber]    Script Date: 6/12/2014 12:59:57 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND name = N'IX_Order_AccessionNumber')
CREATE NONCLUSTERED INDEX [IX_Order_AccessionNumber] ON [dbo].[Order]
(
	[AccessionNumber] ASC,
	[ServerPartitionGUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [INDEXES]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Order_PatientId]    Script Date: 6/12/2014 12:59:57 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND name = N'IX_Order_PatientId')
CREATE NONCLUSTERED INDEX [IX_Order_PatientId] ON [dbo].[Order]
(
	[PatientId] ASC,
	[IssuerOfPatientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Order_PatientsName]    Script Date: 6/12/2014 12:59:57 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND name = N'IX_Order_PatientsName')
CREATE NONCLUSTERED INDEX [IX_Order_PatientsName] ON [dbo].[Order]
(
	[PatientsName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Procedure_Identifier]    Script Date: 6/12/2014 12:59:57 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ProcedureCode]') AND name = N'IX_Procedure_Identifier')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Procedure_Identifier] ON [dbo].[ProcedureCode]
(
	[Identifier] ASC,
	[ServerPartitionGUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [INDEXES]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Staff_Identifier]    Script Date: 6/12/2014 12:59:57 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Staff]') AND name = N'IX_Staff_Identifier')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Staff_Identifier] ON [dbo].[Staff]
(
	[ServerPartitionGUID] ASC,
	[Identifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Order_GUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Order] ADD  CONSTRAINT [DF_Order_GUID]  DEFAULT (newid()) FOR [GUID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_OrderStatusEnum_GUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[OrderStatusEnum] ADD  CONSTRAINT [DF_OrderStatusEnum_GUID]  DEFAULT (newid()) FOR [GUID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Procedure_GUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ProcedureCode] ADD  CONSTRAINT [DF_Procedure_GUID]  DEFAULT (newid()) FOR [GUID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_Staff_GUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Staff] ADD  CONSTRAINT [DF_Staff_GUID]  DEFAULT (newid()) FOR [GUID]
END

GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_OrderStatusEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_OrderStatusEnum] FOREIGN KEY([OrderStatusEnum])
REFERENCES [dbo].[OrderStatusEnum] ([Enum])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_OrderStatusEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_OrderStatusEnum]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_Procedure]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Procedure] FOREIGN KEY([RequestedProcedureCodeGUID])
REFERENCES [dbo].[ProcedureCode] ([GUID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_Procedure]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Procedure]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_ServerPartition]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_Staff_EnteredBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Staff_EnteredBy] FOREIGN KEY([EnteredByStaffGUID])
REFERENCES [dbo].[Staff] ([GUID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_Staff_EnteredBy]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Staff_EnteredBy]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_Staff_ReferringStaff]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order]  WITH CHECK ADD  CONSTRAINT [FK_Order_Staff_ReferringStaff] FOREIGN KEY([ReferringStaffGUID])
REFERENCES [dbo].[Staff] ([GUID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Order_Staff_ReferringStaff]') AND parent_object_id = OBJECT_ID(N'[dbo].[Order]'))
ALTER TABLE [dbo].[Order] CHECK CONSTRAINT [FK_Order_Staff_ReferringStaff]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProcedureCode_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProcedureCode]'))
ALTER TABLE [dbo].[ProcedureCode]  WITH CHECK ADD  CONSTRAINT [FK_ProcedureCode_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ProcedureCode_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[ProcedureCode]'))
ALTER TABLE [dbo].[ProcedureCode] CHECK CONSTRAINT [FK_ProcedureCode_ServerPartition]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Staff_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Staff]'))
ALTER TABLE [dbo].[Staff]  WITH CHECK ADD  CONSTRAINT [FK_Staff_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Staff_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Staff]'))
ALTER TABLE [dbo].[Staff] CHECK CONSTRAINT [FK_Staff_ServerPartition]
GO



IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding [OrderStatusEnum] values'
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


IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding QCStatusEnum and QCUpdateTimeUtc to Study Table'

GO
ALTER TABLE dbo.Study ADD
	QCStatusEnum smallint NULL
GO
ALTER TABLE dbo.Study ADD
	QCUpdateTimeUtc datetime NULL
GO

/****** Object:  Table [dbo].[QCStatusEnum]    Script Date: 06/03/2014 12:48:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QCStatusEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[QCStatusEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_QCStatusEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_QCStatusEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
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

declare @na as smallint
declare @failed as smallint
declare @pass as smallint

select @na=[Enum] from QCStatusEnum where Lookup='NA'
select @failed=[Enum] from QCStatusEnum where Lookup='Failed'
select @pass=[Enum] from QCStatusEnum where Lookup='Passed'

update Study set QCStatusEnum = @failed where QCOutput like '%Status_:_Failed%'
update Study set QCStatusEnum = @pass where QCOutput like '%Status_:_Pass%'
update Study set QCStatusEnum = @na where QCStatusEnum is null

ALTER TABLE Study ALTER COLUMN QCStatusEnum smallint NOT NULL

ALTER TABLE dbo.Study ADD CONSTRAINT FK_Study_QCStatusEnum FOREIGN KEY ( QCStatusEnum ) 
REFERENCES dbo.QCStatusEnum	( Enum )
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IX_Study_StudyDate')
	DROP INDEX [IX_Study_StudyDate] ON [dbo].[Study]
GO	 
CREATE NONCLUSTERED INDEX [IX_Study_StudyDate] ON [dbo].[Study] 
(
	[StudyDate] ASC,
	[QCStatusEnum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IX_Study_QCUpdateTime')
CREATE NONCLUSTERED INDEX [IX_Study_QCUpdateTime] ON [dbo].[Study]
(
	[QCUpdateTimeUtc] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [INDEXES]
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating RT Dose Storage to be an Image SOP Class'

UPDATE [ImageServer].[dbo].[ServerSopClass] SET [NonImage] = 0
WHERE [SopClassUid] = '1.2.840.10008.5.1.4.1.1.481.2' -- 'RT Dose Storage'
GO

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding OrderKey column to Study Table'

ALTER TABLE dbo.[Order] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.[Order]', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.[Order]', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.[Order]', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.Study ADD
	OrderGUID uniqueidentifier NULL
GO

ALTER TABLE dbo.Study ADD CONSTRAINT FK_Study_Order FOREIGN KEY	( OrderGUID ) 
REFERENCES dbo.[Order] ( GUID )


IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding [ServiceLockTypeEnum] of PartitionOrderPurge'

INSERT INTO [ImageServer].[dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),303,'PartitionOrderPurge','Partition Order Purge','This service purges orders not linked to studies on a partition.')
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding [ServerRuleTypeEnum] of StudyAutoRoute'

INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),108,'StudyAutoRoute','Study Auto Routing','A DICOM auto-routing rule for studies')
GO


IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding ScheduledTime to ExternalRequestQueue and updating ExternalRequestQueue clustered index'

ALTER TABLE dbo.ExternalRequestQueue ADD
	ScheduledTime datetime NOT NULL CONSTRAINT DF_ExternalRequestQueue_ScheduledTime DEFAULT getdate()
GO
DROP INDEX IXC_ExternalRequestQueue_InsertTime ON dbo.ExternalRequestQueue
GO
CREATE CLUSTERED INDEX IXC_ExternalRequestQueue_ScheduledTime ON dbo.ExternalRequestQueue
	(
	ScheduledTime
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON QUEUES
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Update ProcessStudy default WorkQueueTypeProperties table'
GO

Update dbo.WorkQueueTypeProperties
set ProcessDelaySeconds=5
where WorkQueueTypeEnum=100 and ProcessDelaySeconds=10

Update dbo.WorkQueueTypeProperties
set ExpireDelaySeconds=30
where WorkQueueTypeEnum=100 and ExpireDelaySeconds=120

Update dbo.WorkQueueTypeProperties
set FailureDelaySeconds=60
where WorkQueueTypeEnum=100 and FailureDelaySeconds=180


IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Add new StudyAutoRoute WorkQueueTypeEnum and WorkQueueTypeProperties'
GO

INSERT INTO [ImageServer].[dbo].[WorkQueueTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),117,'StudyAutoRoute','Study Auto Route','DICOM Auto-route request of a Study.')
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

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Add new StudyCompress and SopCompress StudyHistoryTypeEnum'
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




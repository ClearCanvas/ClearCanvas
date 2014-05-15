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
BEGIN TRANSACTION
GO
CREATE TABLE dbo.[ProcedureCode]
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	Identifier varchar(20) NOT NULL,
	Text nvarchar(199) NULL,
	CodingSystem varchar(20) NULL
	)  ON STATIC
GO
ALTER TABLE dbo.[ProcedureCode] ADD CONSTRAINT
	DF_ProcedureCode_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.[ProcedureCode] ADD CONSTRAINT
	PK_ProcedureCode PRIMARY KEY CLUSTERED 
	(
	GUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON STATIC

GO
CREATE NONCLUSTERED INDEX IX_ProcedureCode_Identifier ON dbo.[ProcedureCode]
	(
	Identifier
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO
ALTER TABLE dbo.[ProcedureCode] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.[ProcedureCode]', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.[ProcedureCode]', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.[ProcedureCode]', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.Staff
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	Identifier nvarchar(15) NOT NULL,
	FamilyName nvarchar(194) NOT NULL,
	GivenName nvarchar(30) NOT NULL,
	MiddleName nvarchar(3) NULL,
	Suffix nvarchar(20) NULL,
	Prefix nvarchar(20) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Staff ADD CONSTRAINT
	DF_Staff_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.Staff ADD CONSTRAINT
	PK_Staff PRIMARY KEY CLUSTERED 
	(
	GUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.Staff SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Staff', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Staff', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Staff', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
ALTER TABLE dbo.ServerPartition SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.ServerPartition', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.ServerPartition', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.ServerPartition', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.OrderStatusEnum
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	Enum smallint NOT NULL,
	Lookup varchar(32) NOT NULL,
	Description nvarchar(32) NOT NULL,
	LongDescription nvarchar(512) NOT NULL
	)  ON STATIC
GO
ALTER TABLE dbo.OrderStatusEnum ADD CONSTRAINT
	DF_OrderStatusEnum_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.OrderStatusEnum ADD CONSTRAINT
	PK_OrderStatusEnum PRIMARY KEY CLUSTERED 
	(
	Enum
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON STATIC

GO
ALTER TABLE dbo.OrderStatusEnum SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.OrderStatusEnum', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.OrderStatusEnum', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.OrderStatusEnum', 'Object', 'CONTROL') as Contr_Per BEGIN TRANSACTION
GO
CREATE TABLE dbo.[Order]
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	ServerPartitionGUID uniqueidentifier NOT NULL,
	OrderStatusEnum smallint NOT NULL,
	InsertTime datetime NOT NULL,
	UpdatedTime datetime NOT NULL,
	PatientGUID uniqueidentifier NOT NULL,
	AccessionNumber varchar(64) NOT NULL,
	ScheduledDateTime datetime NOT NULL,
	RequestedProcedureCodeGUID uniqueidentifier NOT NULL,
	EnteredByStaffGUID uniqueidentifier NULL,
	ReferringStaffGUID uniqueidentifier NULL,
	Priority varchar(2) NOT NULL,
	PatientClass varchar(1) NULL,
	ReasonForStudy nvarchar(199) NULL,
	PointOfCare nvarchar(20) NULL,
	Room nvarchar(20) NULL,
	Bed nvarchar(20) NULL,
	StudyInstanceUid varchar(64) NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.[Order] ADD CONSTRAINT
	DF_Order_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.[Order] ADD CONSTRAINT
	PK_Order PRIMARY KEY CLUSTERED 
	(
	GUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_Order_ScheduledDateTime ON dbo.[Order]
	(
	ScheduledDateTime
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO
CREATE NONCLUSTERED INDEX IX_Order_AccessionNumber ON dbo.[Order]
	(
	AccessionNumber,
	ServerPartitionGUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO
ALTER TABLE dbo.[Order] ADD CONSTRAINT
	FK_Order_OrderStatusEnum FOREIGN KEY
	(
	OrderStatusEnum
	) REFERENCES dbo.OrderStatusEnum
	(
	Enum
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Order] ADD CONSTRAINT
	FK_Order_ServerPartition FOREIGN KEY
	(
	ServerPartitionGUID
	) REFERENCES dbo.ServerPartition
	(
	GUID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Order] ADD CONSTRAINT
	FK_Order_Staff_ReferringStaff FOREIGN KEY
	(
	ReferringStaffGUID
	) REFERENCES dbo.Staff
	(
	GUID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Order] ADD CONSTRAINT
	FK_Order_Staff_EnteredBy FOREIGN KEY
	(
	EnteredByStaffGUID
	) REFERENCES dbo.Staff
	(
	GUID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Order] ADD CONSTRAINT
	FK_Order_ProcedureCode FOREIGN KEY
	(
	RequestedProcedureCodeGUID
	) REFERENCES dbo.[ProcedureCode]
	(
	GUID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.[Order] SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.[Order]', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.[Order]', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.[Order]', 'Object', 'CONTROL') as Contr_Per 

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
           (newid(),101,'NW','New','New Order')
GO

INSERT INTO [ImageServer].[dbo].[OrderStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),102,'OC','Canceled','Order Cancelled')
GO

INSERT INTO [ImageServer].[dbo].[OrderStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),103,'DC','Discontinued','Order Discontinued')
GO

INSERT INTO [ImageServer].[dbo].[OrderStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),104,'CM','Completed','Order Completed')
GO

INSERT INTO [ImageServer].[dbo].[OrderStatusEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),105,'IP','In Process','Order In Process')
GO

IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding '



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

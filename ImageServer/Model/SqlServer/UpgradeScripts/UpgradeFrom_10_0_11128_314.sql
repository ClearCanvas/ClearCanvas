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
           (newid(),101,'Research','Research','An ImageServer research Partition')
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

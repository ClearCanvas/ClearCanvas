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


PRINT N'Adding new ServiceLockTypeEnum, SyncDataAccess'
GO
INSERT INTO [dbo].[ServiceLockTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),300,'SyncDataAccess','Synchronize Data Access','This service periodically synchronizes the deletion status of Authority Groups on the Administrative Services with Data Access granted to studies on the ImageServer.')
GO
UPDATE [dbo].[ServiceLockTypeEnum] SET [LongDescription] = 'This service scans the contents of a filesystem and reapplies Study Processing rules to all studies on the filesystem that have not been archived.  Studies that have been archived will have Study Archived and Data Access rules applied.'
	WHERE [Lookup] = 'FilesystemStudyProcess'

GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding new ServerRuleTypeEnum, DataAccess'
GO
INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),106,'DataAccess','Data Access','A rule to specify the Authority Groups that have access to a study')
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Adding Table DataAccessGroup'

CREATE TABLE dbo.DataAccessGroup
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	AuthorityGroupOID uniqueidentifier NOT NULL,
	Deleted bit NOT NULL
	)  ON STATIC
GO
ALTER TABLE dbo.DataAccessGroup ADD CONSTRAINT
	DF_DataAccessGroup_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.DataAccessGroup ADD CONSTRAINT
	DF_DataAccessGroup_Deleted DEFAULT 0 FOR Deleted
GO
ALTER TABLE dbo.DataAccessGroup ADD CONSTRAINT
	PK_DataAccessGroup PRIMARY KEY CLUSTERED 
	(
	GUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON STATIC

GO
CREATE NONCLUSTERED INDEX IX_DataAccessGroup_AuthorityGroupOID ON dbo.DataAccessGroup
	(
	AuthorityGroupOID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

PRINT N'Adding Table StudyDataAccess'
GO
CREATE TABLE dbo.StudyDataAccess
	(
	GUID uniqueidentifier NOT NULL ROWGUIDCOL,
	StudyStorageGUID uniqueidentifier NOT NULL,
	DataAccessGroupGUID uniqueidentifier NOT NULL
	)  ON [PRIMARY]
GO
DECLARE @v sql_variant 
SET @v = N'Table for granting access to studies via Authority Groups'
EXECUTE sp_addextendedproperty N'MS_Description', @v, N'SCHEMA', N'dbo', N'TABLE', N'StudyDataAccess', NULL, NULL
GO
ALTER TABLE dbo.StudyDataAccess ADD CONSTRAINT
	DF_StudyDataAccess_GUID DEFAULT (newid()) FOR GUID
GO
ALTER TABLE dbo.StudyDataAccess ADD CONSTRAINT
	PK_StudyDataAccess PRIMARY KEY CLUSTERED 
	(
	GUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_StudyDataAccess_DataAcessGroupGUID ON dbo.StudyDataAccess
	(
	DataAccessGroupGUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO
CREATE NONCLUSTERED INDEX IX_StudyDataAccess_StudyStorageGUID ON dbo.StudyDataAccess
	(
	StudyStorageGUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO
ALTER TABLE dbo.StudyDataAccess ADD CONSTRAINT
	FK_StudyDataAccess_DataAccessGroup FOREIGN KEY
	(
	DataAccessGroupGUID
	) REFERENCES dbo.DataAccessGroup
	(
	GUID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
ALTER TABLE dbo.StudyDataAccess ADD CONSTRAINT
	FK_StudyDataAccess_StudyStorage FOREIGN KEY
	(
	StudyStorageGUID
	) REFERENCES dbo.StudyStorage
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

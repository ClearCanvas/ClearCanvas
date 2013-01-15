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

PRINT N'Inserting new rows into [dbo].[ServerRuleTypeEnum]'
GO
INSERT INTO [ImageServer].[dbo].[ServerRuleTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),105,'SopCompress','SOP Compress','A rule to specify when a SOP Instance should be compressed (during initial processing)')
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Inserting new rows into [dbo].[StudyHistoryTypeEnum]'
GO
INSERT INTO [ImageServer].[dbo].[StudyHistoryTypeEnum]
           ([GUID],[Enum],[Lookup],[Description],[LongDescription])
     VALUES
           (newid(),202,'Reprocessed','Study was reprocessed','Study was reprocessed.')
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Updating [dbo].[ServiceLockTypeEnum]'
GO
UPDATE [dbo].[ServiceLockTypeEnum]
  SET [Description] = 'Filesystem Reapply Rules'
WHERE Lookup='FilesystemStudyProcess'
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

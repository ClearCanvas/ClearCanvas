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

PRINT N'Truncating ApplicationLog.Message and ApplicationLog.Exception columns to 2000 characters'
UPDATE ApplicationLog SET Exception = LEFT(Exception,2000), Message = LEFT(Message,2000)
GO

PRINT N'Changing ApplicationLog.Message column to NVARCHAR'
GO
ALTER TABLE dbo.ApplicationLog 
ALTER COLUMN [Message] 
NVARCHAR(2000) NOT NULL
GO

PRINT N'Changing index ApplicationLog.Exception column to NVARCHAR'
GO
ALTER TABLE dbo.ApplicationLog 
ALTER COLUMN [Exception] 
NVARCHAR(2000)
GO

PRINT N'Dropping Constraint DF_WorkQueueTypeProperties_ReadLock'
IF EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_WorkQueueTypeProperties_ReadLock]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]'))
Begin
ALTER TABLE [dbo].[WorkQueueTypeProperties] DROP  CONSTRAINT DF_WorkQueueTypeProperties_ReadLock
End
GO

PRINT N'Dropping Constraint DF_WorkQueueTypeProperties_WriteLock'
IF EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_WorkQueueTypeProperties_WriteLock]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]'))
Begin
ALTER TABLE [dbo].[WorkQueueTypeProperties] DROP  CONSTRAINT DF_WorkQueueTypeProperties_WriteLock
End
GO

PRINT N'Dropping Constraint DF_StudyStorage_Lock'
IF EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_StudyStorage_Lock]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyStorage]'))
Begin
ALTER TABLE [dbo].[StudyStorage] DROP  CONSTRAINT DF_StudyStorage_Lock
End
GO

PRINT N'Creating Constraint DF_StudyStorage_WriteLock'
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_StudyStorage_WriteLock]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyStorage]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_StudyStorage_WriteLock]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[StudyStorage] ADD  CONSTRAINT DF_StudyStorage_WriteLock  DEFAULT ((0)) FOR [WriteLock]
END
End
GO


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

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

PRINT N'Adding Support for Enhanced US Volume Storage'
GO
DECLARE @SopClassGUID uniqueidentifier
DECLARE @ServerPartitionGUID uniqueidentifier

SET @SopClassGUID = NEWID()

INSERT INTO [ImageServer].[dbo].[ServerSopClass] ([GUID],[SopClassUid],[Description],[NonImage])
VALUES (@SopClassGUID, '1.2.840.10008.5.1.4.1.1.6.2', 'Enhanced US Volume Storage', 0);

DECLARE partition_cursor CURSOR FOR SELECT GUID From ServerPartition
OPEN partition_cursor
FETCH NEXT FROM partition_cursor INTO @ServerPartitionGUID
WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO [ImageServer].[dbo].[PartitionSopClass]
			([GUID],[ServerPartitionGUID],[ServerSopClassGUID],[Enabled])
		VALUES (newid(), @ServerPartitionGUID, @SopClassGUID, 1)
    
    FETCH NEXT FROM partition_cursor INTO @ServerPartitionGUID
END 
CLOSE partition_cursor;
DEALLOCATE partition_cursor;


IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO

PRINT N'Fixing Breast Tomo Storage'
GO
UPDATE [ImageServer].[dbo].[ServerSopClass] SET NonImage=0 where [SopClassUid] = '1.2.840.10008.5.1.4.1.1.13.1.3'


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

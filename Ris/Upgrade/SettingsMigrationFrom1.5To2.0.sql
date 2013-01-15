USE [Ris]

DECLARE @oldVersionString NVARCHAR(30)
DECLARE @newVersionString NVARCHAR(30)
DECLARE @oldDocumentOID UNIQUEIDENTIFIER 
DECLARE @oldDocumentName NVARCHAR(255)
DECLARE @oldUser NVARCHAR(50) 
DECLARE @oldInstanceKey NVARCHAR(100) 
DECLARE @newOID UNIQUEIDENTIFIER


SET @oldVersionString = '00001.00005'  
SET @newVersionString = '00002.00000'

DECLARE cursorConfigurationDocument CURSOR FOR  
SELECT [OID_], [DocumentName_],  [User_], [InstanceKey_] FROM [ConfigurationDocument_]
WHERE [DocumentVersionString_] = @oldVersionString
ORDER BY [DocumentName_]

OPEN cursorConfigurationDocument   
FETCH NEXT FROM cursorConfigurationDocument INTO @oldDocumentOID, @oldDocumentName, @oldUser, @oldInstanceKey

WHILE @@FETCH_STATUS = 0   
BEGIN   
		IF EXISTS 
			(SELECT [OID_] FROM [ConfigurationDocument_] 
			 WHERE [DocumentName_] = @oldDocumentName 
			 AND ([User_] = @oldUser OR ([User_] is NULL AND @oldUser is NULL))
			 AND [DocumentVersionString_] = @newVersionString)

			--If configuration already exists for the version being installed just update it using the older value
			BEGIN

				UPDATE [ConfigurationDocumentBody_] 
				SET [DocumentText_] = (SELECT [DocumentText_] FROM [ConfigurationDocumentBody_] WHERE [DocumentOID_] = @oldDocumentOID)				
				WHERE [DocumentOID_] = ( SELECT [OID_] FROM [ConfigurationDocument_] 
												WHERE [DocumentName_] = @oldDocumentName 
												AND ([User_] = @oldUser OR ([User_] is NULL AND @oldUser is NULL))
												AND [DocumentVersionString_] = @newVersionString )


			END
		ELSE
			BEGIN
				SET @newOID = NEWID()		

				INSERT INTO [ConfigurationDocument_]
				   ([OID_]
				   ,[Version_]
				   ,[DocumentName_]
				   ,[DocumentVersionString_]
				   ,[User_]
				   ,[InstanceKey_])
				VALUES
				   (@newOID
				   ,1
				   ,@oldDocumentName
				   ,@newVersionString
				   ,@oldUser
				   ,@oldInstanceKey)


				INSERT INTO [ConfigurationDocumentBody_]
				   ([DocumentOID_]
				   ,[Version_]
				   ,[DocumentText_])
				SELECT
				   @newOID
				   ,1
				   ,(SELECT [DocumentText_] FROM [ConfigurationDocumentBody_] WHERE [DocumentOID_] = @oldDocumentOID)
			END

		FETCH NEXT FROM cursorConfigurationDocument INTO @oldDocumentOID, @oldDocumentName, @oldUser, @oldInstanceKey   
END   

CLOSE cursorConfigurationDocument   
DEALLOCATE cursorConfigurationDocument
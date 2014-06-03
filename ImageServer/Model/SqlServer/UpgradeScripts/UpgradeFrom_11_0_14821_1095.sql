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

/****** Object:  Table [dbo].[Order]    Script Date: 5/15/2014 6:47:33 PM ******/
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
	[PatientGUID] [uniqueidentifier] NOT NULL,
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
 CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrderStatusEnum]    Script Date: 5/15/2014 6:47:33 PM ******/
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
/****** Object:  Table [dbo].[ProcedureCode]    Script Date: 5/15/2014 6:47:33 PM ******/
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
/****** Object:  Table [dbo].[Staff]    Script Date: 5/15/2014 6:47:33 PM ******/
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
	[FamilyName] [nvarchar](194) NOT NULL,
	[GivenName] [nvarchar](30) NOT NULL,
	[MiddleName] [nvarchar](3) NULL,
	[Suffix] [nvarchar](20) NULL,
	[Prefix] [nvarchar](20) NULL,
 CONSTRAINT [PK_Staff] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Order_AccessionNumber]    Script Date: 5/15/2014 6:47:33 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND name = N'IX_Order_AccessionNumber')
CREATE NONCLUSTERED INDEX [IX_Order_AccessionNumber] ON [dbo].[Order]
(
	[AccessionNumber] ASC,
	[ServerPartitionGUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [INDEXES]
GO
/****** Object:  Index [IX_Order_ScheduledDateTime]    Script Date: 5/15/2014 6:47:33 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Order]') AND name = N'IX_Order_ScheduledDateTime')
CREATE NONCLUSTERED INDEX [IX_Order_ScheduledDateTime] ON [dbo].[Order]
(
	[ScheduledDateTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [INDEXES]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Procedure_Identifier]    Script Date: 5/15/2014 6:47:33 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ProcedureCode]') AND name = N'IX_Procedure_Identifier')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Procedure_Identifier] ON [dbo].[ProcedureCode]
(
	[Identifier] ASC,
	[ServerPartitionGUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [INDEXES]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Staff_Identifier]    Script Date: 5/15/2014 6:47:33 PM ******/
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


UPDATE [ImageServer].[dbo].[ServerSopClass] SET [NonImage] = 0
WHERE [SopClassUid] = '1.2.840.10008.5.1.4.1.1.481.2' -- 'RT Dose Storage'
GO

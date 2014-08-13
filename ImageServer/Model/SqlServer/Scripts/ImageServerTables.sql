USE [ImageServer]
GO
/****** Object:  Table [dbo].[ServerTransferSyntax]    Script Date: 07/16/2008 23:49:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServerTransferSyntax]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ServerTransferSyntax](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ServerTransferSyntax_GUID]  DEFAULT (newid()),
	[Uid] [varchar](64) NOT NULL,
	[Description] [nvarchar](256) NOT NULL,
	[Lossless] [bit] NOT NULL,
 CONSTRAINT [PK_ServerTransferSyntax] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DuplicateSopPolicyEnum]    Script Date: 07/16/2008 23:48:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DuplicateSopPolicyEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DuplicateSopPolicyEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_DuplicateSopPolicyEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_DuplicateSopPolicyEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ServerPartition]    Script Date: 07/16/2008 23:49:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServerPartition]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ServerPartition](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ServerPartition_GUID]  DEFAULT (newid()),
	[Enabled] [bit] NOT NULL,
	[Description] [nvarchar](128) NOT NULL,
	[AeTitle] [varchar](16) COLLATE Latin1_General_CS_AS NOT NULL,
	[Port] [int] NOT NULL,
	[PartitionFolder] [nvarchar](16) NOT NULL,
	[AcceptAnyDevice] [bit] NOT NULL CONSTRAINT [DF_ServerPartition_AcceptAnyDevice]  DEFAULT ((1)),
	[AuditDeleteStudy] [bit] NOT NULL CONSTRAINT [DF_ServerPartition_AuditDeleteStudy]  DEFAULT ((0)),
	[AutoInsertDevice] [bit] NOT NULL CONSTRAINT [DF_ServerPartition_AutoInsertDevice]  DEFAULT ((1)),
	[DefaultRemotePort] [int] NOT NULL CONSTRAINT [DF_ServerPartition_DefaultRemotePort]  DEFAULT ((104)),
	[StudyCount] [int] NOT NULL CONSTRAINT [DF_ServerPartition_StudyCount]  DEFAULT ((0)),
	[DuplicateSopPolicyEnum] [smallint] NOT NULL,
	[MatchAccessionNumber] bit NOT NULL CONSTRAINT [DF_ServerPartition_MatchAccessionNumber]  DEFAULT ((1)),
	[MatchIssuerOfPatientId] bit NOT NULL CONSTRAINT [DF_ServerPartition_MatchIssuerOfPatientId]  DEFAULT ((1)),
	[MatchPatientId] bit NOT NULL CONSTRAINT [DF_ServerPartition_MatchPatientId]  DEFAULT ((1)),
	[MatchPatientsBirthDate] bit NOT NULL CONSTRAINT [DF_ServerPartition_MatchPatientsBirthDate]  DEFAULT ((1)),
	[MatchPatientsName] bit NOT NULL CONSTRAINT [DF_ServerPartition_MatchPatientsName]  DEFAULT ((1)),
	[MatchPatientsSex] bit NOT NULL CONSTRAINT [DF_ServerPartition_MatchPatientsSex]  DEFAULT ((1)),
	[AcceptLatestReport] bit NOT NULL CONSTRAINT [DF_ServerPartition_AcceptLatestReport] DEFAULT ((1)),
	[ServerPartitionTypeEnum] [smallint] NOT NULL CONSTRAINT [DF_ServerPartition_ServerPartitionTypeEnum] DEFAULT ((100))

 CONSTRAINT [PK_ServerPartition] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServerPartition]') AND name = N'IX_ServerPartition')
CREATE UNIQUE NONCLUSTERED INDEX [IX_ServerPartition] ON [dbo].[ServerPartition] 
(
	[AeTitle] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
/****** Object:  Table [dbo].[ServerPartitionAlternateAeTitle]    Script Date: 5/6/2013 12:49:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ServerPartitionAlternateAeTitle](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[AeTitle] [varchar](16) NOT NULL,
	[Port] [int] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[AllowStorage] [bit] NOT NULL,
	[AllowKOPR] [bit] NOT NULL,
	[AllowRetrieve] [bit] NOT NULL,
	[AllowQuery] [bit] NOT NULL,
 CONSTRAINT [PK_ServerPartitionAlternateAeTitle] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ServerPartitionTypeEnum]    Script Date: 5/6/2013 12:49:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ServerPartitionTypeEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_ServerPartitionTypeEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [STATIC]
) ON [STATIC]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[ServerPartitionAlternateAeTitle] ADD  CONSTRAINT [DF_ServerPartitionAlternateAeTitle_GUID]  DEFAULT (newid()) FOR [GUID]
GO
ALTER TABLE [dbo].[ServerPartitionTypeEnum] ADD  CONSTRAINT [DF_ServerPartitionTypeEnum_GUID]  DEFAULT (newid()) FOR [GUID]
GO
ALTER TABLE [dbo].[ServerPartition]  WITH CHECK ADD  CONSTRAINT [FK_ServerPartition_ServerPartitionTypeEnum] FOREIGN KEY([ServerPartitionTypeEnum])
REFERENCES [dbo].[ServerPartitionTypeEnum] ([Enum])
GO
ALTER TABLE [dbo].[ServerPartition] CHECK CONSTRAINT [FK_ServerPartition_ServerPartitionTypeEnum]
GO
ALTER TABLE [dbo].[ServerPartitionAlternateAeTitle]  WITH CHECK ADD  CONSTRAINT [FK_ServerPartitionAlternateAeTitle_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
ALTER TABLE [dbo].[ServerPartitionAlternateAeTitle] CHECK CONSTRAINT [FK_ServerPartitionAlternateAeTitle_ServerPartition]
GO

/****** Object:  Table [dbo].[WorkQueueStatusEnum]    Script Date: 07/16/2008 23:49:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueueStatusEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WorkQueueStatusEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_WorkQueueStatusEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_WorkQueueStatusEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[StudyStatusEnum]    Script Date: 07/16/2008 23:49:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StudyStatusEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StudyStatusEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_StudyStatusEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_StudyStatusEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[QueueStudyStateEnum]    Script Date: 08/26/2008 15:22:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueueStudyStateEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[QueueStudyStateEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_QueueStudyStateEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_QueueStudyStateEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WorkQueueTypeEnum]    Script Date: 07/16/2008 23:49:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WorkQueueTypeEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_WorkQueueTypeEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_WorkQueueTypeEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ServerSopClass]    Script Date: 07/16/2008 23:49:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServerSopClass]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ServerSopClass](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_SopClass_GUID]  DEFAULT (newid()),
	[SopClassUid] [varchar](64) NOT NULL,
	[Description] [nvarchar](128) NOT NULL,
	[NonImage] [bit] NOT NULL,
	[ImplicitOnly] [bit] NOT NULL DEFAULT(0)
 CONSTRAINT [PK_SopClass] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServerSopClass]') AND name = N'IX_SopClass_SopClassUid')
CREATE UNIQUE NONCLUSTERED INDEX [IX_SopClass_SopClassUid] ON [dbo].[ServerSopClass] 
(
	[SopClassUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
GO
/****** Object:  Table [dbo].[ServiceLockTypeEnum]    Script Date: 07/16/2008 23:49:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServiceLockTypeEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ServiceLockTypeEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ServiceLockTypeEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_ServiceLockTypeEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ServerRuleTypeEnum]    Script Date: 07/16/2008 23:49:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServerRuleTypeEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ServerRuleTypeEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ServerRuleTypeEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_ServerRuleTypeEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ServerRuleApplyTimeEnum]    Script Date: 07/16/2008 23:49:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServerRuleApplyTimeEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ServerRuleApplyTimeEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ServerRuleApplyTimeEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_ServerRuleApplyTimeEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FilesystemTierEnum]    Script Date: 01/09/2008 15:03:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FilesystemTierEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FilesystemTierEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_FilesystemTierEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_FilesystemTier] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FilesystemQueueTypeEnum]    Script Date: 01/09/2008 15:03:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FilesystemQueueTypeEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FilesystemQueueTypeEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_FilesystemQueueTypeEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_FilesystemQueueTypeEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WorkQueuePriorityEnum]    Script Date: 03/12/2008 14:30:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[[WorkQueuePriorityEnum]]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WorkQueuePriorityEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_WorkQueuePriorityEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_WorkQueuePriorityEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[DevicePreferredTransferSyntax]    Script Date: 01/09/2008 15:03:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DevicePreferredTransferSyntax]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DevicePreferredTransferSyntax](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_DevicePreferredTransferSyntax_GUID]  DEFAULT (newid()),
	[DeviceGUID] [uniqueidentifier] NOT NULL,
	[ServerSopClassGUID] [uniqueidentifier] NOT NULL,
	[ServerTransferSyntaxGUID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_DevicePreferredTransferSyntax] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DevicePreferredTransferSyntax]') AND name = N'IX_DevicePreferredTransferSyntax')
CREATE CLUSTERED INDEX [IX_DevicePreferredTransferSyntax] ON [dbo].[DevicePreferredTransferSyntax] 
(
	[DeviceGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServerRule]    Script Date: 01/09/2008 15:03:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServerRule]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ServerRule](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ServerRule_GUID]  DEFAULT (newid()),
	[RuleName] [nvarchar](128) NOT NULL,
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[ServerRuleTypeEnum] [smallint] NOT NULL,
	[ServerRuleApplyTimeEnum] [smallint] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[DefaultRule] [bit] NOT NULL,
	[ExemptRule] [bit] NOT NULL CONSTRAINT [DF_ServerRule_NotRule]  DEFAULT ((0)),
	[RuleXml] [xml] NOT NULL,
 CONSTRAINT [PK_ServerRule] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[WorkQueue]    Script Date: 01/09/2008 15:04:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WorkQueue](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_WorkQueue_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[StudyStorageGUID] [uniqueidentifier] NOT NULL,
	[DeviceGUID] [uniqueidentifier] NULL,
	[StudyHistoryGUID] [uniqueidentifier] NULL,
	[WorkQueueTypeEnum] [smallint] NOT NULL,
	[WorkQueueStatusEnum] [smallint] NOT NULL, 
	[WorkQueuePriorityEnum] [smallint] NOT NULL CONSTRAINT [DF_WorkQueue_WorkQueuePriorityEnum]  DEFAULT ((200)),
	[ProcessorID] [varchar](128) NULL,
	[GroupID] [varchar] (64) NULL,
	[ExpirationTime] [datetime] NULL,
	[ScheduledTime] [datetime] NOT NULL,
	[InsertTime] [datetime] NOT NULL CONSTRAINT [DF_WorkQueue_InsertTime]  DEFAULT (getdate()),
	[LastUpdatedTime] [datetime] NULL,
	[FailureCount] [int] NOT NULL CONSTRAINT [DF_WorkQueue_FailureCount]  DEFAULT ((0)),
	[FailureDescription] [nvarchar](512) NULL,
	[Data] [xml] NULL,
	[ExternalRequestQueueGUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_WorkQueue] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [QUEUES]
) ON [QUEUES]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueue]') AND name = N'IX_WorkQueue_ScheduledTime')
CREATE NONCLUSTERED INDEX [IX_WorkQueue_ScheduledTime] ON [dbo].[WorkQueue] 
(
	[ScheduledTime] ASC,
	[WorkQueueStatusEnum] ASC,
	[WorkQueueTypeEnum] ASC
)
INCLUDE ( [StudyStorageGUID], [WorkQueuePriorityEnum]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueue]') AND name = N'IX_WorkQueue_StudyStorageGUID')
CREATE NONCLUSTERED INDEX [IX_WorkQueue_StudyStorageGUID] ON [dbo].[WorkQueue] 
(
	[StudyStorageGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueue]') AND name = N'IX_WorkQueue_GroupID')
CREATE NONCLUSTERED INDEX [IX_WorkQueue_GroupID] ON [dbo].[WorkQueue] 
(
	[GroupID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [INDEXES]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueue]') AND name = N'IX_WorkQueue_DeviceGUID')
CREATE NONCLUSTERED INDEX [IX_WorkQueue_DeviceGUID] ON [dbo].[WorkQueue] 
(
	[DeviceGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO

/****** Object:  Table [dbo].[Series]    Script Date: 01/09/2008 15:03:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Series]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Series](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_Series_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[StudyGUID] [uniqueidentifier] NOT NULL,
	[SeriesInstanceUid] [varchar](64) NOT NULL,
	[Modality] [varchar](16) NOT NULL,
	[SeriesNumber] [varchar](12) NULL,
	[SeriesDescription] [nvarchar](64) NULL,
	[NumberOfSeriesRelatedInstances] [int] NOT NULL,
	[PerformedProcedureStepStartDate] [varchar](8) NULL,
	[PerformedProcedureStepStartTime] [varchar](16) NULL,
	[SourceApplicationEntityTitle] [varchar](16) NULL,
 CONSTRAINT [PK_Series] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Series]') AND name = N'IX_Series_StudyGUID_SeriesInstanceUid')
CREATE UNIQUE CLUSTERED INDEX [IX_Series_StudyGUID_SeriesInstanceUid] ON [dbo].[Series] 
(
	[StudyGUID] ASC,
	[SeriesInstanceUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Series]') AND name = N'IX_Series_Modality')
CREATE NONCLUSTERED INDEX [IX_Series_Modality] ON [dbo].[Series] 
(
	[Modality] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
/****** Object:  Table [dbo].[Study]    Script Date: 11/22/2010 13:42:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Study](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_Study_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[StudyStorageGUID] [uniqueidentifier] NULL,
	[PatientGUID] [uniqueidentifier] NOT NULL,
	[SpecificCharacterSet] varchar(128) NULL,
	[StudyInstanceUid] [varchar](64) NOT NULL,
	[PatientsName] [nvarchar](64) NULL,
	[PatientId] [nvarchar](64) NULL,
	[IssuerOfPatientId] [nvarchar](64) NULL,
	[PatientsBirthDate] [varchar](8) NULL,
	[PatientsAge] [varchar](4) NULL,
	[PatientsSex] [varchar](2) NULL,
	[StudyDate] [varchar](8) NULL,
	[StudyTime] [varchar](16) NULL,
	[AccessionNumber] [nvarchar](16) NULL,
	[StudyId] [nvarchar](16) NULL,
	[StudyDescription] [nvarchar](64) NULL,
	[ReferringPhysiciansName] [nvarchar](64) NULL,
	[NumberOfStudyRelatedSeries] [int] NOT NULL,
	[NumberOfStudyRelatedInstances] [int] NOT NULL,
	[StudySizeInKB] [decimal](18, 0) NULL,
	ResponsiblePerson nvarchar(64) NULL,
	ResponsibleOrganization nvarchar(64) NULL,
	QueryXml xml NULL,
	QCStatusEnum [smallint] NOT NULL,
	QCOutput varchar(max) NULL,
	QCUpdateTimeUtc datetime NULL,
	OrderGUID uniqueidentifier NULL,
 CONSTRAINT [PK_Study] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IXC_Study_PatientsName')
CREATE CLUSTERED INDEX [IXC_Study_PatientsName] ON [dbo].[Study] 
(
	[PatientsName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IX_Study_AccessionNumber')
CREATE NONCLUSTERED INDEX [IX_Study_AccessionNumber] ON [dbo].[Study] 
(
	[AccessionNumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IX_Study_PatientGUID')
CREATE NONCLUSTERED INDEX [IX_Study_PatientGUID] ON [dbo].[Study] 
(
	[PatientGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IX_Study_PatientId')
CREATE NONCLUSTERED INDEX [IX_Study_PatientId] ON [dbo].[Study] 
(
	[PatientId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IX_Study_ReferringPhysiciansName')
CREATE NONCLUSTERED INDEX [IX_Study_ReferringPhysiciansName] ON [dbo].[Study] 
(
	[ReferringPhysiciansName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IX_Study_StudyDate')
CREATE NONCLUSTERED INDEX [IX_Study_StudyDate] ON [dbo].[Study] 
(
	[StudyDate] ASC,
	[QCStatusEnum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IX_Study_StudyDescription')
CREATE NONCLUSTERED INDEX [IX_Study_StudyDescription] ON [dbo].[Study] 
(
	[StudyDescription] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IX_Study_StudyInstanceUid')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Study_StudyInstanceUid] ON [dbo].[Study] 
(
	[StudyInstanceUid] ASC,
	[ServerPartitionGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IX_Study_StudyStorageGUID')
CREATE NONCLUSTERED INDEX [IX_Study_StudyStorageGUID] ON [dbo].[Study] 
(
	[StudyStorageGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Study]') AND name = N'IX_Study_QCUpdateTime')
CREATE NONCLUSTERED INDEX [IX_Study_QCUpdateTime] ON [dbo].[Study]
(
	[QCUpdateTimeUtc] DESC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [INDEXES]
GO


/****** Object:  Table [dbo].[PartitionSopClass]    Script Date: 01/09/2008 15:03:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PartitionSopClass]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PartitionSopClass](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_PartitionSopClass_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[ServerSopClassGUID] [uniqueidentifier] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_PartitionSopClass] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[StudyStorage]    Script Date: 01/09/2008 15:04:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StudyStorage]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StudyStorage](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_StudyStorage_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[StudyInstanceUid] [varchar](64) NOT NULL,
	[InsertTime] [datetime] NOT NULL CONSTRAINT [DF_StudyStorage_InsertTime]  DEFAULT (getdate()),
	[LastAccessedTime] [datetime] NOT NULL CONSTRAINT [DF_StudyStorage_LastAccessedTime]  DEFAULT (getdate()),
	[WriteLock] [bit] NOT NULL,
	[ReadLock] [smallint] NOT NULL,
    [StudyStatusEnum] [smallint] NOT NULL,
	[QueueStudyStateEnum] [smallint] NOT NULL,
 CONSTRAINT [PK_StudyStorage] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StudyStorage]') AND name = N'IX_StudyStorage_PartitionGUID_StudyInstanceUid')
CREATE UNIQUE NONCLUSTERED INDEX [IX_StudyStorage_PartitionGUID_StudyInstanceUid] ON [dbo].[StudyStorage] 
(
	[ServerPartitionGUID] ASC,
	[StudyInstanceUid] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
/****** Object:  Default [DF_StudyStorage_WriteLock]    Script Date: 10/30/2009 12:15:04 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_StudyStorage_WriteLock]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyStorage]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_StudyStorage_WriteLock]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[StudyStorage] ADD  CONSTRAINT DF_StudyStorage_WriteLock  DEFAULT ((0)) FOR [WriteLock]
END
End
GO
/****** Object:  Default [DF_StudyStorage_ReadLock]    Script Date: 10/30/2009 12:15:04 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_StudyStorage_ReadLock]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyStorage]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_StudyStorage_ReadLock]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[StudyStorage] ADD  CONSTRAINT [DF_StudyStorage_ReadLock]  DEFAULT ((0)) FOR [ReadLock]
END
End
GO
/****** Object:  Table [dbo].[Patient]    Script Date: 01/09/2008 15:03:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Patient]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Patient](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_Patient_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[PatientsName] [nvarchar](64) NULL,
	[PatientId] [nvarchar](64) NULL,
	[IssuerOfPatientId] [nvarchar](64) NULL,
	[NumberOfPatientRelatedStudies] [int] NOT NULL,
	[NumberOfPatientRelatedSeries] [int] NOT NULL,
	[NumberOfPatientRelatedInstances] [int] NOT NULL,
	[SpecificCharacterSet] varchar(128) NULL
 CONSTRAINT [PK_Patient] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Device]    Script Date: 04/23/2008 23:48:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Device]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Device](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_Device_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[AeTitle] [varchar](16) NOT NULL,
	[IpAddress] [varchar](16) NULL,
	[Port] [int] NOT NULL,
	[Description] [nvarchar](256) NULL,
	[Dhcp] [bit] NOT NULL,
	[Enabled] [bit] NOT NULL,
	[AllowStorage] [bit] NOT NULL CONSTRAINT [DF_Device_StorageFlag]  DEFAULT ((0)),
	[AcceptKOPR] [bit] NOT NULL CONSTRAINT [DF_Device_AcceptKOPRFlag]  DEFAULT ((0)),
	[AllowRetrieve] [bit] NOT NULL CONSTRAINT [DF_Device_AllowRetrieve]  DEFAULT ((0)),
	[AllowQuery] [bit] NOT NULL CONSTRAINT [DF_Device_AllowQuery]  DEFAULT ((0)),
	[AllowAutoRoute] [bit] NOT NULL CONSTRAINT [DF_Device_AllowAutoRoute]  DEFAULT ((1)),
	[ThrottleMaxConnections] [smallint] NOT NULL CONSTRAINT [DF_Device_MaxConnections]  DEFAULT ((-1)),
    [LastAccessedTime] [datetime] NOT NULL CONSTRAINT [DF_Device_LastAccessedTime]  DEFAULT (getdate()),
	[DeviceTypeEnum] [smallint] NOT NULL CONSTRAINT [DF_Device_DeviceTypeEnum]  DEFAULT ((100)),
CONSTRAINT [PK_Device] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Device]') AND name = N'IX_Device_ServerPartitionGUID_AeTitle')
CREATE NONCLUSTERED INDEX [IX_Device_ServerPartitionGUID_AeTitle] ON [dbo].[Device] 
(
	[ServerPartitionGUID] ASC,
	[AeTitle] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
/****** Object:  Table [dbo].[FilesystemQueue]    Script Date: 01/09/2008 15:03:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FilesystemQueue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FilesystemQueue](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_FilesystemQueue_GUID]  DEFAULT (newid()),
	[FilesystemQueueTypeEnum] [smallint] NOT NULL,
	[StudyStorageGUID] [uniqueidentifier] NOT NULL,
	[FilesystemGUID] [uniqueidentifier] NOT NULL,
	[ScheduledTime] [datetime] NOT NULL,
	[SeriesInstanceUid] [varchar](64) NULL,
	[QueueXml] [xml] NULL,
 CONSTRAINT [PK_FilesystemQueue] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
) ON [QUEUES]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FilesystemQueue]') AND name = N'IXC_FilesystemQueue')
CREATE CLUSTERED INDEX [IXC_FilesystemQueue] ON [dbo].[FilesystemQueue] 
(
	[FilesystemGUID] ASC,
	[ScheduledTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [QUEUES]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FilesystemQueue]') AND name = N'IX_FilesystemQueue_StudyStorageGUID')
CREATE UNIQUE NONCLUSTERED INDEX [IX_FilesystemQueue_StudyStorageGUID] ON [dbo].[FilesystemQueue] 
(
	[StudyStorageGUID] ASC,
	[FilesystemQueueTypeEnum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
/****** Object:  Table [dbo].[ServiceLock]    Script Date: 01/09/2008 15:04:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServiceLock]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ServiceLock](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ServiceLock_GUID]  DEFAULT (newid()),
	[ServiceLockTypeEnum] [smallint] NOT NULL,
	[ProcessorId] [varchar](128) NULL,
	[Lock] [bit] NOT NULL,
	[ScheduledTime] [datetime] NOT NULL,
	[FilesystemGUID] [uniqueidentifier] NULL,
	[Enabled] [bit] NOT NULL CONSTRAINT [DF_ServiceLock_Enabled]  DEFAULT ((1)),
	[State] [xml] NULL,
	[ServerPartitionGUID] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ServiceLock] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FilesystemStudyStorage]    Script Date: 07/16/2008 23:48:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FilesystemStudyStorage]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[FilesystemStudyStorage](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_StorageFilesystem_GUID]  DEFAULT (newid()),
	[StudyStorageGUID] [uniqueidentifier] NOT NULL,
	[FilesystemGUID] [uniqueidentifier] NOT NULL,
	[ServerTransferSyntaxGUID] [uniqueidentifier] NOT NULL,
	[StudyFolder] [varchar](8) NOT NULL,
 CONSTRAINT [PK_StorageFilesystem] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FilesystemStudyStorage]') AND name = N'IX_StorageFilesystem_StudyStorageGUID')
CREATE CLUSTERED INDEX [IX_StorageFilesystem_StudyStorageGUID] ON [dbo].[FilesystemStudyStorage] 
(
	[StudyStorageGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[FilesystemStudyStorage]') AND name = N'IX_StorageFilesystem_FilesystemGUID')
CREATE NONCLUSTERED INDEX [IX_StorageFilesystem_FilesystemGUID] ON [dbo].[FilesystemStudyStorage] 
(
	[FilesystemGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
/****** Object:  Table [dbo].[WorkQueueUid]    Script Date: 01/09/2008 15:04:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueueUid]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WorkQueueUid](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_WorkQueueInstance_GUID]  DEFAULT (newid()),
	[WorkQueueGUID] [uniqueidentifier] NOT NULL,
	[SeriesInstanceUid] [varchar](64) NULL,
	[SopInstanceUid] [varchar](64) NULL,
	[Failed] [bit] NOT NULL CONSTRAINT [DF_WorkQueueUid_Failed]  DEFAULT ((0)),
	[Duplicate] [bit] NOT NULL CONSTRAINT [DF_WorkQueueUid_Duplicate]  DEFAULT ((0)),
	[Extension] [varchar](10) NULL,
	[FailureCount] [smallint] NOT NULL CONSTRAINT [DF_WorkQueueUid_FailureCount]  DEFAULT ((0)),
	[GroupID] [varchar] (64) NULL,
	[RelativePath] [varchar] (256) NULL,
	[WorkQueueUidData] [xml] NULL
 CONSTRAINT [PK_WorkQueueUid] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [QUEUES]
) ON [QUEUES]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueueUid]') AND name = N'IX_WorkQueueUid')
CREATE CLUSTERED INDEX [IX_WorkQueueUid] ON [dbo].[WorkQueueUid] 
(
	[WorkQueueGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 65) ON [QUEUES]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueueUid]') AND name = N'IX_WorkQueueUid_GroupID')
CREATE NONCLUSTERED INDEX [IX_WorkQueueUid_GroupID] ON [dbo].[WorkQueueUid] 
(
	[GroupID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[RequestAttributes]    Script Date: 01/09/2008 15:03:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RequestAttributes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RequestAttributes](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_RequestAttribute_GUID]  DEFAULT (newid()),
	[SeriesGUID] [uniqueidentifier] NOT NULL,
	[RequestedProcedureId] [nvarchar](16) NULL,
	[ScheduledProcedureStepId] [nvarchar](16) NULL,
 CONSTRAINT [PK_RequestAttribute] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RequestAttributes]') AND name = N'IX_RequestAttribute_SeriesGUID')
CREATE CLUSTERED INDEX [IX_RequestAttribute_SeriesGUID] ON [dbo].[RequestAttributes] 
(
	[SeriesGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Filesystem]    Script Date: 01/09/2008 15:03:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Filesystem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Filesystem](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_Filesystem_GUID]  DEFAULT (newid()),
	[FilesystemPath] [nvarchar](256) NOT NULL,
	[Enabled] [bit] NOT NULL,
	[ReadOnly] [bit] NOT NULL,
	[WriteOnly] [bit] NOT NULL,
	[Description] [nvarchar](128) NULL,
	[FilesystemTierEnum] [smallint] NOT NULL,
	[LowWatermark] [decimal](8, 4) NOT NULL CONSTRAINT [DF_Filesystem_LowWatermark]  DEFAULT ((80.00)),
	[HighWatermark] [decimal](8, 4) NOT NULL CONSTRAINT [DF_Filesystem_HighWatermark]  DEFAULT ((90.00)),
 CONSTRAINT [PK_Filesystem] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
/****** Object:  Table [dbo].[PartitionTransferSyntax]    Script Date: 06/24/2008 16:42:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PartitionTransferSyntax]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PartitionTransferSyntax](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_PartitionTransferSyntax_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[ServerTransferSyntaxGUID] [uniqueidentifier] NOT NULL,
	[Enabled] [bit] NOT NULL,
 CONSTRAINT [PK_PartitionTransferSyntax] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ArchiveTypeEnum]    Script Date: 07/08/2008 18:10:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArchiveTypeEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ArchiveTypeEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ArchiveTypeEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_ArchiveTypeEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ArchiveQueueStatusEnum]    Script Date: 07/08/2008 18:10:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArchiveQueueStatusEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ArchiveQueueStatusEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ArchiveQueueStatusEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_ArchiveQueueStatusEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RestoreQueueStatusEnum]    Script Date: 07/08/2008 18:10:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RestoreQueueStatusEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RestoreQueueStatusEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_RestoreQueueStatusEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_RestoreQueueStatusEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PartitionArchive]    Script Date: 07/08/2008 18:10:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PartitionArchive]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PartitionArchive](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_PartitionArchive_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[ArchiveTypeEnum] [smallint] NOT NULL,
	[Description] [nvarchar](128) NOT NULL,
	[Enabled] [bit] NOT NULL,
	[ReadOnly] [bit] NOT NULL,
	[ArchiveDelayHours] [int] NOT NULL,
	[ConfigurationXml] [xml] NULL,
 CONSTRAINT [PK_PartitionArchive] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
/****** Object:  Table [dbo].[RestoreQueue]    Script Date: 01/23/2009 16:59:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RestoreQueue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[RestoreQueue](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_RestoreQueue_GUID]  DEFAULT (newid()),
	[ArchiveStudyStorageGUID] [uniqueidentifier] NOT NULL,
	[StudyStorageGUID] [uniqueidentifier] NOT NULL,
	[ScheduledTime] [datetime] NOT NULL,
	[RestoreQueueStatusEnum] [smallint] NOT NULL,
	[ProcessorId] [varchar](128) NULL,
	[FailureDescription] [nvarchar](512) NULL,
 CONSTRAINT [PK_RestoreQueue] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
) ON [QUEUES]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RestoreQueue]') AND name = N'IXC_RestoreQueue')
CREATE CLUSTERED INDEX [IXC_RestoreQueue] ON [dbo].[RestoreQueue] 
(
	[ScheduledTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [QUEUES]
GO
CREATE NONCLUSTERED INDEX [IX_RestoreQueue] ON [dbo].[RestoreQueue] 
(
	[StudyStorageGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO

/****** Object:  Table [dbo].[ArchiveStudyStorage]    Script Date: 01/23/2009 16:59:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArchiveStudyStorage]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ArchiveStudyStorage](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_StorageArchive_GUID]  DEFAULT (newid()),
	[PartitionArchiveGUID] [uniqueidentifier] NOT NULL,
	[StudyStorageGUID] [uniqueidentifier] NOT NULL,
	[ServerTransferSyntaxGUID] [uniqueidentifier] NOT NULL,
	[ArchiveTime] [datetime] NOT NULL,
	[ArchiveXml] [xml] NULL,
 CONSTRAINT [PK_StorageArchive] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
) ON [PRIMARY]
END
GO
CREATE CLUSTERED INDEX [IXC_ArchiveStudyStorage] ON [dbo].[ArchiveStudyStorage] 
(
	[StudyStorageGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[ArchiveQueue]    Script Date: 01/23/2009 16:59:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ArchiveQueue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ArchiveQueue](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ArchiveQueue_GUID]  DEFAULT (newid()),
	[PartitionArchiveGUID] [uniqueidentifier] NOT NULL,
	[ScheduledTime] [datetime] NOT NULL,
	[StudyStorageGUID] [uniqueidentifier] NOT NULL,
	[ArchiveQueueStatusEnum] [smallint] NOT NULL,
	[ProcessorId] [varchar](128) NULL,
 	[FailureDescription] [nvarchar](512) NULL,
CONSTRAINT [PK_ArchiveQueue] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
) ON [QUEUES]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ArchiveQueue]') AND name = N'IXC_ArchiveQueue')
CREATE CLUSTERED INDEX [IXC_ArchiveQueue] ON [dbo].[ArchiveQueue] 
(
	[ScheduledTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [QUEUES]
GO
CREATE NONCLUSTERED INDEX [IX_ArchiveQueue] ON [dbo].[ArchiveQueue] 
(
	[StudyStorageGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO


/****** Object:  Table [dbo].[AlertCategoryEnum]    Script Date: 07/16/2008 23:49:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlertCategoryEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AlertCategoryEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_AlertCategoryEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_AlertCategoryEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF

/****** Object:  Table [dbo].[AlertLevelEnum]    Script Date: 07/16/2008 23:49:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlertLevelEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AlertLevelEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_AlertLevelEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_AlertLevelEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF


GO
/****** Object:  Table [dbo].[Alert]    Script Date: 01/23/2009 16:59:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Alert]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Alert](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_Alert_GUID]  DEFAULT (newid()),
	[InsertTime] [datetime] NOT NULL,
	[Component] [nvarchar](50)  NOT NULL,
	[TypeCode] [int] NOT NULL,
	[Source] [nvarchar](256) NOT NULL,
	[AlertLevelEnum] [smallint] NOT NULL,
	[AlertCategoryEnum] [smallint] NOT NULL,
	[Content] [xml] NOT NULL,
 CONSTRAINT [PK_SystemAlert] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Table [dbo].[StudyIntegrityReasonEnum]    Script Date: 09/05/2008 11:48:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StudyIntegrityReasonEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StudyIntegrityReasonEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_StudyIntegrityReasonEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_StudyIntegrityStatusEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[StudyIntegrityQueue]    Script Date: 01/23/2009 16:59:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudyIntegrityQueue](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_StudyIntegrityQueue_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[StudyStorageGUID] [uniqueidentifier] NOT NULL,
	[InsertTime] [datetime] NOT NULL CONSTRAINT [DF_StudyIntegrityQueue_InsertTime]  DEFAULT (getdate()),
	[Description] [nvarchar](1024),
	[StudyData] [xml] NOT NULL,
	[Details] [xml] NULL,
	[StudyIntegrityReasonEnum] [smallint] NOT NULL,
	[GroupID] [varchar] (64) NULL
 CONSTRAINT [PK_StudyIntegrityQueue] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [QUEUES]
) ON [QUEUES]
GO
CREATE NONCLUSTERED INDEX [IX_StudyIntegrityQueue] ON [dbo].[StudyIntegrityQueue] 
(
	[StudyStorageGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO

CREATE NONCLUSTERED INDEX [IX_StudyIntegrityQueue_GroupID] ON [dbo].[StudyIntegrityQueue] 
(
	[GroupID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[StudyIntegrityQueueUid]    Script Date: 04/30/2009 19:55:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StudyIntegrityQueueUid](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_StudyIntegrityQueueUid_GUID]  DEFAULT (newid()),
	[StudyIntegrityQueueGUID] [uniqueidentifier] NOT NULL,
	[SeriesDescription] [nvarchar](64) NULL,
	[SeriesInstanceUid] [varchar](64) NOT NULL,
	[SopInstanceUid] [varchar](64) NOT NULL,
	[RelativePath] [varchar] (256) NULL,
 CONSTRAINT [PK_StudyIntegrityQueueUid] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [INDEXES]
) ON [QUEUES]

GO

ALTER TABLE [dbo].[StudyIntegrityQueueUid]  WITH CHECK ADD  CONSTRAINT [FK_StudyIntegrityQueueUid_StudyIntegrityQueue] FOREIGN KEY([StudyIntegrityQueueGUID])
REFERENCES [dbo].[StudyIntegrityQueue] ([GUID])

/****** Object:  Index [IX_StudyIntegrityQueueUid]    Script Date: 05/01/2009 15:48:04 ******/
CREATE CLUSTERED INDEX [IX_StudyIntegrityQueueUid] ON [dbo].[StudyIntegrityQueueUid] 
(
	[StudyIntegrityQueueGUID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [QUEUES]

GO
/****** Object:  Index [IX_StudyIntegrityQueueUid_SeriesInstanceUid]    Script Date: 05/01/2009 15:49:26 ******/
CREATE NONCLUSTERED INDEX [IX_StudyIntegrityQueueUid_SeriesInstanceUid] ON [dbo].[StudyIntegrityQueueUid] 
(
	[SeriesInstanceUid] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]

GO

/****** Object:  Table [dbo].[StudyHistory]    Script Date: 01/23/2009 16:59:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StudyHistory](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_StudyHistory_GUID]  DEFAULT (newid()),
	[InsertTime] [datetime] NOT NULL CONSTRAINT [DF_StudyHistory_InsertTime]  DEFAULT (getdate()),
	[StudyStorageGUID] [uniqueidentifier] NOT NULL,
	[DestStudyStorageGUID] [uniqueidentifier] NULL,
	[StudyHistoryTypeEnum] [smallint] NOT NULL,
	[StudyData] [xml] NOT NULL,
	[ChangeDescription] [xml] NULL,
 CONSTRAINT [PK_StudyHistory] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [IX_StudyHistory] ON [dbo].[StudyHistory] 
(
	[StudyStorageGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_StudyHistory_DestStudyStorageGUID ON dbo.StudyHistory
	(
	DestStudyStorageGUID
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO

/****** Object:  Table [dbo].[StudyHistoryTypeEnum]    Script Date: 09/26/2008 23:49:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StudyHistoryTypeEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StudyHistoryTypeEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_StudyHistoryTypeEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_StudyHistoryTypeEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO

/****** Object:  Table [dbo].[StudyDeleteRecord]    Script Date: 12/16/2008 15:06:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StudyDeleteRecord](
	[GUID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_StudyDeleteRecord_GUID]  DEFAULT (newid()),
	[Timestamp] [datetime] NOT NULL,
	[Reason] [nvarchar](1024)  NULL,
	[ServerPartitionAE] [varchar](64)  NOT NULL,
	[FilesystemGUID] [uniqueidentifier] NOT NULL,
	[BackupPath] [nvarchar](256)  NULL,
	[StudyInstanceUid] [varchar](64)  NOT NULL,
	[AccessionNumber] [varchar](64)  NULL,
	[PatientId] [varchar](64)  NULL,
	[PatientsName] [nvarchar](256)  NULL,
	[StudyId] [nvarchar](64)  NULL,
	[StudyDescription] [nvarchar](64)  NULL,
	[StudyDate] [varchar](16)  NULL,
	[StudyTime] [varchar](32)  NULL,
	[ArchiveInfo] [xml] NULL,
	[ExtendedInfo] [nvarchar](max)  NULL,
 CONSTRAINT [PK_StudyDeleteRecord] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
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

/****** Object:  Table [dbo].[ApplicationLog]    Script Date: 01/12/2009 15:36:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationLog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ApplicationLog](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_ApplicationLog_GUID]  DEFAULT (newid()),
	[Host] [varchar](50) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[LogLevel] [varchar](5) NOT NULL,
	[Thread] [varchar](50) NOT NULL,
	[Message] [nvarchar](2000) NOT NULL,
	[Exception] [nvarchar](2000) NULL,
 CONSTRAINT [PK_ApplicationLog] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [QUEUES]
) ON [QUEUES]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ApplicationLog]') AND name = N'IX_ApplicationLog')
CREATE CLUSTERED INDEX [IX_ApplicationLog] ON [dbo].[ApplicationLog] 
(
	[Timestamp] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [QUEUES]
GO

/****** Object:  Table [dbo].[DatabaseVersion_]    Script Date: 02/16/2009 15:36:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DatabaseVersion_]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DatabaseVersion_](
	[Major_] [nvarchar](5) NOT NULL,
	[Minor_] [nvarchar](5) NOT NULL,	
	[Build_] [nvarchar](5) NOT NULL,
	[Revision_] [nvarchar](5) NOT NULL
) ON [PRIMARY]
END
GO



/****** Object:  Index [IX_StudyDeleteRecord]    Script Date: 11/28/2008 13:26:28 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_StudyDeleteRecord] ON [dbo].[StudyDeleteRecord] 
(
	[GUID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO

/****** Object:  Index [IX_StudyDeleteRecord_AcccessionNumber]    Script Date: 11/28/2008 13:26:56 ******/
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_AcccessionNumber] ON [dbo].[StudyDeleteRecord] 
(
	[AccessionNumber] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]

GO
/****** Object:  Index [IX_StudyDeleteRecord_PatientId]    Script Date: 11/28/2008 13:27:11 ******/
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_PatientId] ON [dbo].[StudyDeleteRecord] 
(
	[PatientId] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]

GO
/****** Object:  Index [IX_StudyDeleteRecord_PatientsName]    Script Date: 11/28/2008 13:27:19 ******/
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_PatientsName] ON [dbo].[StudyDeleteRecord] 
(
	[PatientsName] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO

/****** Object:  Index [IX_StudyDeleteRecord_ServerPartition]    Script Date: 12/04/2008 14:54:55 ******/
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_ServerPartition] ON [dbo].[StudyDeleteRecord] 
(
	[ServerPartitionAE] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO

/****** Object:  Index [IX_StudyDeleteRecord_StudyInstanceUid]    Script Date: 11/28/2008 13:27:35 ******/
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_StudyInstanceUid] ON [dbo].[StudyDeleteRecord] 
(
	[StudyInstanceUid] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]

GO
/****** Object:  Index [IX_StudyDeleteRecord_StudyInstanceUidServerPartition]    Script Date: 12/04/2008 14:55:57 ******/
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_StudyInstanceUidServerPartition] ON [dbo].[StudyDeleteRecord] 
(
	[StudyInstanceUid] ASC,
	[ServerPartitionAE] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StudyDeleteRecord_Timestamp]    Script Date: 11/28/2008 13:27:49 ******/
CREATE NONCLUSTERED INDEX [IX_StudyDeleteRecord_Timestamp] ON [dbo].[StudyDeleteRecord] 
(
	[Timestamp] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO


GO
/****** Object:  Table [dbo].[CannedText]    Script Date: 02/23/2009 20:02:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CannedText](
	[GUID] [uniqueidentifier] NOT NULL,
	[Label] [nvarchar](50)  NOT NULL,
	[Category] [nvarchar](255)  NOT NULL,
	[Text] [nvarchar](1024)  NOT NULL,
 CONSTRAINT [PK_CannedText] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


/****** Object:  Index [IX_CannedText_Category]    Script Date: 02/23/2009 20:04:17 ******/
CREATE NONCLUSTERED INDEX [IX_CannedText_Category] ON [dbo].[CannedText] 
(
	[Category] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]


GO
/****** Object:  Index [IX_CannedText]    Script Date: 01/28/2010 15:04:33 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_CannedText] ON [dbo].[CannedText] 
(
	[Label] ASC,
	[Category] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

/****** Object:  Table [dbo].[DeviceTypeEnum]    Script Date: 01/09/2008 15:03:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeviceTypeEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DeviceTypeEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [DF_DeviceTypeEnum_GUID]  DEFAULT (newid()),
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_DeviceTypeEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO


/****** Object:  Table [dbo].[WorkQueueTypeProperties]    Script Date: 09/14/2009 21:30:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[WorkQueueTypeProperties](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[WorkQueueTypeEnum] [smallint] NOT NULL,
	[WorkQueuePriorityEnum] [smallint] NOT NULL,
	[MemoryLimited] [bit] NOT NULL,
	[AlertFailedWorkQueue] [bit] NOT NULL,
	[MaxFailureCount] [int] NOT NULL,
	[ProcessDelaySeconds] [int] NOT NULL,
	[FailureDelaySeconds] [int] NOT NULL,
	[DeleteDelaySeconds] [int] NOT NULL,
	[PostponeDelaySeconds] [int] NOT NULL,
	[ExpireDelaySeconds] [int] NOT NULL,
	[MaxBatchSize] [int] NOT NULL,
	[QueueStudyStateEnum] [smallint] NULL,
	[QueueStudyStateOrder] [smallint] NULL,	
	[ReadLock] [bit] NOT NULL,
	[WriteLock] [bit] NOT NULL,
 CONSTRAINT [PK_WorkQueueTypeProperties] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]') AND name = N'IX_WorkQueueTypeProperties')
CREATE UNIQUE NONCLUSTERED INDEX [IX_WorkQueueTypeProperties] ON [dbo].[WorkQueueTypeProperties] 
(
	[WorkQueueTypeEnum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
/****** Object:  Default [DF_WorkQueueTypeProperties_GUID]    Script Date: 09/14/2009 21:30:31 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_WorkQueueTypeProperties_GUID]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_WorkQueueTypeProperties_GUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[WorkQueueTypeProperties] ADD  CONSTRAINT [DF_WorkQueueTypeProperties_GUID]  DEFAULT (newid()) FOR [GUID]
END
End

GO
/****** Object:  Index [IX_WorkQueueTypeProperties_QueueStudyStateEnum]    Script Date: 10/26/2009 16:49:07 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]') AND name = N'IX_WorkQueueTypeProperties_QueueStudyStateEnum')
CREATE NONCLUSTERED INDEX [IX_WorkQueueTypeProperties_QueueStudyStateEnum] ON [dbo].[WorkQueueTypeProperties] 
(
	[QueueStudyStateEnum] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [INDEXES]
GO

/****** Object:  Index [IX_WorkQueueTypeProperties_QueueStudyStateOrder]    Script Date: 10/26/2009 16:50:02 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]') AND name = N'IX_WorkQueueTypeProperties_QueueStudyStateOrder')
CREATE NONCLUSTERED INDEX [IX_WorkQueueTypeProperties_QueueStudyStateOrder] ON [dbo].[WorkQueueTypeProperties] 
(
	[QueueStudyStateOrder] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [INDEXES]


/****** Object:  Table [dbo].[DataAccessGroup]    Script Date: 06/22/2011 14:11:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DataAccessGroup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[DataAccessGroup](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[AuthorityGroupOID] [uniqueidentifier] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_DataAccessGroup] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [STATIC]
) ON [STATIC]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DataAccessGroup]') AND name = N'IX_DataAccessGroup_AuthorityGroupOID')
CREATE NONCLUSTERED INDEX [IX_DataAccessGroup_AuthorityGroupOID] ON [dbo].[DataAccessGroup] 
(
	[AuthorityGroupOID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StudyDataAccess]    Script Date: 06/22/2011 14:11:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StudyDataAccess]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StudyDataAccess](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[StudyStorageGUID] [uniqueidentifier] NOT NULL,
	[DataAccessGroupGUID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_StudyDataAccess] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StudyDataAccess]') AND name = N'IX_StudyDataAccess_DataAcessGroupGUID')
CREATE NONCLUSTERED INDEX [IX_StudyDataAccess_DataAcessGroupGUID] ON [dbo].[StudyDataAccess] 
(
	[DataAccessGroupGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StudyDataAccess]') AND name = N'IX_StudyDataAccess_StudyStorageGUID')
CREATE NONCLUSTERED INDEX [IX_StudyDataAccess_StudyStorageGUID] ON [dbo].[StudyDataAccess] 
(
	[StudyStorageGUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'StudyDataAccess', NULL,NULL))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Table for granting access to studies via Authority Groups' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'StudyDataAccess'
GO
/****** Object:  Default [DF_DataAccessGroup_GUID]    Script Date: 06/22/2011 14:11:47 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_DataAccessGroup_GUID]') AND parent_object_id = OBJECT_ID(N'[dbo].[DataAccessGroup]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_DataAccessGroup_GUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DataAccessGroup] ADD  CONSTRAINT [DF_DataAccessGroup_GUID]  DEFAULT (newid()) FOR [GUID]
END


End
GO
/****** Object:  Default [DF_DataAccessGroup_Deleted]    Script Date: 06/22/2011 14:11:47 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_DataAccessGroup_Deleted]') AND parent_object_id = OBJECT_ID(N'[dbo].[DataAccessGroup]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_DataAccessGroup_Deleted]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[DataAccessGroup] ADD  CONSTRAINT [DF_DataAccessGroup_Deleted]  DEFAULT ((0)) FOR [Deleted]
END


End
GO
/****** Object:  Default [DF_StudyDataAccess_GUID]    Script Date: 06/22/2011 14:11:47 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_StudyDataAccess_GUID]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyDataAccess]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_StudyDataAccess_GUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[StudyDataAccess] ADD  CONSTRAINT [DF_StudyDataAccess_GUID]  DEFAULT (newid()) FOR [GUID]
END


End
GO

/****** Object:  Table [dbo].[ServerPartitionDataAccess]    Script Date: 01/01/2012 23:25:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServerPartitionDataAccess]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ServerPartitionDataAccess](
	[GUID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_ServerPartitionDataAccess_GUID]  DEFAULT (newid()),
	[ServerPartitionGUID] [uniqueidentifier] NOT NULL,
	[DataAccessGroupGUID] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ServerPartitionDataAccess] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO

/****** Object:  Index [IX_ServerPartitionDataAccess_DataAccessGroupGUID]    Script Date: 01/01/2012 23:34:21 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServerPartitionDataAccess]') AND name = N'IX_ServerPartitionDataAccess_DataAccessGroupGUID')
CREATE NONCLUSTERED INDEX [IX_ServerPartitionDataAccess_DataAccessGroupGUID] ON [dbo].[ServerPartitionDataAccess] 
(
	[DataAccessGroupGUID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]
GO

/****** Object:  Index [IX_ServerPartitionDataAccess_ServerPartitionGUID]    Script Date: 01/01/2012 23:35:24 ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ServerPartitionDataAccess]') AND name = N'IX_ServerPartitionDataAccess_ServerPartitionGUID')
CREATE NONCLUSTERED INDEX [IX_ServerPartitionDataAccess_ServerPartitionGUID] ON [dbo].[ServerPartitionDataAccess] 
(
	[ServerPartitionGUID] ASC
)WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF) ON [PRIMARY]


/****** Object:  Table [dbo].[ExternalRequestQueue]    Script Date: 7/3/2014 3:01:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExternalRequestQueue]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ExternalRequestQueue](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[ExternalRequestQueueStatusEnum] [smallint] NULL,
	[RequestType] [varchar](48) NOT NULL,
	[OperationToken] [varchar](64) NULL,
	[RequestId] [varchar](64) NULL,
	[RequestXml] [xml] NOT NULL,
	[StateXml] [xml] NULL,
	[InsertTime] [datetime] NOT NULL,
	[DeletionTime] [datetime] NULL,
	[Revision] [int] NOT NULL CONSTRAINT DF_ExternalRequestQueue_Revision DEFAULT 1,
	[ScheduledTime] [datetime] NOT NULL CONSTRAINT [DF_ExternalRequestQueue_ScheduledTime]  DEFAULT (getdate()),
 CONSTRAINT [PK_ExternalRequestQueue] PRIMARY KEY NONCLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [INDEXES]
) ON [QUEUES] TEXTIMAGE_ON [QUEUES]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExternalRequestQueueStatusEnum]    Script Date: 5/14/2013 4:36:19 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExternalRequestQueueStatusEnum]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ExternalRequestQueueStatusEnum](
	[GUID] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Enum] [smallint] NOT NULL,
	[Lookup] [varchar](32) NOT NULL,
	[Description] [nvarchar](32) NOT NULL,
	[LongDescription] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_ExternalRequestQueueStatusEnum] PRIMARY KEY CLUSTERED 
(
	[Enum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [STATIC]
) ON [STATIC]
END
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [IXC_ExternalRequestQueue_ScheduledTime]    Script Date: 7/3/2014 3:01:08 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ExternalRequestQueue]') AND name = N'IXC_ExternalRequestQueue_ScheduledTime')
CREATE CLUSTERED INDEX [IXC_ExternalRequestQueue_ScheduledTime] ON [dbo].[ExternalRequestQueue]
(
	[ScheduledTime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [QUEUES]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ExternalRequestQueue_OperationToken]    Script Date: 5/14/2013 4:36:19 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ExternalRequestQueue]') AND name = N'IX_ExternalRequestQueue_OperationToken')
CREATE NONCLUSTERED INDEX [IX_ExternalRequestQueue_OperationToken] ON [dbo].[ExternalRequestQueue]
(
	[OperationToken] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [INDEXES]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ExternalRequestQueue_RequestId]    Script Date: 7/9/2013 4:36:19 PM ******/
CREATE NONCLUSTERED INDEX IX_ExternalRequestQueue_RequestId ON dbo.ExternalRequestQueue
(
	[RequestId] ASC
) WITH( PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON INDEXES
GO
/****** Object:  Index [IX_ExternalRequestQueue_RequestType]    Script Date: 5/14/2013 4:36:19 PM ******/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ExternalRequestQueue]') AND name = N'IX_ExternalRequestQueue_RequestType')
CREATE NONCLUSTERED INDEX [IX_ExternalRequestQueue_RequestType] ON [dbo].[ExternalRequestQueue]
(
	[RequestType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [INDEXES]
GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ExternalRequestQueue_GUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ExternalRequestQueue] ADD  CONSTRAINT [DF_ExternalRequestQueue_GUID]  DEFAULT (newid()) FOR [GUID]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ExternalRequestQueue_InsertTime]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ExternalRequestQueue] ADD  CONSTRAINT [DF_ExternalRequestQueue_InsertTime]  DEFAULT (getdate()) FOR [InsertTime]
END

GO
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[DF_ExternalRequestQueueStatusEnum_GUID]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[ExternalRequestQueueStatusEnum] ADD  CONSTRAINT [DF_ExternalRequestQueueStatusEnum_GUID]  DEFAULT (newid()) FOR [GUID]
END
GO


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



/****** Object:  ForeignKey [FK_ArchiveQueue_ArchiveQueueStatusEnum]    Script Date: 07/17/2008 00:49:15 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArchiveQueue_ArchiveQueueStatusEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArchiveQueue]'))
ALTER TABLE [dbo].[ArchiveQueue]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveQueue_ArchiveQueueStatusEnum] FOREIGN KEY([ArchiveQueueStatusEnum])
REFERENCES [dbo].[ArchiveQueueStatusEnum] ([Enum])
GO
ALTER TABLE [dbo].[ArchiveQueue] CHECK CONSTRAINT [FK_ArchiveQueue_ArchiveQueueStatusEnum]
GO
/****** Object:  ForeignKey [FK_ArchiveQueue_PartitionArchive]    Script Date: 07/17/2008 00:49:15 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArchiveQueue_PartitionArchive]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArchiveQueue]'))
ALTER TABLE [dbo].[ArchiveQueue]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveQueue_PartitionArchive] FOREIGN KEY([PartitionArchiveGUID])
REFERENCES [dbo].[PartitionArchive] ([GUID])
GO
ALTER TABLE [dbo].[ArchiveQueue] CHECK CONSTRAINT [FK_ArchiveQueue_PartitionArchive]
GO
/****** Object:  ForeignKey [FK_ArchiveQueue_StudyStorage]    Script Date: 07/17/2008 00:49:15 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArchiveQueue_StudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArchiveQueue]'))
ALTER TABLE [dbo].[ArchiveQueue]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveQueue_StudyStorage] FOREIGN KEY([StudyStorageGUID])
REFERENCES [dbo].[StudyStorage] ([GUID])
GO
ALTER TABLE [dbo].[ArchiveQueue] CHECK CONSTRAINT [FK_ArchiveQueue_StudyStorage]
GO
/****** Object:  ForeignKey [FK_ArchiveStudyStorage_PartitionArchive]    Script Date: 07/17/2008 00:49:18 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArchiveStudyStorage_PartitionArchive]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArchiveStudyStorage]'))
ALTER TABLE [dbo].[ArchiveStudyStorage]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveStudyStorage_PartitionArchive] FOREIGN KEY([PartitionArchiveGUID])
REFERENCES [dbo].[PartitionArchive] ([GUID])
GO
ALTER TABLE [dbo].[ArchiveStudyStorage] CHECK CONSTRAINT [FK_ArchiveStudyStorage_PartitionArchive]
GO
/****** Object:  ForeignKey [FK_ArchiveStudyStorage_ServerTransferSyntax]    Script Date: 07/17/2008 00:49:18 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArchiveStudyStorage_ServerTransferSyntax]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArchiveStudyStorage]'))
ALTER TABLE [dbo].[ArchiveStudyStorage]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveStudyStorage_ServerTransferSyntax] FOREIGN KEY([ServerTransferSyntaxGUID])
REFERENCES [dbo].[ServerTransferSyntax] ([GUID])
GO
ALTER TABLE [dbo].[ArchiveStudyStorage] CHECK CONSTRAINT [FK_ArchiveStudyStorage_ServerTransferSyntax]
GO
/****** Object:  ForeignKey [FK_ArchiveStudyStorage_StudyStorage]    Script Date: 07/17/2008 00:49:18 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ArchiveStudyStorage_StudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[ArchiveStudyStorage]'))
ALTER TABLE [dbo].[ArchiveStudyStorage]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveStudyStorage_StudyStorage] FOREIGN KEY([StudyStorageGUID])
REFERENCES [dbo].[StudyStorage] ([GUID])
GO
ALTER TABLE [dbo].[ArchiveStudyStorage] CHECK CONSTRAINT [FK_ArchiveStudyStorage_StudyStorage]
GO
/****** Object:  ForeignKey [FK_Device_ServerPartition]    Script Date: 07/17/2008 00:49:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Device_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Device]'))
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
ALTER TABLE [dbo].[Device] CHECK CONSTRAINT [FK_Device_ServerPartition]
GO
/****** Object:  ForeignKey [FK_Device_DeviceTypeEnum]    Script Date: 09/08/2009 11:45:26 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Device_DeviceTypeEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[Device]'))
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_DeviceTypeEnum] FOREIGN KEY([DeviceTypeEnum])
REFERENCES [dbo].[DeviceTypeEnum] ([Enum])
GO
ALTER TABLE [dbo].[Device] CHECK CONSTRAINT [FK_Device_DeviceTypeEnum]
GO
/****** Object:  ForeignKey [FK_DevicePreferredTransferSyntax_Device]    Script Date: 07/17/2008 00:49:25 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DevicePreferredTransferSyntax_Device]') AND parent_object_id = OBJECT_ID(N'[dbo].[DevicePreferredTransferSyntax]'))
ALTER TABLE [dbo].[DevicePreferredTransferSyntax]  WITH CHECK ADD  CONSTRAINT [FK_DevicePreferredTransferSyntax_Device] FOREIGN KEY([DeviceGUID])
REFERENCES [dbo].[Device] ([GUID])
GO
ALTER TABLE [dbo].[DevicePreferredTransferSyntax] CHECK CONSTRAINT [FK_DevicePreferredTransferSyntax_Device]
GO
/****** Object:  ForeignKey [FK_DevicePreferredTransferSyntax_ServerSopClass]    Script Date: 07/17/2008 00:49:25 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DevicePreferredTransferSyntax_ServerSopClass]') AND parent_object_id = OBJECT_ID(N'[dbo].[DevicePreferredTransferSyntax]'))
ALTER TABLE [dbo].[DevicePreferredTransferSyntax]  WITH CHECK ADD  CONSTRAINT [FK_DevicePreferredTransferSyntax_ServerSopClass] FOREIGN KEY([ServerSopClassGUID])
REFERENCES [dbo].[ServerSopClass] ([GUID])
GO
ALTER TABLE [dbo].[DevicePreferredTransferSyntax] CHECK CONSTRAINT [FK_DevicePreferredTransferSyntax_ServerSopClass]
GO
/****** Object:  ForeignKey [FK_DevicePreferredTransferSyntax_ServerTransferSyntax]    Script Date: 07/17/2008 00:49:25 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_DevicePreferredTransferSyntax_ServerTransferSyntax]') AND parent_object_id = OBJECT_ID(N'[dbo].[DevicePreferredTransferSyntax]'))
ALTER TABLE [dbo].[DevicePreferredTransferSyntax]  WITH CHECK ADD  CONSTRAINT [FK_DevicePreferredTransferSyntax_ServerTransferSyntax] FOREIGN KEY([ServerTransferSyntaxGUID])
REFERENCES [dbo].[ServerTransferSyntax] ([GUID])
GO
ALTER TABLE [dbo].[DevicePreferredTransferSyntax] CHECK CONSTRAINT [FK_DevicePreferredTransferSyntax_ServerTransferSyntax]
GO
/****** Object:  ForeignKey [FK_Filesystem_FilesystemTierEnum]    Script Date: 07/17/2008 00:49:29 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Filesystem_FilesystemTierEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[Filesystem]'))
ALTER TABLE [dbo].[Filesystem]  WITH CHECK ADD  CONSTRAINT [FK_Filesystem_FilesystemTierEnum] FOREIGN KEY([FilesystemTierEnum])
REFERENCES [dbo].[FilesystemTierEnum] ([Enum])
GO
ALTER TABLE [dbo].[Filesystem] CHECK CONSTRAINT [FK_Filesystem_FilesystemTierEnum]
GO
/****** Object:  ForeignKey [FK_FilesystemQueue_Filesystem]    Script Date: 07/17/2008 00:49:31 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FilesystemQueue_Filesystem]') AND parent_object_id = OBJECT_ID(N'[dbo].[FilesystemQueue]'))
ALTER TABLE [dbo].[FilesystemQueue]  WITH CHECK ADD  CONSTRAINT [FK_FilesystemQueue_Filesystem] FOREIGN KEY([FilesystemGUID])
REFERENCES [dbo].[Filesystem] ([GUID])
GO
ALTER TABLE [dbo].[FilesystemQueue] CHECK CONSTRAINT [FK_FilesystemQueue_Filesystem]
GO
/****** Object:  ForeignKey [FK_FilesystemQueue_FilesystemQueueTypeEnum]    Script Date: 07/17/2008 00:49:31 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FilesystemQueue_FilesystemQueueTypeEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[FilesystemQueue]'))
ALTER TABLE [dbo].[FilesystemQueue]  WITH CHECK ADD  CONSTRAINT [FK_FilesystemQueue_FilesystemQueueTypeEnum] FOREIGN KEY([FilesystemQueueTypeEnum])
REFERENCES [dbo].[FilesystemQueueTypeEnum] ([Enum])
GO
ALTER TABLE [dbo].[FilesystemQueue] CHECK CONSTRAINT [FK_FilesystemQueue_FilesystemQueueTypeEnum]
GO
/****** Object:  ForeignKey [FK_FilesystemQueue_StudyStorage]    Script Date: 07/17/2008 00:49:31 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FilesystemQueue_StudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[FilesystemQueue]'))
ALTER TABLE [dbo].[FilesystemQueue]  WITH CHECK ADD  CONSTRAINT [FK_FilesystemQueue_StudyStorage] FOREIGN KEY([StudyStorageGUID])
REFERENCES [dbo].[StudyStorage] ([GUID])
GO
ALTER TABLE [dbo].[FilesystemQueue] CHECK CONSTRAINT [FK_FilesystemQueue_StudyStorage]
GO
/****** Object:  ForeignKey [FK_FilesystemStudyStorage_Filesystem]    Script Date: 07/17/2008 00:49:34 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FilesystemStudyStorage_Filesystem]') AND parent_object_id = OBJECT_ID(N'[dbo].[FilesystemStudyStorage]'))
ALTER TABLE [dbo].[FilesystemStudyStorage]  WITH CHECK ADD  CONSTRAINT [FK_FilesystemStudyStorage_Filesystem] FOREIGN KEY([FilesystemGUID])
REFERENCES [dbo].[Filesystem] ([GUID])
GO
ALTER TABLE [dbo].[FilesystemStudyStorage] CHECK CONSTRAINT [FK_FilesystemStudyStorage_Filesystem]
GO
/****** Object:  ForeignKey [FK_FilesystemStudyStorage_ServerTransferSyntax]    Script Date: 07/17/2008 00:49:34 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FilesystemStudyStorage_ServerTransferSyntax]') AND parent_object_id = OBJECT_ID(N'[dbo].[FilesystemStudyStorage]'))
ALTER TABLE [dbo].[FilesystemStudyStorage]  WITH CHECK ADD  CONSTRAINT [FK_FilesystemStudyStorage_ServerTransferSyntax] FOREIGN KEY([ServerTransferSyntaxGUID])
REFERENCES [dbo].[ServerTransferSyntax] ([GUID])
GO
ALTER TABLE [dbo].[FilesystemStudyStorage] CHECK CONSTRAINT [FK_FilesystemStudyStorage_ServerTransferSyntax]
GO
/****** Object:  ForeignKey [FK_FilesystemStudyStorage_StudyStorage]    Script Date: 07/17/2008 00:49:34 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FilesystemStudyStorage_StudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[FilesystemStudyStorage]'))
ALTER TABLE [dbo].[FilesystemStudyStorage]  WITH CHECK ADD  CONSTRAINT [FK_FilesystemStudyStorage_StudyStorage] FOREIGN KEY([StudyStorageGUID])
REFERENCES [dbo].[StudyStorage] ([GUID])
GO
ALTER TABLE [dbo].[FilesystemStudyStorage] CHECK CONSTRAINT [FK_FilesystemStudyStorage_StudyStorage]
GO
/****** Object:  ForeignKey [FK_PartitionArchive_ArchiveTypeEnum]    Script Date: 07/17/2008 00:49:38 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PartitionArchive_ArchiveTypeEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[PartitionArchive]'))
ALTER TABLE [dbo].[PartitionArchive]  WITH CHECK ADD  CONSTRAINT [FK_PartitionArchive_ArchiveTypeEnum] FOREIGN KEY([ArchiveTypeEnum])
REFERENCES [dbo].[ArchiveTypeEnum] ([Enum])
GO
ALTER TABLE [dbo].[PartitionArchive] CHECK CONSTRAINT [FK_PartitionArchive_ArchiveTypeEnum]
GO
/****** Object:  ForeignKey [FK_PartitionArchive_ServerPartition]    Script Date: 07/17/2008 00:49:38 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PartitionArchive_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[PartitionArchive]'))
ALTER TABLE [dbo].[PartitionArchive]  WITH CHECK ADD  CONSTRAINT [FK_PartitionArchive_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
ALTER TABLE [dbo].[PartitionArchive] CHECK CONSTRAINT [FK_PartitionArchive_ServerPartition]
GO
/****** Object:  ForeignKey [FK_PartitionSopClass_ServerPartition]    Script Date: 07/17/2008 00:49:39 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PartitionSopClass_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[PartitionSopClass]'))
ALTER TABLE [dbo].[PartitionSopClass]  WITH CHECK ADD  CONSTRAINT [FK_PartitionSopClass_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
ALTER TABLE [dbo].[PartitionSopClass] CHECK CONSTRAINT [FK_PartitionSopClass_ServerPartition]
GO
/****** Object:  ForeignKey [FK_PartitionSopClass_ServerSopClass]    Script Date: 07/17/2008 00:49:39 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PartitionSopClass_ServerSopClass]') AND parent_object_id = OBJECT_ID(N'[dbo].[PartitionSopClass]'))
ALTER TABLE [dbo].[PartitionSopClass]  WITH CHECK ADD  CONSTRAINT [FK_PartitionSopClass_ServerSopClass] FOREIGN KEY([ServerSopClassGUID])
REFERENCES [dbo].[ServerSopClass] ([GUID])
GO
ALTER TABLE [dbo].[PartitionSopClass] CHECK CONSTRAINT [FK_PartitionSopClass_ServerSopClass]
GO
/****** Object:  ForeignKey [FK_PartitionTransferSyntax_ServerPartition]    Script Date: 07/17/2008 00:49:40 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PartitionTransferSyntax_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[PartitionTransferSyntax]'))
ALTER TABLE [dbo].[PartitionTransferSyntax]  WITH CHECK ADD  CONSTRAINT [FK_PartitionTransferSyntax_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
ALTER TABLE [dbo].[PartitionTransferSyntax] CHECK CONSTRAINT [FK_PartitionTransferSyntax_ServerPartition]
GO
/****** Object:  ForeignKey [FK_PartitionTransferSyntax_ServerTransferSyntax]    Script Date: 07/17/2008 00:49:41 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PartitionTransferSyntax_ServerTransferSyntax]') AND parent_object_id = OBJECT_ID(N'[dbo].[PartitionTransferSyntax]'))
ALTER TABLE [dbo].[PartitionTransferSyntax]  WITH CHECK ADD  CONSTRAINT [FK_PartitionTransferSyntax_ServerTransferSyntax] FOREIGN KEY([ServerTransferSyntaxGUID])
REFERENCES [dbo].[ServerTransferSyntax] ([GUID])
GO
ALTER TABLE [dbo].[PartitionTransferSyntax] CHECK CONSTRAINT [FK_PartitionTransferSyntax_ServerTransferSyntax]
GO
/****** Object:  ForeignKey [FK_Patient_ServerPartition]    Script Date: 07/17/2008 00:49:43 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Patient_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Patient]'))
ALTER TABLE [dbo].[Patient]  WITH CHECK ADD  CONSTRAINT [FK_Patient_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
ALTER TABLE [dbo].[Patient] CHECK CONSTRAINT [FK_Patient_ServerPartition]
GO
/****** Object:  ForeignKey [FK_RequestAttribute_Series]    Script Date: 07/17/2008 00:49:44 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RequestAttribute_Series]') AND parent_object_id = OBJECT_ID(N'[dbo].[RequestAttributes]'))
ALTER TABLE [dbo].[RequestAttributes]  WITH CHECK ADD  CONSTRAINT [FK_RequestAttribute_Series] FOREIGN KEY([SeriesGUID])
REFERENCES [dbo].[Series] ([GUID])
GO
ALTER TABLE [dbo].[RequestAttributes] CHECK CONSTRAINT [FK_RequestAttribute_Series]
GO
/****** Object:  ForeignKey [FK_RestoreQueue_ArchiveStudyStorage]    Script Date: 07/17/2008 00:49:46 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RestoreQueue_ArchiveStudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[RestoreQueue]'))
ALTER TABLE [dbo].[RestoreQueue]  WITH CHECK ADD  CONSTRAINT [FK_RestoreQueue_ArchiveStudyStorage] FOREIGN KEY([ArchiveStudyStorageGUID])
REFERENCES [dbo].[ArchiveStudyStorage] ([GUID])
GO
ALTER TABLE [dbo].[RestoreQueue] CHECK CONSTRAINT [FK_RestoreQueue_ArchiveStudyStorage]
GO
/****** Object:  ForeignKey [FK_RestoreQueue_RestoreQueueStatusEnum]    Script Date: 07/17/2008 00:49:46 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RestoreQueue_RestoreQueueStatusEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[RestoreQueue]'))
ALTER TABLE [dbo].[RestoreQueue]  WITH CHECK ADD  CONSTRAINT [FK_RestoreQueue_RestoreQueueStatusEnum] FOREIGN KEY([RestoreQueueStatusEnum])
REFERENCES [dbo].[RestoreQueueStatusEnum] ([Enum])
GO
ALTER TABLE [dbo].[RestoreQueue] CHECK CONSTRAINT [FK_RestoreQueue_RestoreQueueStatusEnum]
GO
/****** Object:  ForeignKey [FK_RestoreQueue_StudyStorage]    Script Date: 07/17/2008 00:49:46 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_RestoreQueue_StudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[RestoreQueue]'))
ALTER TABLE [dbo].[RestoreQueue]  WITH CHECK ADD  CONSTRAINT [FK_RestoreQueue_StudyStorage] FOREIGN KEY([StudyStorageGUID])
REFERENCES [dbo].[StudyStorage] ([GUID])
GO
ALTER TABLE [dbo].[RestoreQueue] CHECK CONSTRAINT [FK_RestoreQueue_StudyStorage]
GO
/****** Object:  ForeignKey [FK_Series_ServerPartition]    Script Date: 07/17/2008 00:49:51 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Series_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Series]'))
ALTER TABLE [dbo].[Series]  WITH CHECK ADD  CONSTRAINT [FK_Series_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
ALTER TABLE [dbo].[Series] CHECK CONSTRAINT [FK_Series_ServerPartition]
GO
/****** Object:  ForeignKey [FK_Series_Study]    Script Date: 07/17/2008 00:49:51 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Series_Study]') AND parent_object_id = OBJECT_ID(N'[dbo].[Series]'))
ALTER TABLE [dbo].[Series]  WITH CHECK ADD  CONSTRAINT [FK_Series_Study] FOREIGN KEY([StudyGUID])
REFERENCES [dbo].[Study] ([GUID])
GO
ALTER TABLE [dbo].[Series] CHECK CONSTRAINT [FK_Series_Study]
GO
/****** Object:  ForeignKey [FK_ServerPartition_DuplicateSopPolicyEnum]    Script Date: 07/17/2008 00:49:54 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServerPartition_DuplicateSopPolicyEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServerPartition]'))
ALTER TABLE [dbo].[ServerPartition]  WITH CHECK ADD  CONSTRAINT [FK_ServerPartition_DuplicateSopPolicyEnum] FOREIGN KEY([DuplicateSopPolicyEnum])
REFERENCES [dbo].[DuplicateSopPolicyEnum] ([Enum])
GO
ALTER TABLE [dbo].[ServerPartition] CHECK CONSTRAINT [FK_ServerPartition_DuplicateSopPolicyEnum]
GO
/****** Object:  ForeignKey [FK_ServerRule_ServerPartition]    Script Date: 07/17/2008 00:49:57 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServerRule_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServerRule]'))
ALTER TABLE [dbo].[ServerRule]  WITH CHECK ADD  CONSTRAINT [FK_ServerRule_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
ALTER TABLE [dbo].[ServerRule] CHECK CONSTRAINT [FK_ServerRule_ServerPartition]
GO
/****** Object:  ForeignKey [FK_ServerRule_ServerRuleApplyTimeEnum]    Script Date: 07/17/2008 00:49:57 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServerRule_ServerRuleApplyTimeEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServerRule]'))
ALTER TABLE [dbo].[ServerRule]  WITH CHECK ADD  CONSTRAINT [FK_ServerRule_ServerRuleApplyTimeEnum] FOREIGN KEY([ServerRuleApplyTimeEnum])
REFERENCES [dbo].[ServerRuleApplyTimeEnum] ([Enum])
GO
ALTER TABLE [dbo].[ServerRule] CHECK CONSTRAINT [FK_ServerRule_ServerRuleApplyTimeEnum]
GO
/****** Object:  ForeignKey [FK_ServerRule_ServerRuleTypeEnum]    Script Date: 07/17/2008 00:49:57 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServerRule_ServerRuleTypeEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServerRule]'))
ALTER TABLE [dbo].[ServerRule]  WITH CHECK ADD  CONSTRAINT [FK_ServerRule_ServerRuleTypeEnum] FOREIGN KEY([ServerRuleTypeEnum])
REFERENCES [dbo].[ServerRuleTypeEnum] ([Enum])
GO
ALTER TABLE [dbo].[ServerRule] CHECK CONSTRAINT [FK_ServerRule_ServerRuleTypeEnum]
GO
/****** Object:  ForeignKey [FK_ServiceLock_Filesystem]    Script Date: 07/17/2008 00:50:05 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceLock_Filesystem]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceLock]'))
ALTER TABLE [dbo].[ServiceLock]  WITH CHECK ADD  CONSTRAINT [FK_ServiceLock_Filesystem] FOREIGN KEY([FilesystemGUID])
REFERENCES [dbo].[Filesystem] ([GUID])
GO
ALTER TABLE [dbo].[ServiceLock] CHECK CONSTRAINT [FK_ServiceLock_Filesystem]
GO
/****** Object:  ForeignKey [FK_ServiceLock_ServiceLockTypeEnum]    Script Date: 07/17/2008 00:50:05 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServiceLock_ServiceLockTypeEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServiceLock]'))
ALTER TABLE [dbo].[ServiceLock]  WITH CHECK ADD  CONSTRAINT [FK_ServiceLock_ServiceLockTypeEnum] FOREIGN KEY([ServiceLockTypeEnum])
REFERENCES [dbo].[ServiceLockTypeEnum] ([Enum])
GO
ALTER TABLE [dbo].[ServiceLock]  CHECK CONSTRAINT [FK_ServiceLock_ServiceLockTypeEnum]
GO
/****** Object:  ForeignKey [FK_ServiceLock_ServerPartitionGUID]    Script Date: 08/31/2013 00:00:00 ******/
ALTER TABLE [dbo].[ServiceLock] WITH CHECK ADD CONSTRAINT [FK_ServiceLock_ServerPartitionGUID] FOREIGN KEY([ServerPartitionGUID]) 
REFERENCES [dbo].[ServerPartition] 	([GUID]) 
GO
ALTER TABLE [dbo].[ServiceLock]  CHECK CONSTRAINT [FK_ServiceLock_ServerPartitionGUID]
GO
/****** Object:  ForeignKey [FK_Study_Patient]    Script Date: 07/17/2008 00:50:12 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Study_Patient]') AND parent_object_id = OBJECT_ID(N'[dbo].[Study]'))
ALTER TABLE [dbo].[Study]  WITH CHECK ADD  CONSTRAINT [FK_Study_Patient] FOREIGN KEY([PatientGUID])
REFERENCES [dbo].[Patient] ([GUID])
GO
ALTER TABLE [dbo].[Study] CHECK CONSTRAINT [FK_Study_Patient]
GO
/****** Object:  ForeignKey [FK_Study_ServerPartition]    Script Date: 07/17/2008 00:50:12 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Study_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[Study]'))
ALTER TABLE [dbo].[Study]  WITH CHECK ADD  CONSTRAINT [FK_Study_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
ALTER TABLE [dbo].[Study] CHECK CONSTRAINT [FK_Study_ServerPartition]
GO
/****** Object:  ForeignKey [FK_Study_StudyStorage]    Script Date: 05/08/2009 14:29:21 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Study_StudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[Study]'))
ALTER TABLE [dbo].[Study]  WITH CHECK ADD  CONSTRAINT [FK_Study_StudyStorage] FOREIGN KEY([StudyStorageGUID])
REFERENCES [dbo].[StudyStorage] ([GUID])
GO
ALTER TABLE [dbo].[Study] CHECK CONSTRAINT [FK_Study_StudyStorage]
GO
/****** Object:  ForeignKey [FK_StudyStorage_QueueStudyStateEnum]    Script Date: 10/22/2008 16:46:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyStorage_QueueStudyStateEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyStorage]'))
ALTER TABLE [dbo].[StudyStorage]  WITH CHECK ADD  CONSTRAINT [FK_StudyStorage_QueueStudyStateEnum] FOREIGN KEY([QueueStudyStateEnum])
REFERENCES [dbo].[QueueStudyStateEnum] ([Enum])
GO
ALTER TABLE [dbo].[StudyStorage] CHECK CONSTRAINT [FK_StudyStorage_QueueStudyStateEnum]
GO
/****** Object:  ForeignKey [FK_StudyStorage_ServerPartition]    Script Date: 10/22/2008 16:46:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyStorage_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyStorage]'))
ALTER TABLE [dbo].[StudyStorage]  WITH CHECK ADD  CONSTRAINT [FK_StudyStorage_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
ALTER TABLE [dbo].[StudyStorage] CHECK CONSTRAINT [FK_StudyStorage_ServerPartition]
GO
/****** Object:  ForeignKey [FK_StudyStorage_StudyStatusEnum]    Script Date: 10/22/2008 16:46:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyStorage_StudyStatusEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyStorage]'))
ALTER TABLE [dbo].[StudyStorage]  WITH CHECK ADD  CONSTRAINT [FK_StudyStorage_StudyStatusEnum] FOREIGN KEY([StudyStatusEnum])
REFERENCES [dbo].[StudyStatusEnum] ([Enum])
GO
ALTER TABLE [dbo].[StudyStorage] CHECK CONSTRAINT [FK_StudyStorage_StudyStatusEnum]
GO
/****** Object:  ForeignKey [FK_WorkQueue_Device]    Script Date: 07/17/2008 00:50:20 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueue_Device]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueue]'))
ALTER TABLE [dbo].[WorkQueue]  WITH CHECK ADD  CONSTRAINT [FK_WorkQueue_Device] FOREIGN KEY([DeviceGUID])
REFERENCES [dbo].[Device] ([GUID])
GO
ALTER TABLE [dbo].[WorkQueue] CHECK CONSTRAINT [FK_WorkQueue_Device]
GO
/****** Object:  ForeignKey [FK_WorkQueue_ServerPartition]    Script Date: 07/17/2008 00:50:20 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueue_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueue]'))
ALTER TABLE [dbo].[WorkQueue]  WITH CHECK ADD  CONSTRAINT [FK_WorkQueue_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO
ALTER TABLE [dbo].[WorkQueue] CHECK CONSTRAINT [FK_WorkQueue_ServerPartition]
GO
/****** Object:  ForeignKey [FK_WorkQueue_StudyStorage]    Script Date: 07/17/2008 00:50:21 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueue_StudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueue]'))
ALTER TABLE [dbo].[WorkQueue]  WITH CHECK ADD  CONSTRAINT [FK_WorkQueue_StudyStorage] FOREIGN KEY([StudyStorageGUID])
REFERENCES [dbo].[StudyStorage] ([GUID])
GO
ALTER TABLE [dbo].[WorkQueue] CHECK CONSTRAINT [FK_WorkQueue_StudyStorage]
GO
/****** Object:  ForeignKey [FK_WorkQueue_WorkQueuePriorityEnum]    Script Date: 07/17/2008 00:50:21 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueue_WorkQueuePriorityEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueue]'))
ALTER TABLE [dbo].[WorkQueue]  WITH CHECK ADD  CONSTRAINT [FK_WorkQueue_WorkQueuePriorityEnum] FOREIGN KEY([WorkQueuePriorityEnum])
REFERENCES [dbo].[WorkQueuePriorityEnum] ([Enum])
GO
ALTER TABLE [dbo].[WorkQueue] CHECK CONSTRAINT [FK_WorkQueue_WorkQueuePriorityEnum]
GO
/****** Object:  ForeignKey [FK_WorkQueue_WorkQueueStatusEnum]    Script Date: 07/17/2008 00:50:21 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueue_WorkQueueStatusEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueue]'))
ALTER TABLE [dbo].[WorkQueue]  WITH CHECK ADD  CONSTRAINT [FK_WorkQueue_WorkQueueStatusEnum] FOREIGN KEY([WorkQueueStatusEnum])
REFERENCES [dbo].[WorkQueueStatusEnum] ([Enum])
GO
ALTER TABLE [dbo].[WorkQueue] CHECK CONSTRAINT [FK_WorkQueue_WorkQueueStatusEnum]
GO

/****** Object:  ForeignKey [FK_WorkQueue_StudyHistory]    Script Date: 09/08/2008 11:50:21 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueue_StudyHistory]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueue]'))
ALTER TABLE [dbo].[WorkQueue]  WITH CHECK ADD  CONSTRAINT [FK_WorkQueue_StudyHistory] FOREIGN KEY([StudyHistoryGUID])
REFERENCES [dbo].[StudyHistory] ([GUID])
GO
/****** Object:  ForeignKey [FK_WorkQueue_ExternalRequestQueue]    Script Date: 05/21/2013 11:50:21 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueue_ExternalRequestQueue]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueue]'))
ALTER TABLE dbo.WorkQueue ADD CONSTRAINT [FK_WorkQueue_ExternalRequestQueue] FOREIGN KEY ([ExternalRequestQueueGUID]) 
REFERENCES [dbo].[ExternalRequestQueue]	([GUID]) 

/****** Object:  ForeignKey [FK_WorkQueue_WorkQueueTypeEnum]    Script Date: 07/17/2008 00:50:21 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueue_WorkQueueTypeEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueue]'))
ALTER TABLE [dbo].[WorkQueue]  WITH CHECK ADD  CONSTRAINT [FK_WorkQueue_WorkQueueTypeEnum] FOREIGN KEY([WorkQueueTypeEnum])
REFERENCES [dbo].[WorkQueueTypeEnum] ([Enum])
GO
ALTER TABLE [dbo].[WorkQueue] CHECK CONSTRAINT [FK_WorkQueue_WorkQueueTypeEnum]
GO
/****** Object:  ForeignKey [FK_WorkQueueUid_WorkQueue]    Script Date: 07/17/2008 00:50:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueueUid_WorkQueue]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueueUid]'))
ALTER TABLE [dbo].[WorkQueueUid]  WITH CHECK ADD  CONSTRAINT [FK_WorkQueueUid_WorkQueue] FOREIGN KEY([WorkQueueGUID])
REFERENCES [dbo].[WorkQueue] ([GUID])
GO
ALTER TABLE [dbo].[WorkQueueUid] CHECK CONSTRAINT [FK_WorkQueueUid_WorkQueue]
GO

/****** Object:  ForeignKey [FK_Alert_AlertCategoryEnum]    Script Date: 07/24/2008 00:50:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Alert_AlertCategoryEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[Alert]'))
ALTER TABLE [dbo].[Alert]  WITH CHECK ADD  CONSTRAINT [FK_Alert_AlertCategoryEnum] FOREIGN KEY([AlertCategoryEnum])
REFERENCES [dbo].[AlertCategoryEnum] ([Enum])
GO

/****** Object:  ForeignKey [FK_Alert_AlertLevelEnum]    Script Date: 07/24/2008 00:50:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Alert_AlertLevelEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[Alert]'))
ALTER TABLE [dbo].[Alert]  WITH CHECK ADD  CONSTRAINT [FK_Alert_AlertLevelEnum] FOREIGN KEY([AlertLevelEnum])
REFERENCES [dbo].[AlertLevelEnum] ([Enum])
GO

/****** Object:  ForeignKey [FK_StudyIntegrityQueue_ServerPartition]    Script Date: 09/05/2008 11:50:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyIntegrityQueue_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyIntegrityQueue]'))
ALTER TABLE [dbo].[StudyIntegrityQueue]  WITH CHECK ADD  CONSTRAINT [FK_StudyIntegrityQueue_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO

/****** Object:  ForeignKey [FK_StudyIntegrityQueue_StudyIntegrityReasonEnum]    Script Date: 09/05/2008 11:50:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyIntegrityQueue_StudyIntegrityReasonEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyIntegrityQueue]'))
ALTER TABLE [dbo].[StudyIntegrityQueue]  WITH CHECK ADD  CONSTRAINT [FK_StudyIntegrityQueue_StudyIntegrityReasonEnum] FOREIGN KEY([StudyIntegrityReasonEnum])
REFERENCES [dbo].[StudyIntegrityReasonEnum] ([Enum])
GO

/****** Object:  ForeignKey [FK_StudyIntegrityQueue_StudyStorage]    Script Date: 09/08/2008 11:50:28 ******/
ALTER TABLE [dbo].[StudyIntegrityQueue]  WITH CHECK ADD  CONSTRAINT [FK_StudyIntegrityQueue_StudyStorage] FOREIGN KEY([StudyStorageGUID])
REFERENCES [dbo].[StudyStorage] ([GUID])
GO

/****** Object:  ForeignKey [FK_StudyIntegrityQueueUid_StudyIntegrityQueue]    Script Date: 09/05/2008 11:50:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyIntegrityQueueUid_StudyIntegrityQueue]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyIntegrityQueueUid]'))
ALTER TABLE [dbo].[StudyIntegrityQueueUid]  WITH CHECK ADD  CONSTRAINT [FK_StudyIntegrityQueueUid_StudyIntegrityQueue] FOREIGN KEY([StudyIntegrityQueueGUID])
REFERENCES [dbo].[StudyIntegrityQueue] ([GUID])
GO

/****** Object:  ForeignKey [FK_StudyHistory_StudyStorage]    Script Date: 09/05/2008 11:50:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyHistory_StudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyHistory]'))
ALTER TABLE [dbo].[StudyHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudyHistory_StudyStorage] FOREIGN KEY([StudyStorageGUID])
REFERENCES [dbo].[StudyStorage] ([GUID])
GO

/****** Object:  ForeignKey [FK_StudyHistory_StudyStorage]    Script Date: 09/26/2008 16:50:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyHistory_DestStudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyHistory]'))
ALTER TABLE [dbo].[StudyHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudyHistory_DestStudyStorage] FOREIGN KEY([DestStudyStorageGUID])
REFERENCES [dbo].[StudyStorage] ([GUID])
GO

/****** Object:  ForeignKey [FK_StudyHistory_StudyStorage]    Script Date: 09/26/2008 16:50:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyHistory_StudyHistoryTypeEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyHistory]'))
ALTER TABLE [dbo].[StudyHistory]  WITH CHECK ADD  CONSTRAINT [FK_StudyHistory_StudyHistoryTypeEnum] FOREIGN KEY([StudyHistoryTypeEnum])
REFERENCES [dbo].[StudyHistoryTypeEnum] ([Enum])
GO

/****** Object:  ForeignKey [FK_StudyDeleteRecord_Filesystem]    Script Date: 11/30/2008 16:50:28 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyDeleteRecord_Filesystem]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyDeleteRecord]'))
ALTER TABLE [dbo].[StudyDeleteRecord]  WITH CHECK ADD  CONSTRAINT [FK_StudyDeleteRecord_Filesystem] FOREIGN KEY([FilesystemGUID])
REFERENCES [dbo].[Filesystem] ([GUID])
GO

/****** Object:  ForeignKey [FK_WorkQueueTypeProperties_WorkQueuePriorityEnum]    Script Date: 09/14/2009 21:30:31 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueueTypeProperties_WorkQueuePriorityEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]'))
ALTER TABLE [dbo].[WorkQueueTypeProperties]  WITH CHECK ADD  CONSTRAINT [FK_WorkQueueTypeProperties_WorkQueuePriorityEnum] FOREIGN KEY([WorkQueuePriorityEnum])
REFERENCES [dbo].[WorkQueuePriorityEnum] ([Enum])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueueTypeProperties_WorkQueuePriorityEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]'))
ALTER TABLE [dbo].[WorkQueueTypeProperties] CHECK CONSTRAINT [FK_WorkQueueTypeProperties_WorkQueuePriorityEnum]
GO
/****** Object:  ForeignKey [FK_WorkQueueTypeProperties_WorkQueueTypeEnum]    Script Date: 09/14/2009 21:30:31 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueueTypeProperties_WorkQueueTypeEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]'))
ALTER TABLE [dbo].[WorkQueueTypeProperties]  WITH CHECK ADD  CONSTRAINT [FK_WorkQueueTypeProperties_WorkQueueTypeEnum] FOREIGN KEY([WorkQueueTypeEnum])
REFERENCES [dbo].[WorkQueueTypeEnum] ([Enum])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueueTypeProperties_WorkQueueTypeEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]'))
ALTER TABLE [dbo].[WorkQueueTypeProperties] CHECK CONSTRAINT [FK_WorkQueueTypeProperties_WorkQueueTypeEnum]
GO

/****** Object:  ForeignKey [FK_WorkQueueTypeProperties_QueueStudyStateEnum]    Script Date: 10/26/2009 16:30:20 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_WorkQueueTypeProperties_WorkQueueTypeEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[WorkQueueTypeProperties]'))
ALTER TABLE [dbo].[WorkQueueTypeProperties]  WITH CHECK ADD  CONSTRAINT [FK_WorkQueueTypeProperties_QueueStudyStateEnum] FOREIGN KEY([QueueStudyStateEnum])
REFERENCES [dbo].[QueueStudyStateEnum] ([Enum])


/****** Object:  ForeignKey [FK_StudyDataAccess_DataAccessGroup]    Script Date: 06/22/2011 14:11:47 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyDataAccess_DataAccessGroup]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyDataAccess]'))
ALTER TABLE [dbo].[StudyDataAccess]  WITH CHECK ADD  CONSTRAINT [FK_StudyDataAccess_DataAccessGroup] FOREIGN KEY([DataAccessGroupGUID])
REFERENCES [dbo].[DataAccessGroup] ([GUID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyDataAccess_DataAccessGroup]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyDataAccess]'))
ALTER TABLE [dbo].[StudyDataAccess] CHECK CONSTRAINT [FK_StudyDataAccess_DataAccessGroup]
GO
/****** Object:  ForeignKey [FK_StudyDataAccess_StudyStorage]    Script Date: 06/22/2011 14:11:47 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyDataAccess_StudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyDataAccess]'))
ALTER TABLE [dbo].[StudyDataAccess]  WITH CHECK ADD  CONSTRAINT [FK_StudyDataAccess_StudyStorage] FOREIGN KEY([StudyStorageGUID])
REFERENCES [dbo].[StudyStorage] ([GUID])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StudyDataAccess_StudyStorage]') AND parent_object_id = OBJECT_ID(N'[dbo].[StudyDataAccess]'))
ALTER TABLE [dbo].[StudyDataAccess] CHECK CONSTRAINT [FK_StudyDataAccess_StudyStorage]
GO

/****** Object:  ForeignKey [FK_StudyDataAccess_StudyStorage]    Script Date: 06/22/2011 14:11:47 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServerPartitionDataAccess_DataAccessGroup]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServerPartitionDataAccess]'))
ALTER TABLE [dbo].[ServerPartitionDataAccess]  WITH CHECK ADD  CONSTRAINT [FK_ServerPartitionDataAccess_DataAccessGroup] FOREIGN KEY([DataAccessGroupGUID])
REFERENCES [dbo].[DataAccessGroup] ([GUID])
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ServerPartitionDataAccess_ServerPartition]') AND parent_object_id = OBJECT_ID(N'[dbo].[ServerPartitionDataAccess]'))
ALTER TABLE [dbo].[ServerPartitionDataAccess]  WITH CHECK ADD  CONSTRAINT [FK_ServerPartitionDataAccess_ServerPartition] FOREIGN KEY([ServerPartitionGUID])
REFERENCES [dbo].[ServerPartition] ([GUID])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ExternalRequestQueue_ExternalRequestQueueStatusEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExternalRequestQueue]'))
ALTER TABLE [dbo].[ExternalRequestQueue]  WITH CHECK ADD  CONSTRAINT [FK_ExternalRequestQueue_ExternalRequestQueueStatusEnum] FOREIGN KEY([ExternalRequestQueueStatusEnum])
REFERENCES [dbo].[ExternalRequestQueueStatusEnum] ([Enum])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ExternalRequestQueue_ExternalRequestQueueStatusEnum]') AND parent_object_id = OBJECT_ID(N'[dbo].[ExternalRequestQueue]'))
ALTER TABLE [dbo].[ExternalRequestQueue] CHECK CONSTRAINT [FK_ExternalRequestQueue_ExternalRequestQueueStatusEnum]
GO

ALTER TABLE dbo.Study ADD CONSTRAINT FK_Study_Order FOREIGN KEY	( OrderGUID	) 
REFERENCES dbo.[Order] (GUID)
GO

ALTER TABLE dbo.Study ADD CONSTRAINT FK_Study_QCStatusEnum FOREIGN KEY ( QCStatusEnum ) 
REFERENCES dbo.QCStatusEnum	( Enum )
GO
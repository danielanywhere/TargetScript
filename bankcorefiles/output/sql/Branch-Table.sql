USE [AnyDatabaseName]
GO
IF OBJECT_ID('dbo.[DF_bnkBranch_BranchID]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkBranch] DROP CONSTRAINT [DF_bnkBranch_BranchID]
GO
IF OBJECT_ID('dbo.[DF_bnkBranch_BranchTicket]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkBranch] DROP CONSTRAINT [DF_bnkBranch_BranchTicket]
GO
IF OBJECT_ID('dbo.[DF_bnkBranch_Name]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkBranch] DROP CONSTRAINT [DF_bnkBranch_Name]
GO
IF OBJECT_ID('dbo.[DF_bnkBranch_Address]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkBranch] DROP CONSTRAINT [DF_bnkBranch_Address]
GO
IF OBJECT_ID('dbo.[DF_bnkBranch_City]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkBranch] DROP CONSTRAINT [DF_bnkBranch_City]
GO
IF OBJECT_ID('dbo.[DF_bnkBranch_State]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkBranch] DROP CONSTRAINT [DF_bnkBranch_State]
GO
IF OBJECT_ID('dbo.[DF_bnkBranch_ZipCode]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkBranch] DROP CONSTRAINT [DF_bnkBranch_ZipCode]
GO
IF OBJECT_ID('dbo.[bnkBranch]', 'U') IS NOT NULL
	DROP TABLE [dbo].[bnkBranch]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[bnkBranch](
	[BranchID] int IDENTITY(1,1) NOT NULL,
	[BranchTicket] uniqueidentifier ROWGUIDCOL NOT NULL,
	[Name] varchar (32) NOT NULL,
	[Address] varchar (255) NOT NULL,
	[City] varchar (32) NOT NULL,
	[State] varchar (2) NOT NULL,
	[ZipCode] varchar (16) NOT NULL,
	PRIMARY KEY (BranchID)
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[bnkBranchID] ADD CONSTRAINT [DF_bnkBranch_BranchID] DEFAULT ((0)) FOR [BranchID]
GO
ALTER TABLE [dbo].[bnkBranchTicket] ADD CONSTRAINT [DF_bnkBranch_BranchTicket] DEFAULT (newid()) FOR [BranchTicket]
GO

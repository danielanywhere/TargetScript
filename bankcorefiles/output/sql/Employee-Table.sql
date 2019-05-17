USE [AnyDatabaseName]
GO
IF OBJECT_ID('dbo.[DF_bnkEmployee_EmployeeID]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkEmployee] DROP CONSTRAINT [DF_bnkEmployee_EmployeeID]
GO
IF OBJECT_ID('dbo.[DF_bnkEmployee_EmployeeTicket]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkEmployee] DROP CONSTRAINT [DF_bnkEmployee_EmployeeTicket]
GO
IF OBJECT_ID('dbo.[DF_bnkEmployee_FirstName]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkEmployee] DROP CONSTRAINT [DF_bnkEmployee_FirstName]
GO
IF OBJECT_ID('dbo.[DF_bnkEmployee_LastName]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkEmployee] DROP CONSTRAINT [DF_bnkEmployee_LastName]
GO
IF OBJECT_ID('dbo.[DF_bnkEmployee_DateStarted]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkEmployee] DROP CONSTRAINT [DF_bnkEmployee_DateStarted]
GO
IF OBJECT_ID('dbo.[DF_bnkEmployee_DateEnded]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkEmployee] DROP CONSTRAINT [DF_bnkEmployee_DateEnded]
GO
IF OBJECT_ID('dbo.[DF_bnkEmployee_Title]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkEmployee] DROP CONSTRAINT [DF_bnkEmployee_Title]
GO
IF OBJECT_ID('dbo.[DF_bnkEmployee_TIN]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkEmployee] DROP CONSTRAINT [DF_bnkEmployee_TIN]
GO
IF OBJECT_ID('dbo.[bnkEmployee]', 'U') IS NOT NULL
	DROP TABLE [dbo].[bnkEmployee]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[bnkEmployee](
	[EmployeeID] int IDENTITY(1,1) NOT NULL,
	[EmployeeTicket] uniqueidentifier ROWGUIDCOL NOT NULL,
	[FirstName] varchar (32) NOT NULL,
	[LastName] varchar (32) NOT NULL,
	[DateStarted] smalldatetime NOT NULL,
	[DateEnded] smalldatetime NOT NULL,
	[Title] varchar (255) NOT NULL,
	[TIN] varchar (16) NOT NULL,
	PRIMARY KEY (EmployeeID)
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[bnkEmployeeID] ADD CONSTRAINT [DF_bnkEmployee_EmployeeID] DEFAULT ((0)) FOR [EmployeeID]
GO
ALTER TABLE [dbo].[bnkEmployeeTicket] ADD CONSTRAINT [DF_bnkEmployee_EmployeeTicket] DEFAULT (newid()) FOR [EmployeeTicket]
GO

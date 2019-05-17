USE [AnyDatabaseName]
GO
IF OBJECT_ID('dbo.[DF_bnkAccount_AccountID]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkAccount] DROP CONSTRAINT [DF_bnkAccount_AccountID]
GO
IF OBJECT_ID('dbo.[DF_bnkAccount_AccountTicket]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkAccount] DROP CONSTRAINT [DF_bnkAccount_AccountTicket]
GO
IF OBJECT_ID('dbo.[DF_bnkAccount_AccountStatus]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkAccount] DROP CONSTRAINT [DF_bnkAccount_AccountStatus]
GO
IF OBJECT_ID('dbo.[DF_bnkAccount_BalanceAvailable]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkAccount] DROP CONSTRAINT [DF_bnkAccount_BalanceAvailable]
GO
IF OBJECT_ID('dbo.[DF_bnkAccount_BalancePending]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkAccount] DROP CONSTRAINT [DF_bnkAccount_BalancePending]
GO
IF OBJECT_ID('dbo.[DF_bnkAccount_BranchID]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkAccount] DROP CONSTRAINT [DF_bnkAccount_BranchID]
GO
IF OBJECT_ID('dbo.[DF_bnkAccount_CustomerID]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkAccount] DROP CONSTRAINT [DF_bnkAccount_CustomerID]
GO
IF OBJECT_ID('dbo.[DF_bnkAccount_DateClosed]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkAccount] DROP CONSTRAINT [DF_bnkAccount_DateClosed]
GO
IF OBJECT_ID('dbo.[DF_bnkAccount_DateLastActivity]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkAccount] DROP CONSTRAINT [DF_bnkAccount_DateLastActivity]
GO
IF OBJECT_ID('dbo.[DF_bnkAccount_DateOpened]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkAccount] DROP CONSTRAINT [DF_bnkAccount_DateOpened]
GO
IF OBJECT_ID('dbo.[DF_bnkAccount_EmployeeID]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkAccount] DROP CONSTRAINT [DF_bnkAccount_EmployeeID]
GO
IF OBJECT_ID('dbo.[bnkAccount]', 'U') IS NOT NULL
	DROP TABLE [dbo].[bnkAccount]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[bnkAccount](
	[AccountID] int IDENTITY(1,1) NOT NULL,
	[AccountTicket] uniqueidentifier ROWGUIDCOL NOT NULL,
	[AccountStatus] varchar (32) NOT NULL,
	[BalanceAvailable] float NOT NULL,
	[BalancePending] float NOT NULL,
	[BranchID] int NOT NULL,
	[CustomerID] int NOT NULL,
	[DateClosed] smalldatetime NOT NULL,
	[DateLastActivity] smalldatetime NOT NULL,
	[DateOpened] smalldatetime NOT NULL,
	[EmployeeID] int NOT NULL,
	PRIMARY KEY (AccountID)
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[bnkAccountID] ADD CONSTRAINT [DF_bnkAccount_AccountID] DEFAULT ((0)) FOR [AccountID]
GO
ALTER TABLE [dbo].[bnkAccountTicket] ADD CONSTRAINT [DF_bnkAccount_AccountTicket] DEFAULT (newid()) FOR [AccountTicket]
GO
ALTER TABLE [dbo].[bnkBalanceAvailable] ADD CONSTRAINT [DF_bnkAccount_BalanceAvailable] DEFAULT ((0)) FOR [BalanceAvailable]
GO
ALTER TABLE [dbo].[bnkBalancePending] ADD CONSTRAINT [DF_bnkAccount_BalancePending] DEFAULT ((0)) FOR [BalancePending]
GO
ALTER TABLE [dbo].[bnkBranchID] ADD CONSTRAINT [DF_bnkAccount_BranchID] DEFAULT ((0)) FOR [BranchID]
GO
ALTER TABLE [dbo].[bnkCustomerID] ADD CONSTRAINT [DF_bnkAccount_CustomerID] DEFAULT ((0)) FOR [CustomerID]
GO
ALTER TABLE [dbo].[bnkEmployeeID] ADD CONSTRAINT [DF_bnkAccount_EmployeeID] DEFAULT ((0)) FOR [EmployeeID]
GO

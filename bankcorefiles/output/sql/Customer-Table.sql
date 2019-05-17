USE [AnyDatabaseName]
GO
IF OBJECT_ID('dbo.[DF_bnkCustomer_CustomerID]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkCustomer] DROP CONSTRAINT [DF_bnkCustomer_CustomerID]
GO
IF OBJECT_ID('dbo.[DF_bnkCustomer_CustomerTicket]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkCustomer] DROP CONSTRAINT [DF_bnkCustomer_CustomerTicket]
GO
IF OBJECT_ID('dbo.[DF_bnkCustomer_Name]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkCustomer] DROP CONSTRAINT [DF_bnkCustomer_Name]
GO
IF OBJECT_ID('dbo.[DF_bnkCustomer_Address]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkCustomer] DROP CONSTRAINT [DF_bnkCustomer_Address]
GO
IF OBJECT_ID('dbo.[DF_bnkCustomer_City]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkCustomer] DROP CONSTRAINT [DF_bnkCustomer_City]
GO
IF OBJECT_ID('dbo.[DF_bnkCustomer_State]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkCustomer] DROP CONSTRAINT [DF_bnkCustomer_State]
GO
IF OBJECT_ID('dbo.[DF_bnkCustomer_ZipCode]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkCustomer] DROP CONSTRAINT [DF_bnkCustomer_ZipCode]
GO
IF OBJECT_ID('dbo.[DF_bnkCustomer_TIN]', 'D') IS NOT NULL
	ALTER TABLE [dbo].[bnkCustomer] DROP CONSTRAINT [DF_bnkCustomer_TIN]
GO
IF OBJECT_ID('dbo.[bnkCustomer]', 'U') IS NOT NULL
	DROP TABLE [dbo].[bnkCustomer]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[bnkCustomer](
	[CustomerID] int IDENTITY(1,1) NOT NULL,
	[CustomerTicket] uniqueidentifier ROWGUIDCOL NOT NULL,
	[Name] varchar (32) NOT NULL,
	[Address] varchar (255) NOT NULL,
	[City] varchar (32) NOT NULL,
	[State] varchar (2) NOT NULL,
	[ZipCode] varchar (16) NOT NULL,
	[TIN] varchar (12) NOT NULL,
	PRIMARY KEY (CustomerID)
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[bnkCustomerID] ADD CONSTRAINT [DF_bnkCustomer_CustomerID] DEFAULT ((0)) FOR [CustomerID]
GO
ALTER TABLE [dbo].[bnkCustomerTicket] ADD CONSTRAINT [DF_bnkCustomer_CustomerTicket] DEFAULT (newid()) FOR [CustomerTicket]
GO

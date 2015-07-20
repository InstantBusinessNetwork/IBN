USE master
CREATE DATABASE [{Database}]
ALTER DATABASE [{Database}] SET AUTO_SHRINK ON, RECOVERY SIMPLE
GO
USE [{Database}]
--EXEC sp_adduser '{Login}', '{Login}', 'db_owner'
CREATE USER [{Login}] FOR LOGIN [{Login}]
EXEC sp_addrolemember 'db_owner', '{Login}'

IF FULLTEXTSERVICEPROPERTY('IsFullTextInstalled') = 1
	EXEC sp_fulltext_database 'disable'

CREATE TABLE [dbo].[_InitializationParameters](
	[Name] [nvarchar](255) NOT NULL,
	[Value] [ntext] NULL,
)

INSERT INTO [_InitializationParameters] ([Name],[Value]) VALUES (N'CompanyName',N'{CompanyName}')
INSERT INTO [_InitializationParameters] ([Name],[Value]) VALUES (N'DefaultLocale',N'{DefaultLocale}')
INSERT INTO [_InitializationParameters] ([Name],[Value]) VALUES (N'AdminAccountName',N'{AdminAccountName}')
INSERT INTO [_InitializationParameters] ([Name],[Value]) VALUES (N'AdminPassword',N'{AdminPassword}')
INSERT INTO [_InitializationParameters] ([Name],[Value]) VALUES (N'AdminFirstName',N'{AdminFirstName}')
INSERT INTO [_InitializationParameters] ([Name],[Value]) VALUES (N'AdminLastName',N'{AdminLastName}')
INSERT INTO [_InitializationParameters] ([Name],[Value]) VALUES (N'AdminEmail',N'{AdminEmail}')

CREATE TABLE [dbo].[PortalConfig](
	[SettingId] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[Value] [ntext] NOT NULL CONSTRAINT [DF_PortalConfig_Value]  DEFAULT (N''),
 CONSTRAINT [PK_PortalConfig] PRIMARY KEY CLUSTERED ([SettingId] ASC)
)
CREATE UNIQUE NONCLUSTERED INDEX [IX_PortalConfig] ON [dbo].[PortalConfig]
(
	[Key] ASC
)

INSERT INTO [PortalConfig] ([Key],[Value]) VALUES (N'system.isactive',N'{IsActive}')
INSERT INTO [PortalConfig] ([Key],[Value]) VALUES (N'system.scheme',N'{Scheme}')
INSERT INTO [PortalConfig] ([Key],[Value]) VALUES (N'system.host',N'{Host}')
INSERT INTO [PortalConfig] ([Key],[Value]) VALUES (N'system.port',N'{Port}')

CREATE TABLE [dbo].[DatabaseVersion](
	[Major] [int] NOT NULL CONSTRAINT [DF_DatabaseVersion_Major] DEFAULT ((0)),
	[Minor] [int] NOT NULL CONSTRAINT [DF_DatabaseVersion_Minor] DEFAULT ((0)),
	[Build] [int] NOT NULL CONSTRAINT [DF_DatabaseVersion_Build] DEFAULT ((0)),
	[State] [int] NOT NULL CONSTRAINT [DF_DatabaseVersion_State] DEFAULT ((0))
)

INSERT INTO [DatabaseVersion] ([State]) VALUES (2) -- Initialize

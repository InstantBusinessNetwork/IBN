USE master
CREATE DATABASE [{Database}]
ALTER DATABASE [{Database}] SET AUTO_SHRINK ON, RECOVERY SIMPLE
GO
USE [{Database}]
CREATE USER [{Login}] FOR LOGIN [{Login}]
EXEC sp_addrolemember 'db_owner', '{Login}'

CREATE TABLE [dbo].[DatabaseVersion](
	[Major] [int] NOT NULL CONSTRAINT [DF_DatabaseVersion_Major] DEFAULT ((0)),
	[Minor] [int] NOT NULL CONSTRAINT [DF_DatabaseVersion_Minor] DEFAULT ((0)),
	[Build] [int] NOT NULL CONSTRAINT [DF_DatabaseVersion_Build] DEFAULT ((0)),
	[State] [int] NOT NULL CONSTRAINT [DF_DatabaseVersion_State] DEFAULT ((0))
)

INSERT INTO [DatabaseVersion] ([State]) VALUES (2) -- Initialize
GO

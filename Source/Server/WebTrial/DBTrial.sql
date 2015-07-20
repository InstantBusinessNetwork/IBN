IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'IBN41_Trial')
	DROP DATABASE [IBN41_Trial]
GO
CREATE DATABASE [IBN41_Trial]
GO
exec sp_dboption N'IBN41_Trial', N'autoclose', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'bulkcopy', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'trunc. log', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'torn page detection', N'true'
GO
exec sp_dboption N'IBN41_Trial', N'read only', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'dbo use', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'single', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'autoshrink', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'ANSI null default', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'recursive triggers', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'ANSI nulls', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'concat null yields null', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'cursor close on commit', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'default to local cursor', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'quoted identifier', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'ANSI warnings', N'false'
GO
exec sp_dboption N'IBN41_Trial', N'auto create statistics', N'true'
GO
exec sp_dboption N'IBN41_Trial', N'auto update statistics', N'true'
GO
use [IBN41_Trial]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TRIAL_REQUEST_ADD]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[TRIAL_REQUEST_ADD]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TRIAL_REQUEST_DELETE]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[TRIAL_REQUEST_DELETE]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TRIAL_REQUEST_GET]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[TRIAL_REQUEST_GET]
GO
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[TRIAL_REQUESTS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[TRIAL_REQUESTS]
GO
CREATE TABLE [dbo].[TRIAL_REQUESTS] (
	[RequestID] [int] IDENTITY (1, 1) NOT NULL ,
	[CompanyName] [nvarchar] (250) NOT NULL ,
	[SizeOfGroup] [nvarchar] (50) NOT NULL ,
	[Description] [ntext] NULL ,
	[Domain] [nvarchar] (255) NOT NULL ,
	[FirstName] [nvarchar] (100) NOT NULL ,
	[LastName] [nvarchar] (100) NOT NULL ,
	[Email] [nvarchar] (100) NOT NULL ,
	[Phone] [nvarchar] (100) NULL ,
	[Country] [nvarchar] (100) NOT NULL ,
	[TimeZone] [nvarchar] (150) NOT NULL ,
	[Login] [nvarchar] (50) NOT NULL ,
	[Password] [nvarchar] (50) NOT NULL ,
	[ResellerGUID] [uniqueidentifier] NOT NULL ,
	[TrialResult] [int] NULL ,
	[TrialResultComments] [ntext] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[TRIAL_REQUESTS] WITH NOCHECK ADD
	CONSTRAINT [PK_TRIAL_REQUESTS] PRIMARY KEY  CLUSTERED
	(
		[RequestID]
	)  ON [PRIMARY]
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO
CREATE PROCEDURE TRIAL_REQUEST_ADD
       @RequestID as int ,
       @CompanyName as nvarchar(250) ,
       @SizeOfGroup as nvarchar(50) ,
       @Description as ntext ,
       @Domain as nvarchar(255) ,
       @FirstName as nvarchar(100) ,
       @LastName as nvarchar(100) ,
       @Email as nvarchar(100) ,
       @Phone as nvarchar(100) ,
       @Country as nvarchar(100) ,
       @TimeZone as nvarchar(150) ,
       @Login as nvarchar(50) ,
       @Password as nvarchar(50) ,
       @ResellerGUID as uniqueidentifier ,
       @TrialResult as int,
       @TrialResultComments as ntext,
       @retval int output as
       if EXISTS(SELECT RequestID FROM TRIAL_REQUESTS WHERE RequestID = @RequestID)
           begin
               UPDATE TRIAL_REQUESTS
 	SET CompanyName=@CompanyName, SizeOfGroup=@SizeOfGroup, Description=@Description,
	Domain=@Domain, FirstName=@FirstName, LastName=@LastName, Email=@Email, Phone=@Phone,
	Country=@Country, TimeZone=@TimeZone, Login=@Login, Password=@Password, ResellerGUID=@ResellerGUID,
	TrialResult = @TrialResult, TrialResultComments = @TrialResultComments
	WHERE RequestID = @RequestID
               select @retval = @@identity
           end
       else
           begin
               INSERT INTO TRIAL_REQUESTS (CompanyName,SizeOfGroup,Description,Domain,FirstName,LastName,Email,Phone,Country,TimeZone,Login,Password,ResellerGUID,TrialResult,TrialResultComments) VALUES(@CompanyName, @SizeOfGroup, @Description, @Domain, @FirstName, @LastName, @Email, @Phone, @Country, @TimeZone, @Login, @Password, @ResellerGUID,@TrialResult,@TrialResultComments)
               select @retval = @@identity
           end
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS OFF
GO
CREATE PROCEDURE TRIAL_REQUEST_DELETE
       @RequestID as int,
       @retval int output as
DELETE FROM TRIAL_REQUESTS  WHERE RequestID = @RequestID
select @retval = @@rowcount
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS OFF
GO
CREATE PROCEDURE TRIAL_REQUEST_GET
       @RequestID as int as
SELECT RequestID, CompanyName, SizeOfGroup, Description, Domain, FirstName, LastName, Email, Phone, Country, TimeZone, Login, Password, ResellerGUID FROM TRIAL_REQUESTS  WHERE RequestID = @RequestID or @RequestID=0
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

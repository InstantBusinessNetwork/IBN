
-- Remove SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SmtpBoxInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SmtpBoxInsert]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SmtpBoxUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SmtpBoxUpdate]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SmtpBoxDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SmtpBoxDelete]
GO 
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SmtpBoxSelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SmtpBoxSelect]
GO

-- Create SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SmtpBoxInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SmtpBoxInsert]
GO
CREATE PROCEDURE [dbo].[mc_SmtpBoxInsert]
@Name AS NVarChar(4000),
@Server AS NVarChar(4000),
@Port AS Int,
@SecureConnection AS Int,
@Authenticate AS Bit,
@User AS NVarChar(4000),
@Password AS NVarChar(4000),
@IsDefault AS Bit,
@CheckUid AS UniqueIdentifier,
@Checked AS Bit,
@SmtpBoxId AS Int = NULL OUTPUT
AS
BEGIN
SET NOCOUNT ON;

INSERT INTO [SmtpBox]
(
[Name],
[Server],
[Port],
[SecureConnection],
[Authenticate],
[User],
[Password],
[IsDefault],
[CheckUid],
[Checked])
VALUES(
@Name,
@Server,
@Port,
@SecureConnection,
@Authenticate,
@User,
@Password,
@IsDefault,
@CheckUid,
@Checked)
SELECT @SmtpBoxId = SCOPE_IDENTITY();

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SmtpBoxUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SmtpBoxUpdate]
GO
CREATE PROCEDURE [dbo].[mc_SmtpBoxUpdate]
@Name AS NVarChar(4000),
@Server AS NVarChar(4000),
@Port AS Int,
@SecureConnection AS Int,
@Authenticate AS Bit,
@User AS NVarChar(4000),
@Password AS NVarChar(4000),
@IsDefault AS Bit,
@CheckUid AS UniqueIdentifier,
@Checked AS Bit,
@SmtpBoxId AS Int
AS
BEGIN
SET NOCOUNT ON;

UPDATE [SmtpBox] SET
[Name] = @Name,
[Server] = @Server,
[Port] = @Port,
[SecureConnection] = @SecureConnection,
[Authenticate] = @Authenticate,
[User] = @User,
[Password] = @Password,
[IsDefault] = @IsDefault,
[CheckUid] = @CheckUid,
[Checked] = @Checked WHERE
[SmtpBoxId] = @SmtpBoxId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SmtpBoxDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SmtpBoxDelete]
GO
CREATE PROCEDURE [dbo].[mc_SmtpBoxDelete]
@SmtpBoxId AS Int
AS
BEGIN
SET NOCOUNT ON;

DELETE FROM [SmtpBox]
WHERE
[SmtpBoxId] = @SmtpBoxId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SmtpBoxSelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SmtpBoxSelect]
GO
CREATE PROCEDURE [dbo].[mc_SmtpBoxSelect]
@SmtpBoxId AS Int
AS
BEGIN
SET NOCOUNT ON;

SELECT [t01].[SmtpBoxId] AS [SmtpBoxId], [t01].[Name] AS [Name], [t01].[Server] AS [Server], [t01].[Port] AS [Port], [t01].[SecureConnection] AS [SecureConnection], [t01].[Authenticate] AS [Authenticate], [t01].[User] AS [User], [t01].[Password] AS [Password], [t01].[IsDefault] AS [IsDefault], [t01].[CheckUid] AS [CheckUid], [t01].[Checked] AS [Checked]
FROM [SmtpBox] AS [t01]
WHERE ([t01].[SmtpBoxId]=@SmtpBoxId)

END

-- End
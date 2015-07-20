
-- Remove SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WebDavStorageElementPropertiesInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesInsert]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WebDavStorageElementPropertiesUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesUpdate]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WebDavStorageElementPropertiesDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesDelete]
GO 
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WebDavStorageElementPropertiesSelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesSelect]
GO

-- Create SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WebDavStorageElementPropertiesInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesInsert]
GO
CREATE PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesInsert]
@ObjectTypeId AS Int,
@ObjectId AS Int,
@Key AS NChar(4000),
@Value AS NText,
@ProperyId AS Int = NULL OUTPUT
AS
BEGIN
SET NOCOUNT ON;

INSERT INTO [WebDavStorageElementProperties]
(
[ObjectTypeId],
[ObjectId],
[Key],
[Value])
VALUES(
@ObjectTypeId,
@ObjectId,
@Key,
@Value)
SELECT @ProperyId = SCOPE_IDENTITY();

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WebDavStorageElementPropertiesUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesUpdate]
GO
CREATE PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesUpdate]
@ObjectTypeId AS Int,
@ObjectId AS Int,
@Key AS NChar(4000),
@Value AS NText,
@ProperyId AS Int
AS
BEGIN
SET NOCOUNT ON;

UPDATE [WebDavStorageElementProperties] SET
[ObjectTypeId] = @ObjectTypeId,
[ObjectId] = @ObjectId,
[Key] = @Key,
[Value] = @Value WHERE
[ProperyId] = @ProperyId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WebDavStorageElementPropertiesDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesDelete]
GO
CREATE PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesDelete]
@ProperyId AS Int
AS
BEGIN
SET NOCOUNT ON;

DELETE FROM [WebDavStorageElementProperties]
WHERE
[ProperyId] = @ProperyId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WebDavStorageElementPropertiesSelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesSelect]
GO
CREATE PROCEDURE [dbo].[mc_WebDavStorageElementPropertiesSelect]
@ProperyId AS Int
AS
BEGIN
SET NOCOUNT ON;

SELECT [t01].[ProperyId] AS [ProperyId], [t01].[ObjectTypeId] AS [ObjectTypeId], [t01].[ObjectId] AS [ObjectId], [t01].[Key] AS [Key], [t01].[Value] AS [Value]
FROM [WebDavStorageElementProperties] AS [t01]
WHERE ([t01].[ProperyId]=@ProperyId)

END

-- End
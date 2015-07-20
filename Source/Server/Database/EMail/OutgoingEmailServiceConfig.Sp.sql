
-- Remove SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_OutgoingEmailServiceConfigInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigInsert]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_OutgoingEmailServiceConfigUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigUpdate]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_OutgoingEmailServiceConfigDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigDelete]
GO 
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_OutgoingEmailServiceConfigSelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigSelect]
GO

-- Create SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_OutgoingEmailServiceConfigInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigInsert]
GO
CREATE PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigInsert]
@ServiceType AS Int,
@ServiceKey AS Int,
@SmtpBoxId AS Int,
@OutgoingEmailServiceConfigId AS Int = NULL OUTPUT
AS
BEGIN
SET NOCOUNT ON;

INSERT INTO [OutgoingEmailServiceConfig]
(
[ServiceType],
[ServiceKey],
[SmtpBoxId])
VALUES(
@ServiceType,
@ServiceKey,
@SmtpBoxId)
SELECT @OutgoingEmailServiceConfigId = SCOPE_IDENTITY();

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_OutgoingEmailServiceConfigUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigUpdate]
GO
CREATE PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigUpdate]
@ServiceType AS Int,
@ServiceKey AS Int,
@SmtpBoxId AS Int,
@OutgoingEmailServiceConfigId AS Int
AS
BEGIN
SET NOCOUNT ON;

UPDATE [OutgoingEmailServiceConfig] SET
[ServiceType] = @ServiceType,
[ServiceKey] = @ServiceKey,
[SmtpBoxId] = @SmtpBoxId WHERE
[OutgoingEmailServiceConfigId] = @OutgoingEmailServiceConfigId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_OutgoingEmailServiceConfigDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigDelete]
GO
CREATE PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigDelete]
@OutgoingEmailServiceConfigId AS Int
AS
BEGIN
SET NOCOUNT ON;

DELETE FROM [OutgoingEmailServiceConfig]
WHERE
[OutgoingEmailServiceConfigId] = @OutgoingEmailServiceConfigId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_OutgoingEmailServiceConfigSelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigSelect]
GO
CREATE PROCEDURE [dbo].[mc_OutgoingEmailServiceConfigSelect]
@OutgoingEmailServiceConfigId AS Int
AS
BEGIN
SET NOCOUNT ON;

SELECT [t01].[OutgoingEmailServiceConfigId] AS [OutgoingEmailServiceConfigId], [t01].[ServiceType] AS [ServiceType], [t01].[ServiceKey] AS [ServiceKey], [t01].[SmtpBoxId] AS [SmtpBoxId]
FROM [OutgoingEmailServiceConfig] AS [t01]
WHERE ([t01].[OutgoingEmailServiceConfigId]=@OutgoingEmailServiceConfigId)

END

-- End
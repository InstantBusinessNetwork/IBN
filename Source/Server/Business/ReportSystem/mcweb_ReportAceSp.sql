
-- Remove SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_mcweb_ReportAceInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_mcweb_ReportAceInsert]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_mcweb_ReportAceUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_mcweb_ReportAceUpdate]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_mcweb_ReportAceDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_mcweb_ReportAceDelete]
GO 
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_mcweb_ReportAceSelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_mcweb_ReportAceSelect]
GO

-- Create SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_mcweb_ReportAceInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_mcweb_ReportAceInsert]
GO
CREATE PROCEDURE [dbo].[mc_mcweb_ReportAceInsert]
@ReportId AS UniqueIdentifier,
@Role AS NVarChar(4000),
@PrincipalId AS Int,
@Allow AS Bit,
@ReportAceId AS Int = NULL OUTPUT
AS
BEGIN
SET NOCOUNT ON;

INSERT INTO [mcweb_ReportAce]
(
[ReportId],
[Role],
[PrincipalId],
[Allow])
VALUES(
@ReportId,
@Role,
@PrincipalId,
@Allow)
SELECT @ReportAceId = SCOPE_IDENTITY();

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_mcweb_ReportAceUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_mcweb_ReportAceUpdate]
GO
CREATE PROCEDURE [dbo].[mc_mcweb_ReportAceUpdate]
@ReportId AS UniqueIdentifier,
@Role AS NVarChar(4000),
@PrincipalId AS Int,
@Allow AS Bit,
@ReportAceId AS Int
AS
BEGIN
SET NOCOUNT ON;

UPDATE [mcweb_ReportAce] SET
[ReportId] = @ReportId,
[Role] = @Role,
[PrincipalId] = @PrincipalId,
[Allow] = @Allow WHERE
[ReportAceId] = @ReportAceId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_mcweb_ReportAceDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_mcweb_ReportAceDelete]
GO
CREATE PROCEDURE [dbo].[mc_mcweb_ReportAceDelete]
@ReportAceId AS Int
AS
BEGIN
SET NOCOUNT ON;

DELETE FROM [mcweb_ReportAce]
WHERE
[ReportAceId] = @ReportAceId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_mcweb_ReportAceSelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_mcweb_ReportAceSelect]
GO
CREATE PROCEDURE [dbo].[mc_mcweb_ReportAceSelect]
@ReportAceId AS Int
AS
BEGIN
SET NOCOUNT ON;

SELECT [t01].[ReportAceId] AS [ReportAceId], [t01].[ReportId] AS [ReportId], [t01].[Role] AS [Role], [t01].[PrincipalId] AS [PrincipalId], [t01].[Allow] AS [Allow]
FROM [mcweb_ReportAce] AS [t01]
WHERE ([t01].[ReportAceId]=@ReportAceId)

END

-- End
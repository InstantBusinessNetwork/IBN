
-- Remove SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_METADATAInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATAInsert]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_METADATAUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATAUpdate]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_METADATADelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATADelete]
GO 
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_METADATASelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATASelect]
GO

-- Create SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_METADATAInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATAInsert]
GO
CREATE PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATAInsert]
@Id AS UniqueIdentifier,
@UniqueId AS UniqueIdentifier,
@Prefix AS BigInt,
@ReplicaId AS UniqueIdentifier,
@CurrentVersion AS NVarChar(4000),
@CreationVersion AS NVarChar(4000),
@Uri AS UniqueIdentifier,
@IsDeleted AS Bit,
@Timestamp AS BigInt
AS
BEGIN
SET NOCOUNT ON;

INSERT INTO [SYNCHRONIZATION_METADATA]
(
[Id],
[UniqueId],
[Prefix],
[ReplicaId],
[CurrentVersion],
[CreationVersion],
[Uri],
[IsDeleted],
[Timestamp])
VALUES(
@Id,
@UniqueId,
@Prefix,
@ReplicaId,
@CurrentVersion,
@CreationVersion,
@Uri,
@IsDeleted,
@Timestamp)

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_METADATAUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATAUpdate]
GO
CREATE PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATAUpdate]
@UniqueId AS UniqueIdentifier,
@Prefix AS BigInt,
@ReplicaId AS UniqueIdentifier,
@CurrentVersion AS NVarChar(4000),
@CreationVersion AS NVarChar(4000),
@Uri AS UniqueIdentifier,
@IsDeleted AS Bit,
@Timestamp AS BigInt,
@Id AS UniqueIdentifier
AS
BEGIN
SET NOCOUNT ON;

UPDATE [SYNCHRONIZATION_METADATA] SET
[UniqueId] = @UniqueId,
[Prefix] = @Prefix,
[ReplicaId] = @ReplicaId,
[CurrentVersion] = @CurrentVersion,
[CreationVersion] = @CreationVersion,
[Uri] = @Uri,
[IsDeleted] = @IsDeleted,
[Timestamp] = @Timestamp WHERE
[Id] = @Id

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_METADATADelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATADelete]
GO
CREATE PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATADelete]
@Id AS UniqueIdentifier
AS
BEGIN
SET NOCOUNT ON;

DELETE FROM [SYNCHRONIZATION_METADATA]
WHERE
[Id] = @Id

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_METADATASelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATASelect]
GO
CREATE PROCEDURE [dbo].[mc_SYNCHRONIZATION_METADATASelect]
@Id AS UniqueIdentifier
AS
BEGIN
SET NOCOUNT ON;

SELECT [t01].[Id] AS [Id], [t01].[UniqueId] AS [UniqueId], [t01].[Prefix] AS [Prefix], [t01].[ReplicaId] AS [ReplicaId], [t01].[CurrentVersion] AS [CurrentVersion], [t01].[CreationVersion] AS [CreationVersion], [t01].[Uri] AS [Uri], [t01].[IsDeleted] AS [IsDeleted], [t01].[Timestamp] AS [Timestamp]
FROM [SYNCHRONIZATION_METADATA] AS [t01]
WHERE ([t01].[Id]=@Id)

END

-- End

-- Remove SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_REPLICAInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICAInsert]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_REPLICAUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICAUpdate]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_REPLICADelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICADelete]
GO 
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_REPLICASelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICASelect]
GO

-- Create SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_REPLICAInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICAInsert]
GO
CREATE PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICAInsert]
@ReplicaId AS UniqueIdentifier,
@PrincipalId AS Int,
@TickCount AS Int,
@ReplicaKeyMap AS NText,
@CurrentKnowledge AS NText,
@ForgottenKnowledge AS NText,
@ConflictLog AS NText,
@TombstoneLog AS NText,
@LastDeletedItemsCleanupTime AS DateTime
AS
BEGIN
SET NOCOUNT ON;

INSERT INTO [SYNCHRONIZATION_REPLICA]
(
[ReplicaId],
[PrincipalId],
[TickCount],
[ReplicaKeyMap],
[CurrentKnowledge],
[ForgottenKnowledge],
[ConflictLog],
[TombstoneLog],
[LastDeletedItemsCleanupTime])
VALUES(
@ReplicaId,
@PrincipalId,
@TickCount,
@ReplicaKeyMap,
@CurrentKnowledge,
@ForgottenKnowledge,
@ConflictLog,
@TombstoneLog,
@LastDeletedItemsCleanupTime)

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_REPLICAUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICAUpdate]
GO
CREATE PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICAUpdate]
@PrincipalId AS Int,
@TickCount AS Int,
@ReplicaKeyMap AS NText,
@CurrentKnowledge AS NText,
@ForgottenKnowledge AS NText,
@ConflictLog AS NText,
@TombstoneLog AS NText,
@LastDeletedItemsCleanupTime AS DateTime,
@ReplicaId AS UniqueIdentifier
AS
BEGIN
SET NOCOUNT ON;

UPDATE [SYNCHRONIZATION_REPLICA] SET
[PrincipalId] = @PrincipalId,
[TickCount] = @TickCount,
[ReplicaKeyMap] = @ReplicaKeyMap,
[CurrentKnowledge] = @CurrentKnowledge,
[ForgottenKnowledge] = @ForgottenKnowledge,
[ConflictLog] = @ConflictLog,
[TombstoneLog] = @TombstoneLog,
[LastDeletedItemsCleanupTime] = @LastDeletedItemsCleanupTime WHERE
[ReplicaId] = @ReplicaId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_REPLICADelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICADelete]
GO
CREATE PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICADelete]
@ReplicaId AS UniqueIdentifier
AS
BEGIN
SET NOCOUNT ON;

DELETE FROM [SYNCHRONIZATION_REPLICA]
WHERE
[ReplicaId] = @ReplicaId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_SYNCHRONIZATION_REPLICASelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICASelect]
GO
CREATE PROCEDURE [dbo].[mc_SYNCHRONIZATION_REPLICASelect]
@ReplicaId AS UniqueIdentifier
AS
BEGIN
SET NOCOUNT ON;

SELECT [t01].[ReplicaId] AS [ReplicaId], [t01].[PrincipalId] AS [PrincipalId], [t01].[TickCount] AS [TickCount], [t01].[ReplicaKeyMap] AS [ReplicaKeyMap], [t01].[CurrentKnowledge] AS [CurrentKnowledge], [t01].[ForgottenKnowledge] AS [ForgottenKnowledge], [t01].[ConflictLog] AS [ConflictLog], [t01].[TombstoneLog] AS [TombstoneLog], [t01].[LastDeletedItemsCleanupTime] AS [LastDeletedItemsCleanupTime]
FROM [SYNCHRONIZATION_REPLICA] AS [t01]
WHERE ([t01].[ReplicaId]=@ReplicaId)

END

-- End
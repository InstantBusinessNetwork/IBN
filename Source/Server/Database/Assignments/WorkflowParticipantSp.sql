
-- Remove SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WorkflowParticipantInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_WorkflowParticipantInsert]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WorkflowParticipantUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_WorkflowParticipantUpdate]
GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WorkflowParticipantDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_WorkflowParticipantDelete]
GO 
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WorkflowParticipantSelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) DROP PROCEDURE [dbo].[mc_WorkflowParticipantSelect]
GO

-- Create SP
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WorkflowParticipantInsert]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_WorkflowParticipantInsert]
GO
CREATE PROCEDURE [dbo].[mc_WorkflowParticipantInsert]
@UserId AS Int,
@WorkflowInstanceId AS UniqueIdentifier,
@ObjectId AS Int,
@ObjectType AS Int,
@WorkflowParticipantId AS Int = NULL OUTPUT
AS
BEGIN
SET NOCOUNT ON;

INSERT INTO [WorkflowParticipant]
(
[UserId],
[WorkflowInstanceId],
[ObjectId],
[ObjectType])
VALUES(
@UserId,
@WorkflowInstanceId,
@ObjectId,
@ObjectType)
SELECT @WorkflowParticipantId = SCOPE_IDENTITY();

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WorkflowParticipantUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_WorkflowParticipantUpdate]
GO
CREATE PROCEDURE [dbo].[mc_WorkflowParticipantUpdate]
@UserId AS Int,
@WorkflowInstanceId AS UniqueIdentifier,
@ObjectId AS Int,
@ObjectType AS Int,
@WorkflowParticipantId AS Int
AS
BEGIN
SET NOCOUNT ON;

UPDATE [WorkflowParticipant] SET
[UserId] = @UserId,
[WorkflowInstanceId] = @WorkflowInstanceId,
[ObjectId] = @ObjectId,
[ObjectType] = @ObjectType WHERE
[WorkflowParticipantId] = @WorkflowParticipantId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WorkflowParticipantDelete]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_WorkflowParticipantDelete]
GO
CREATE PROCEDURE [dbo].[mc_WorkflowParticipantDelete]
@WorkflowParticipantId AS Int
AS
BEGIN
SET NOCOUNT ON;

DELETE FROM [WorkflowParticipant]
WHERE
[WorkflowParticipantId] = @WorkflowParticipantId

END

GO
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id (N'[dbo].[mc_WorkflowParticipantSelect]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[mc_WorkflowParticipantSelect]
GO
CREATE PROCEDURE [dbo].[mc_WorkflowParticipantSelect]
@WorkflowParticipantId AS Int
AS
BEGIN
SET NOCOUNT ON;

SELECT [t01].[WorkflowParticipantId] AS [WorkflowParticipantId], [t01].[UserId] AS [UserId], [t01].[WorkflowInstanceId] AS [WorkflowInstanceId], [t01].[ObjectId] AS [ObjectId], [t01].[ObjectType] AS [ObjectType]
FROM [WorkflowParticipant] AS [t01]
WHERE ([t01].[WorkflowParticipantId]=@WorkflowParticipantId)

END

-- End
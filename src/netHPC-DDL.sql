IF OBJECT_ID('ExecutionNode') IS NOT NULL
	DROP TABLE [ExecutionNode]
GO

IF OBJECT_ID('WorkItem') IS NOT NULL
	DROP TABLE [WorkItem]
GO

IF OBJECT_ID('Event') IS NOT NULL
	DROP TABLE [Event]
GO

IF OBJECT_ID('Execution') IS NOT NULL
	DROP TABLE [Execution]
GO

IF OBJECT_ID('Node') IS NOT NULL
	DROP TABLE [Node]
GO

IF OBJECT_ID('Algorithm') IS NOT NULL
	DROP TABLE [Algorithm]
GO

--
-- [Algorithm]
--
CREATE TABLE [Algorithm] (
	[algorithmId]			INT					NOT NULL	IDENTITY(1, 1),
	[name]					NVARCHAR(50)		NOT NULL,
	[description]			NVARCHAR(150)		NULL,
	[dateCreated]			DATETIME			NOT NULL,
	[dateModified]			DATETIME			NOT NULL,
	[assembly]				VARBINARY(MAX)		NOT NULL,
	CONSTRAINT [Algorithm_PK] PRIMARY KEY ([algorithmId]))
GO

--
-- [Node]
--
CREATE TABLE [Node] (
	[nodeId]				INT					NOT NULL	IDENTITY(1, 1),
	[name]					NVARCHAR(50)		NOT NULL,
	[description]			NVARCHAR(150)		NULL,
	[dateCreated]			DATETIME			NOT NULL,
	[lastReport]			DATETIME			NOT NULL,
	[status]				TINYINT				NOT NULL,	
	[numOfExecUnits]		INT					NOT NULL,
	[speedMHz]				INT					NOT NULL,
	CONSTRAINT [Node_PK] PRIMARY KEY ([nodeId]))
GO

--
-- [Execution]
--
CREATE TABLE [Execution] (
	[algorithmId]			INT					NOT NULL,
	[executionId]			INT					NOT NULL,
	[name]					NVARCHAR(50)		NOT NULL,
	[description]			NVARCHAR(150)		NULL,
	[parameters]			NVARCHAR(MAX)		NULL,
	[dateStart]				DATETIME			NOT NULL,
	[dateFinish]			DATETIME			NULL,
	[status]				TINYINT				NOT NULL,
	CONSTRAINT [Execution_PK] PRIMARY KEY ([algorithmId], [executionId]),
	CONSTRAINT [Algorithm_FK01] FOREIGN KEY ([algorithmId]) REFERENCES [Algorithm] ([algorithmId]))
GO

--
-- [ExecutionNode]
--
CREATE TABLE [ExecutionNode] (
	[algorithmId]			INT					NOT NULL,
	[executionId]			INT					NOT NULL,
	[nodeId]				INT					NOT NULL,	
	CONSTRAINT [ExecutionNode_PK] PRIMARY KEY ([algorithmId], [executionId], [nodeId]),
	CONSTRAINT [Execution_FK01] FOREIGN KEY ([algorithmId], [executionId]) REFERENCES [Execution] ([algorithmId], [executionId]),
	CONSTRAINT [Node_FK01] FOREIGN KEY ([nodeId]) REFERENCES [Node] ([nodeId]))
GO

--
-- [WorkItem]
--
CREATE TABLE [WorkItem] (
	[algorithmId]			INT					NOT NULL,
	[executionId]			INT					NOT NULL,
	[nodeId]				INT					NOT NULL,
	[execUnitId]			INT					NOT NULL,
	[workItemId]			INT					NOT NULL,
	[dateCreated]			DATETIME			NOT NULL,
	[timeElapsed]			BIGINT				NOT NULL,
	[text]					NVARCHAR(MAX)		NULL,
	CONSTRAINT [WorkItem_PK] PRIMARY KEY ([algorithmId], [executionId], [nodeId], [execUnitId], [workItemId]),
	CONSTRAINT [Execution_FK02] FOREIGN KEY ([algorithmId], [executionId]) REFERENCES [Execution] ([algorithmId], [executionId]),
	CONSTRAINT [Node_FK02] FOREIGN KEY ([nodeId]) REFERENCES [Node] ([nodeId]))
GO

--
-- [Event]
--
CREATE TABLE [Event] (
	[algorithmId]			INT					NOT NULL,
	[executionId]			INT					NOT NULL,
	[nodeId]				INT					NOT NULL,		
	[execUnitId]			INT					NOT NULL,	
	[eventId]				INT					NOT NULL,
	[eventName]				NVARCHAR(100)		NOT NULL,
	[dateCreated]			DATETIME			NOT NULL,
	[timeElapsed]			BIGINT				NOT NULL,
	[internal]				BIT					NOT NULL,
	[text]					NVARCHAR(MAX)		NULL,
	CONSTRAINT [Event_PK] PRIMARY KEY ([algorithmId], [executionId], [nodeId], [execUnitId], [eventId]),
	CONSTRAINT [Execution_FK03] FOREIGN KEY ([algorithmId], [executionId]) REFERENCES [Execution] ([algorithmId], [executionId]),
	CONSTRAINT [Node_FK03] FOREIGN KEY ([nodeId]) REFERENCES [Node] ([nodeId]))
GO

--
-- [ExecutionInsert]
--
IF OBJECT_ID('[ExecutionInsert]') IS NOT NULL
	DROP PROCEDURE [ExecutionInsert]

CREATE PROCEDURE [ExecutionInsert] (
	@algorithmId			INT,
	@name					NVARCHAR(50),
	@description			NVARCHAR(150),
	@parameters				NVARCHAR(MAX),
	@dateStart				DATETIME,
	@dateFinish				DATETIME,
	@status					TINYINT) AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE
	BEGIN TRANSACTION
	
	DECLARE @executionId	INT

	SELECT @executionId = COALESCE(MAX(executionId), 0) + 1 FROM [Execution] WHERE [algorithmId] = @algorithmId

	INSERT [Execution] ([algorithmId], [executionId], [name], [description], [parameters], [dateStart], [dateFinish], [status]) 
	VALUES (@algorithmId, @executionId, @name, @description, @parameters, @dateStart, @dateFinish, @status) 

	COMMIT TRANSACTION
END
GO

CREATE PROCEDURE [ExecutionUpdate] (
	@algorithmId			INT,
	@executionId			INT,
	@name					NVARCHAR(50),
	@description			NVARCHAR(150),
	@parameters				NVARCHAR(MAX),
	@dateStart				DATETIME,
	@dateFinish				DATETIME,
	@status					TINYINT) AS
BEGIN
	UPDATE [Execution] SET [name] = @name, [description] = @description, [parameters] = @parameters, [dateStart] = @dateStart, [dateFinish] = @dateFinish, [status] = @status
	WHERE [algorithmId] = @algorithmId AND [executionId] = @executionId
END
GO


CREATE PROCEDURE [ExecutionDelete] (
	@algorithmId			INT,
	@executionId			INT) AS
BEGIN
	DELETE FROM [Execution] WHERE [algorithmId] = @algorithmId AND [executionId] = @executionId
END
GO
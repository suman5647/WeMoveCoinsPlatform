CREATE TABLE [dbo].[AuditTrail] (
    [Id]                BIGINT            IDENTITY (1, 1) NOT NULL,
    [OrderId]           BIGINT            NULL,
    [Status]            BIGINT            NOT NULL,
    [Message]           NVARCHAR (MAX) NULL,
    [Created]           DATETIME       NOT NULL,
    [AuditTrailLevelId] BIGINT            NULL,
    CONSTRAINT [PK_AuditTrail] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AuditTrail_AuditTrailLevel] FOREIGN KEY ([AuditTrailLevelId]) REFERENCES [dbo].[AuditTrailLevel] ([Id]),
    CONSTRAINT [FK_AuditTrail_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order] ([Id]),
    CONSTRAINT [FK_AuditTrail_AuditTrailStatus] FOREIGN KEY ([Status]) REFERENCES [dbo].[AuditTrailStatus] ([Id])
);
GO

CREATE NONCLUSTERED INDEX IX_AuditTrail_Created 
    ON [AuditTrail]([Created] ASC); 
GO

CREATE NONCLUSTERED INDEX IX_AuditTrail_Level
    ON [AuditTrail]([AuditTrailLevelId] ASC);   
GO  

CREATE NONCLUSTERED INDEX IX_AuditTrail_Status
    ON [AuditTrail]([Status] ASC);   
GO  


CREATE TABLE [dbo].[AuditTrailStatus] (
    [Id]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Text] NVARCHAR (20) NULL,
    CONSTRAINT [PK_AuditTrailSatus] PRIMARY KEY CLUSTERED ([Id] ASC)
);




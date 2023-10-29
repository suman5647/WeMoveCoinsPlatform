CREATE TABLE [dbo].[AuditTrailLevel] (
    [Id]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Text] NVARCHAR (20) NULL,
    CONSTRAINT [PK_AuditTrailLevel] PRIMARY KEY CLUSTERED ([Id] ASC)
);




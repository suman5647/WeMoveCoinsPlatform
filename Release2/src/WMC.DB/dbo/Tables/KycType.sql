CREATE TABLE [dbo].[KycType] (
    [Id]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Text] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_KycType] PRIMARY KEY CLUSTERED ([Id] ASC)
);




CREATE TABLE [dbo].[Language] (
    [Id]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Code] NVARCHAR (2)  NOT NULL,
    [Text] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([Id] ASC)
);




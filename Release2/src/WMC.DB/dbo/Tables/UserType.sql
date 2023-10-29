CREATE TABLE [dbo].[UserType] (
    [Id]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Text] NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_UserType] PRIMARY KEY CLUSTERED ([Id] ASC)
);




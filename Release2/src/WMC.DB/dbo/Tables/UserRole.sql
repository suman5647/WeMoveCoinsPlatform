CREATE TABLE [dbo].[UserRole] (
    [Id]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Text] NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([Id] ASC)
);




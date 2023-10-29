CREATE TABLE [dbo].[TransactionType] (
    [Id]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Text] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_TransactionType] PRIMARY KEY CLUSTERED ([Id] ASC)
);




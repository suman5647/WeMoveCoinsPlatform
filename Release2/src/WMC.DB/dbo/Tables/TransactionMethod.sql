CREATE TABLE [dbo].[TransactionMethod] (
    [Id]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Text] NVARCHAR (20) NOT NULL,
    CONSTRAINT [PK_TransactionMethod] PRIMARY KEY CLUSTERED ([Id] ASC)
);




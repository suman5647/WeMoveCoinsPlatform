CREATE TABLE [dbo].[Transaction] (
    [Id]          BIGINT IDENTITY (100, 1) NOT NULL,
    [OrderId]     BIGINT          NULL,
    [MethodId]    BIGINT          NOT NULL,
    [Type]        BIGINT          NOT NULL,
    [ExtRef]      NVARCHAR (MAX)  NULL,
    [Amount]      DECIMAL (18, 8) NULL,
    [Currency]    BIGINT          NOT NULL,
    [Info]        NVARCHAR (MAX)  NULL,
    [Completed]   DATETIME        NULL,
    [FromAccount] BIGINT          NULL,
    [ToAccount]   BIGINT          NULL,
    [Reconsiled]  DATETIME        NULL,
    [Exported]    DATETIME        NULL,
    [BatchNumber] NVARCHAR(256)   NULL, -- multiple entry happens on a transaction like: paid 1000 and 10 is commision, and 5 is transaction fee, etc
    CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Transaction_Account_From] FOREIGN KEY ([FromAccount]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_Transaction_Account_To] FOREIGN KEY ([ToAccount]) REFERENCES [dbo].[Account] ([Id]),
    CONSTRAINT [FK_Transaction_Currency] FOREIGN KEY ([Currency]) REFERENCES [dbo].[Currency] ([Id]),
    CONSTRAINT [FK_Transaction_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order] ([Id]),
    CONSTRAINT [FK_Transaction_TransactionMethod] FOREIGN KEY ([MethodId]) REFERENCES [dbo].[TransactionMethod] ([Id]),
    CONSTRAINT [FK_Transaction_TransactionType] FOREIGN KEY ([Type]) REFERENCES [dbo].[TransactionType] ([Id])
);




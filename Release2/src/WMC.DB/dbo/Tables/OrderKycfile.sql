CREATE TABLE [dbo].[OrderKycfile] (
    [Id]        BIGINT IDENTITY (100, 1) NOT NULL,
    [OrderId]   BIGINT NOT NULL,
    [KycfileId] BIGINT NOT NULL,
    CONSTRAINT [PK_OrderKycfile] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrderKycfile_KycFile] FOREIGN KEY ([KycfileId]) REFERENCES [dbo].[KycFile] ([Id]),
    CONSTRAINT [FK_OrderKycfile_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order] ([Id])
);




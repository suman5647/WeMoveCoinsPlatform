CREATE TABLE [dbo].[OrderType] (
    [Id]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Text] NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_OrderType] PRIMARY KEY CLUSTERED ([Id] ASC)
);




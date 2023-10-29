CREATE TABLE [dbo].[OrderStatus] (
    [Id]          BIGINT            IDENTITY (1, 1) NOT NULL,
    [Text]        NVARCHAR (30)  NULL,
    [Description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_OrderStatus] PRIMARY KEY CLUSTERED ([Id] ASC)
);




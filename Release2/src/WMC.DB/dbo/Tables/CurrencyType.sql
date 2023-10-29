CREATE TABLE [dbo].[CurrencyType] (
    [Id]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Text] NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_CurrencyType] PRIMARY KEY CLUSTERED ([Id] ASC)
);


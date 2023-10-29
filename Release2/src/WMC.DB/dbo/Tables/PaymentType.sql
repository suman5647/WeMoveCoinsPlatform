CREATE TABLE [dbo].[PaymentType] (
    [Id]   BIGINT           IDENTITY (1, 1) NOT NULL,
    [Code] NVARCHAR (10) NOT NULL,
    [Text] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_PaymentType] PRIMARY KEY CLUSTERED ([Id] ASC)
);




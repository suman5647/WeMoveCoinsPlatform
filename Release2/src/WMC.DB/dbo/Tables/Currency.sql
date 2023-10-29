CREATE TABLE [dbo].[Currency] (
    [Id]                  BIGINT             IDENTITY (5, 1) NOT NULL,
    [CurrencyTypeId]      BIGINT             NULL,
    [Code]                NVARCHAR (3)    NOT NULL,
    [Text]                NVARCHAR (40)   NULL,
    [YourPayCurrencyCode] NVARCHAR (10)   NULL,
    [PayLikeCurrencyCode] NVARCHAR (10)   NULL,
    [PayLikeDetails]      NVARCHAR (MAX)  NULL,
    [FXMarkUp]            DECIMAL (18, 8) NULL,
    [BitgoSettings]       NVARCHAR (MAX)  NULL,
    [IsActive]            BIT             DEFAULT ((0)) NOT NULL,
	[PaymentTypeAcceptance] INT           NOT NULL CONSTRAINT [DF_Currency_PaymentTypeAcceptance] DEFAULT 0,
    [MinorUnits] INT NULL, 
    CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Currency_CurrencyType] FOREIGN KEY ([CurrencyTypeId]) REFERENCES [dbo].[CurrencyType] ([Id])
);






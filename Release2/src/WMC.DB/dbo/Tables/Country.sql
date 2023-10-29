CREATE TABLE [dbo].[Country] (
    [Id]               BIGINT             IDENTITY (1, 1) NOT NULL,
    [Code]             NVARCHAR (2)    NOT NULL,
    [Text]             NVARCHAR (50)   NOT NULL,
    [PhoneCode]        INT             NULL,
    [CurrencyId]       BIGINT             NULL,
    [PhoneNumberStyle] NVARCHAR(20)    NULL,
    [CultureCode]      NVARCHAR(10)    NULL,
    [TrustValue]       DECIMAL (18, 8) NULL,
    [AlphaSupport]     BIT             NULL,
    [Note]             NVARCHAR (200)  NULL,
    [CardFee]          DECIMAL (18, 8) NULL,
    [AddTx]            DATETIME        NULL,
	[PaymentGateWaysAccepted] INT      NOT NULL CONSTRAINT [DF_Country_PaymentGateWaysAccepted] DEFAULT 0,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Country_Currency] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[Currency] ([Id])
);






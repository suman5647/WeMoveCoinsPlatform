﻿CREATE TABLE [dbo].[Order] (
    [Id]                            BIGINT              IDENTITY (100, 1) NOT NULL,
    [Number]                        NVARCHAR (8)     NOT NULL,
    [UserId]                        BIGINT              NOT NULL,
    [Status]                        BIGINT              NOT NULL,
    [Type]                          BIGINT              NOT NULL,
    [RequestInfo]                   NVARCHAR (MAX)   NULL,
    [TermsIsAgreed]                 DATETIME         NULL,
    [Quoted]                        DATETIME         NULL,
    [Rate]                          DECIMAL (18, 8)  NULL,
    [QuoteSource]                   NVARCHAR (50)    NULL,
    [Amount]                        DECIMAL (18, 8)  NULL,
    [BTCAmount]                     DECIMAL (18, 8)  NULL,
    [CurrencyId]                    BIGINT              NOT NULL,
    [CommissionProcent]             DECIMAL (18, 8)  NULL,
    [CardNumber]                    NCHAR (20)       NULL,
    [PaymentType]                   BIGINT              NOT NULL,
    [CryptoAddress]                 NVARCHAR (50)    NULL,
    [AccountNumber]                 NVARCHAR (35)    NULL,
    [SwiftBIC]                      NVARCHAR (11)    NULL,
    [RecieverName]                  NVARCHAR (35)    NULL,
    [RecieverRef]                   NVARCHAR (20)    NULL,
    [RecieverText]                  NVARCHAR (35)    NULL,
    [CurrencyCode]                  NVARCHAR (3)     NULL,
    [WireType]                      NVARCHAR (2)     NULL,
    [WireCost]                      NVARCHAR (1)     NULL,
    [Note]                          NVARCHAR (MAX)   NULL,
    [ExtRef]                        NVARCHAR (50)    NULL,
    [Name]                          NVARCHAR (50)    NULL,
    [Email]                         NVARCHAR (50)    NULL,
    [IP]                            NVARCHAR (20)    NULL,
    [TransactionHash]               NVARCHAR (MAX)   NULL,
    [SiteId]                        BIGINT              NULL,
    [PaymentGatewayType]            NVARCHAR (50)    NULL,
    [RateBase]                      DECIMAL (18, 8)  NULL,
    [RateHome]                      DECIMAL (18, 8)  NULL,
    [RateBooks]                     DECIMAL (18, 8)  NULL,
    [Approved]                      DATETIME         NULL,
    [ApprovedBy]                    BIGINT              NULL,
    [CountryCode]                   NVARCHAR (100)   NULL,
    [TxSecret]                      NVARCHAR (4)     NULL,
    [CardApproved]                  DATETIME         NULL,
    [RiskScore]                     DECIMAL (18, 8)  NULL,
    [IpCode]                        NVARCHAR (2)     NULL,
    [CreditCardUserIdentity]        UNIQUEIDENTIFIER NULL,
    [TxSecrectVerificationAttempts] BIGINT              NULL,
    [Referrer]                      NVARCHAR (MAX)   NULL,
    [Origin]                        NVARCHAR (MAX)   NULL,
    [MinersFee]                     DECIMAL (18, 8)  NULL,
    [BccAddress]                    VARCHAR (50)     NULL,
    [PartnerId]                     VARCHAR (50)     NULL,
    [Direction]                     BIT              NULL,
    [IBAN]                          NVARCHAR (100)   NULL,
    [Reg]                           NVARCHAR (50)    NULL,
    [CCRG]                          NVARCHAR (50)    NULL,
    [CouponId]                      BIGINT              NULL,
    [FixedFee]                      DECIMAL (18, 8)  NULL,
    [DiscountAmount]                DECIMAL (18, 8)  NULL,
    [OurFee]                        DECIMAL (18, 8)  NULL,
    [FxMarkUp]                      DECIMAL (18, 8)  NULL,
    [Spread]                        DECIMAL (18, 8)  NULL,
    [CryptoCurrencyId]              BIGINT           NOT NULL,
    [LockKey]                       NVARCHAR (50)    NULL,
    [LockUntil]                     DATETIME         NULL,
    [DateOfBirth] DATETIME NULL, 
    [ReferenceId] NVARCHAR(50) NULL, 
    [MerchantCode] NVARCHAR(10)  NOT NULL, 
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Order_User_Approved] FOREIGN KEY ([ApprovedBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Order_Coupon] FOREIGN KEY ([CouponId]) REFERENCES [dbo].[Coupon] ([Id]),
    CONSTRAINT [FK_Order_Currency] FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[Currency] ([Id]),
    CONSTRAINT [FK_Order_CryptoCurrencyId_Currency] FOREIGN KEY ([CryptoCurrencyId]) REFERENCES [dbo].[Currency] ([Id]),
    CONSTRAINT [FK_Order_PaymentType] FOREIGN KEY ([PaymentType]) REFERENCES [dbo].[PaymentType] ([Id]),
    CONSTRAINT [FK_Order_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_Order_Status] FOREIGN KEY ([Status]) REFERENCES [dbo].[OrderStatus] ([Id]),
    CONSTRAINT [FK_Order_OrderType] FOREIGN KEY ([Type]) REFERENCES [dbo].[OrderType] ([Id]),
    CONSTRAINT [FK_Order_Site] FOREIGN KEY ([SiteId]) REFERENCES [dbo].[Site] ([Id]),
    UNIQUE NONCLUSTERED ([Number] ASC)
    --CONSTRAINT [FK_Merchant_MerchantCode] FOREIGN KEY ([MerchantCode]) REFERENCES [dbo].[Merchant] ([MerchantCode])
);



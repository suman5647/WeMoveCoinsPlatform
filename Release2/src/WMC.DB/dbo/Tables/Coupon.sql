CREATE TABLE [dbo].[Coupon] (
    [Id]               BIGINT            IDENTITY (100, 1) NOT NULL,
    [CouponCode]       NVARCHAR(50)   NOT NULL,
    [Description]      NVARCHAR(150)  NOT NULL,
    [Discount]         DECIMAL (18, 8) NOT NULL,
    [MaxTxnCount]      BIGINT            NULL,
    [MinTxnLimit]      DECIMAL (18, 8)          NULL,
    [MaxTxnLimit]      DECIMAL (18, 8)          NULL,
    [MaxTotalTxnLimit] DECIMAL (18, 8)          NULL,
    [FromDate]         DATETIME           NULL,
    [ToDate]           DATETIME           NULL,
    [Region]           NVARCHAR(10)   NULL,
    [CryptoCurrency]   NVARCHAR(10)   NULL,
    [Type]             NVARCHAR(10)   NULL,
    [ReferredBy]       NVARCHAR(50)   NULL,
    [IsActive]         BIT            NULL,
    CONSTRAINT [PK_Coupon] PRIMARY KEY CLUSTERED ([Id] ASC)
);




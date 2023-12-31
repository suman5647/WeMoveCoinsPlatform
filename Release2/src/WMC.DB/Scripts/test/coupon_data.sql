/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[Coupon] ON;
GO
MERGE INTO [dbo].[Coupon] AS Target
USING 
(
	VALUES 
	(1, N'FirstTime', N'test coupon', CAST(55.000 AS Decimal(18, 8)), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1)
	,(2, N'Get25', N'25% discount', CAST(25.000 AS Decimal(18, 8)), NULL, NULL, NULL, NULL, CAST(N'2017-12-01' AS Date), CAST(N'2018-04-08' AS Date), NULL, NULL, NULL, NULL, 1)
	,(7, N'5times30', N'Can be used five times', CAST(100.000 AS Decimal(18, 8)), NULL, NULL, NULL, NULL, CAST(N'2017-12-01' AS Date), CAST(N'2018-05-08' AS Date), NULL, NULL, NULL, NULL, 1)
)
AS Source ([Id], [CouponCode], [Description], [Discount], [MaxTxnCount], [MinTxnLimit], [MaxTxnLimit], [MaxTotalTxnLimit], [FromDate], [ToDate], [Region], [CryptoCurrency], [Type], [ReferredBy], [IsActive])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [CouponCode] = Source.[CouponCode],
			[Description] = Source.[Description],
			[Discount] = Source.[Discount],
			[MaxTxnCount] = Source.[MaxTxnCount],
			[MinTxnLimit] = Source.[MinTxnLimit],
			[MaxTxnLimit] = Source.[MaxTxnLimit],
			[MaxTotalTxnLimit] = Source.[MaxTotalTxnLimit],
			[FromDate] = Source.[FromDate],
			[ToDate] = Source.[ToDate],
			[Region] = Source.[Region],
			[CryptoCurrency] = Source.[CryptoCurrency],
			[ReferredBy] = Source.[ReferredBy],
			[IsActive] = Source.[IsActive]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [CouponCode], [Description], [Discount], [MaxTxnCount], [MinTxnLimit], [MaxTxnLimit], [MaxTotalTxnLimit], [FromDate], [ToDate], [Region], [CryptoCurrency], [Type], [ReferredBy], [IsActive])
VALUES ([Id], [CouponCode], [Description], [Discount], [MaxTxnCount], [MinTxnLimit], [MaxTxnLimit], [MaxTotalTxnLimit], [FromDate], [ToDate], [Region], [CryptoCurrency], [Type], [ReferredBy], [IsActive])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[Coupon] OFF

GO
/* *************************************************************** */
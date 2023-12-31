/* *************************************************************** */
--SET IDENTITY_INSERT [dbo].[Coupon] ON;
--GO
--MERGE INTO [dbo].[Coupon] AS Target
--USING 
--(
--	VALUES 
--	(101, N'AMIT4961', N'WMC00005606', CAST(10.000 AS Numeric(6, 3)), NULL, NULL, NULL, NULL, CAST(N'2018-04-05' AS Date), CAST(N'2018-05-01' AS Date), NULL, NULL, NULL, NULL, 1),
--	(102, N'WMC25', N'WMC25', CAST(25.000 AS Numeric(6, 3)), NULL, NULL, NULL, NULL, CAST(N'2018-04-20' AS Date), CAST(N'2018-05-01' AS Date), NULL, NULL, NULL, NULL, 1)
--)
--AS Source ([Id], [CouponCode], [Description], [Discount], [MaxTxnCount], [MinTxnLimit], [MaxTxnLimit], [MaxTotalTxnLimit], [FromDate], [ToDate], [Region], [CryptoCurrency], [Type], [ReferredBy], [IsActive])
--ON Target.[Id] = Source.[Id]
---- update matched rows
--WHEN MATCHED THEN
--UPDATE SET  [CouponCode] = Source.[CouponCode],
--			[Description] = Source.[Description],
--			[Discount] = Source.[Discount],
--			[MaxTxnCount] = Source.[MaxTxnCount],
--			[MinTxnLimit] = Source.[MinTxnLimit],
--			[MaxTxnLimit] = Source.[MaxTxnLimit],
--			[MaxTotalTxnLimit] = Source.[MaxTotalTxnLimit],
--			[FromDate] = Source.[FromDate],
--			[ToDate] = Source.[ToDate],
--			[Region] = Source.[Region],
--			[CryptoCurrency] = Source.[CryptoCurrency],
--			[ReferredBy] = Source.[ReferredBy],
--			[IsActive] = Source.[IsActive]
---- insert new rows
--WHEN NOT MATCHED BY TARGET THEN
--INSERT ([Id], [CouponCode], [Description], [Discount], [MaxTxnCount], [MinTxnLimit], [MaxTxnLimit], [MaxTotalTxnLimit], [FromDate], [ToDate], [Region], [CryptoCurrency], [Type], [ReferredBy], [IsActive])
--VALUES ([Id], [CouponCode], [Description], [Discount], [MaxTxnCount], [MinTxnLimit], [MaxTxnLimit], [MaxTotalTxnLimit], [FromDate], [ToDate], [Region], [CryptoCurrency], [Type], [ReferredBy], [IsActive])
--WHEN NOT MATCHED BY SOURCE THEN DELETE;

--SET IDENTITY_INSERT [dbo].[Coupon] OFF

--GO
/* *************************************************************** */
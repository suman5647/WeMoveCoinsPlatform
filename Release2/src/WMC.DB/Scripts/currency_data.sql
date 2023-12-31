/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[CurrencyType] ON
GO
MERGE INTO [dbo].[CurrencyType] AS Target
USING 
(
	VALUES 
	(1, N'Fiat'),
	(2, N'Digital')
)
AS Source ([Id], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text])
VALUES ([Id], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[CurrencyType] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[Currency] ON
GO
MERGE INTO [dbo].[Currency] AS Target
USING 
(
	VALUES 
		  (3,1,N'EUR', N'Euro', N'978', N'EUR', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.00 AS Decimal(18, 8)), N'NULL',1,7, 2)
		,(12,1,N'DKK', N'Danish krone', N'208', N'DKK', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(2.00 AS Decimal(18, 8)), N'NULL',1,7, 2)
		,(15,1,N'ISK', N'Icelandic Króna', N'352', N'ISK', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'NULL',0,0, 0)
		,(16,1,N'CHF', N'Swiss franc', N'756', N'CHF', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(2.50 AS Decimal(18, 8)), N'NULL',0,4, 2)
		,(19,1,N'NOK', N'Norwegian krone', N'578', N'NOK', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(3.50 AS Decimal(18, 8)), N'NULL',0,4, 2)
		,(23,1,N'SEK', N'Swedish krona', N'752', N'SEK', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(3.50 AS Decimal(18, 8)), N'NULL',0,4, 2)
		,(24,1,N'TRY', N'Turkish lira', N'949', N'TRY', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'NULL',0,0, 2)
		,(25,1,N'GBP', N'Pound sterling', N'826', N'GBP', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(3.50 AS Decimal(18, 8)), N'NULL',1,4, 2)
		,(26,2,N'BTC', N'Bitcoin', N'NULL', N'NULL', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'{"DefaultWalletId":"5bf6863102b32a481e90c8d394060dd7","DefaultAmount":100000.0,"TestCurrency":"TBTC","KrakenCode":"XXBT","KrakenEurPairCode":"XXBTZEUR","MinersFee":{"size":374,"feeRate":1000000,"fee":0.00005,"payGoFee":0,"payGoFeeString":"0"},"PassPhrase":"miHNZqLZRfF9h;pi4V9xxrvN","TxUnit":100000000}',1,7, 8)
		,(27,2,N'ETH', N'Ether', N'NULL', N'NULL', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'NULL',0,0, 18)
		,(42,1,N'ALL', N'Albanian Lek', N'8', N'NULL', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'NULL',0,0, 2)
		,(43,1,N'AZN', N'Azerbaijani New Manat', N'944', N'AZN', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'NULL',0,0, 2)
		,(44,1,N'BYR', N'Belarusian Ruble', N'974', N'BYR', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'NULL',0,0, 0)
		,(45,1,N'BAM', N'Bosnia and Herzegovina Convertible Marka', N'977', N'BAM', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'NULL',0,0, 2)
		,(46,1,N'HRK', N'Croatian Kuna', N'191', N'HRK', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'NULL',0,0,2)
		,(47,1,N'CZK', N'Czech Republic Koruna', N'103', N'CZK', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(4.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(48,1,N'GEL', N'Georgian Lari', N'981', N'GEL', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4, 2)
		,(49,1,N'HUF', N'Hungarian Florin', N'348', N'HUF', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(3.50 AS Decimal(18, 8)), N'NULL',0,4, 2)
		,(50,1,N'MKD', N'Macedonian Denar', N'807', N'MKD', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'NULL',0,0,2)
		,(51,1,N'MDL', N'Moldovan leu', N'498', N'MDL', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(52,1,N'PLN', N'Polish Zloty', N'985', N'PLN', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'NULL',0,0,2)
		,(53,1,N'RSD', N'Serbian Dinar', N'941', N'RSD', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(54,1,N'ZAR', N'South African Rand', N'710', N'ZAR', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(55,1,N'INR', N'Indian Rupees', N'356', N'INR', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(3.50 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(70,1,N'USD', N'US Dollar', N'840', N'USD', N'[{"SiteId": 1, "PublicKey": "7c014f26-5195-4363-aff0-80740b374db3", "AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}, {"SiteId": 2,"PublicKey": "7c014f26-5195-4363-aff0-80740b374db3","AppKey": "cb35d3c2-02f3-4c28-95fa-97e4131e93b0"}]', CAST(1.05 AS Decimal(18, 8)), N'NULL',0,0,2)
		,(71,2,N'LTC', N'Litecoin', N'0', N'LTC', N'NULL', CAST(1.05 AS Decimal(18, 8)), N'{"DefaultWalletId":"5b06a4b680bb15cd07400a55fe78bed4","DefaultAmount":100000.0,"TestCurrency":"TLTC","KrakenCode":"XLTC","KrakenEurPairCode":"XLTCZEUR","MinersFee":{"size":374,"feeRate":100000,"fee":0.00003,"payGoFee":0,"payGoFeeString":"0"},"PassPhrase":"LTC2","TxUnit":100000000}',0,0,8)
		,(72,1,N'AED', N'UAE Dirham', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(73,1,N'ARS', N'Argentine Peso', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(74,1,N'AUD', N'Australian Dollar', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(75,1,N'BHD', N'Bahraini Dinar', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,3)
		,(76,1,N'CAD', N'Canadian Dollar', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(77,1,N'CLP', N'Chilean Peso', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,0)
		,(78,1,N'CNY', N'Chinese Yuan Renminbi', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(79,1,N'CRC', N'Costa Rican Colon', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(80,1,N'GTQ', N'Guatemalan Quetzal', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(81,1,N'HKD', N'Hong Kong Dollar', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(82,1,N'JOD', N'Jordanian Dinar', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,3)
		,(83,1,N'JPY', N'Japanese Yen', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,0)
		,(84,1,N'KRW', N'South Korean Won', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,0)
		,(85,1,N'KWD', N'Kuwaiti Dinar', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,3)
		,(86,1,N'KZT', N'Kazakhstani Tenge', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(87,1,N'MXN', N'Mexican Peso', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(88,1,N'NZD', N'New Zealand Dollar', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(89,1,N'PHP', N'Philippine Peso', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(90,1,N'SGD', N'Singapore Dollar', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(91,1,N'TWD', N'New Taiwan Dollar', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,2)
		,(92,1,N'VND', N'Vietnamese Dong', N'0', N'EUR', N'NULL', CAST(1.00 AS Decimal(18, 8)), N'NULL',0,4,0)
)
AS Source ([Id], [CurrencyTypeId], [Code], [Text], [YourPayCurrencyCode], [PayLikeCurrencyCode], [PayLikeDetails], [FXMarkUp], [BitgoSettings], [IsActive], [PaymentTypeAcceptance], [MinorUnits])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [CurrencyTypeId] = Source.[CurrencyTypeId],
			[Code] = Source.[Code],
			[Text] = Source.[Text],
			[YourPayCurrencyCode] = Source.[YourPayCurrencyCode],
			[PayLikeCurrencyCode] = Source.[PayLikeCurrencyCode],
			[PayLikeDetails] = Source.[PayLikeDetails],
			[FXMarkUp] = Source.[FXMarkUp],
			[BitgoSettings] = Source.[BitgoSettings],
			[IsActive] = Source.[IsActive],
			[PaymentTypeAcceptance] = Source.[PaymentTypeAcceptance],
			[MinorUnits] = Source.[MinorUnits]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [CurrencyTypeId], [Code], [Text], [YourPayCurrencyCode], [PayLikeCurrencyCode], [PayLikeDetails], [FXMarkUp], [BitgoSettings], [IsActive], [PaymentTypeAcceptance], [MinorUnits])
VALUES ([Id], [CurrencyTypeId], [Code], [Text], [YourPayCurrencyCode], [PayLikeCurrencyCode], [PayLikeDetails], [FXMarkUp], [BitgoSettings], [IsActive], [PaymentTypeAcceptance], [MinorUnits])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[Currency] OFF

GO
/* *************************************************************** */
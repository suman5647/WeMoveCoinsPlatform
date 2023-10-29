SET IDENTITY_INSERT [dbo].[Account] ON;
GO
MERGE INTO [dbo].[Account] AS Target
USING 
( 
	VALUES  --(todo: currencyId can be removed 3, and put 0--any , 1--fiat, 2--digital)
	(1, N'1010', 1, 3, 1, 0, N'FromAccount', N'Sale of BTC'), 
	(2, N'5900', 0, 0, 0, 0, N'', N'Enlightenment - EUR'),
	(3, N'5950', 1, 26, 2, 0, N'FromAccount', N'BitGo Hotwallet BTC'),
	(4, N'5910', 0, 0, 0, 0, N'', N'Enlightenment - Bitcoin Wallet'),
	(5, N'5970', 0, 0, 0, 0, N'', N''),--obsolete
	(6, N'5980', 0, 0, 0, 0, N'', N'Current Assets in Total'),
	(7, N'5821', 0, 0, 0, 0, N'', N'Fælleskassen DKK'),
	(8, N'5610', 1, 0, 1, 0, N'ToAccount', N'Receivables - Acquirer 1'),
	(9, N'1310', 1, 26, 2, 0, N'ToAccount', N'Cost of Goods Sold BTC'),
	(10, N'1331', 0, 0, 0, 0, N'', N'Verification - Twillio'),
	(11, N'1361', 0, 0, 0, 0, N'', N''),--obsolete
	(12, N'1321', 0, 0, 0, 0, N'', N'Card fee Acquirer 2'),
	(13, N'1341', 0, 0, 0, 0, N'', N'Exchange Transfer Fee'),
	(14, N'1362', 0, 0, 0, 0, N'', N''), --obsolete
	(15, N'6715', 0, 0, 0, 0, N'', N'Payable Commission - Partner 1'),
	(16, N'6716', 0, 0, 0, 0, N'', N'Payable Commission - Partner 2'),
	(17, N'6717', 0, 0, 0, 0, N'', N'Payable Commission - Partner 3'),
	(18, N'1370', 0, 0, 0, 0, N'', N''), --obsolete
	(19, N'5620', 0, 0, 0, 0, N'', N'Receivables - Acquirer 2'),
	(20, N'5822', 1, 3, 1, 0, N'ToAccount', N'Transferwise EUR'),
	(21, N'4485', 0, 0, 0, 0, N'', N''), --obsolete
	(22, N'4486', 0, 0, 0, 0, N'', N''), --obsolete
	(23, N'4487', 0, 0, 0, 0, N'', N''), --obsolete
	(24, N'3720', 0, 0, 0, 0, N'', N''), --obsolete
	(25, N'1020', 2, 26, 1, 0, N'FromAccount', N'Purchase of BTC'),
	(26, N'1312', 2, 12, 2, 0, N'ToAccount', N'Cost of Goods Sold FIAT'),
	(27, N'5822', 2, 3, 2, 0, N'FromAccount', N'Transferwise EUR'),
	(28, N'5950', 2, 26, 1, 0, N'ToAccount', N'BitGo Hotwallet BTC'),
	(29, N'1332', 1, 26, 2, 1, N'ToAccount', N'Miners Fee'),
	(30, N'1340', 1, 26, 2, 2, N'ToAccount', N'BitGo Transfer Fee'),
	(31, N'6850', 1, 12, 1, 0, N'ToAccount', N'Interpersonal Account - Fredrik Grothe-Eberhardt'),
	(32, N'6850', 2, 12, 2, 0, N'FromAccount', N'Interpersonal Account - Fredrik Grothe-Eberhardt'),
	(33, N'5823', 1, 25, 1, 0, N'ToAccount', N'Transferwise GBP'),
	(34, N'5823', 2, 25, 2, 0, N'FromAccount', N'Transferwise GBP'),
	(35, N'1312', 2, 3, 2, 0, N'ToAccount', N'Cost of Goods Sold FIAT'),
	(36, N'1312', 2, 25, 2, 0, N'ToAccount', N'Cost of Goods Sold FIAT'),
	(40, N'1010', 1, 12, 1, 0, N'FromAccount', N'Sale of BTC'),
	(41, N'1010', 1, 25, 1, 0, N'FromAccount', N'Sale of BTC'),
	(42, N'1010', 1, 0, 1, 0, N'FromAccount', N'Sale of BTC')
)					   
AS Source ([Id], [Text],[Type],[Currency],[TransactionType],[ParticularType],[ValueFor],[Description])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text],
			[Type] = Source.[Type],
			[Currency] = Source.[Currency],
			[TransactionType] = Source.[TransactionType],
			[ParticularType] = Source.[ParticularType],
			[ValueFor] = Source.[ValueFor],
			[Description] = Source.[Description]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text],[Type],[Currency],[TransactionType],[ParticularType],[ValueFor],[Description])
VALUES ([Id], [Text],[Type],[Currency],[TransactionType],[ParticularType],[ValueFor],[Description]); 
--WHEN NOT MATCHED BY SOURCE THEN DELETE;
SET IDENTITY_INSERT [dbo].[Account] OFF
GO
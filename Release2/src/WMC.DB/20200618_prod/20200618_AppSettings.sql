/* *************************************************************** */
--SET IDENTITY_INSERT [dbo].[AppSettings] ON;
--GO
MERGE INTO [dbo].[AppSettings] AS Target
USING 
(
	VALUES 
	(N'TwilioTestDetails', N'[{"SiteId":1,"From":"WeMoveCoins","FromNumber" : "+4592451774", "AccountSid" : "ACd347c46bfc5615c707986ce13c4f1123", "AuthToken" : "05431c74327f0e7ef5c35aa557617340", "Message" : "Hi {0}. In order to process your bitcoin order, please verify yourself using the following code: \n\n{1}"},{"SiteId":2,"From":"123bitcoin","FromNumber" : "+4592451774", "AccountSid" : "ACd347c46bfc5615c707986ce13c4f1123", "AuthToken" : "05431c74327f0e7ef5c35aa557617340", "Message" : "Hi {0}. In order to process your bitcoin order, please verify yourself using the following code: \n\n{1}"},{"SiteId":3,"From":"Hafniatrading","FromNumber" : "+4592451774", "AccountSid" : "ACd347c46bfc5615c707986ce13c4f1123", "AuthToken" : "05431c74327f0e7ef5c35aa557617340", "Message" : "Hi {0}. In order to process your bitcoin order, please verify yourself using the following code: \n\n{1}"},{"SiteId":5,"From":"Monni","FromNumber" : "+4592451774", "AccountSid" : "ACd347c46bfc5615c707986ce13c4f1123", "AuthToken" : "05431c74327f0e7ef5c35aa557617340", "Message" : "Hi {0}. In order to process your bitcoin order, please verify yourself using the following code: \n\n{1}"},{"SiteId":21,"From":"Localhost","FromNumber" : "+4592451774", "AccountSid" : "ACd347c46bfc5615c707986ce13c4f1123", "AuthToken" : "05431c74327f0e7ef5c35aa557617340", "Message" : "Hi {0}. In order to process your bitcoin order, please verify yourself using the following code: \n\n{1}"}]', NULL, 0),
	(N'TwilioProdDetails', N'[{"SiteId":1,"From":"Monni","FromNumber" : "+4592451774", "AccountSid" : "ACd347c46bfc5615c707986ce13c4f1123", "AuthToken" : "05431c74327f0e7ef5c35aa557617340", "Message" : "Hi {0}. In order to process your bitcoin order, please verify yourself using the following code: \n\n{1}"},{"SiteId":2,"From":"123bitcoin","FromNumber" : "+4592451774", "AccountSid" : "ACd347c46bfc5615c707986ce13c4f1123", "AuthToken" : "05431c74327f0e7ef5c35aa557617340", "Message" : "Hi {0}. In order to process your bitcoin order, please verify yourself using the following code: \n\n{1}"},{"SiteId":3,"From":"Hafniatrading","FromNumber" : "+4592451774", "AccountSid" : "ACd347c46bfc5615c707986ce13c4f1123", "AuthToken" : "05431c74327f0e7ef5c35aa557617340", "Message" : "Hi {0}. In order to process your bitcoin order, please verify yourself using the following code: \n\n{1}"},{"SiteId":4,"From":"WeMoveCoins","FromNumber" : "+4592451774", "AccountSid" : "ACd347c46bfc5615c707986ce13c4f1123", "AuthToken" : "05431c74327f0e7ef5c35aa557617340", "Message" : "Hi {0}. In order to process your bitcoin order, please verify yourself using the following code: \n\n{1}"}]', NULL, 0),
	(N'TransactionLimits', N'{"DayTransactionLimit":4,"PerTransactionAmountLimit":6750,"DayTransactionAmountLimit":6750,"MonthTransactionAmountLimit":13500}', NULL,0),
	(N'CreditCardLimits', N'{"PerTransactionAmountLimit":500,"DayTransactionAmountLimit":500,"MonthTransactionAmountLimit":1000}', NULL,0)
)
AS Source ([ConfigKey], [ConfigValue], [ConfigDescription], [IsEncrypted])
ON Target.[ConfigKey] = Source.[ConfigKey]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [ConfigValue] = Source.[ConfigValue],
			[ConfigDescription] = Source.[ConfigDescription],
			[IsEncrypted] = Source.[IsEncrypted]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([ConfigKey], [ConfigValue], [ConfigDescription], [IsEncrypted])
VALUES ([ConfigKey], [ConfigValue], [ConfigDescription], [IsEncrypted])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

--SET IDENTITY_INSERT [dbo].[AppSettings] OFF

--GO
/* *************************************************************** */



-- SitePaymentMethodDetails, SMTPServerSettings, CreditCardGatewayName, SellPaymentMethodDetails, TransactionLimits, QuickPayDetails
MERGE INTO [dbo].[AppSettings] AS Target
USING 
(
	VALUES 
	 (N'SitePaymentMethodDetails', N'[{"SiteId":1,"Spread":4,"Methods":[{"Name":"Bank","Fee":0,"Commission":0,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}, {"Name":"CreditCard","Fee":0,"Commission":9.9,"OrderSizeBoundary":{"Min":40,"Max":500}}]}, {"SiteId":2,"Spread":4,"Methods":[{"Name":"Bank","Fee":0,"Commission":0,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}, {"Name":"CreditCard","Fee":0,"Commission":9.9,"OrderSizeBoundary":{"Min":40,"Max":500}}]},{"SiteId":3,"Spread":4,"Methods":[{"Name":"Bank","Fee":0,"Commission":0,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}]},{"SiteId":4,"Spread":4,"Methods":[{"Name":"Bank","Fee":0,"Commission":0,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}, {"Name":"CreditCard","Fee":0,"Commission":9.9,"OrderSizeBoundary":{"Min":40,"Max":500}}]}]', NULL, 0)
	,(N'SMTPServerSettings', N'{"ServerName":"smtp.gmail.com","ServerPort":"465","UserId":"hafniatrading@gmail.com","Password":"i]K2tEeku9L7bHPogcgUZL","Templates":[{"TemplateName":"OrderPaid","TemplateUrl":"~/Sites/{SiteName}/EmailTemplates/PaymentReceived.html","Subject":"Payment confirmation - Transaction pending"},{"TemplateName":"OrderCompleted","TemplateUrl":"~/Sites/{SiteName}/EmailTemplates/OrderCompleted.html","Subject":"Payout Completed"},{"TemplateName":"PaymentInstructions","TemplateUrl":"~/Sites/{SiteName}/EmailTemplates/PaymentInstructions.html","Subject":"Payment Instructions for your order at WeMoveCoins.com"},{"TemplateName":"OrderCancelled","TemplateUrl":"~/Sites/{SiteName}/EmailTemplates/OrderCancelled.html","Subject":"YOUR ORDER {{OrderNumber}} HAS BEEN CANCELLED"},{"TemplateName":"KYCDeclined","TemplateUrl":"~/Sites/{SiteName}/EmailTemplates/KYC_Declined.html","Subject":"YOUR ORDER {{OrderNumber}} HAS BEEN DECLINED"},{"TemplateName":"KYCRequest","TemplateUrl":"~/Sites/{SiteName}/EmailTemplates/KYC_Request.html","Subject":"KYC Request"},{"TemplateName":"RequestKycDocuments","TemplateUrl":"~/Sites/{SiteName}/EmailTemplates/RequestKycDocuments.html","Subject":"Request KYC Documents"}]}', NULL, 0)
	,(N'CreditCardGatewayName', N'QuickPay', N'QuickPay', 0)
	,(N'SellPaymentMethodDetails', N'[{"SiteId":0,"Spread":2,"Methods":[{"Name":"Bank","Fee":0,"Commission":0,"FixedFee":0,"OrderSizeBoundary":{"Min":13.36,"Max":3342.25}}]}]', NULL, 0)
	,(N'TransactionLimits', N'{"DayTransactionLimit":4,"PerTransactionAmountLimit":500,"DayTransactionAmountLimit":500,"MonthTransactionAmountLimit":1000}', NULL, 0)
	,(N'QuickPayDetails',N'[{"SiteId":1,"SiteName":"monni", "MerchantId":106870,"APIAgreementId":381681, "UserAPIKey":"8ec7a842d8ac97185ec2fea3a77de04410bf40dd48e680574d095f51eb898088", "PaymentWindowAgreementId":381680, "PaymentWindowAPIKey":"b722543a7eef9450909e684ed9418596814090108412d8ef7da5ada33984095e ", "MerchantPrivateKey":"d67038bb4953d548f36c550d4ee313f5f7fb99d6cfba510062822af105e82481", "PaymentMethod":"creditcard"}, {"SiteId":4,"SiteName":"wemovecoins","MerchantId":106870,"APIAgreementId":381681, "UserAPIKey":"8ec7a842d8ac97185ec2fea3a77de04410bf40dd48e680574d095f51eb898088","PaymentWindowAgreementId":381683, "PaymentWindowAPIKey":"b722543a7eef9450909e684ed9418596814090108412d8ef7da5ada33984095e ", "MerchantPrivateKey":"d67038bb4953d548f36c550d4ee313f5f7fb99d6cfba510062822af105e82481","PaymentMethod":"creditcard"}, {"SiteId":2,"SiteName":"123bitcoin","MerchantId":106870,"APIAgreementId":381681, "UserAPIKey":"8ec7a842d8ac97185ec2fea3a77de04410bf40dd48e680574d095f51eb898088","PaymentWindowAgreementId":381689, "PaymentWindowAPIKey":"b722543a7eef9450909e684ed9418596814090108412d8ef7da5ada33984095e ","MerchantPrivateKey":"d67038bb4953d548f36c550d4ee313f5f7fb99d6cfba510062822af105e82481","PaymentMethod":"creditcard,mobilepay"}]',NULL, 0)
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
VALUES ([ConfigKey], [ConfigValue], [ConfigDescription], [IsEncrypted]);
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[User] ON;
GO
MERGE INTO [dbo].[User] AS Target
USING 
(
	VALUES 
	(1, 2, 1, NULL, N'LAdzgx6N4xtYiMIVY', N'Fredrik Grothe-Eberhardt', N'FGM', N'+4561300956', 557201, N'fredrik@grothe-eberhardt.dk', NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, CAST(N'2016-06-29T12:45:33.383' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL,NULL,NULL, NULL,NULL,NUll,NULL, NULL,NULL,NULL)
	,(50, 2, 7, NULL, N'Tku@12345@', N'Tarun Upadhyay', N'TU', N'+918553952524', 370677, N'tarun.upadhyay@advayas.com', NULL, NULL, NULL, NULL, NULL, 44, NULL, NULL, NULL, CAST(N'2018-02-06T13:02:55.287' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL,NULL,NULL, NULL,NULL,NUll,NULL, NULL,NULL,NULL)
	,(90, 2, 1, NULL, N'ngaijqkqcq@6!4%P', N'Thorkild Grothe-Møller', N'TGM', N'+4525853002', NULL, N'tgm@blocktech.dk', NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, CAST(N'2016-06-29T12:27:29.240' AS DateTime), 1, N'[{"SiteId":1,"Methods":[{"Name":"CreditCard","DisplayName":null,"Fee":"6.9","FixedFee":null,"OrderSizeBoundary":{"Min":10.0,"Max":1200.0}},{"Name":"Bank","DisplayName":null,"Fee":"3.9","FixedFee":null,"OrderSizeBoundary":{"Min":40.0,"Max":70000.0}}]},{"SiteId":2,"Methods":[{"Name":"CreditCard","DisplayName":null,"Fee":"6.9","FixedFee":null,"OrderSizeBoundary":{"Min":10.0,"Max":1200.0}},{"Name":"Bank","DisplayName":null,"Fee":"3.9","FixedFee":null,"OrderSizeBoundary":{"Min":40.0,"Max":70000.0}}]},{"SiteId":15,"Methods":[{"Name":"CreditCard","DisplayName":null,"Fee":"6.9","FixedFee":null,"OrderSizeBoundary":{"Min":10.0,"Max":1200.0}}]},{"SiteId":16,"Methods":[{"Name":"CreditCard","DisplayName":null,"Fee":"6.9","FixedFee":null,"OrderSizeBoundary":{"Min":10.0,"Max":1200.0}}]},{"SiteId":17,"Methods":[{"Name":"CreditCard","DisplayName":null,"Fee":"6.9","FixedFee":null,"OrderSizeBoundary":{"Min":10.0,"Max":1200.0}}]},{"SiteId":19,"Methods":[{"Name":"CreditCard","DisplayName":null,"Fee":"6.9","FixedFee":null,"OrderSizeBoundary":{"Min":10.0,"Max":1200.0}},{"Name":"Bank","DisplayName":null,"Fee":"3.9","FixedFee":null,"OrderSizeBoundary":{"Min":40.0,"Max":70000.0}}]},{"SiteId":20,"Methods":[{"Name":"CreditCard","DisplayName":null,"Fee":"6.9","FixedFee":null,"OrderSizeBoundary":{"Min":10.0,"Max":1200.0}},{"Name":"Bank","DisplayName":null,"Fee":"3.9","FixedFee":null,"OrderSizeBoundary":{"Min":40.0,"Max":70000.0}}]},{"SiteId":21,"Methods":[{"Name":"CreditCard","DisplayName":null,"Fee":"6.9","FixedFee":null,"OrderSizeBoundary":{"Min":10.0,"Max":1200.0}}]},{"SiteId":0,"Methods":[{"Name":"Bank","DisplayName":null,"Fee":"3.7","FixedFee":8.0,"OrderSizeBoundary":{"Min":10.0,"Max":69999.0}}]}]', NULL, NULL, 24, NULL, NULL,NULL,NULL, NULL,NULL,NUll,NULL, NULL,NULL,NULL),
	(5,2,2, NULL, NULL, 'FaceTec', 'FT', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2020-09-15T11:00:00.000' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL,NULL,NULL, NULL,NULL,NUll,NULL, NULL,NULL,NULL,NULL)
)
AS Source ([Id], [RoleId], [UserType], [Login], [Password], [Fname], [Lname], [Phone], [PhoneVerificationCode], [Email], [Address], [Address2], [Zip], [City], [Region], [CountryId], [LanguageId],[Commission], [KycNote], [Created], [Newsletter], [PaymentMethodDetails], [Predecessor], [Trusted], [TrustedBy], [Blocked], [BlockedBy],[Tier],[TierTwoApproved],[TierTwoApprovedBy],[TierThreeApproved],[TierThreeApprovedBy],[PasswordSalt],[UserRiskLevel],[TransactionLimitsDetails],[SellPaymentMethodDetails],[CreditCardLimitsDetails])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [RoleId] = Source.[RoleId],
			[UserType] = Source.[UserType],
			[Login] = Source.[Login],
			[Password] = Source.[Password],
			[Fname] = Source.[Fname],
			[Lname] = Source.[Lname],
			[Phone] = Source.[Phone],
			[PhoneVerificationCode] = Source.[PhoneVerificationCode],
			[Email] = Source.[Email],
			[Address] = Source.[Address],
			[Address2] = Source.[Address2],
			[Zip] = Source.[Zip],
			[City] = Source.[City],
			[Region] = Source.[Region],
			[CountryId] = Source.[CountryId],
			[LanguageId] = Source.[LanguageId],
			[Commission] = Source.[Commission],
			[KycNote] = Source.[KycNote],
			[Created] = Source.[Created],
			[Newsletter] = Source.[Newsletter],
			[PaymentMethodDetails] = Source.[PaymentMethodDetails],
			[Predecessor] = Source.[Predecessor],
			[Blocked] = Source.[Blocked],
			[BlockedBy] = Source.[BlockedBy],
			[Tier] = Source.[Tier],
			[TierTwoApproved] = Source.[TierTwoApproved],
			[TierTwoApprovedBy] = Source.[TierTwoApprovedBy],
			[TierThreeApproved] = Source.[TierThreeApproved],
		    [TierThreeApprovedBy] = Source.[TierThreeApprovedBy],
			[PasswordSalt]  = Source.[PasswordSalt],
			[UserRiskLevel]  = Source.[UserRiskLevel],
			[TransactionLimitsDetails] =  Source.[Tier],
			[SellPaymentMethodDetails] = Source.[SellPaymentMethodDetails],
			[CreditCardLimitsDetails] = Source.[CreditCardLimitsDetails]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [RoleId], [UserType], [Login], [Password], [Fname], [Lname], [Phone], [PhoneVerificationCode], [Email], [Address], [Address2], [Zip], [City], [Region], [CountryId], [LanguageId], [Commission], [KycNote], [Created], [Newsletter], [PaymentMethodDetails], [Predecessor], [Blocked], [BlockedBy],[Tier],[TierTwoApproved],[TierTwoApprovedBy],[TierThreeApproved],[TierThreeApprovedBy],[PasswordSalt],[UserRiskLevel],[TransactionLimitsDetails],[SellPaymentMethodDetails],[CreditCardLimitsDetails])
VALUES ([Id], [RoleId], [UserType], [Login], [Password], [Fname], [Lname], [Phone], [PhoneVerificationCode], [Email], [Address], [Address2], [Zip], [City], [Region], [CountryId], [LanguageId], [Commission], [KycNote], [Created], [Newsletter], [PaymentMethodDetails], [Predecessor], [Blocked], [BlockedBy],[Tier],[TierTwoApproved],[TierTwoApprovedBy],[TierThreeApproved],[TierThreeApprovedBy],[PasswordSalt],[UserRiskLevel],[TransactionLimitsDetails],[SellPaymentMethodDetails],[CreditCardLimitsDetails])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[User] OFF

GO
/* *************************************************************** */


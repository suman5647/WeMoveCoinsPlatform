/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[User] ON;
GO
MERGE INTO [dbo].[User] AS Target
USING 
(
	VALUES 
	 (10, 2, 1, NULL, N'ngaijqkqcq@6!4%P', N'Thorkild Grothe-Møller', N'TGM', N'+4525853002', NULL, N'tgm@blocktech.dk', NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, GETUTCDATE(), 1, NULL, NULL, NULL, 24, NULL, NULL,0,NUll,NULL, NULL,NULL,NULL,0, NULL,NULL,NULL)
	,(11, 2, 1, NULL, N'LAdzgx6N4xtYiMIVY', N'Fredrik Grothe-Eberhardt', N'FGM', N'+4561300956', 557201, N'fredrik@grothe-eberhardt.dk', NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, GETUTCDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL,0,NULL, NULL,NULL,NUll,NULL, 0,NULL,NULL, NULL)
	,(12, 2, 1, NULL, N'ZkaEREZBVYdHuMugyDoi9sHL', N'Katja Grothe-Eberhardt', N'KGE', N'+4541718429', 442805, N'kge@wemovecoins.com', NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, NULL, GETUTCDATE(), 0, NULL, NULL, NULL, NULL, NULL, NULL,0,NULL, NULL,NULL,NUll,NULL, 0,NULL,NULL, NULL)
	,(51, 2, 1, NULL, N'Compu@ueen', N'Shiju Madamchery', N'SM', N'+919686622751', 654321, N'shiju@blocktech.dk', NULL, NULL, NULL, NULL, NULL, 44, NULL, NULL, NULL, GETUTCDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL,0,NULL, NULL,NULL,NUll,NULL, 0,NULL,NULL, NULL)
	,(52, 2, 7, NULL, N'Tku@12345@', N'Tarun Upadhyay', N'TU', N'+918553952524', 370677, N'tarun@blocktech.dk', NULL, NULL, NULL, NULL, NULL, 44, NULL, NULL, NULL, GETUTCDATE(), NULL, NULL, NULL, NULL, NULL, NULL, NULL,0,NULL, NULL,NULL,NUll,NULL, 0,NULL,NULL, NULL)
	,(5,2,2, NULL, NULL, 'FaceTec', 'FT', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, CAST(N'2020-09-15T11:00:00.000' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL,0,NULL, NULL,NULL,NUll,NULL, 0,NULL,NULL,NULL)
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
			[TransactionLimitsDetails] =  Source.[TransactionLimitsDetails],
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

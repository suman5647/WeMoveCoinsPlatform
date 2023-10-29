SET IDENTITY_INSERT [auth].[User] ON

MERGE INTO [auth].[User] AS Target
USING 
(
	VALUES 
	 (1, 'false', N'System', 1, N'system@wemovecoins.com', N'+00 000000000', N'+Wd2WhEHbZX0DsQ6I+6Xfb6wiNwJCiv4', N'IivkC/toOlY=', N'System', N'', N'User', N'System User, No login previlage')
	,(2, 'false', N'Anonymous', 1, N'anonymous@wemovecoins.com', N'+00 000000000', N'+Wd2WhEHbZX0DsQ6I+6Xfb6wiNwJCiv4', N'IivkC/toOlY=', N'anonymous', N'', N'User', N'Anonymous User, No login previlage')
	,(99, 'true', N'shijum', 4, N'shijuprakasan@live.com', N'+91 9686622751', N'+Wd2WhEHbZX0DsQ6I+6Xfb6wiNwJCiv4', N'IivkC/toOlY=', N'Shiju', N'', N'Madamchery', N'Shiju Madamchery')
)
AS Source ([UserId],[IsApproved],[UserName],[UserRoleID],[Email],[PhoneNumber],[EncryptedPassword],[PasswordSalt],[FirstName],[MiddleName],[LastName],[Comment])
ON Target.[UserId] = Source.[UserId]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [UserName] = Source.[UserName],
			[IsApproved] = Source.[IsApproved],
			[UserRoleID] = Source.[UserRoleID],
			[Email] = Source.[Email],
			[PhoneNumber] = Source.[PhoneNumber],
			[EncryptedPassword] = Source.[EncryptedPassword],
			[PasswordSalt] = Source.[PasswordSalt],
			[FirstName] = Source.[FirstName],
			[MiddleName] = Source.[MiddleName],
			[LastName] = Source.[LastName],
			[Comment] = Source.[Comment]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([UserId],[IsApproved],[UserName],[UserRoleID],[Email],[PhoneNumber],[EncryptedPassword],[PasswordSalt],[FirstName],[MiddleName],[LastName],[Comment])
VALUES ([UserId],[IsApproved],[UserName],[UserRoleID],[Email],[PhoneNumber],[EncryptedPassword],[PasswordSalt],[FirstName],[MiddleName],[LastName],[Comment])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [auth].[User] OFF

GO
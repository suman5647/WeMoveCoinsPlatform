
SET IDENTITY_INSERT [auth].[Application] ON

MERGE INTO [auth].[Application] AS Target
USING 
(
	VALUES (1, N'WeMoveCoins web Application', N'WMC.Web', N'ABCDEFGHIJKLMNOPQRSTUVWXYZ', N'abcdefghijklmnopqrstuvwxyz', N'0123456789', N'!"#$%&''()*+,-./:;<=>?@[\]^_`{|}~', 8, 20, 43200, 5, 15, 8, 200)
)
AS Source ([Id], [Description], [Name], [Password_RequiredParameter1], [Password_RequiredParameter2], [Password_RequiredParameter3], [Password_RequiredParameter4], [Password_RequiredMinimumLength], [Password_RequiredMaximumLength], [Password_ResetTokenExpirationMinutes], [Login_MaximumInvalidAttempts], [Login_UnlockDurationMinutes], [UserName_RequiredMinimumLength], [UserName_RequiredMaximumLength])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Description] = Source.[Description], 
			[Name] = Source.[Name], 
			[Password_RequiredParameter1] = Source.[Password_RequiredParameter1], 
			[Password_RequiredParameter2] = Source.[Password_RequiredParameter2], 
			[Password_RequiredParameter3] = Source.[Password_RequiredParameter3], 
			[Password_RequiredParameter4] = Source.[Password_RequiredParameter4], 
			[Password_RequiredMinimumLength] = Source.[Password_RequiredMinimumLength], 
			[Password_RequiredMaximumLength] = Source.[Password_RequiredMaximumLength], 
			[Password_ResetTokenExpirationMinutes] = Source.[Password_ResetTokenExpirationMinutes], 
			[Login_MaximumInvalidAttempts] = Source.[Login_MaximumInvalidAttempts], 
			[Login_UnlockDurationMinutes] = Source.[Login_UnlockDurationMinutes], 
			[UserName_RequiredMinimumLength] = Source.[UserName_RequiredMinimumLength], 
			[UserName_RequiredMaximumLength] = Source.[UserName_RequiredMaximumLength]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Description], [Name], [Password_RequiredParameter1], [Password_RequiredParameter2], [Password_RequiredParameter3], [Password_RequiredParameter4], [Password_RequiredMinimumLength], [Password_RequiredMaximumLength], [Password_ResetTokenExpirationMinutes], [Login_MaximumInvalidAttempts], [Login_UnlockDurationMinutes], [UserName_RequiredMinimumLength], [UserName_RequiredMaximumLength])
VALUES ([Id], [Description], [Name], [Password_RequiredParameter1], [Password_RequiredParameter2], [Password_RequiredParameter3], [Password_RequiredParameter4], [Password_RequiredMinimumLength], [Password_RequiredMaximumLength], [Password_ResetTokenExpirationMinutes], [Login_MaximumInvalidAttempts], [Login_UnlockDurationMinutes], [UserName_RequiredMinimumLength], [UserName_RequiredMaximumLength])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [auth].[Application] OFF

GO

SET IDENTITY_INSERT [auth].[Role] ON

MERGE INTO [auth].[Role] AS Target
USING 
(
VALUES
   (1, N'system',N'System', 'none') -- System, Anonymous, etc users

  ,(4, N'basic',N'Basic', 'none') -- none/default

  ,(5, N'siteadmin',N'Site Administrator', 'site') -- Site Administrator (access only to own site)
  ,(6, N'accouts',N'Account Handling', 'internal') -- Account Handling

  ,(10, N'administrator',N'Administrator', 'internal') -- Administrator - Order Handling/Compliance Handling
  ,(11, N'orderhandling',N'Order Handling', 'internal') -- Order Handling (evaluate, execute, reject orders)
  ,(12, N'compliancehandling',N'Compliance Handling', 'internal') -- Compliance Handling (taking care of approving/rejecting of ID only)
  
  ,(15, N'businesspartner',N'Business Partner', 'vendor') -- Business Partner (access to dashboard with KPI's only)
  ,(16, N'affiliates',N'Affiliates', 'vendor') -- Affiliates (access to orders (readonly) with specific PartnerId only)
  
  ,(20, N'customer',N'Customer', 'customer')  -- Customer (access to own orders only (readonly))
  ,(21, N'public',N'Public', 'none') -- Public user no use now
)
AS Source (RoleID,RoleCode,RoleName,RoleType)
ON Target.RoleID = Source.RoleID
-- update matched rows
WHEN MATCHED THEN
UPDATE SET RoleCode = Source.RoleCode,
			RoleName = Source.RoleName,
			RoleType = Source.RoleType		
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT (RoleID,RoleCode,RoleName,RoleType)
VALUES (RoleID,RoleCode,RoleName,RoleType)
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [auth].[Role] OFF

GO

SET IDENTITY_INSERT [auth].[Claim] ON

MERGE INTO [auth].[Claim] AS Target
USING 
(
VALUES
  -- Site Admin/Accountent Claims (0 - 99)
   (1, N'WMC_Claims_manageUser', N'Manage Users - Create, Update, Activate, Lock User')
  ,(2, N'WMC_Claims_viewUsers', N'View All Users - View only')
  ,(3, N'WMC_Claims_manageSite', N'Manage Site - On/Off Work')
  ,(4, N'WMC_Claims_managePayment', N'Manage Payment - Preference')
  ,(5, N'WMC_Claims_manageAccount', N'Manage Accounting')
  ,(6,N'WMC_Claims_manageVendor', N'Manage Vendor')
  ,(7,N'WMC_Claims_manageVendortransactions', N'Manage Vendor Transactions')
  ,(8,N'WMC_Claims_viewVendor', N'View Vendor')
  ,(9,N'WMC_Claims_viewVendortransactions', N'View Vendor Transactions')
  -- Internal User Claims (100 - 199)
  ,(100,N'WMC_Claims_viewSiteStatus', N'View Site Status On/Off Work')
  ,(101,N'WMC_Claims_viewPaymentPreference', N'View Payment Preference')

  ,(102,N'WMC_Claims_viewOrders', N'View Customer Order Queue')
  ,(103,N'WMC_Claims_viewCustomerDetails', N'View Customer Details')
  ,(104,N'WMC_Claims_viewCustomerTransactions', N'View Customer Transactions')

  ,(105,N'WMC_Claims_manageOrderQueue', N'Manage Customer Order Queue')
  ,(106,N'WMC_Claims_manageCustomerTransactions', N'Review/Approve Customer Payment Transaction')
  
  ,(107,N'WMC_Claims_viewCustomerKYCs', N'View Customer KYCs')
  ,(108,N'WMC_Claims_manageCustomerKYCs', N'Request/Approve Customer KYCs')
  -- Client Claims (200 - 299)
  ,(200,N'WMC_Claims_dashboardKPIs', N'View Dashboard KPIs')
  ,(250,N'WMC_Claims_viewChanelOrders', N'View chanel specific orders')
  -- Customer, API, App (300 - 399)
  ,(300,N'WMC_Claims_my_profile', N'View Customer Profile')
  ,(301,N'WMC_Claims_my_ransactions', N'View Customer Transactions')
  ,(302,N'WMC_Claims_my_notifications', N'View Customer Notifications')

  ,(303,N'WMC_Claims_buy', N'Buy Crypto Currency')
  ,(304,N'WMC_Claims_sell', N'Sell Crypto Currency')
  ,(350,N'WMC_Claims_Api_viewprice', N'Api access to view current price to buy and sell')
  ,(351,N'WMC_Claims_Api_transactions', N'Api acces to view user transactions')
)
AS Source (ClaimID,ClaimCode,ClaimName)
ON Target.ClaimID = Source.ClaimID
-- update matched rows
WHEN MATCHED THEN
UPDATE SET ClaimCode = Source.ClaimCode,  ClaimName = Source.ClaimName
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT (ClaimID,ClaimCode,ClaimName)
VALUES (ClaimID,ClaimCode,ClaimName)
-- Delete
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [auth].[Claim] OFF

GO

MERGE INTO [auth].[RoleClaim] AS Target
USING 
(
VALUES
-- 05, Site Administrator (access only to own site)
   (5,1)
  ,(5,2)
  ,(5,3)
  ,(5,4)
  ,(5,5)
  ,(5,6)
  ,(5,7)
  ,(5,8)
  ,(5,9)
  ,(5,100)
  ,(5,101)
  ,(5,300)
-- 06, Account Handling
  ,(6,5)
  ,(6,300)
-- 10, Administrator - Order Handling/Compliance Handling
  ,(10,2)
  ,(10,3)
  ,(10,4)
  ,(10,5)
  ,(10,6)
  ,(10,7)
  ,(10,8)
  ,(10,9)
  ,(10,100)
  ,(10,101)
  ,(10,102)
  ,(10,103)
  ,(10,104)
  ,(10,105)
  ,(10,106)
  ,(10,107)
  ,(10,108)
  ,(10,200)
  ,(10,300)
-- 11, Order Handling (evaluate, execute, reject orders)
  ,(11,100)
  ,(11,101)
  ,(11,102)
  ,(11,103)
  ,(11,104)
  ,(11,105)
  ,(11,106)
  ,(11,200)
  ,(11,300)
-- 12, Compliance Handling (taking care of approving/rejecting of ID only)
  ,(12,100)
  ,(12,101)
  ,(12,102)
  ,(12,103)
  ,(12,104)
  ,(12,107)
  ,(12,108)
  ,(12,200)
  ,(12,300)
-- 15, Business Partner (access to dashboard with KPI's only)
  ,(15,200)
  ,(15,300)
-- 16, Affiliates (access to orders (readonly) with specific PartnerId only)
  ,(16,250)
  ,(16,300)
-- 20, Customer (access to own orders only (readonly))
  ,(20,300)
  ,(20,301)
  ,(20,302)
  ,(20,303)
  ,(20,304)
  ,(20,350)
  ,(20,351)
  )
AS Source (RoleID,ClaimID)
ON Target.RoleID = Source.RoleID AND Target.ClaimID = Source.ClaimID
-- update matched rows
WHEN MATCHED THEN
UPDATE SET RoleID = Source.RoleID,  ClaimID = Source.ClaimID
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT (RoleID,ClaimID)
VALUES (RoleID,ClaimID)
-- Delete
WHEN NOT MATCHED BY SOURCE THEN DELETE;

GO

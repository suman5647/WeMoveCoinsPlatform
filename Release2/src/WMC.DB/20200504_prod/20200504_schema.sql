--GO
--SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

--SET NUMERIC_ROUNDABORT OFF;


--GO
--:setvar DatabaseName "app.hafniatrading.com"
--:setvar DefaultFilePrefix "app.hafniatrading.com"
--:setvar DefaultDataPath "C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLDEV\MSSQL\DATA\"
--:setvar DefaultLogPath "C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLDEV\MSSQL\DATA\"

--GO
--:on error exit
--GO
--/*
--Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
--To re-enable the script after enabling SQLCMD mode, execute the following:
--SET NOEXEC OFF; 
--*/
--:setvar __IsSqlCmdEnabled "True"
--GO
--IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
--    BEGIN
--        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
--        SET NOEXEC ON;
--    END


--GO
--USE [$(DatabaseName)];


--GO
PRINT N'Altering [dbo].[Country]...';


GO
ALTER TABLE [dbo].[Country]
    ADD [PaymentGateWaysAccepted] INT CONSTRAINT [DF_Country_PaymentGateWaysAccepted] DEFAULT 0 NOT NULL;


GO
PRINT N'Altering [dbo].[Currency]...';


GO
ALTER TABLE [dbo].[Currency]
    ADD [PaymentTypeAcceptance] INT CONSTRAINT [DF_Currency_PaymentTypeAcceptance] DEFAULT 0 NOT NULL;


GO
PRINT N'Altering [dbo].[Order]...';


GO
ALTER TABLE [dbo].[Order]
    ADD [LockKey]   NVARCHAR (50) NULL,
        [LockUntil] DATETIME      NULL;


GO
PRINT N'Altering [dbo].[User]...';


GO
ALTER TABLE [dbo].[User]
    ADD [SellPaymentMethodDetails] NVARCHAR (MAX) NULL;


GO
PRINT N'Creating [auth].[Claim].[IDX_Auth_Claim_ClaimCode]...';


GO
CREATE NONCLUSTERED INDEX [IDX_Auth_Claim_ClaimCode]
    ON [auth].[Claim]([ClaimCode] ASC);


GO
PRINT N'Creating [auth].[Claim].[IDX_Auth_Claim_ClaimName]...';


GO
CREATE NONCLUSTERED INDEX [IDX_Auth_Claim_ClaimName]
    ON [auth].[Claim]([ClaimName] ASC);


GO
PRINT N'Creating [auth].[Role].[IDX_Auth_Role_RoleCode]...';


GO
CREATE NONCLUSTERED INDEX [IDX_Auth_Role_RoleCode]
    ON [auth].[Role]([RoleCode] ASC);


GO
PRINT N'Creating [auth].[Role].[IDX_Auth_Role_RoleName]...';


GO
CREATE NONCLUSTERED INDEX [IDX_Auth_Role_RoleName]
    ON [auth].[Role]([RoleName] ASC);


GO
PRINT N'Creating [auth].[User].[IDX_Auth_User_Email]...';


GO
CREATE NONCLUSTERED INDEX [IDX_Auth_User_Email]
    ON [auth].[User]([Email] ASC);


GO
PRINT N'Creating [auth].[User].[IDX_Auth_User_UserName]...';


GO
CREATE NONCLUSTERED INDEX [IDX_Auth_User_UserName]
    ON [auth].[User]([UserName] ASC);


GO
PRINT N'Creating [dbo].[DF_User_Tire]...';


GO
ALTER TABLE [dbo].[User]
    ADD CONSTRAINT [DF_User_Tire] DEFAULT 0 FOR [Tier];


GO
PRINT N'Creating [dbo].[FK_KycFile_User]...';


GO
ALTER TABLE [dbo].[KycFile] WITH NOCHECK
    ADD CONSTRAINT [FK_KycFile_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creating [dbo].[FK_KycFile_User_Approved]...';


GO
ALTER TABLE [dbo].[KycFile] WITH NOCHECK
    ADD CONSTRAINT [FK_KycFile_User_Approved] FOREIGN KEY ([ApprovedBy]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creating [dbo].[FK_KycFile_User_Obsolete]...';


GO
ALTER TABLE [dbo].[KycFile] WITH NOCHECK
    ADD CONSTRAINT [FK_KycFile_User_Obsolete] FOREIGN KEY ([ObsoleteBy]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creating [dbo].[FK_KycFile_User_Rejected]...';


GO
ALTER TABLE [dbo].[KycFile] WITH NOCHECK
    ADD CONSTRAINT [FK_KycFile_User_Rejected] FOREIGN KEY ([RejectedBy]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creating [dbo].[FK_Order_User]...';


GO
ALTER TABLE [dbo].[Order] WITH NOCHECK
    ADD CONSTRAINT [FK_Order_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creating [dbo].[FK_Order_User_Approved]...';


GO
ALTER TABLE [dbo].[Order] WITH NOCHECK
    ADD CONSTRAINT [FK_Order_User_Approved] FOREIGN KEY ([ApprovedBy]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creating [dbo].[FK_User_Country]...';


GO
ALTER TABLE [dbo].[User] WITH NOCHECK
    ADD CONSTRAINT [FK_User_Country] FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Country] ([Id]);


GO
PRINT N'Creating [dbo].[FK_User_Language]...';


GO
ALTER TABLE [dbo].[User] WITH NOCHECK
    ADD CONSTRAINT [FK_User_Language] FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language] ([Id]);


GO
PRINT N'Creating [dbo].[FK_User_User_Predecessor]...';


GO
ALTER TABLE [dbo].[User] WITH NOCHECK
    ADD CONSTRAINT [FK_User_User_Predecessor] FOREIGN KEY ([Predecessor]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creating [dbo].[FK_User_User_Trusted]...';


GO
ALTER TABLE [dbo].[User] WITH NOCHECK
    ADD CONSTRAINT [FK_User_User_Trusted] FOREIGN KEY ([TrustedBy]) REFERENCES [dbo].[User] ([Id]);


GO
PRINT N'Creating [dbo].[FK_User_UserRole]...';


GO
ALTER TABLE [dbo].[User] WITH NOCHECK
    ADD CONSTRAINT [FK_User_UserRole] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[UserRole] ([Id]);


GO
PRINT N'Creating [dbo].[FK_User_UserType]...';


GO
ALTER TABLE [dbo].[User] WITH NOCHECK
    ADD CONSTRAINT [FK_User_UserType] FOREIGN KEY ([UserType]) REFERENCES [dbo].[UserType] ([Id]);


GO
PRINT N'Refreshing [dbo].[TransactionDetail]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[TransactionDetail]';


GO
PRINT N'Refreshing [dbo].[TransactionList]...';


GO
EXECUTE sp_refreshsqlmodule N'[dbo].[TransactionList]';


GO
PRINT N'Checking existing data against newly created constraints';


GO

ALTER TABLE [dbo].[KycFile] WITH CHECK CHECK CONSTRAINT [FK_KycFile_User];

ALTER TABLE [dbo].[KycFile] WITH CHECK CHECK CONSTRAINT [FK_KycFile_User_Approved];

ALTER TABLE [dbo].[KycFile] WITH CHECK CHECK CONSTRAINT [FK_KycFile_User_Obsolete];

ALTER TABLE [dbo].[KycFile] WITH CHECK CHECK CONSTRAINT [FK_KycFile_User_Rejected];

ALTER TABLE [dbo].[Order] WITH CHECK CHECK CONSTRAINT [FK_Order_User];

ALTER TABLE [dbo].[Order] WITH CHECK CHECK CONSTRAINT [FK_Order_User_Approved];

ALTER TABLE [dbo].[User] WITH CHECK CHECK CONSTRAINT [FK_User_Country];

ALTER TABLE [dbo].[User] WITH CHECK CHECK CONSTRAINT [FK_User_Language];

ALTER TABLE [dbo].[User] WITH CHECK CHECK CONSTRAINT [FK_User_User_Predecessor];

ALTER TABLE [dbo].[User] WITH CHECK CHECK CONSTRAINT [FK_User_User_Trusted];

ALTER TABLE [dbo].[User] WITH CHECK CHECK CONSTRAINT [FK_User_UserRole];

ALTER TABLE [dbo].[User] WITH CHECK CHECK CONSTRAINT [FK_User_UserType];


GO
PRINT N'Update complete.';


GO

CREATE TABLE [auth].[Application] (
    [Id]                                   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Description]                          NVARCHAR (MAX) NULL,
    [Name]                                 NVARCHAR (MAX) NOT NULL,
    [Password_RequiredParameter1]          NVARCHAR (MAX) NOT NULL,
    [Password_RequiredParameter2]          NVARCHAR (MAX) NOT NULL,
    [Password_RequiredParameter3]          NVARCHAR (MAX) NOT NULL,
    [Password_RequiredParameter4]          NVARCHAR (MAX) NOT NULL,
    [Password_RequiredMinimumLength]       BIGINT   NOT NULL,
    [Password_RequiredMaximumLength]       BIGINT   NOT NULL,
    [Password_ResetTokenExpirationMinutes] BIGINT   NOT NULL,
    [Login_MaximumInvalidAttempts]         BIGINT   NOT NULL,
    [Login_UnlockDurationMinutes]          BIGINT   NOT NULL,
    [UserName_RequiredMinimumLength]       BIGINT   NOT NULL,
    [UserName_RequiredMaximumLength]       BIGINT   NOT NULL,
    CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO
ALTER TABLE [auth].[Application]  ADD CONSTRAINT [DEF_Auth_Application_Password_RequiredParameter1]  DEFAULT 'ABCDEFGHIJKLMNOPQRSTUVWXYZ' FOR [Password_RequiredParameter1];
GO
ALTER TABLE [auth].[Application]  ADD CONSTRAINT [DEF_Auth_Application_Password_RequiredParameter2]  DEFAULT 'abcdefghijklmnopqrstuvwxyz' FOR [Password_RequiredParameter2];
GO
ALTER TABLE [auth].[Application]  ADD CONSTRAINT [DEF_Auth_Application_Password_RequiredParameter3]  DEFAULT '0123456789' FOR [Password_RequiredParameter3];
GO
ALTER TABLE [auth].[Application]  ADD CONSTRAINT [DEF_Auth_Application_Password_RequiredMinimumLength]  DEFAULT 8 FOR [Password_RequiredMinimumLength];
GO
ALTER TABLE [auth].[Application]  ADD CONSTRAINT [DEF_Auth_Application_Password_RequiredMaximumLength]  DEFAULT 15 FOR [Password_RequiredMaximumLength];
GO
ALTER TABLE [auth].[Application]  ADD CONSTRAINT [DEF_Auth_Application_Password_ResetTokenExpirationMinutes]  DEFAULT 43200 FOR [Password_ResetTokenExpirationMinutes];
GO
ALTER TABLE [auth].[Application]  ADD CONSTRAINT [DEF_Auth_Application_Login_MaximumInvalidAttempts]  DEFAULT 5 FOR [Login_MaximumInvalidAttempts];
GO
ALTER TABLE [auth].[Application]  ADD CONSTRAINT [DEF_Auth_Application_Login_UnlockDurationMinutes]  DEFAULT 15 FOR [Login_UnlockDurationMinutes];
GO
ALTER TABLE [auth].[Application]  ADD CONSTRAINT [DEF_Auth_Application_UserName_RequiredMinimumLength]  DEFAULT 8 FOR [UserName_RequiredMinimumLength];
GO
ALTER TABLE [auth].[Application]  ADD CONSTRAINT [DEF_Auth_Application_UserName_RequiredMaximumLength]  DEFAULT 200 FOR [UserName_RequiredMaximumLength];

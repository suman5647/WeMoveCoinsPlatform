CREATE TABLE [auth].[User] (
    [UserId]									BIGINT IDENTITY(1,1)	NOT NULL,
    [UserName]									NVARCHAR (50)			NOT NULL,
    [UserRoleID]								BIGINT					NOT NULL,
    [Email]										NVARCHAR (250)			NOT NULL,
    [PhoneNumber]								NVARCHAR (25)			NULL,
    [EncryptedPassword]							NVARCHAR (MAX)			NOT NULL,
    [PasswordSalt]								NVARCHAR (MAX)			NOT NULL, -- default: true
    [IsApproved]								BIT						NOT NULL, -- default:false
    [IsLockedOut]								BIT						NOT NULL, -- Not used
    [PasswordFailuresSinceLastSuccess]			INT						NOT NULL, -- Not used
    [CreateDate]								DATETIME				NOT NULL, -- default: current date time
    [FirstName]									NVARCHAR (50)			NULL,
    [MiddleName]								NVARCHAR (50)			NULL,
    [LastName]									NVARCHAR (50)			NULL,
    [Comment]									NVARCHAR (MAX)			NULL, -- Not used
    [EmailVerified]								BIT						NULL, -- Not used
    [PhoneVerified]								BIT						NULL, -- Not used
    [LastActivityDate]							DATETIMEOFFSET				NULL, -- Not used
    [LastLockoutDate]							DATETIMEOFFSET				NULL, -- Not used
    [LastLoginDate]								DATETIMEOFFSET				NULL, -- Not used
    [LastPasswordChangedDate]					DATETIMEOFFSET				NULL, -- Not used
    [LastPasswordFailureDate]					DATETIMEOFFSET				NULL, -- Not used
    [LastApprovedDate]							DATETIMEOFFSET				NULL, -- Not used
    [EmailVerifiedDate]							DATETIMEOFFSET				NULL, -- Not used
    [PhoneVerifiedDate]							DATETIMEOFFSET				NULL, -- Not used
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_Auth_User_UserRoleId] FOREIGN KEY ([UserRoleId]) REFERENCES [auth].[Role] ([RoleId]) ON DELETE CASCADE
);
GO
CREATE NONCLUSTERED INDEX [IDX_Auth_User_UserName] ON [auth].[User]([UserName] ASC);
GO
CREATE NONCLUSTERED INDEX [IDX_Auth_User_Email] ON [auth].[User]([Email] ASC);
GO
ALTER TABLE [auth].[User]  ADD CONSTRAINT [DEF_Auth_User_UserRoleID]  DEFAULT 4 FOR [UserRoleID];
GO
ALTER TABLE [auth].[User]  ADD CONSTRAINT [DEF_Auth_User_CreateDate]  DEFAULT GETUTCDATE() FOR [CreateDate];
GO
ALTER TABLE [auth].[User]  ADD CONSTRAINT [DEF_Auth_User_IsApproved]  DEFAULT 'false' FOR [IsApproved];
GO
ALTER TABLE [auth].[User]  ADD CONSTRAINT [DEF_Auth_User_IsLockedOut]  DEFAULT 'false' FOR [IsLockedOut];
GO
ALTER TABLE [auth].[User]  ADD CONSTRAINT [DEF_Auth_User_PasswordFailuresSinceLastSuccess]  DEFAULT 0 FOR [PasswordFailuresSinceLastSuccess];

CREATE TABLE [auth].[Role] (
    [RoleId]					BIGINT IDENTITY(1,1)	NOT NULL,
    [RoleCode]					NVARCHAR(50)			NOT NULL,
    [RoleName]					NVARCHAR(50)			NOT NULL,
    [RoleType]					NVARCHAR(20)			NOT NULL, -- internal/external (none, system, site - are inbuild roles, not te be used as application roles)
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleId] ASC),
    CONSTRAINT [CHK_Auth_Role_RoleType] CHECK ([RoleType]='vendor' OR [RoleType]='customer' OR [RoleType]='internal' OR [RoleType]='site' OR [RoleType]='none')
);
GO
CREATE NONCLUSTERED INDEX [IDX_Auth_Role_RoleCode] ON [auth].[Role]([RoleCode] ASC);
GO
CREATE NONCLUSTERED INDEX [IDX_Auth_Role_RoleName] ON [auth].[Role]([RoleName] ASC);
GO
ALTER TABLE [auth].[Role]  ADD CONSTRAINT [DEF_Auth_Role_RoleType]  DEFAULT ('none') FOR [RoleType];

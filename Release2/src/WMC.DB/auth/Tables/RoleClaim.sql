CREATE TABLE [auth].[RoleClaim] (
    [RoleId]					BIGINT		NOT NULL,
    [ClaimId]					BIGINT		NOT NULL,
    CONSTRAINT [PK_RoleClaim] PRIMARY KEY CLUSTERED ([RoleId], [ClaimId]),
    CONSTRAINT [FK_Auth_RoleClaim_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [auth].[Role] ([RoleId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Auth_RoleClaim_ClaimId] FOREIGN KEY ([ClaimId]) REFERENCES [auth].[Claim] ([ClaimId]) ON DELETE CASCADE
);

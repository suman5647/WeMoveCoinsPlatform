CREATE TABLE [auth].[Claim] (
    [ClaimId]					BIGINT IDENTITY(1,1)	NOT NULL,
    [ClaimCode]					NVARCHAR(50)			NOT NULL,
    [ClaimName]					NVARCHAR(50)			NOT NULL,
    CONSTRAINT [PK_Claim] PRIMARY KEY CLUSTERED ([ClaimId] ASC),
);
GO
CREATE NONCLUSTERED INDEX [IDX_Auth_Claim_ClaimCode] ON [auth].[Claim]([ClaimCode] ASC);
GO
CREATE NONCLUSTERED INDEX [IDX_Auth_Claim_ClaimName] ON [auth].[Claim]([ClaimName] ASC);

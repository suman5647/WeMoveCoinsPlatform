CREATE TABLE [auth].[Token] (
    [TokenId]					BIGINT IDENTITY(1,1)	NOT NULL,
    [UserId]					BIGINT					NOT NULL,
    [JwtToken]					NVARCHAR(max)			NOT NULL,
    [TokenType]					NVARCHAR(max)			NOT NULL,
    [Scope]						NVARCHAR (MAX)			NULL,
    [CreatedOn]					DATETIMEOFFSET				NOT NULL,
    CONSTRAINT [PK_Token] PRIMARY KEY CLUSTERED ([TokenId] ASC),
    CONSTRAINT [FK_Auth_Token_UserId] FOREIGN KEY ([UserId]) REFERENCES [auth].[User] ([UserId]) ON DELETE CASCADE
);
GO
ALTER TABLE [auth].[Token]  ADD CONSTRAINT [DEF_Auth_Token_CreatedOn]  DEFAULT GETUTCDATE() FOR [CreatedOn];

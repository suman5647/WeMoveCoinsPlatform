CREATE TABLE [dbo].[Site] (
    [Id]                 BIGINT            IDENTITY (1, 1) NOT NULL,
    [Text]               NVARCHAR (50)  NOT NULL,
    [Url]                NVARCHAR (50)  NULL,
    [CurrencyId]         BIGINT            NULL,
    [GoogleTagManagerId] NVARCHAR (20)  NULL,
    [SMTPServerSettings] NVARCHAR (MAX) NULL,
    [TrustPilotAddress]  NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Site] PRIMARY KEY CLUSTERED ([Id] ASC)
);




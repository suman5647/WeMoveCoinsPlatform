CREATE TABLE [dbo].[AppSettings] (
    [Id]                BIGINT            IDENTITY (1, 1) NOT NULL,
    [ConfigKey]         NVARCHAR (50)  NOT NULL,
    [ConfigValue]       NVARCHAR (MAX) NOT NULL,
    [ConfigDescription] NVARCHAR (100) NULL,
    [IsEncrypted]		BIT NOT NULL CONSTRAINT [DF_AppSettings_IsEncrypted] DEFAULT 'false', 
    CONSTRAINT [PK_AppSettings] PRIMARY KEY CLUSTERED ([Id] ASC)
);


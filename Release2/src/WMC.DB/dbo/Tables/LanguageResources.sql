CREATE TABLE [dbo].[LanguageResources]  (
    [Id]           BIGINT                IDENTITY (1, 1) NOT NULL,
    [Key]          NVARCHAR (60)      NOT NULL,
    [Value]        NVARCHAR (1000)    NULL,
    [Language]     NVARCHAR (10)      NOT NULL,
    [Sites]        NVARCHAR (200)     NULL,
    [Usages]       NVARCHAR (MAX)     NULL,
    [ValueParams]  NVARCHAR (MAX)     NULL,
    [CreationDate] DATETIME  CONSTRAINT [DF_LanguageResources_CreationDate] DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_LanguageResources] PRIMARY KEY CLUSTERED ([Id] ASC),
    -- CONSTRAINT [UQ_LanguageResources_Language] UNIQUE NONCLUSTERED ([Key] ASC, [Language] ASC, [Sites] ASC),
    -- CONSTRAINT [UQ_LanguageResources_Language2] UNIQUE NONCLUSTERED ([Key] ASC, [Language] ASC, [Sites] ASC),
    UNIQUE NONCLUSTERED ([Key] ASC, [Language] ASC, [Sites] ASC)
);


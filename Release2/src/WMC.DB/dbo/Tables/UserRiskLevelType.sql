CREATE TABLE [dbo].[UserRiskLevelType]
(   [Id]          INT            IDENTITY (0, 1) NOT NULL,
    [Text]        NVARCHAR (30)  NULL,
    CONSTRAINT [PK_UserRiskLevelType] PRIMARY KEY CLUSTERED ([Id] ASC)
)

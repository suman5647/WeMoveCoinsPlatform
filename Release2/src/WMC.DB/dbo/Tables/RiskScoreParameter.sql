CREATE TABLE [dbo].[RiskScoreParameter] (
    [Id]          BIGINT             IDENTITY (1, 1) NOT NULL,
    [Code]        NVARCHAR (20)   NOT NULL,
    [Description] NVARCHAR (MAX)  NULL,
    [Value]       DECIMAL (18, 8) NOT NULL,
    CONSTRAINT [PK_RiskScoreParameter] PRIMARY KEY CLUSTERED ([Id] ASC)
);




CREATE TABLE [dbo].[SanctionsList]
(
	[Id] BIGINT NOT NULL IDENTITY(1,1),
	[Name1] [nvarchar](max) NOT NULL,
	[Name2] [nvarchar](max) NULL,
	[Name3] [nvarchar](max) NULL,
	[Name4] [nvarchar](max) NULL,
	[Name5] [nvarchar](max) NULL,
	[Name6] [nvarchar](max) NULL,
	[DOB] DATETIME NULL,
	[CountryOfResidance] [nvarchar](max) NULL,
	[Summary] [nvarchar](max) NULL,
	[FromSource] [int] NOT NULL,
	CONSTRAINT [PK_Sanction] PRIMARY KEY CLUSTERED ([Id] ASC)
)

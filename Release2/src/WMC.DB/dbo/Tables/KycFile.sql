CREATE TABLE [dbo].[KycFile] (
    [Id]               BIGINT            IDENTITY (1, 1) NOT NULL,
    [UserId]           BIGINT            NOT NULL,
    [Type]             BIGINT            NOT NULL,
    [Note]             NVARCHAR (MAX) NULL,
    [UniqueFilename]   NVARCHAR (200) NOT NULL,
    [OriginalFilename] NVARCHAR (200) NOT NULL,
    [Requested]        DATETIME       NULL,
    [Uploaded]         DATETIME       NULL,
    [Rejected]         DATETIME       NULL,
    [RejectedBy]       BIGINT            NULL,
    [Approved]         DATETIME       NULL,
    [ApprovedBy]       BIGINT            NULL,
    [Obsolete]         DATETIME       NULL,
    [ObsoleteBy]       BIGINT            NULL,
    [SessionID]		   NVARCHAR(MAX)  NULL, 
    [FaceTecStatus]    BIGINT  NULL, 
    CONSTRAINT [PK_KycFile] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_KycFile_User] FOREIGN KEY ([UserId]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_KycFile_User_Rejected] FOREIGN KEY ([RejectedBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_KycFile_User_Approved] FOREIGN KEY ([ApprovedBy]) REFERENCES [dbo].[User] ([Id]),
    -- CONSTRAINT [FK_KycFile_User_Rejected2] FOREIGN KEY ([RejectedBy]) REFERENCES [dbo].[User] ([Id]),
    -- CONSTRAINT [FK_KycFile_User_Approved2] FOREIGN KEY ([ApprovedBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_KycFile_User_Obsolete] FOREIGN KEY ([ObsoleteBy]) REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_KycFile_KycType] FOREIGN KEY ([Type]) REFERENCES [dbo].[KycType] ([Id])
);




PRINT N'Altering [dbo].[User]...';


GO
ALTER TABLE [dbo].[User]
    ADD [CreditCardLimitsDetails] NVARCHAR(MAX) NULL;
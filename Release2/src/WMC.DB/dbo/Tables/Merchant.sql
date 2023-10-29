CREATE TABLE [dbo].[Merchant]
(
    [Id] BIGINT NOT NULL,
    [MerchantName] NVARCHAR(50) NULL, 
    [MerchantCode] NVARCHAR(10) NOT NULL,  
    [MerchantWebhookURL] NVARCHAR(MAX)  NULL
    CONSTRAINT [PK_Merchant] PRIMARY KEY ([MerchantCode] ASC)
);

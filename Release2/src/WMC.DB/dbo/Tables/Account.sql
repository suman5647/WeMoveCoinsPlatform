CREATE TABLE [dbo].[Account] (
    [Id]				BIGINT           IDENTITY (1, 1) NOT NULL,
    [Text]				NVARCHAR (30)	NOT NULL,	-- Account Reference
    [Description]		NVARCHAR (256)	NULL,		-- Account Reference Description
    [Type]				BIGINT			NULL,		-- BUY/SELL  (101-BitcoinMinersFees, 102-BitGoFees)
    [Currency]			BIGINT			NULL,		-- 
    [ValueFor]			NVARCHAR (256)	NULL,		-- FROM ACCOUNT/TO ACCOUNT
    [TransactionType]	INT				NULL,		-- IN/OUT
    [ParticularType]	INT				NULL,		-- 0: NonFee, 1: MinersFee, 2: BitGoFee
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([Id] ASC)
);




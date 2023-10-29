SET IDENTITY_INSERT [dbo].[AuditTrailLevel] ON;
GO
MERGE INTO [dbo].[AuditTrailLevel] AS Target
USING 
(
	VALUES 
	(1, N'Debug')
	,(2, N'Info')
	,(3, N'Warn')
	,(4, N'Error')
	,(5, N'Fatal')
	,(6, N'CreditCardAccepted')
	,(7, N'CreditCardRejected')
	,(8, N'CreditCardTerminated')
)
AS Source ([Id], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text])
VALUES ([Id], [Text]);
--WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[AuditTrailLevel] OFF

GO
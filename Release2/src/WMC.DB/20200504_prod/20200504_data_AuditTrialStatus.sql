SET IDENTITY_INSERT [dbo].[AuditTrailStatus] ON;
GO
MERGE INTO [dbo].[AuditTrailStatus] AS Target
USING 
(
	VALUES 
	(16, N'QuickPay')
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

SET IDENTITY_INSERT [dbo].[AuditTrailStatus] OFF

GO
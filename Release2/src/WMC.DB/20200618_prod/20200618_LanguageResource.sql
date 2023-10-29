MERGE INTO [dbo].[LanguageResources] AS Target
USING 
(
	VALUES 
	(GETUTCDATE(), N'', N'DocumentsHeading', N'In order to complete your registration and accept your order, we kindly ask you to provide the following identity documents', N'Verification',NULL)
	,(GETUTCDATE(), N'da', N'DocumentsHeading', N'For at fuldføre din registrering og acceptere din ordre, beder vi dig om at give følgende identitetsdokumenter', N'Verification',NULL)
	,(GETUTCDATE(), N'de', N'DocumentsHeading', N'Um Ihre Registrierung abzuschließen und Ihre Bestellung anzunehmen, bitten wir Sie, die folgenden Ausweisdokumente vorzulegen', N'Verification',NULL)
	,(GETUTCDATE(), N'es', N'DocumentsHeading', N'Para completar su registro y aceptar su pedido, le rogamos que proporcione los siguientes documentos de identidad.', N'Verification',NULL)
	,(GETUTCDATE(), N'fr', N'DocumentsHeading', N'Afin de finaliser votre inscription et d`accepter votre commande, nous vous prions de bien vouloir fournir les pièces d`identité suivantes', N'Verification',NULL)
    ,(GETUTCDATE(), N'', N'ErrorMessage', N'Please do not upload Corrupted file', N'Verification',NULL)
	,(GETUTCDATE(), N'da', N'ErrorMessage', N'Venligst upload ikke beskadiget fil', N'Verification',NULL)
    ,(GETUTCDATE(), N'de', N'ErrorMessage', N'Bitte laden Sie keine beschädigte Datei hoch', N'Verification',NULL)
    ,(GETUTCDATE(), N'es', N'ErrorMessage', N'Por favor no cargue el archivo dañado', N'Verification',NULL)
	,(GETUTCDATE(), N'fr', N'ErrorMessage', N'Veuillez ne pas télécharger le fichier corrompu', N'Verification',NULL)
	,(GETUTCDATE(), N'', N'Maximumsize', N'.jpg, pdf maximum size of 5 mb', N'Verification',NULL)
	,(GETUTCDATE(), N'da', N'Maximumsize', N'.jpg, pdf maksimal størrelse på 5 mb', N'Verification',NULL)
	,(GETUTCDATE(), N'de', N'Maximumsize', N'..jpg, pdf maximale Größe von 5 MB ', N'Verification',NULL)
	,(GETUTCDATE(), N'es', N'Maximumsize', N'jpg, pdf tamaño máximo de 5 mb', N'Verification',NULL)
	,(GETUTCDATE(), N'fr', N'Maximumsize', N'jpg, pdf taille maximale de 5 Mo', N'Verification',NULL)
)	
AS Source ([CreationDate], [Language], [Key], [Value], [Usages], [ValueParams])
ON Target.[Key] = Source.[Key] AND Target.[Language] = Source.[Language] -- AND Target.[Sites] = Source.[Sites] -- AND Source.[CreationDate] >= Target.[CreationDate]
-- update matched rows
WHEN MATCHED THEN
-- donot update it, just mock
UPDATE SET   [Value] = Source.[Value]
			,[Usages] = Source.[Usages]
			,[ValueParams] = Source.[ValueParams]
-- UPDATE SET  [CreationDate] = Source.[CreationDate]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Key], [Value], [Language], [CreationDate], [Usages], [ValueParams])
VALUES ([Key], [Value], [Language], [CreationDate], [Usages], [ValueParams]);
--WHEN NOT MATCHED BY SOURCE THEN DELETE;

--SET IDENTITY_INSERT [dbo].[LanguageResources] OFF
GO
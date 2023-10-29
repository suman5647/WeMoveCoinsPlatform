/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
--IF EXISTS (SELECT 1	FROM INFORMATION_SCHEMA.COLUMNS	WHERE TABLE_NAME = 'ProductVersionStageSection'	AND COLUMN_NAME = 'IsEnable') 
--BEGIN
--	UPDATE ProductVersionStageSection
--	SET IsEnable = 1
--	WHERE IsEnable IS NULL
--END
--IF EXISTS (SELECT 1	FROM INFORMATION_SCHEMA.COLUMNS	WHERE TABLE_NAME = 'Field'	AND COLUMN_NAME = 'IsMultiValue') 
--BEGIN
--	UPDATE Field
--	SET IsMultiValue = 0
--	WHERE IsMultiValue IS NULL
--END


/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


:r .\test\site_data.sql
:r .\test\auth_factory.sql
:r .\test\currency_data.sql
:r .\test\factory_data.sql
:r .\test\languageresource_data.sql
:r .\test\users_data.sql
:r .\test\auth_users.sql
:r .\test\appsettings_data.sql
:r .\test\coupon_data.sql

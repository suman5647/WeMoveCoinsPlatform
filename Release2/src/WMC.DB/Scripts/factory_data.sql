/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[Country] ON
GO
MERGE INTO [dbo].[Country] AS Target
USING 
(
	VALUES 
	(1, N'DK', N'Denmark', 45, 12, N'99 99 99 99', N'da-DK', CAST(1.00000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(2, N'AL', N'Albania', 355, 42, N'99 999 9999', N'sq-AL', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(3, N'AZ', N'Azerbaijan', 994, 43, N'99 999 9999', N'az-Cyrl-AZ', CAST(0.70000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(4, N'AT', N'Austria', 43, 3, N'699 999 9999', N'de-AT', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(5, N'BY', N'Belarus', 375, 44, N'99 999 9999', N'be-BY', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(6, N'BE', N'Belgium', 32, 3, N'499 999 999', N'nl-BE', CAST(0.70000000 AS Decimal(18, 8)), 0, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(7, N'BA', N'Bosnia & Hercegovina', 387, 45, N'69 999 999', N'hr-BA', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(8, N'BG', N'Bulgaria', 359, 3, N'99 999 9999', N'bg-BG', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 0)
	,(9, N'HR', N'Croatia', 385, 46, N'99 999 9999', N'hr-HR', CAST(0.70000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(10, N'CZ', N'Czech Republic', 420, 47, N'999 999 999', N'cs-CZ', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(11, N'EE', N'Estonia', 372, 3, N'5 999 9999', N'et-EE', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(12, N'FO', N'Faroe Islands', 298, 12, N'999 999', N'fo-FO', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(13, N'FI', N'Finland', 358, 3, N'99 999 9999', N'fi-FI', CAST(1.00000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(14, N'FR', N'France', 33, 3, N'9 99 99 99 99', N'fr-FR', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(15, N'GE', N'Georgia', 981, 48, N'9999 99 99 99', N'ka-GE', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(16, N'DE', N'Germany', 49, 3, N'999 999 9999[9]', N'de-AT', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(17, N'GR', N'Greece', 30, 3, N'699 999 9999', N'el-GR', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(18, N'HU', N'Hungary', 36, 49, N'99 999 999', N'hu-HU', CAST(0.70000000 AS Decimal(18, 8)), 0, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(19, N'IS', N'Iceland', 354, 15, N'999 999 999', N'is-IS', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(20, N'IE', N'Ireland', 353, 3, N'99 999 9999', N'en-IE', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(21, N'IT', N'Italy', 39, 3, N'399 999 9999', N'it-IT', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(22, N'LV', N'Latvia', 371, 3, N'2999 9999', N'lv-LV', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(23, N'LI', N'Liechtenstein', 423, 16, N'799 9999', N'de-LI', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 0)
	,(24, N'LT', N'Lithuania', 370, 3, N'699 999 99', N'lt-LT', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(25, N'LU', N'Luxembourg', 352, 3, N'6 9999 9999', N'de-LU', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(26, N'MC', N'Monaco', 377, 3, N'9999 9999', N'fr-MC', CAST(0.70000000 AS Decimal(18, 8)), 0, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(27, N'MA', N'Montenegro', 382, 3, N'69 999 999', N'ar-MA', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(28, N'NL', N'Netherlands', 31, 3, N'6 9999 9999', N'nl-NL', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(29, N'NO', N'Norway', 47, 19, N'99 99 99 99', N'nb-NO', CAST(1.00000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(30, N'PL', N'Poland', 48, 52, N'999 999 999', N'pl-PL', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(31, N'PT', N'Portugal', 351, 3, N'9 9999 9999', N'pt-PT', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(34, N'SK', N'Slovakia', 421, 3, N'9 99 999 999', N'sk-SK', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(35, N'SI', N'Slovenia', 386, 3, N'99 999 999', N'sl-SI', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(36, N'ES', N'Spain', 34, 3, N'999 999 999', N'es-ES', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(37, N'SE', N'Sweden', 46, 23, N'99 999 9999', N'sv-SE', CAST(1.00000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(38, N'CH', N'Switzerland', 41, 16, N'99 999 9999', N'fr-CH', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(39, N'TR', N'Turkey', 90, 24, N'599 999 9999', N'tr-TR', CAST(0.70000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(41, N'GB', N'United Kingdom', 44, 25, N'999 999 9999', N'en-GB', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(44, N'IN', N'India', 91, 55, N'99 9999 9999', N'en-IN', CAST(0.70000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(45, N'MT', N'Malta', 356, 3, N'9999 9999', N'mt-MT', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(46, N'CY', N'Cyprus', 357, 3, N'9999 9999', N'en-CY', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(47, N'GE', N'Georgia', 995, 3, N'999 99 99 99', N'en-GE', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(48, N'MK', N'Macedonia', 389, 50, N'99 999 999', N'mk-MK', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(49, N'MD', N'Moldova', 373, 51, N'99 999 999', N'en-MD', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(50, N'RS', N'Serbia', 381, 53, N'99 999 999 (9)', N'sr-Cyrl-CS', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(51, N'ZA', N'South Africa', 27, 54, N'99 999 9999', N'en-ZA', CAST(0.70000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(52, N'GL', N'Greenland', 299, 12, N'999 999', N'da-DK', CAST(1.00000000 AS Decimal(18, 8)), 1, NULL, CAST(4.20 AS Decimal(18, 8)), NULL, 0)
	,(53, N'RO', N'Romania', 40, 3, N'999 999 999', N'ro-RO ', CAST(0.70000000 AS Decimal(18, 8)), 0, NULL, CAST(4.20 AS Decimal(18, 8)), NULL, 0)
	,(54, N'GL', N'Greenland', 299, 12, N'999 999', N'da-DK', CAST(1.00000000 AS Decimal(18, 8)), 0, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 7)
	,(55, N'RO', N'Romania', 40, 3, N'999 999 999', N'ro-RO ', CAST(0.70000000 AS Decimal(18, 8)), 0, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 0)
	,(56, N'CF', N'Central African Repu', 236, 3, N'99 99 99 99', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(57, N'CD', N'Democratic Republic ', 243, 3, N'99 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, NULL, NULL, 0)
	,(58, N'VG', N'Virgin Islands Briti', 1284, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(59, N'TC', N'Turks & Caicos Islan', 1649, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(60, N'VC', N'St Vinvent & The Gre', 1784, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(61, N'AF', N'Afghanistan', 93, 3, N'99 999 9999', N'ps-AF', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, NULL, NULL, 0)
	,(62, N'DZ', N'Algeria', 213, 3, N'999 99 99 99', N'ar-DZ', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(63, N'AD', N'Andorra', 376, 3, N'999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(64, N'AO', N'Angola', 244, 3, N'999 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(65, N'AI', N'Anguilla', 1264, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(66, N'AG', N'Antigua & Barbuda', 1268, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(67, N'AR', N'Argentina', 54, 73, N'999 999 9999', N'es-AR', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(68, N'AM', N'Armenia', 374, 3, N'99 999 999', N'hy-AM', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(69, N'AW', N'Aruba', 297, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(70, N'AU', N'Australia', 61, 74, N'4 9999 9999', N'en-AU', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(71, N'BS', N'Bahamas', 1242, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(72, N'BH', N'Bahrain', 973, 75, N'9999 9999', N'ar-BH', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(73, N'BD', N'Bangladesh', 880, 3, N'9999 9999', N'bn-BD', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(74, N'BB', N'Barbados', 1246, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(75, N'BZ', N'Belize', 501, 3, N'999 9999', N'en-BZ', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(76, N'BJ', N'Benin', 229, 3, N'99 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(77, N'BM', N'Bermuda', 1441, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(78, N'BT', N'Bhutan', 975, 3, N'99 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(79, N'BO', N'Bolivia', 591, 3, N'9 999 9999', N'quz-BO', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(80, N'BW', N'Botswana', 267, 3, N'99 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(81, N'BR', N'Brazil', 55, 3, N'99 9 9999 9999', N'pt-BR', CAST(0.10000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(82, N'BN', N'Brunei Darussalam', 673, 3, N'999 9999', N'ms-BN', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(83, N'BF', N'Bukina Faso', 226, 3, N'99 99 99 99', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(84, N'BI', N'Burundi', 257, 3, N'9999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(85, N'KH', N'Cambodia', 855, 3, N'99 999 999', N'km-KH', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(86, N'CM', N'Cameroon', 237, 3, N'9999 99999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(87, N'CA', N'Canada', 1, 76, N'999 999 9999', N'en-CA', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(88, N'CV', N'Cape Verde', 238, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(89, N'KY', N'Cayman Islands', 1345, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(90, N'TC', N'Chad', 235, 3, N'99 99 99 99', N'', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(91, N'CL', N'Chile', 56, 77, N'9 9999 9999', N'arn-CL', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(92, N'CN', N'China', 86, 78, N'999 9999 9999', N'zh-CN', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(93, N'CO', N'Colombia', 57, 3, N'999 999 9999', N'es-CO', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(94, N'KM', N'Comoros', 269, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(95, N'CK', N'Cook Islands', 682, 3, N'99 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(96, N'Cr', N'Costa Rica', 506, 79, N'9999 9999', N'es-CR', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(97, N'CI', N'Cote divoire', 225, 3, N'99 99 99 99', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(98, N'CU', N'Cuba', 53, 3, N'9 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(99, N'DJ', N'Djibouti', 253, 3, N'99 99 99 99', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(100, N'DM', N'Dominica', 1767, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(101, N'DO', N'Dominican Republic', 1809, 3, N'999 9999', N'es-DO', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(102, N'EC', N'Ecuador', 593, 3, N'99 999 9999', N'es-EC', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(103, N'EG', N'Egypt', 20, 3, N'999 999 9999', N'ar-EG', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(104, N'SV', N'El Salvador', 503, 3, N'9999 9999', N'es-SV', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(105, N'GQ', N'Equatorial Guinea', 240, 3, N'999 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(106, N'FK', N'Falkland Islands', 500, 3, N'99 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(107, N'FJ', N'Fiji', 679, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(108, N'GF', N'French Guiana', 594, 3, N'999 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(109, N'PF', N'French Polynesia', 689, 3, N'999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(110, N'GA', N'Gabon', 241, 3, N'9 99 99 99', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(111, N'GM', N'Gambia', 220, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(112, N'GH', N'Ghana', 233, 3, N'99 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, NULL, NULL, 0)
	,(113, N'GI', N'Gibraltar', 350, 3, N'9999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(114, N'GD', N'Grenada', 1473, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(115, N'GP', N'Guadeloupe', 590, 3, N'999 99 99 99', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(116, N'GU', N'Guam', 1671, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(117, N'GT', N'Guatamala', 502, 80, N'9999 9999', N'es-GT', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(118, N'GN', N'Guinea', 224, 3, N'9999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(119, N'GW', N'Guinea-Bissau', 245, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(120, N'GG', N'Gurnsey', 441481, 3, N'99 9999[9]', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(121, N'GY', N'Guyana', 592, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(122, N'HT', N'Haiti', 509, 3, N'9999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(123, N'HN', N'Honduras', 504, 3, N'9999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(124, N'HK', N'Hong Kong', 852, 81, N'9999 9999', N'zh-HK', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(125, N'ID', N'Indonesia', 62, 3, N'99 999 9999', N'id-ID', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(126, N'IR', N'Iran', 98, 3, N'999 999 9999', N'fa-IR', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(127, N'IQ', N'Iraq', 964, 3, N'999 999 9999', N'ar-IQ', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(128, N'IM', N'Isle of Man', 441624, 3, N'99 9999[9]', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(129, N'IL', N'Israel', 972, 3, N'99 999 9999', N'he-IL', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(130, N'JM', N'Jamaica', 1876, 3, N'999 9999', N'en-JM', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(131, N'JP', N'Japan', 81, 83, N'99 9999 9999', N'ja-JP', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(132, N'JE', N'Jersey', 441534, 3, N'99 9999[9]', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(133, N'JO', N'Jordan', 962, 82, N'9 9999 9999', N'ar-JO', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(134, N'KZ', N'Kazakhstan', 7, 86, N'999 999 9999', N'kk-KZ', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(135, N'KE', N'Kenya', 254, 3, N'999 999 999', N'sw-KE', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(136, N'KW', N'Kuwait', 965, 85, N'9999 9999', N'ar-KW', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(137, N'KQ', N'Kyrgyzstan', 996, 3, N'999 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(138, N'LA', N'Laos PDR', 856, 3, N'99 99 999 999', N'lo-LA', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(139, N'LB', N'Lebanon', 961, 3, N'9 999 999', N'ar-LB', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(140, N'LS', N'Lesotho', 266, 3, N'99 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(141, N'LR', N'Liberia', 231, 3, N'9 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(142, N'LY', N'Libya', 218, 3, N'99 999 9999', N'ar-LY', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(143, N'MO', N'Macau', 853, 3, N'9999 9999', N'zh-MO', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(144, N'MG', N'Madagascar', 261, 3, N'99 99 999 99', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(145, N'MW', N'Malawi', 265, 3, N'99 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(146, N'MY', N'Malaysia', 60, 3, N'99 9999 9999', N'en-MY', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(147, N'MV', N'Maldives', 960, 3, N'999 9999', N'dv-MV', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(148, N'ML', N'Mali', 223, 3, N'9999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(149, N'MQ', N'Martinique', 596, 3, N'999 99 99 99', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(150, N'MR', N'Mauritania', 222, 3, N'9999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(151, N'MU', N'Mauritius', 230, 3, N'9999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(152, N'YT', N'Mayotte', 262, 3, N'999 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(153, N'MX', N'Mexico', 52, 87, N'99 9 999 9999', N'es-MX', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(154, N'MN', N'Mongolia', 976, 3, N'9999 9999', N'mn-MN', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(155, N'MA', N'Morocco', 212, 3, N'99 999 9999', N'ar-MA', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(156, N'MZ', N'Mozambique', 258, 3, N'99 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(157, N'MM', N'Myanmar', 95, 3, N'9 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(158, N'NA', N'Namibia', 264, 3, N'99 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(159, N'NR', N'Nauru', 674, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(160, N'NP', N'Nepal', 977, 3, N'99 9999 9999', N'ne-NP', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(161, N'NL', N'Netherlands', 31, 3, N'6 9999 9999', N'nl-NL', CAST(0.70000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00 AS Decimal(18, 8)), NULL, 0)
	,(162, N'NC', N'New Caledonia', 687, 3, N'999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(163, N'NZ', N'New Zealand', 64, 88, N'9999 9999[99]', N'en-NZ', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(164, N'NI', N'Nicaragua', 505, 3, N'9999 9999', N'es-NI', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(165, N'NE', N'Niger', 227, 3, N'99 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL,  0)
	,(166, N'NG', N'Nigeria', 234, 3, N'99 9999 9999', N'ha-Latn-NG', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(167, N'MP', N'Norfolk Island', 6723, 3, N'99 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(168, N'OM', N'Oman', 968, 3, N'9999 9999', N'ar-OM', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(169, N'PK', N'Pakistan', 92, 3, N'999 999 9999', N'ur-PK', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, NULL, NULL, 0)
	,(170, N'PS', N'Palestine', 970, 3, N'99 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(171, N'PA', N'Panama', 507, 3, N'9999 9999', N'es-PA', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, NULL, NULL, 0)
	,(172, N'PG', N'Papua New Guinea', 675, 3, N'999 99 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(173, N'PY', N'Paraguay', 595, 3, N'999 999 999', N'es-PY', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(174, N'PE', N'Peru', 51, 3, N'999 999 999', N'es-PE', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(175, N'PH', N'Philippines', 63, 89, N'999 999 9999', N'en-PH', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(176, N'PR', N'Puerto Rico', 1787, 3, N'999 9999', N'es-PR', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(177, N'PR', N'Puerto Rico', 1939, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(178, N'QA', N'Qatar', 974, 3, N'99 999 999', N'ar-QA', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(179, N'RE', N'Reunion', 262, 3, N'999 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(180, N'RU', N'Russia', 7, 3, N'999 999 9999', N'ru-RU', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(181, N'RW', N'Rwanda', 250, 3, N'999 999 999', N'rw-RW', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(182, N'WS', N'Samoa', 685, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(183, N'SM', N'San Marino', 378, 3, N'999 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(184, N'ST', N'Sao Tome & Principe', 239, 3, N'99 99999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(185, N'SA', N'Saudi Arabia', 966, 3, N'5 9999 9999', N'ar-SA', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(186, N'SN', N'Senegal', 221, 3, N'99 999 9999', N'wo-SN', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(187, N'SC', N'Seychelles', 248, 3, N'9 99 99 99', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(188, N'SL', N'Sierra Leone', 232, 3, N'99 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(189, N'SG', N'Singapore', 65, 90, N'9999 9999', N'en-SG', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(190, N'SB', N'Solomon Islands', 677, 3, N'99 99 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(191, N'SO', N'Somalia', 252, 3, N'99 999 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(192, N'KR', N'South Korea', 82, 84, N'99 9999 9999', N'ko-KR', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(193, N'SS', N'South Sudan', 211, 3, N'9999 99999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(194, N'LK', N'Sri Lanka', 94, 3, N'99 999 9999', N'si-LK', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(195, N'KN', N'St Kitts & Nevis', 1869, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(196, N'LC', N'St Lucia', 1758, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(197, N'SD', N'Sudan', 249, 3, N'99 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(198, N'SR', N'Suriname', 597, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(199, N'SZ', N'Swaziland', 268, 3, N'9999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(200, N'SY', N'Syria', 963, 3, N'99 999 9999', N'syr-SY', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, NULL, NULL, 0)
	,(201, N'TW', N'Taiwan', 886, 91, N'999 999 999', N'zh-TW', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 7)
	,(202, N'TJ', N'Tajikistan', 992, 3, N'999 999 999 ', N'tg-Cyrl-TJ', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(203, N'TZ', N'Tanzania', 255, 3, N'99 999 9999 ', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(204, N'TH', N'Thailand', 66, 3, N'9 9999 9999', N'th-TH', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(205, N'TL', N'Timor-Leste', 670, 3, N'9999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(206, N'TG', N'Togo', 228, 3, N'99 99 99 99', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(207, N'TO', N'Tonga', 676, 3, N'99 999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(208, N'TT', N'Trinidad & Tobago', 1868, 3, N'999 9999', N'en-TT', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(209, N'TN', N'Tunisia', 216, 3, N'99 999 999', N'ar-TN', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(210, N'TM', N'Turkmenistan', 993, 3, N'99 999 999', N'tk-TM', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(211, N'UG', N'Uganda', 256, 3, N'99 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(212, N'UA', N'Ukraine', 380, 3, N'99 999 9999', N'uk-UA', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(213, N'AE', N'United Arab Emirates', 971, 72, N'99 999 9999', N'ar-AE', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(214, N'UY', N'Uruguay', 598, 3, N'9 999 9999', N'es-UY', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(215, N'UZ', N'Uzbekistan', 998, 3, N'99 999 9999', N'uz-Latn-UZ', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(216, N'VU', N'Vanuatu', 678, 3, N'999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(217, N'VE', N'Venezuela', 58, 3, N'999 999 9999', N'es-VE', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(218, N'VN', N'Vietnam', 84, 92, N'999 999 999', N'vi-VN', CAST(0.40000000 AS Decimal(18, 8)), 0, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(219, N'YE', N'Yemen', 967, 3, N'999 999 999', N'ar-YE', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(220, N'ZM', N'Zambia', 260, 3, N'99 999 9999', N'en-US', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(221, N'ZW', N'Zimbabwe', 263, 3, N'99 999 9999', N'en-ZW', CAST(0.40000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(223, N'US', N'United States', 1, 70, N'999 999 9999', N'en-US', CAST(0.00000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(224, N'AN', N'Netherlands Antilles', 599, 3, N'6 9999 9999', N'nl-NL', CAST(0.00000000 AS Decimal(18, 8)), 1, NULL, CAST(3.50 AS Decimal(18, 8)), NULL, 0)
	,(225, N'CG', N'Republic of the Congo', 242, 3, N'6 9999 9999', N'nl-NL', CAST(0.00000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(226, N'ER', N'Eritrea', 291, 3, N'999 999 9999', N'en-US', CAST(0.00000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(227, N'ET', N'Ethiopia', 251, 3, N'6 9999 9999', N'nl-NL', CAST(0.00000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(228, N'KP', N'North Korea', 850, 3, N'6 9999 9999', N'nl-NL', CAST(0.00000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(229, N'MS', N'Montserrat', 1, 3, N'999 999 9999', N'en-US', CAST(0.00000000 AS Decimal(18, 8)), 1, NULL, NULL, NULL, 0)
	,(230, N'XK', N'Kosovo', 383, 3, N'999 999 9999', N'sq-XK', CAST(0.00000000 AS Decimal(18, 8)), 1, NULL, CAST(0.00000000 AS Decimal(18, 8)), NULL, 7)
)
AS Source ([Id], [Code], [Text], [PhoneCode], [CurrencyId], [PhoneNumberStyle], [CultureCode], [TrustValue], [AlphaSupport], [Note], [CardFee], [AddTx], [PaymentGateWaysAccepted])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Code] = Source.[Code],
			[Text] = Source.[Text],
			[PhoneCode] = Source.[PhoneCode],
			[CurrencyId] = Source.[CurrencyId],
			[PhoneNumberStyle] = Source.[PhoneNumberStyle],
			[CultureCode] = Source.[CultureCode],
			[TrustValue] = Source.[TrustValue],
			[AlphaSupport] = Source.[AlphaSupport],
			[Note] = Source.[Note],
			[CardFee] = Source.[CardFee],
			[PaymentGateWaysAccepted] = Source.[PaymentGateWaysAccepted]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Code], [Text], [PhoneCode], [CurrencyId], [PhoneNumberStyle], [CultureCode], [TrustValue], [AlphaSupport], [Note], [CardFee], [PaymentGateWaysAccepted])
VALUES ([Id], [Code], [Text], [PhoneCode], [CurrencyId], [PhoneNumberStyle], [CultureCode], [TrustValue], [AlphaSupport], [Note], [CardFee], [PaymentGateWaysAccepted])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[Country] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[Account] ON;
GO
MERGE INTO [dbo].[Account] AS Target
USING 
( 
	VALUES  --(todo: currencyId can be removed 3, and put 0--any , 1--fiat, 2--digital)
	(1, N'1010', 1, 3, 1, 0, N'FromAccount', N'Sale of BTC'), 
	(40, N'1010', 1, 12, 1, 0, N'FromAccount', N'Sale of BTC'),
	(41, N'1010', 1, 25, 1, 0, N'FromAccount', N'Sale of BTC'),
	(2, N'5900', 0, 0, 0, 0, N'', N'Enlightenment - EUR'),
	(3, N'5950', 1, 26, 2, 0, N'FromAccount', N'BitGo Hotwallet BTC'),
	(4, N'5910', 0, 0, 0, 0, N'', N'Enlightenment - Bitcoin Wallet'),
	(5, N'5970', 0, 0, 0, 0, N'', N''),--obsolete
	(6, N'5980', 0, 0, 0, 0, N'', N'Current Assets in Total'),
	(7, N'5821', 0, 0, 0, 0, N'', N'Fælleskassen DKK'),
	(8, N'5610', 1, 0, 1, 0, N'ToAccount', N'Receivables - Acquirer 1'),
	(9, N'1310', 1, 26, 2, 0, N'ToAccount', N'Cost of Goods Sold BTC'),
	(10, N'1331', 0, 0, 0, 0, N'', N'Verification - Twillio'),
	(11, N'1361', 0, 0, 0, 0, N'', N''),--obsolete
	(12, N'1321', 0, 0, 0, 0, N'', N'Card fee Acquirer 2'),
	(13, N'1341', 0, 0, 0, 0, N'', N'Exchange Transfer Fee'),
	(14, N'1362', 0, 0, 0, 0, N'', N''), --obsolete
	(15, N'6715', 0, 0, 0, 0, N'', N'Payable Commission - Partner 1'),
	(16, N'6716', 0, 0, 0, 0, N'', N'Payable Commission - Partner 2'),
	(17, N'6717', 0, 0, 0, 0, N'', N'Payable Commission - Partner 3'),
	(18, N'1370', 0, 0, 0, 0, N'', N''), --obsolete
	(19, N'5620', 0, 0, 0, 0, N'', N'Receivables - Acquirer 2'),
	(20, N'5822', 1, 3, 1, 0, N'ToAccount', N'Transferwise EUR'),
	(21, N'4485', 0, 0, 0, 0, N'', N''), --obsolete
	(22, N'4486', 0, 0, 0, 0, N'', N''), --obsolete
	(23, N'4487', 0, 0, 0, 0, N'', N''), --obsolete
	(24, N'3720', 0, 0, 0, 0, N'', N''), --obsolete
	(25, N'1020', 2, 26, 1, 0, N'FromAccount', N'Purchase of BTC'),
	(26, N'1312', 2, 12, 2, 0, N'ToAccount', N'Cost of Goods Sold FIAT'),
	(35, N'1312', 2, 3, 2, 0, N'ToAccount', N'Cost of Goods Sold FIAT'),
	(36, N'1312', 2, 25, 2, 0, N'ToAccount', N'Cost of Goods Sold FIAT'),
	(27, N'5822', 2, 3, 2, 0, N'FromAccount', N'Transferwise EUR'),
	(28, N'5950', 2, 26, 1, 0, N'ToAccount', N'BitGo Hotwallet BTC'),
	(29, N'1332', 1, 26, 2, 1, N'ToAccount', N'Miners Fee'),
	(30, N'1340', 1, 26, 2, 2, N'ToAccount', N'BitGo Transfer Fee'),
	(31, N'6850', 1, 12, 1, 0, N'ToAccount', N'Interpersonal Account - Fredrik Grothe-Eberhardt'),
	(43, N'6842', 0, 12, 1, 0, N'ToAccount', N'Interpersonal Account - Fredrik Grothe-Eberhardt'),
	(32, N'6850', 2, 12, 2, 0, N'FromAccount', N'Interpersonal Account - Fredrik Grothe-Eberhardt'),
	(33, N'5823', 1, 25, 1, 0, N'ToAccount', N'Transferwise GBP'),
	(34, N'5823', 2, 25, 2, 0, N'FromAccount', N'Transferwise GBP'),
	(42, N'1010', 1, 0, 1, 0, N'FromAccount', N'Sale of BTC')
)					   
AS Source ([Id], [Text],[Type],[Currency],[TransactionType],[ParticularType],[ValueFor],[Description])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text],
			[Type] = Source.[Type],
			[Currency] = Source.[Currency],
			[TransactionType] = Source.[TransactionType],
			[ParticularType] = Source.[ParticularType],
			[ValueFor] = Source.[ValueFor],
			[Description] = Source.[Description]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text],[Type],[Currency],[TransactionType],[ParticularType],[ValueFor],[Description])
VALUES ([Id], [Text],[Type],[Currency],[TransactionType],[ParticularType],[ValueFor],[Description])
WHEN NOT MATCHED BY SOURCE THEN DELETE;
SET IDENTITY_INSERT [dbo].[Account] OFF
GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[AuditTrailLevel] ON;
GO
MERGE INTO [dbo].[AuditTrailLevel] AS Target
USING 
(
	VALUES 
	(1, N'Debug'),
	(2, N'Info'),
	(3, N'Warn'),
	(4, N'Error'),
	(5, N'Fatal'),
	(6, N'CreditCardAccepted'),
	(7, N'CreditCardRejected'),
	(8, N'CreditCardTerminated')
)
AS Source ([Id], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text])
VALUES ([Id], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[AuditTrailLevel] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[AuditTrailStatus] ON;
GO
MERGE INTO [dbo].[AuditTrailStatus] AS Target
USING 
(
	VALUES 
	(1, N'User Login'),
	(2, N'YourPay'),
	(3, N'Twilio'),
	(4, N'IPInfo'),
	(5, N'BitGo'),
	(6, N'Application Error'),
	(7, N'OrderBook'),
	(8, N'PayLike'),
	(9, N'Payment Error'),
	(10, N'SentEmail'),
	(11, N'TrustLogic'),
	(12, N'TxSercret'),
	(13, N'Admin'),
	(14, N'TrustPay'),
	(15, N'Chainalysis'),
	(16, N'QuickPay'),
	(17, N'FaceTec')
)
AS Source ([Id], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text])
VALUES ([Id], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[AuditTrailStatus] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[KycType] ON;
GO
MERGE INTO [dbo].[KycType] AS Target
USING 
(
	VALUES 
	(1, N'PhotoID'),
	(2, N'ProofOfRecidency'),
	(3, N'CardApproval'),
	(4, N'SelfieID')
)
AS Source ([Id], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text])
VALUES ([Id], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[KycType] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[Language] ON;
GO
MERGE INTO [dbo].[Language] AS Target
USING 
(
	VALUES 
	(1, N'da', N'Dansk')
)
AS Source ([Id], [Code], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Code] = Source.[Code],
			[Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Code], [Text])
VALUES ([Id], [Code], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[Language] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[OrderStatus] ON;
GO
MERGE INTO [dbo].[OrderStatus] AS Target
USING 
(
	VALUES 
	(0, N'Cancel', NULL),
	(1, N'Quoted', NULL),
	(2, N'Paying', NULL),
	(3, N'Paid', NULL),
	(4, N'KYC Approval Pending', NULL),
	(5, N'KYC Approved', NULL),
	(6, N'KYC Declined', NULL),
	(7, N'AML Approval Pending', NULL),
	(8, N'AML Approved', NULL),
	(9, N'AML Declined', NULL),
	(10, N'Sending', NULL),
	(11, N'Payout awaits approval', NULL),
	(12, N'Sending Aborted', N'Sending Aborted (12) happens if payment has been captured but payout has failed. Reset by change status to Released payment (16)'),
	(13, N'Sent', N'Status right after payout but before confirmation-email and pushover'),
	(14, N'Releasing payment', NULL),
	(15, N'Releasing payment Aborted', N'Releasing Payment Aborted (15) happens if the capture is done but payout failed. Reason could be an attempt to make an extra payout. Reset by change status to "Send" (13)'),
	(16, N'Released payment', N'This is the status just before BitGo is activated. Will be the correct status for a restart of an order if Sending Aborted (12)'),
	(17, N'Completed', NULL),
	(18, N'Payout approved', N'This is the status before payment is captured'),
	(19, N'Payment Aborted', N'Happens if order-amount, order-number and error in transction doesn''t match the transacion from PSP'),
	(20, N'Capture Errored', N'For Reset and where Capture was not successfull then change status back to Payout Approved (18).'),
	(21, N'Compliance Officer Approval', NULL),
	(22, N'Customer Response Pending', NULL),
	(23, N'Order Cancelled', NULL),
	(24, N'KYC Decline', NULL),
	(25, N'Release Errored', NULL),
	(26, N'Enhanced Due Diligence', NULL),
	(27, N'Received Crypto Payment', NULL)
)
AS Source ([Id], [Text], [Description])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text],
			[Description] = Source.[Description]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text], [Description])
VALUES ([Id], [Text], [Description])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[OrderStatus] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[OrderType] ON;
GO
MERGE INTO [dbo].[OrderType] AS Target
USING 
(
	VALUES 
	(1, N'Buy'),
	(2, N'Sell'),
	(3, N'Other')
)
AS Source ([Id], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text])
VALUES ([Id], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[OrderType] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[UserRiskLevelType] ON;
GO
MERGE INTO [dbo].[UserRiskLevelType] AS Target
USING 
(
	VALUES 
	(0,N'LowRisk')
	,(1,N'ElevatedRisk')
	,(2,N'HighRisk')
)
AS Source ([Id], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text])
VALUES ([Id], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[UserRiskLevelType] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[PaymentType] ON;
GO
MERGE INTO [dbo].[PaymentType] AS Target
USING 
(
	VALUES 
	(1, N'BTC', N'Bitcoin payment'),
	(2, N'Bank', N'Bank'),
	(3, N'Card', N'Credit card payment')
)
AS Source ([Id], [Code], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Code] = Source.[Code],
			[Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Code], [Text])
VALUES ([Id], [Code], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[PaymentType] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[TransactionMethod] ON;
GO
MERGE INTO [dbo].[TransactionMethod] AS Target
USING 
(
	VALUES 
	(1, N'CreditCard'),
	(2, N'SEPA'),
	(3, N'BTC'),
	(4, N'ETH'),
	(5, N'Trade'),
	(6, N'Bank')
)
AS Source ([Id], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text])
VALUES ([Id], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[TransactionMethod] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[TransactionType] ON;
GO
MERGE INTO [dbo].[TransactionType] AS Target
USING 
(
	VALUES 
	(1, N'Incomming'),
	(2, N'Outgoing'),
	(3, N'Internal')
)
AS Source ([Id], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text])
VALUES ([Id], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[TransactionType] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[UserRole] ON;
GO
MERGE INTO [dbo].[UserRole] AS Target
USING 
(
	VALUES 
	(1, N'Client'),
	(2, N'Admin'),
	(3, N'SiteMgr')
)
AS Source ([Id], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text])
VALUES ([Id], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[UserRole] OFF

GO
/* *************************************************************** */
SET IDENTITY_INSERT [dbo].[UserType] ON;
GO
MERGE INTO [dbo].[UserType] AS Target
USING 
(
	VALUES 
	(1, N'c2b'),
	(2, N'b2b'),
	(3, N'2'),
	(4, N'15'),
	(5, N'16'),
	(6, N'19'),
	(7, N'1')
)
AS Source ([Id], [Text])
ON Target.[Id] = Source.[Id]
-- update matched rows
WHEN MATCHED THEN
UPDATE SET  [Text] = Source.[Text]
-- insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT ([Id], [Text])
VALUES ([Id], [Text])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[UserType] OFF

GO
/* *************************************************************** */


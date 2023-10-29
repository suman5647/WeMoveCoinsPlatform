CREATE VIEW dbo.TransactionList
AS
SELECT dbo.[Transaction].Id, dbo.[Order].Number, dbo.TransactionType.Text, dbo.[Transaction].ExtRef, dbo.[Transaction].Amount, dbo.Currency.Code, dbo.[Order].Rate, dbo.[Order].CommissionProcent, dbo.[Transaction].Completed, 
                  dbo.Account.Id AS FromAccountID, dbo.Account.Text AS FromAccount, Account_1.Id AS ToAccountID, Account_1.Text AS ToAccount, RateBase, RateHome, RateBooks, SiteId, PartnerId
FROM     dbo.[Transaction] INNER JOIN
                  dbo.[Order] ON dbo.[Transaction].OrderId = dbo.[Order].Id INNER JOIN
                  dbo.Currency ON dbo.[Transaction].Currency = dbo.Currency.Id INNER JOIN
                  dbo.Account ON dbo.[Transaction].FromAccount = dbo.Account.Id INNER JOIN
                  dbo.TransactionType ON dbo.[Transaction].Type = dbo.TransactionType.Id INNER JOIN
                  dbo.Account AS Account_1 ON dbo.[Transaction].ToAccount = Account_1.Id
WHERE  (dbo.[Order].Status = 17) AND (dbo.[Transaction].Completed < CONVERT(DATETIME, '2016-09-05 00:00:00', 102))
UNION
SELECT dbo.[Transaction].Id, dbo.[Order].Number, dbo.TransactionType.Text, dbo.[Transaction].ExtRef, dbo.[Transaction].Amount, dbo.Currency.Code, dbo.[Order].Rate, dbo.[Order].CommissionProcent, dbo.AuditTrail.Created AS Completed, 
                  dbo.Account.Id AS FromAccountID, dbo.Account.Text AS FromAccount, Account_1.Id AS ToAccountID, Account_1.Text AS ToAccount, RateBase, RateHome, RateBooks, SiteId, PartnerId
FROM     dbo.[Transaction] INNER JOIN
                  dbo.[Order] ON dbo.[Transaction].OrderId = dbo.[Order].Id INNER JOIN
                  dbo.Currency ON dbo.[Transaction].Currency = dbo.Currency.Id INNER JOIN
                  dbo.Account ON dbo.[Transaction].FromAccount = dbo.Account.Id INNER JOIN
                  dbo.TransactionType ON dbo.[Transaction].Type = dbo.TransactionType.Id INNER JOIN
                  dbo.Account AS Account_1 ON dbo.[Transaction].ToAccount = Account_1.Id LEFT OUTER JOIN
                  dbo.AuditTrail ON dbo.[Order].Id = dbo.AuditTrail.OrderId
WHERE  (dbo.[Order].Status = 17) AND (dbo.AuditTrail.Status = 5) AND (NOT (dbo.AuditTrail.Message LIKE N'Old amountInBTC%')) AND (NOT (dbo.AuditTrail.Message LIKE N'Error sending BTC%')) AND 
                  (NOT (dbo.AuditTrail.Message LIKE N'BitGo session is locked%')) AND (NOT (dbo.AuditTrail.Message LIKE N'Insufficient balance%')) AND (NOT (dbo.AuditTrail.Message LIKE N'BitGo session is Unauthorized%')) AND (NOT (dbo.AuditTrail.Message LIKE N'Chainalysis%'))

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[13] 4[33] 2[12] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4[27] 2[61] 3) )"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2[66] 3) )"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3) )"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 3
   End
   Begin DiagramPane = 
      PaneHidden = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 17
         Width = 284
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 600
         Width = 1500
         Width = 1752
         Width = 2052
         Width = 1620
         Width = 1344
         Width = 1296
         Width = 1200
         Width = 1200
         Width = 60
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 2112
         Alias = 3012
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 6180
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'TransactionList';




GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'TransactionList';


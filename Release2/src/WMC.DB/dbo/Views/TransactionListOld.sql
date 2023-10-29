CREATE VIEW dbo.TransactionListOld
AS
SELECT dbo.[Transaction].Id, dbo.[Order].Number, dbo.TransactionType.Text, dbo.[Transaction].ExtRef, dbo.[Transaction].Amount, dbo.Currency.Code, dbo.[Order].Rate, dbo.[Order].CommissionProcent, 
                  dbo.[Transaction].Completed, dbo.Account.Id AS FromAccountID, dbo.Account.Text AS FromAccount, Account_1.Id AS ToAccountID, Account_1.Text AS ToAccount, dbo.[Order].Status
FROM     dbo.[Transaction] INNER JOIN
                  dbo.[Order] ON dbo.[Transaction].OrderId = dbo.[Order].Id INNER JOIN
                  dbo.Currency ON dbo.[Transaction].Currency = dbo.Currency.Id INNER JOIN
                  dbo.Account ON dbo.[Transaction].FromAccount = dbo.Account.Id INNER JOIN
                  dbo.TransactionType ON dbo.[Transaction].Type = dbo.TransactionType.Id INNER JOIN
                  dbo.Account AS Account_1 ON dbo.[Transaction].ToAccount = Account_1.Id
WHERE  (dbo.[Order].Status = 17)
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'TransactionListOld';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N' 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'TransactionListOld';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
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
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
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
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Transaction"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 242
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Order"
            Begin Extent = 
               Top = 7
               Left = 290
               Bottom = 170
               Right = 516
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Currency"
            Begin Extent = 
               Top = 7
               Left = 564
               Bottom = 170
               Right = 804
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Account"
            Begin Extent = 
               Top = 7
               Left = 852
               Bottom = 126
               Right = 1046
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TransactionType"
            Begin Extent = 
               Top = 7
               Left = 1094
               Bottom = 126
               Right = 1288
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Account_1"
            Begin Extent = 
               Top = 126
               Left = 852
               Bottom = 245
               Right = 1046
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1200
         Width =', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'TransactionListOld';


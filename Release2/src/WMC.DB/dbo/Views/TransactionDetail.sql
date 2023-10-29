CREATE VIEW dbo.TransactionDetail
AS
SELECT dbo.[Transaction].Id AS TransactionId, dbo.TransactionMethod.Text AS TransactionMethod, dbo.[Transaction].ExtRef AS TransactionExtRef, dbo.[Transaction].Amount AS TransactionAmount, 
                  dbo.Currency.Code AS TransactionCurrency, dbo.[Transaction].Info AS TransactionInfo, dbo.[Transaction].Completed AS TransactionCompleted, dbo.Account.Text AS FromAccount, Account_1.Text AS ToAccount, 
                  dbo.[Transaction].Reconsiled, dbo.[Transaction].Exported, dbo.[Order].Number AS OrderNumber, dbo.[Order].Status AS OrderStatus, dbo.PaymentType.Code AS PaymentType, dbo.[Order].CryptoAddress, 
                  dbo.[Order].Type AS OrderType, dbo.[Order].Quoted, '' AS Rate, '' AS QuoteSource, dbo.[Order].Amount AS OrderAmount, Currency_1.Code AS OrderCurrency, dbo.[Order].CommissionProcent, 
                  dbo.[Order].Note AS OrderNote, dbo.[User].Id AS UserId, dbo.[Order].SiteId, dbo.[User].Fname AS UserFirstName, dbo.[User].Lname AS UserLastName, dbo.Country.PhoneCode, dbo.[User].Phone, dbo.[User].Email, 
                  dbo.[User].Created, dbo.[User].Newsletter
FROM     dbo.[Transaction] INNER JOIN
                  dbo.[Order] ON dbo.[Transaction].OrderId = dbo.[Order].Id INNER JOIN
                  dbo.[User] ON dbo.[Order].UserId = dbo.[User].Id INNER JOIN
                  dbo.Currency ON dbo.[Transaction].Currency = dbo.Currency.Id INNER JOIN
                  dbo.PaymentType ON dbo.[Order].PaymentType = dbo.PaymentType.Id INNER JOIN
                  dbo.TransactionMethod ON dbo.[Transaction].MethodId = dbo.TransactionMethod.Id INNER JOIN
                  dbo.Account ON dbo.[Transaction].FromAccount = dbo.Account.Id INNER JOIN
                  dbo.Account AS Account_1 ON dbo.[Transaction].ToAccount = Account_1.Id INNER JOIN
                  dbo.Country ON dbo.[User].CountryId = dbo.Country.Id INNER JOIN
                  dbo.Currency AS Currency_1 ON dbo.[Order].CurrencyId = Currency_1.Id

GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[76] 4[3] 2[2] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1[64] 4[8] 3) )"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4[67] 2[3] 3) )"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1[56] 3) )"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2[40] 3) )"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4[50] 3) )"
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
         Configuration = "(H (1[87] 4) )"
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
      ActivePaneConfig = 5
   End
   Begin DiagramPane = 
      PaneHidden = 
      Begin Origin = 
         Top = -120
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Transaction"
            Begin Extent = 
               Top = 127
               Left = 48
               Bottom = 290
               Right = 242
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Order"
            Begin Extent = 
               Top = 295
               Left = 48
               Bottom = 458
               Right = 274
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "User"
            Begin Extent = 
               Top = 463
               Left = 48
               Bottom = 626
               Right = 295
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Currency"
            Begin Extent = 
               Top = 179
               Left = 1624
               Bottom = 320
               Right = 1818
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PaymentType"
            Begin Extent = 
               Top = 366
               Left = 961
               Bottom = 507
               Right = 1155
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TransactionMethod"
            Begin Extent = 
               Top = 26
               Left = 1628
               Bottom = 145
               Right = 1822
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Account"
            Begin Extent = 
               Top = 360
               Left = 1639
               Bottom = 479
               Right = 1833
     ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'TransactionDetail';




GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'       End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Account_1"
            Begin Extent = 
               Top = 412
               Left = 1387
               Bottom = 531
               Right = 1581
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Country"
            Begin Extent = 
               Top = 496
               Left = 331
               Bottom = 659
               Right = 525
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Currency_1"
            Begin Extent = 
               Top = 525
               Left = 1199
               Bottom = 689
               Right = 1397
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
      Begin ColumnWidths = 34
         Width = 284
         Width = 1236
         Width = 1704
         Width = 2172
         Width = 1716
         Width = 1764
         Width = 1392
         Width = 2052
         Width = 1236
         Width = 1020
         Width = 1044
         Width = 900
         Width = 1284
         Width = 1116
         Width = 1224
         Width = 3612
         Width = 1200
         Width = 1272
         Width = 1308
         Width = 1380
         Width = 1308
         Width = 1320
         Width = 1752
         Width = 1032
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      PaneHidden = 
      Begin ColumnWidths = 11
         Column = 5328
         Alias = 2628
         Table = 2688
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'TransactionDetail';




GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'TransactionDetail';


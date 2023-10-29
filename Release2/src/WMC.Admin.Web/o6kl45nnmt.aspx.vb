
Partial Class o6kl45nnmt
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        LabelApprovePayout.Text = "Approve Payout (" + WMC.StatusRecordCount("Payout awaits approval").ToString + ")"
        Dim nBank As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM  OrderStatus INNER JOIN [Order] ON OrderStatus.Id = [Order].Status INNER JOIN PaymentType ON [Order].PaymentType = PaymentType.Id WHERE  (OrderStatus.Text = N'Quoted') AND (PaymentType.Text = N'Bank')")
        LabelApproveBankPayment.Text = "Approve Bank Payment (" + nBank.ToString + ")"
        If nBank > 0 Then LabelApproveBankPayment.BackColor = Drawing.Color.Yellow

        Try
            Dim nOrderRevenueYesterday As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks) AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 1, GETDATE()), 0)) AND ([Transaction].Completed < DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND ([Transaction].Type = 1)")
            LabelRevenueYesterday.Text = String.Format("{0:n0} DKK", nOrderRevenueYesterday)
        Catch ex As Exception
            LabelRevenueYesterday.Text = "0 DKK"
        End Try

        Try
            Dim nOrderRevenueToday As Decimal = eQuote.ExecuteScalar("SELECT SUM([Order].Amount / [Order].RateHome * [Order].RateBooks) AS AmountDKK FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN TransactionType ON TransactionType.Id = [Transaction].Type WHERE  (NOT ([Transaction].Completed IS NULL)) AND (NOT ([Order].Amount / [Order].RateHome * [Order].RateBooks IS NULL)) AND ([Transaction].Completed >= DATEADD(day, DATEDIFF(day, 0, GETDATE()), 0)) AND ([Transaction].Type = 1)")
            LabelRevenueToday.Text = String.Format("({0:n0} DKK)", nOrderRevenueToday)
        Catch ex As Exception
            LabelRevenueToday.Text = "(0 DKK)"
        End Try

    End Sub
End Class

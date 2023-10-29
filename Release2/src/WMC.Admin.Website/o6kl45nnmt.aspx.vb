Option Strict Off
Imports BitGoSharp
Imports RestSharp
Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.Net
Imports System.IO
Imports WMC.Logic

Partial Class o6kl45nnmt
    Inherits System.Web.UI.Page

    Dim client As BitGoClient
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim bitGoAccessCodeJson As String = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoAccessCode'")
        Dim bitGoAccessCode = SimpleJson.DeserializeObject(bitGoAccessCodeJson)
        client = New BitGoClient(bitGoAccessCode.Environment, bitGoAccessCode.AccessCode)
        LabelApprovePayout.Text = "Approve Payout (" + WMC.StatusRecordCount("Compliance Officer Approval").ToString + ")"
        Dim nBank As Integer = eQuote.ExecuteScalar("SELECT COUNT(*) FROM  OrderStatus INNER JOIN [Order] ON OrderStatus.Id = [Order].Status INNER JOIN PaymentType ON [Order].PaymentType = PaymentType.Id WHERE  (OrderStatus.Text = N'Quoted') AND (PaymentType.Text = N'Bank')")
        LabelApproveBankPayment.Text = "Approve Bank Payment (" + nBank.ToString + ")"
        'If nBank > 0 Then LabelApproveBankPayment.BackColor = Drawing.Color.Yellow

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
        ButtonLockBitGo.Attributes.Add("onClick", "javascript: return confirm('THIS IS FOR ONLY CRITICAL SITUATIONS. ARE YOU SURE YOU WANT TO LOCK?')")

        'WMC.GetTimeDiffMinuttes(eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoUnlocked'"), LabelBitGoUnlocked)
    End Sub

    'Protected Sub BtnUnlock_Click(sender As Object, e As EventArgs) Handles BtnUnlock.Click
    '    Dim response As IRestResponse = client.Unlock(txtOTP.Text, 3600)
    '    Label2.Text = response.StatusCode.ToString
    '    eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = { fn NOW() } WHERE ConfigKey = 'BitGoUnlocked'")
    '    txtOTP.Text = ""
    '    WMC.GetTimeDiffMinuttes(eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoUnlocked'"), LabelBitGoUnlocked)
    'End Sub

    Protected Sub ButtonLockBitGo_Click(sender As Object, e As EventArgs) Handles ButtonLockBitGo.Click
        WMC.LockBitGo()
    End Sub
End Class

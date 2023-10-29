﻿Option Strict Off
Imports BitGoSharp
Imports RestSharp
Imports System
Imports System.Net
Imports System.IO

Partial Class ApproveBank
    Inherits System.Web.UI.Page

    Dim client As BitGoClient
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (IsNumeric(Session("UserId")) And Session("UserRole") = "Admin") Then Response.Redirect("Default.aspx")
        Dim bitGoAccessCodeJson As String = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoAccessCode'")
        Dim bitGoAccessCode = SimpleJson.DeserializeObject(bitGoAccessCodeJson)
        client = New BitGoClient(bitGoAccessCode.Environment, bitGoAccessCode.AccessCode)
    End Sub

    Protected Sub btnUnlock_Click(sender As Object, e As EventArgs) Handles BtnUnlock.Click
        Dim response As IRestResponse = client.Unlock(txtOTP.Text, CInt(DropDownListUnlockPeriode.SelectedValue))
        Label1.Text = response.StatusCode.ToString
    End Sub

    Protected Sub SqlDataSourceOrderChildNote_Updated(sender As Object, e As SqlDataSourceStatusEventArgs) Handles SqlDataSourceOrderChildNote.Updated
        GridViewOrder.SelectedIndex = -1
        GridViewOrder.DataBind()
        GridViewSubOrders.SelectedIndex = -1
        GridViewSubOrders.DataBind()
    End Sub

    Protected Sub GridViewOrder_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewOrder.RowCommand
        If e.CommandName = "Approve" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            If WMCData.IsOrderStatus("Quoted", nLineId) Then
                Dim tid As Object = eQuote.ExecuteScalar("SELECT [Transaction].Id FROM [Transaction] INNER JOIN TransactionMethod ON [Transaction].MethodId = TransactionMethod.Id WHERE  ([Transaction].OrderId = " + nLineId.ToString + ") AND  (TransactionMethod.Text = N'Bank')")
                If IsNumeric(tid) Then
                    Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Payout approved'")
                    eQuote.ExecuteNonQuery("UPDATE [Order] SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
                    eQuote.ExecuteNonQuery("UPDATE [Transaction] SET Completed = { fn NOW() } WHERE (Id = " + tid.ToString + ")")
                    GridViewOrder.SelectedIndex = -1
                    GridViewOrder.DataBind()
                End If
            End If
        ElseIf e.CommandName = "ReviewKYC" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            If WMCData.IsOrderStatus("Quoted", nLineId) Then
                Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Approval Pending'")
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
                Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)
                eQuote.ExecuteNonQuery("INSERT INTO KycFile (UserId, Type, UniqueFilename, OriginalFilename) VALUES (" + nUserId.ToString + ", 1, N'EnforceKYC', N'EnforceKYC')")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "CancelOrder" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Cancel'")
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "KYCDeclined" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            If WMCData.IsOrderStatus("Quoted", nLineId) Then
                Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Decline'")
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "Select" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)
            Dim sPhoneNumber As String = eQuote.ExecuteScalar("SELECT Phone FROM  [User] WHERE Id=" + nUserId.ToString)
            Dim sCrypto As String = ""
            Dim sEmail As String = ""
            Dim sEmailOrder As String = ""
            Dim sIP As String = ""
            Dim sCardNumber As String = ""
            Try
                sCrypto = eQuote.ExecuteScalar("SELECT CryptoAddress FROM [Order] WHERE Id = " + nLineId.ToString)
                LinkButtonCryptoAddressSum.CommandArgument = sCrypto
                sEmail = eQuote.ExecuteScalar("SELECT Email FROM  [User] WHERE Id=" + nUserId.ToString)
                LinkButtonEmailSum.CommandArgument = sEmail
                sEmailOrder = eQuote.ExecuteScalar("SELECT Email FROM [Order] WHERE Id = " + nLineId.ToString)
                sIP = eQuote.ExecuteScalar("SELECT IP FROM [Order] WHERE Id = " + nLineId.ToString)
                LinkButtonIpSum.CommandArgument = sIP
                sCardNumber = eQuote.ExecuteScalar("SELECT CardNumber FROM [Order] WHERE Id = " + nLineId.ToString)
                LinkButtonCreditCard.CommandArgument = sCardNumber
            Catch ex As Exception
            End Try
            Try
                LabelCompletedOrders.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE Status = 17 GROUP BY UserId HAVING UserId = " + nUserId.ToString).ToString
                LabelCompletedOrders.BackColor = Drawing.Color.SpringGreen
            Catch ex As Exception
                LabelCompletedOrders.Text = "0"
                LabelCompletedOrders.BackColor = Drawing.Color.Yellow
            End Try
            Try
                LabelKYCDeclined.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE Status = 6 GROUP BY UserId HAVING UserId = " + nUserId.ToString).ToString
                LabelKYCDeclined.BackColor = Drawing.Color.Red
            Catch ex As Exception
                LabelKYCDeclined.Text = "0"
                LabelKYCDeclined.BackColor = Drawing.Color.SpringGreen
            End Try
            Try
                LabelPendingOrders.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE Status = 1 GROUP BY UserId HAVING UserId = " + nUserId.ToString).ToString
            Catch ex As Exception
                LabelPendingOrders.Text = "0"
            End Try
            Try
                LinkButtonCryptoAddressSum.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (CryptoAddress = N'" + sCrypto + "')").ToString
                LinkButtonCryptoAddressSum.BackColor = Drawing.Color.Yellow
                LinkButtonCryptoAddressSum.Enabled = True
                If CInt(LinkButtonCryptoAddressSum.Text) = 0 Then
                    LinkButtonCryptoAddressSum.Enabled = False
                    LinkButtonCryptoAddressSum.BackColor = Drawing.Color.White
                End If
            Catch ex As Exception
                LinkButtonCryptoAddressSum.Text = "0"
                LinkButtonCryptoAddressSum.BackColor = Drawing.Color.White
                LinkButtonCryptoAddressSum.Enabled = False
            End Try
            Try
                LinkButtonEmailSum.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (Email = N'" + sEmailOrder + "')").ToString
                LinkButtonEmailSum.BackColor = Drawing.Color.Yellow
                LinkButtonEmailSum.Enabled = True
                If CInt(LinkButtonEmailSum.Text) = 0 Then
                    LinkButtonEmailSum.Enabled = False
                    LinkButtonEmailSum.BackColor = Drawing.Color.White
                End If
            Catch ex As Exception
                LinkButtonEmailSum.Text = "0"
                LinkButtonEmailSum.BackColor = Drawing.Color.White
                LinkButtonEmailSum.Enabled = False
            End Try

            Try
                LinkButtonIpSum.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (IP = N'" + sIP + "')").ToString
                LinkButtonIpSum.BackColor = Drawing.Color.Yellow
                LinkButtonIpSum.Enabled = True
                If CInt(LinkButtonIpSum.Text) = 0 Then
                    LinkButtonIpSum.Enabled = False
                    LinkButtonIpSum.BackColor = Drawing.Color.White
                End If
            Catch ex As Exception
                LinkButtonIpSum.Text = "0"
                LinkButtonIpSum.BackColor = Drawing.Color.White
                LinkButtonIpSum.Enabled = False
            End Try

            Try
                LinkButtonCreditCard.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (CardNumber = N'" + sCardNumber + "')").ToString
                LinkButtonCreditCard.BackColor = Drawing.Color.Yellow
                LinkButtonCreditCard.Enabled = True
                If CInt(LinkButtonCreditCard.Text) = 0 Then
                    LinkButtonCreditCard.Enabled = False
                    LinkButtonCreditCard.BackColor = Drawing.Color.White
                End If
            Catch ex As Exception
                LinkButtonCreditCard.Text = "0"
                LinkButtonCreditCard.BackColor = Drawing.Color.White
                LinkButtonCreditCard.Enabled = False
            End Try
        End If
    End Sub

    Protected Sub GridViewOrder_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewOrder.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Edit Or e.Row.RowState = 5 Or e.Row.RowState = 6 Then
                Dim GetMemoLogScript As String = vbNewLine + vbNewLine + String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + vbNewLine + "------" + vbNewLine
                If CType(e.Row.FindControl("TextBoxNote"), TextBox).Text.Length > 0 Then
                    CType(e.Row.FindControl("TextBoxNote"), TextBox).Text = GetMemoLogScript + CType(e.Row.FindControl("TextBoxNote"), TextBox).Text
                Else
                    CType(e.Row.FindControl("TextBoxNote"), TextBox).Text = GetMemoLogScript
                End If
                CType(e.Row.FindControl("TextBoxNote"), TextBox).Focus()
            ElseIf e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                CType(e.Row.FindControl("ButtonApprove"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonApprove"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonReviewKYC"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonReviewKYC"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonCancelOrder"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonCancelOrder"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonKYCDeclined"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonKYCDeclined"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
            End If
        End If
    End Sub

    Protected Sub GridViewUser_DataBound(sender As Object, e As EventArgs) Handles GridViewUser.DataBound
        'https://ACd32f9f403312208a96707ec9356eb2ea:74dfe135a1259269c3a7817ffd9fd6e6@lookups.twilio.com/v1/PhoneNumbers/+447492195433?AddOns=whitepages_pro_phone_rep
    End Sub

    Protected Sub GridViewUser_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewUser.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkTwilio"), HyperLink)
            If lb.NavigateUrl.Length Then
                lb.NavigateUrl = "https://www.google.dk/#q=%2B" + lb.NavigateUrl.ToString + ""
            End If
        End If
    End Sub

    Protected Sub ButtonPanelClose_Click(sender As Object, e As EventArgs) Handles ButtonPanelClose.Click
        PanelKeyIndicators.Visible = False
    End Sub

    Protected Sub LinkButtonCryptoAddressSum_Click(sender As Object, e As EventArgs) Handles LinkButtonCryptoAddressSum.Click
        Dim sql As String = "SELECT [User].Fname, [User].Phone, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN    Currency ON [Order].CurrencyId = Currency.Id INNER JOIN [User] ON [Order].UserId = [User].Id"
        Dim nLineId As Integer = GridViewOrder.SelectedDataKey(0)
        Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)

        PanelKeyIndicators.Visible = True
        GridViewKeyIndicator.DataSource = eQuote.GetDataReader(sql + " WHERE  ([Order].CryptoAddress = N'" + LinkButtonCryptoAddressSum.CommandArgument + "') AND (NOT (UserId = " + nUserId.ToString + ")) ORDER BY [Order].Id DESC")
        GridViewKeyIndicator.DataBind()
    End Sub

    Protected Sub LinkButtonEmailSum_Click(sender As Object, e As EventArgs) Handles LinkButtonEmailSum.Click
        Dim sql As String = "SELECT [User].Fname, [User].Phone, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN    Currency ON [Order].CurrencyId = Currency.Id INNER JOIN [User] ON [Order].UserId = [User].Id"
        Dim nLineId As Integer = GridViewOrder.SelectedDataKey(0)
        Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)

        PanelKeyIndicators.Visible = True
        GridViewKeyIndicator.DataSource = eQuote.GetDataReader(sql + " WHERE  ([Order].Email = N'" + LinkButtonEmailSum.CommandArgument + "') AND (NOT (UserId = " + nUserId.ToString + ")) ORDER BY [Order].Id DESC")
        GridViewKeyIndicator.DataBind()
    End Sub

    Protected Sub LinkButtonIpSum_Click(sender As Object, e As EventArgs) Handles LinkButtonIpSum.Click
        Dim sql As String = "SELECT [User].Fname, [User].Phone, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN    Currency ON [Order].CurrencyId = Currency.Id INNER JOIN [User] ON [Order].UserId = [User].Id"
        Dim nLineId As Integer = GridViewOrder.SelectedDataKey(0)
        Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)

        PanelKeyIndicators.Visible = True
        GridViewKeyIndicator.DataSource = eQuote.GetDataReader(sql + " WHERE  ([Order].IP = N'" + LinkButtonIpSum.CommandArgument + "') AND (NOT (UserId = " + nUserId.ToString + ")) ORDER BY [Order].Id DESC")
        GridViewKeyIndicator.DataBind()
    End Sub

    Protected Sub LinkButtonCreditCard_Click(sender As Object, e As EventArgs) Handles LinkButtonCreditCard.Click
        Dim sql As String = "SELECT [User].Fname, [User].Phone, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN    Currency ON [Order].CurrencyId = Currency.Id INNER JOIN [User] ON [Order].UserId = [User].Id"
        Dim nLineId As Integer = GridViewOrder.SelectedDataKey(0)
        Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)

        PanelKeyIndicators.Visible = True
        GridViewKeyIndicator.DataSource = eQuote.GetDataReader(sql + " WHERE  ([Order].CardNumber = N'" + LinkButtonCreditCard.CommandArgument + "') AND (NOT (UserId = " + nUserId.ToString + ")) ORDER BY [Order].Id DESC")
        GridViewKeyIndicator.DataBind()
    End Sub
End Class

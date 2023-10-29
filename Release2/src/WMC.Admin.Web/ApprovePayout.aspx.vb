Option Strict Off
Imports BitGoSharp
Imports RestSharp
Imports System.Data.SqlClient
Imports System
Imports System.Net
Imports System.IO
Imports DependencyInjectionFor.NET.Contracts

Partial Class ApprovePayout
    Inherits System.Web.UI.Page
    Dim client As BitGoClient
    Public WeightCcOrigin As Integer = 1
    Public WeightPhoneOrigin As Integer = 50
    Public WeightOrderSpan As Integer = 50
    Public WeightCountryCoherence As Integer = 50
    Public WeightDataConsistency As Integer = 30
    Public WeightKycApprove As Integer = 20
    Public WeighAmountSize As Integer = 50

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
        PanelKeyIndicators.Visible = False
        Dim OrderId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
        If e.CommandName = "Approve" Then
            If WMCData.IsOrderStatus("Payout awaits approval", OrderId) Then
                Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Payout approved'")
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")

                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
                PanelNote.Visible = False
                PanelTrust2.Visible = False
            End If
        ElseIf e.CommandName = "ReviewKYC" Then
            If WMCData.IsOrderStatus("Payout awaits approval", OrderId) Then
                Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, [Order].Number, [Order].SiteId, [Order].Amount, Currency.Code, [Order].Quoted, [Order].Name, [Order].Email, [Order].CardNumber, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [User] ON [Order].UserId = [User].Id WHERE [Order].Id =" + OrderId.ToString)
                If dr.HasRows Then
                    While dr.Read()
                        WMCData.SendEmail("OrderComplianceInspection.htm", dr!SiteId, dr!Name, dr!Email, "YOUR ORDER #" + dr!Number + " WILL BE EVALUATED BY OUR COMPLIANCE DEPARTMENT", dr!Number)
                        Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Compliance Officer Approval'")
                        eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                        AddNote(OrderId, "Move for Compliance Review")
                        GridViewOrder.SelectedIndex = -1
                        GridViewOrder.DataBind()
                        PanelNote.Visible = False
                        PanelTrust2.Visible = False
                    End While
                End If
                dr.Close()
            End If
        ElseIf e.CommandName = "ApproveCard" Then
            eQuote.ExecuteNonQuery("UPDATE [Order] SET CardApproved = { fn NOW() } WHERE (Id = " + OrderId.ToString + ")")
        ElseIf e.CommandName = "CancelOrder" Then
            If WMCData.IsOrderStatus("Payout awaits approval", OrderId) Then
                Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Cancel'")
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
                PanelNote.Visible = False
                PanelTrust2.Visible = False
            End If
        ElseIf e.CommandName = "KYCDeclined" Then
            If WMCData.IsOrderStatus("Payout awaits approval", OrderId) Then
                Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Decline'")
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
                PanelNote.Visible = False
                PanelTrust2.Visible = False
            End If            '
        ElseIf e.CommandName = "Select" Then
            PanelNote.Visible = True
            PanelTrust2.Visible = True
            Dim sql As String = "SELECT  [Order].UserId, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN Currency ON [Order].CurrencyId = Currency.Id"
            Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
            Dim sPhoneNumber As String = eQuote.ExecuteScalar("SELECT Phone FROM  [User] WHERE Id=" + nUserId.ToString)
            Dim sCrypto As String = ""
            Dim sEmail As String = ""
            Dim sEmailOrder As String = ""
            Dim sIP As String = ""
            Dim sCardNumber As String = ""

            '--------------Trust Level Calculation

            LabelCompletedOrders.Text = WMCData.GetOrdersByStatus(OrderId, "Completed")  'eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE Status = 17 WHERE UserId = " + nUserId.ToString).ToString
            LabelKycDeclined.Text = WMCData.GetOrdersByStatus(OrderId, "KYC Decline")
            If IsNumeric(LabelKycDeclined.Text) Then If CInt(LabelKycDeclined.Text) > 0 Then LabelKycDeclined.BackColor = Drawing.Color.FromName("#FFA8C5")

            LabelOrderSpan.Text = WMCData.GetOrderSpan(OrderId)
            LabelOrderSumEUR.Text = String.Format("{0:n0}", WMCData.GetOrderSumEUR(OrderId))
            LabelOrderSumTotalEUR.Text = String.Format("{0:n0}", WMCData.GetOrderSumTotalEUR(OrderId))
            LabeltOrderSum30Days.Text = String.Format("{0:n0}", WMCData.GetOrderSum30DaysEUR(OrderId))
            LabelCountryTrustLevel.Text = String.Format("{0:0.0}", WMCData.GetCountryTrustLevel(OrderId)) 'WMCData.GetCountryTrustLevel(OrderId).ToString

            LabelCC_IsUS.Text = WMCData.IsCardUS(OrderId, LabelCC_IsUS)
            LabelCardApproved.Text = WMCData.IsCardApproved(OrderId).ToString
            'LabelCardRejected.Text = WMCData.IsCardRejected(OrderId, LabelCardRejected).ToString

            LabelNumberOfCC.Text = WMCData.GetDistinctUsedCards(OrderId, LabelNumberOfCC)
            LinkButtonCreditCardUsedElsewhere.Text = WMCData.GetCardUsedElsewhere(OrderId, LinkButtonCreditCardUsedElsewhere)
            LinkButtonEmailUsedElsewhere.Text = WMCData.GetEmailUsedElsewhere(OrderId, LinkButtonEmailUsedElsewhere)
            'LinkButtonIpUsedElsewhere.Text = WMCData.GetIpUsedElsewhere(OrderId, LinkButtonIpUsedElsewhere)
            LabelPhoneCC.Text = WMCData.PhoneCardOrigin_Match(OrderId)
            LabelPhoneIP.Text = WMCData.PhoneIP_Match(OrderId)
            'LabelPhoneCulture.Text = WMCData.GetPhoneCulture_Match(OrderId)
            'LabelCcIP.Text = WMCData.GetPhoneCulture_Match(OrderId)
            LabelDistinctNames.Text = WMCData.GetDifferentNameUser(OrderId, LabelDistinctNames)
            LabelDistinctEmails.Text = WMCData.GetDifferentEmailUser(OrderId, LabelDistinctEmails)

            LabelTrustLevelAction.Text = WMCData.TrustLevelCalculation(OrderId, False, LabelTrustLevelAction)

            ButtonRunAutoSettle.CommandArgument = OrderId.ToString


            'PanelTrustLevel.Visible = False
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
                CType(e.Row.FindControl("ButtonApproveCard"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonApproveCard"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
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


    Protected Sub GridViewTransaction_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewTransaction.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Try
                Dim lb As Label = CType(e.Row.FindControl("LabelInfo"), Label)
                Dim lb2 As Label = CType(e.Row.FindControl("LabelBINInfo"), Label)
                Dim bin As String = lb.Text.Replace(" ", "").Substring(0, 6)

                Dim wrGETURL As WebRequest
                wrGETURL = WebRequest.Create("https://binlist.net/csv/" + bin)

                Dim objStream As Stream

                Try
                    objStream = wrGETURL.GetResponse.GetResponseStream()
                    Dim objReader As New StreamReader(objStream)
                    lb2.Text = objReader.ReadLine
                Catch ex As Exception
                    lb2.Text = "no match"
                End Try
            Catch ex As Exception
            End Try
        End If
    End Sub

    Protected Sub ButtonPanelClose_Click(sender As Object, e As EventArgs) Handles ButtonPanelClose.Click
        PanelKeyIndicators.Visible = False
    End Sub

    Protected Sub LinkButtonEmailUsedElsewhere_Click(sender As Object, e As EventArgs) Handles LinkButtonEmailUsedElsewhere.Click
        Dim sql As String = "SELECT [User].Fname, [User].Phone, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN    Currency ON [Order].CurrencyId = Currency.Id INNER JOIN [User] ON [Order].UserId = [User].Id"
        Dim nLineId As Integer = GridViewOrder.SelectedDataKey(0)
        Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)

        PanelKeyIndicators.Visible = True
        GridViewKeyIndicator.DataSource = eQuote.GetDataReader(sql + " WHERE  ([Order].Email = N'" + LinkButtonEmailUsedElsewhere.CommandArgument + "') AND (NOT (UserId = " + nUserId.ToString + ")) ORDER BY [Order].Id DESC")
        GridViewKeyIndicator.DataBind()
    End Sub

    Protected Sub LinkButtonIpUsedElsewhere_Click(sender As Object, e As EventArgs) Handles LinkButtonIpUsedElsewhere.Click
        Dim sql As String = "SELECT [User].Fname, [User].Phone, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN    Currency ON [Order].CurrencyId = Currency.Id INNER JOIN [User] ON [Order].UserId = [User].Id"
        Dim nLineId As Integer = GridViewOrder.SelectedDataKey(0)
        Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)

        PanelKeyIndicators.Visible = True
        GridViewKeyIndicator.DataSource = eQuote.GetDataReader(sql + " WHERE  ([Order].IP = N'" + LinkButtonIpUsedElsewhere.CommandArgument + "') AND (NOT (UserId = " + nUserId.ToString + ")) ORDER BY [Order].Id DESC")
        GridViewKeyIndicator.DataBind()
    End Sub

    Protected Sub LinkButtonCreditCardUsedElsewhere_Click(sender As Object, e As EventArgs) Handles LinkButtonCreditCardUsedElsewhere.Click
        Dim sql As String = "SELECT [User].Fname, [User].Phone, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN    Currency ON [Order].CurrencyId = Currency.Id INNER JOIN [User] ON [Order].UserId = [User].Id"
        Dim nLineId As Integer = GridViewOrder.SelectedDataKey(0)
        Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)

        PanelKeyIndicators.Visible = True
        GridViewKeyIndicator.DataSource = eQuote.GetDataReader(sql + " WHERE  ([Order].CardNumber = N'" + LinkButtonCreditCardUsedElsewhere.CommandArgument + "') AND (NOT (UserId = " + nUserId.ToString + ")) ORDER BY [Order].Id DESC")
        GridViewKeyIndicator.DataBind()
    End Sub

    Protected Sub ButtonAddNote_Click(sender As Object, e As EventArgs) Handles ButtonAddNote.Click
        Dim nLineId As Integer = GridViewOrder.SelectedValue
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + TextBoxNote.Text + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + nLineId.ToString)
        GridViewOrder.DataBind()
        TextBoxNote.Text = ""
    End Sub

    Sub AddNote(id As Integer, note As String)
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + note + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + id.ToString)
    End Sub

    Protected Sub GridViewSubOrders_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewSubOrders.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkIP"), HyperLink)
            If lb.NavigateUrl.Length Then
                lb.NavigateUrl = "http://www.infosniper.net/index.php?ip_address=" + lb.NavigateUrl.ToString + ""
            End If
        End If
    End Sub

    Protected Sub ButtonRunAutoSettle_Click(sender As Object, e As EventArgs) Handles ButtonRunAutoSettle.Click
        WMCData.TrustLevelCalculation(CInt(ButtonRunAutoSettle.CommandArgument), True)
    End Sub
End Class

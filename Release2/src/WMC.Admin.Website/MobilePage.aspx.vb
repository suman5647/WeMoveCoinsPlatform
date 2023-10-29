Imports System.Data.SqlClient
Imports RestSharp
Imports BitGoSharp
Imports WMC.Logic.Models

Partial Class MobilePage
    Inherits System.Web.UI.Page
    Dim client As BitGoClient

    'Protected Sub btnUnlock_Click(sender As Object, e As EventArgs) Handles BtnUnlock.Click
    '    Dim response As IRestResponse = client.Unlock(txtOTP.Text, CInt(DropDownListUnlockPeriode.SelectedValue))

    '    Label1.Text = response.StatusCode.ToString
    '    eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = { fn NOW() } WHERE ConfigKey = 'BitGoUnlocked'")
    'End Sub

    Protected Sub ButtonLogin_Click(sender As Object, e As EventArgs) Handles ButtonLogin.Click
        If LabelCode.Text = "6759" Then Session("UserId") = 24
        If LabelCode.Text = "5759" Then Session("UserId") = 25
        If LabelCode.Text = "1553" Then Session("UserId") = 15921
        If IsNumeric(Session("UserId")) Then
            PanelContent.Visible = True
            PanelLogin.Visible = False
            Session("ComplianceAvailability") = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'ComplianceAvailability'")
            DropDownListComplianceAvailability.SelectedValue = Session("ComplianceAvailability")
            Dim s As New StringBuilder
            s.Append("SELECT UserRole.Text AS UserRole, [User].Id AS UserId,  [User].Id, [User].Fname FROM UserRole INNER JOIN [User] ON UserRole.Id = [User].RoleId WHERE [User].Id = " + Session("UserId").ToString)
            Dim c As New SqlCommand(s.ToString, eQuote.OpenConnection)
            Dim drUser As SqlDataReader = c.ExecuteReader
            LabelWrongPasswordText.Visible = False
            If drUser.Read() Then
                Session("UserName") = drUser!Fname
                Session("UserRole") = drUser!UserRole
            End If
            If Session("UserRole") = "Admin" Then LabelRole.Text = "Administrator" Else LabelRole.Text = "SiteManager"
            LabelMe.Text = Session("UserName")
            LabelCode.Text = ""
            Response.Redirect("Default.aspx")
        Else
            PanelContent.Visible = False
            PanelLogin.Visible = True
            LabelWrongPasswordText.Visible = True
            LabelCode.Text = ""
        End If

    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim bitGoAccessCodeJson As String = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoAccessCode'")
        Dim bitGoAccessCode = WMC.Logic.SettingsManager.GetDefault.Get("BitGoAccessCode", True).GetJsonData(Of BitGoAccessSettings)
        client = New BitGoClient(bitGoAccessCode.Environment, bitGoAccessCode.AccessCode)
        If Not IsPostBack Then
            LabelCode.Text = ""
        End If
        If IsNumeric(Session("UserId")) Then
            PanelContent.Visible = True
            PanelLogin.Visible = False
            If Session("UserRole") = "Admin" Then LabelRole.Text = "Administrator" Else LabelRole.Text = "SiteManager"
            LabelMe.Text = Session("UserName")
            ButtonRunAutoSettle.Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
            ButtonTrustUser.Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")

            If GridViewOrder.SelectedIndex < 0 Then
                PanelNote.Visible = False
                PanelTrustCalculation.Visible = False
            End If
        Else
            PanelContent.Visible = False
            PanelLogin.Visible = True
        End If
    End Sub

    Protected Sub DropDownListComplianceAvailability_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListComplianceAvailability.SelectedIndexChanged
        Session("ComplianceAvailability") = DropDownListComplianceAvailability.SelectedValue
        eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = '" + DropDownListComplianceAvailability.SelectedValue + "' WHERE ConfigKey = 'ComplianceAvailability'")
    End Sub

    Protected Sub LinkButtonLogout_Click(sender As Object, e As EventArgs) Handles LinkButtonLogout.Click
        Session("UserId") = Nothing
        Response.Redirect("Mobilepage.aspx")
    End Sub

    Protected Sub ButtonAddNote_Click(sender As Object, e As EventArgs) Handles ButtonAddNote.Click
        Dim OrderId As Integer = GridViewOrder.SelectedValue
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + TextBoxNote.Text + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + OrderId.ToString)
        GridViewOrder.DataBind()
        TextBoxNote.Text = ""
    End Sub

    Protected Sub GridViewOrder_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewOrder.RowCommand
        Dim OrderId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
        If e.CommandName = "Select" Then
            'PanelNote.Visible = True
            'PanelTrustCalculation.Visible = True
            'Dim sql As String = "SELECT  [Order].UserId, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN Currency ON [Order].CurrencyId = Currency.Id"
            'Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
            'Dim sPhoneNumber As String = eQuote.ExecuteScalar("SELECT Phone FROM  [User] WHERE Id=" + nUserId.ToString)
            'Dim sCrypto As String = ""
            'Dim sEmail As String = ""
            'Dim sEmailOrder As String = ""
            'Dim sIP As String = ""
            'Dim sCardNumber As String = ""
            'Dim CountryTrust As String = WMC.GetCountryTrustLevel(OrderId)
            'Dim SiteTrust As String = WMC.GetSiteTrustLevel(OrderId)
            'Dim TxLimitEUR As Integer = 150
            'Dim CardNumber As String = eQuote.ExecuteScalar("SELECT CardNumber FROM [Order] WHERE Id =" + OrderId.ToString).ToString.Trim

            ''--------------Trust Level Calculation

            'LabelCardApproved.Text = WMC.IsCardApproved(OrderId, LabelCardApproved)
            'LabelTxLimit.Text = CInt(TxLimitEUR * CountryTrust * SiteTrust).ToString
            'LabelCardTotal.Text = CInt(WMC.GetOrderSumTotalEUR(OrderId, CardNumber) + WMC.GetOrderSumEUR(OrderId)).ToString
            'If CInt(WMC.GetOrderSumTotalEUR(OrderId, CardNumber) + WMC.GetOrderSumEUR(OrderId)) > CInt(TxLimitEUR * CountryTrust * SiteTrust) And Not WMC.IsCardApproved(OrderId) Then
            '    LabelCardTotal.BackColor = Drawing.Color.FromName("#FFFF66")
            'End If
            'LabelPhoneVirtual.Text = WMC.IsPhoneVitual(OrderId, LabelPhoneVirtual)
            'LabelPhoneCardMatch.Text = WMC.PhoneCardOrigin_Match(OrderId)
            'LabelPhoneIpMatch.Text = WMC.PhoneIP_Match(OrderId)
            'LinkButtonCreditCardUsedElsewhere.Text = WMC.GetCardUsedElsewhere(OrderId, LinkButtonCreditCardUsedElsewhere)
            'LinkButtonEmailUsedElsewhere.Text = WMC.GetEmailUsedElsewhere(OrderId, LinkButtonEmailUsedElsewhere)
            'LinkButtonIpUsedElsewhere.Text = WMC.GetIpUsedElsewhere(OrderId, LinkButtonIpUsedElsewhere)
            'LabelCardUS.Text = WMC.IsCardUS(OrderId, LabelCardUS)

            'LabelCompleted.Text = WMC.GetOrdersByStatus(OrderId, "Completed", LabelCompleted)
            'LabelOrderSpan.Text = CInt(WMC.GetOrderSpan(OrderId)).ToString

            ''LabelCardSpan.Text = CInt(WMC.GetOrderSpan(OrderId, CardNumber, LabelCardSpan)).ToString
            ''If CInt(WMC.GetOrderSpan(OrderId, CardNumber)) >= 45 And Not WMC.IsCardApproved(OrderId) Then ButtonApproveCard.Visible = True : ButtonApproveCard.CommandArgument = OrderId.ToString Else ButtonApproveCard.Visible = False

            'LabelOrderSUM.Text = String.Format("{0:n0}", WMC.GetOrderSumEUR(OrderId))
            'LabelOrderTotal.Text = String.Format("{0:n0}", WMC.GetOrderSumTotalEUR(OrderId))
            'LabelOrder30Days.Text = String.Format("{0:n0}", WMC.GetOrderSum30DaysEUR(OrderId))

            'LabelUserIsTrusted.Text = WMC.IsUserTrusted(OrderId, LabelUserIsTrusted)
            'LabelCards.Text = CInt(WMC.GetDistinctUsedCards(OrderId, LabelCards)).ToString
            'LabelNames.Text = CInt(WMC.GetDifferentNameUser(OrderId, LabelNames)).ToString
            'LabelEmails.Text = CInt(WMC.GetDifferentEmailUser(OrderId, LabelEmails)).ToString
            'LabelKycApproved.Text = WMC.IsKycApproved(OrderId, LabelKycApproved)
            'LabelKycDeclined.Text = WMC.GetOrdersByStatus(OrderId, "KYC Decline")
            'LabelPhoneCountry.Text = WMC.GetPhoneCountry(OrderId)
            'LabelCardCountry.Text = WMC.GetCardCountry(OrderId)
            'LabelIPCountry.Text = WMC.GetIpCountry(OrderId)

            'Dim riskscore As Decimal = WMC.TrustLevelCalculation(OrderId, LabelTrustMessage)
            'LabelTrustMessage.Text = "RISKSCORE: " + riskscore.ToString + "; " + WMC.TrustCalculationExecutionLabel(riskscore)

            'LabelTrustMessage.Text = LabelTrustMessage.Text + " (" + eQuote.ExecuteScalar("SELECT TOP 1 Message FROM AuditTrail WHERE OrderId = " + OrderId.ToString + " AND Status=" + eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'Admin'").ToString + "") + ")"

        End If
    End Sub

    Protected Sub ButtonRunAutoSettle_Click(sender As Object, e As EventArgs) Handles ButtonRunAutoSettle.Click
        Dim OrderId As Integer = GridViewOrder.SelectedValue
        WMC.TrustCalculationExecution(OrderId, "Payout awaits approval", WMC.TrustLevelCalculation(OrderId))
        GridViewOrder.DataBind()
        GridViewOrder.SelectedIndex = -1
        PanelNote.Visible = False
        PanelTrustCalculation.Visible = False
    End Sub

    Protected Sub ButtonTrustUser_Click(sender As Object, e As EventArgs) Handles ButtonTrustUser.Click
        WMC.TrustUser(GridViewOrder.SelectedValue, Session("UserId"), True)
        GridViewOrder.DataBind()
        GridViewOrder.SelectedIndex = -1
        PanelNote.Visible = False
        PanelTrustCalculation.Visible = False
    End Sub

    'Protected Sub ButtonApproveCard_Click(sender As Object, e As EventArgs) Handles ButtonApproveCard.Click
    '    WMC.ApproveCard(CInt(ButtonApproveCard.CommandArgument))
    '    GridViewOrder.DataBind()
    '    GridViewOrder.SelectedIndex = -1
    '    PanelNote.Visible = False
    '    PanelTrustCalculation.Visible = False
    'End Sub

    Protected Sub ButtonNumber1_Click(sender As Object, e As EventArgs) Handles ButtonNumber1.Click
        LabelCode.Text += "1"
    End Sub

    Protected Sub ButtonNumber2_Click(sender As Object, e As EventArgs) Handles ButtonNumber2.Click
        LabelCode.Text += "2"
    End Sub

    Protected Sub ButtonNumber3_Click(sender As Object, e As EventArgs) Handles ButtonNumber3.Click
        LabelCode.Text += "3"
    End Sub

    Protected Sub ButtonNumber4_Click(sender As Object, e As EventArgs) Handles ButtonNumber4.Click
        LabelCode.Text += "4"
    End Sub

    Protected Sub ButtonNumber5_Click(sender As Object, e As EventArgs) Handles ButtonNumber5.Click
        LabelCode.Text += "5"
    End Sub

    Protected Sub ButtonNumber6_Click(sender As Object, e As EventArgs) Handles ButtonNumber6.Click
        LabelCode.Text += "6"
    End Sub

    Protected Sub ButtonNumber7_Click(sender As Object, e As EventArgs) Handles ButtonNumber7.Click
        LabelCode.Text += "7"
    End Sub

    Protected Sub ButtonNumber8_Click(sender As Object, e As EventArgs) Handles ButtonNumber8.Click
        LabelCode.Text += "8"
    End Sub

    Protected Sub ButtonNumber9_Click(sender As Object, e As EventArgs) Handles ButtonNumber9.Click
        LabelCode.Text += "9"
    End Sub

    Private Function clipboard() As Object
        Throw New NotImplementedException
    End Function

End Class

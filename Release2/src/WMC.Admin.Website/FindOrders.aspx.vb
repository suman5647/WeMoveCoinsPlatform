Option Strict Off
Imports BitGoSharp
Imports RestSharp
Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.Net
Imports System.IO
Imports WMC.Logic
Imports WMC.Logic.Models

<WMC.PageAuth(True, "Admin")>
Partial Class ApprovePayout
    Inherits WMC.SecurePage

    Dim client As BitGoClient
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'If Not (IsNumeric(Session("UserId")) And (Session("UserRole") = "Admin" Or Session("UserRole") = "SiteMgr")) Then Response.Redirect("Default.aspx")
        Dim bitGoAccessCodeJson As String = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoAccessCode'")
        Dim bitGoAccessCode = WMC.Logic.SettingsManager.GetDefault.Get("BitGoAccessCode", True).GetJsonData(Of BitGoAccessSettings)
        client = New BitGoClient(bitGoAccessCode.Environment, bitGoAccessCode.AccessCode)

        If Session("UserRole") = "SiteMgr" Then
            'GridViewOrder.Columns(10).Visible = False
            'GridViewOrder.Columns(11).Visible = False
            'GridViewOrder.Columns(12).Visible = False
            'GridViewOrder.Columns(14).Visible = False
            'GridViewUser.Columns(8).Visible = False
            'GridViewUser.Columns(9).Visible = False
            PanelUploadKycDoc.Visible = False
        End If
        Try
            Dim phone = Request.QueryString("phone")
            Dim orderId = Request.QueryString("orderId")

            If phone Is Nothing Then
                phone = ""
            End If
            If orderId Is Nothing Then
                orderId = ""
            End If
            If IsNumeric(Request.QueryString("id")) Or phone.Length > 0 Or orderId.Length > 0 Then
                Dim sqlSiteManagerWhere As String = ""
                If Session("UserRole") = "SiteMgr" Then
                    If Session("SiteId") <> 1 Then
                        sqlSiteManagerWhere = " [Order].SiteId = " + Session("SiteId").ToString + " AND "
                    End If
                End If
                GridViewOrder.DataSource = SqlDataSourceOrder
                Dim s As String = SqlDataSourceOrder.SelectCommand
                Dim w As String = ""
                If IsNumeric(Request.QueryString("id")) Then
                    w = "WHERE" + sqlSiteManagerWhere + " [Order].Number = N'" + Request.QueryString("id") + "' "
                ElseIf phone.Length > 0 Then
                    w = "WHERE" + sqlSiteManagerWhere + " [User].Phone = N'+" + Request.QueryString("phone").Trim + "' "
                ElseIf orderId.Length > 0 Then
                    w = "WHERE" + sqlSiteManagerWhere + " [Order].Id = N'" + Request.QueryString("orderId") + "' "
                End If

                SqlDataSourceOrder.SelectCommand = s.Insert(s.IndexOf("ORDER BY"), w)
                GridViewOrder.DataBind()
            End If
        Catch ex As Exception

        End Try


    End Sub

    Protected Sub GridViewOrder_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewOrder.RowCommand
        PanelKeyIndicators.Visible = False
        Dim OrderId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
        If e.CommandName = "MoveCompliance" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Compliance Officer Approval'")
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "CancelOrder" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Cancel'")
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "KYCDeclined" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Decline'")
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "Select" Then
            Dim tl = New WMC.Logic.TrustLogic.TrustLogic
            PanelNote.Visible = True
            PanelKeyIndicators.Visible = False
            PanelTrustCalculation.Visible = True
            Using dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].UserId, [Order].CryptoAddress, [Order].CardNumber, [Order].IP, [Order].Id, [User].Email, [User].Phone, [Order].CreditCardUserIdentity FROM [Order] INNER JOIN [User] ON [Order].UserId = [User].Id WHERE [Order].Id =" + OrderId.ToString)
                Try
                    dr.Read()
                    Try

                    Catch ex As Exception

                    End Try
                    Dim UserId As Integer = dr!UserId
                    Dim PhoneNumber As String = dr!Phone
                    Dim CardNumber As String
                    Try
                        CardNumber = dr!CardNumber
                    Catch ex As Exception
                        CardNumber = ""
                    End Try

                    Dim IP As String = dr!IP
                    Dim Email As String = dr!Email
                    Dim CryptoAddress As String = dr!CryptoAddress
                    Dim CreditCardUserIdentity As String = dr!CreditCardUserIdentity.ToString



                    Dim TxLimitEUR As Integer = 150
                    Dim OrderSumTotalEUR As Decimal = 0
                    Dim CardCountry As String = ""
                    Try
                        OrderSumTotalEUR = tl.GetOrderSumTotalEUR(OrderId, CardNumber)
                        CardCountry = tl.GetCardCountry(OrderId)
                    Catch ex As Exception

                    End Try

                    Dim OrderSumEUR As Decimal = tl.GetOrderSumEUR(OrderId)
                    Dim CardApproved As Boolean = tl.IsCardApproved(OrderId)
                    Dim IsPhoneVitual As Boolean = tl.IsPhoneVitual(OrderId)
                    'Dim SpeedAlert As Boolean = tl.CrossedSpeedLimit(OrderId)
                    Dim CardUsedElsewhere As Integer = tl.GetCardUsedElsewhere(OrderId)
                    Dim EmailUsedElsewhere As Integer = tl.GetEmailUsedElsewhere(OrderId)
                    Dim IpUsedElsewhere As Integer = tl.GetIpUsedElsewhere(OrderId).ToString
                    Dim OrdersByStatus As Integer = WMC.GetOrdersByStatus(UserId, OrderId, "Completed")
                    Dim OrderSpan As Integer = CInt(WMC.GetOrderSpan(OrderId))
                    Dim OrderSpanCard As Integer = CInt(WMC.GetOrderSpan(OrderId, CardNumber))
                    Dim IsUserTrusted As Boolean = tl.IsUserTrusted(OrderId)
                    Dim IsKycApproved As Boolean = tl.IsKycApproved(OrderId)
                    Dim PhoneCountry As String = WMC.GetPhoneCountry(OrderId)
                    'Dim PhoneCountry As String = tl.GetPhoneCountry(OrderId)

                    Dim IpCountry As String
                    Try
                        IpCountry = tl.GetIpCountry(OrderId)
                    Catch ex As Exception
                        IpCountry = "?"
                    End Try

                    'Dim CountryTrust As String = tl.GetCountryTrustLevel(OrderId)
                    Dim CountryTrust As String = WMC.GetCountryTrustLevel(OrderId)
                    Dim SiteTrust As String = tl.GetSiteTrustLevel(OrderId)


                    '--------------Trust Level Calculation

                    LabelCardApproved.Text = CardApproved : If CardApproved Then LabelCardApproved.BackColor = Drawing.Color.FromName("#66FF66")
                    LabelTxLimit.Text = CInt(TxLimitEUR * CountryTrust * SiteTrust).ToString
                    LabelCardTotal.Text = CInt(OrderSumTotalEUR + OrderSumEUR).ToString

                    If CInt(OrderSumTotalEUR + OrderSumTotalEUR) > CInt(TxLimitEUR * CountryTrust * SiteTrust) And Not CardApproved Then
                        LabelCardTotal.BackColor = Drawing.Color.FromName("#FFFF66")
                    Else
                        LabelCardTotal.BackColor = Drawing.Color.FromName("#66FF66")
                    End If
                    LabelPhoneVirtual.Text = IsPhoneVitual : If IsPhoneVitual Then LabelPhoneVirtual.BackColor = Drawing.Color.FromName("#FFA8C5")
                    'LabelSpeedAlert.Text = SpeedAlert : If SpeedAlert Then LabelSpeedAlert.BackColor = Drawing.Color.FromName("#FFA8C5")

                    LinkButtonCreditCardUsedElsewhere.Text = CardUsedElsewhere.ToString : If CardUsedElsewhere > 0 Then LinkButtonCreditCardUsedElsewhere.CommandArgument = CardNumber : LinkButtonCreditCardUsedElsewhere.BackColor = Drawing.Color.FromName("#FFFF66")
                    LinkButtonEmailUsedElsewhere.Text = EmailUsedElsewhere.ToString : If EmailUsedElsewhere > 0 Then LinkButtonEmailUsedElsewhere.CommandArgument = Email : LinkButtonEmailUsedElsewhere.BackColor = Drawing.Color.FromName("#FFFF66")
                    LinkButtonIpUsedElsewhere.Text = IpUsedElsewhere.ToString : If IpUsedElsewhere > 0 Then LinkButtonIpUsedElsewhere.CommandArgument = IP : LinkButtonIpUsedElsewhere.BackColor = Drawing.Color.FromName("#FFFF66")

                    'LinkButtonIpUsedElsewhere.Text = WMC.GetIpUsedElsewhere(OrderId, LinkButtonIpUsedElsewhere)
                    'LinkButtonIpUsedElsewhere.Text = tl.GetIpUsedElsewhere(OrderId).ToString

                    'LabelCardUS.Text = WMC.IsCardUS(OrderId, LabelCardUS)

                    LabelCompleted.Text = WMC.GetOrdersByStatus(UserId, OrderId, "Completed")

                    WMC.GetChainalysisWithdrawalAddress(CryptoAddress, UserId, OrderId, LabelChainalysis)

                    LabelOrderSpan.Text = OrderSpan.ToString
                    'LabelOrderSpan.Text = CInt(tl.).ToString

                    If OrderSpan >= 45 Then CheckBoxIsUserTrusted.BackColor = Drawing.Color.LightGreen Else CheckBoxIsUserTrusted.BackColor = Drawing.Color.White
                    'LabelCardSpan.Text = CInt(WMC.GetOrderSpan(OrderId, CardNumber, LabelCardSpan)).ToString
                    LabelCardSpan.Text = OrderSpanCard.ToString

                    'If CInt(WMC.GetOrderSpan(OrderId, CardNumber)) >= 45 And Not WMC.IsCardApproved(OrderId) Then ButtonApproveCard.Visible = True : ButtonApproveCard.CommandArgument = OrderId.ToString Else ButtonApproveCard.Visible = False

                    'If OrderSpan >= 45 And Not CardApproved Then ButtonApproveCard.Visible = True : ButtonApproveCard.CommandArgument = OrderId.ToString Else ButtonApproveCard.Visible = False

                    'If Not CardApproved Then
                    '    'ButtonApproveCard.CommandArgument = OrderId.ToString
                    '    'ButtonApproveCard.Visible = True
                    '    If OrderSpanCard >= 45 Then
                    '        ButtonApproveCard.BackColor = Drawing.Color.LightGreen
                    '        ButtonApproveCard.CommandName = " (By CardSpan)"
                    '    Else
                    '        ButtonApproveCard.BackColor = Drawing.Color.LightGray
                    '        ButtonApproveCard.CommandName = " (By Admin)"
                    '    End If
                    '    'Else
                    '    '    ButtonApproveCard.Visible = False
                    'End If

                    'ButtonResetTxAttempt.CommandArgument = OrderId.ToString
                    LabelOrderSUM.Text = String.Format("{0:n0}", OrderSumEUR)
                    'LabelOrderTotal.Text = String.Format("{0:n0}", WMC.GetOrderSumTotalEUR(OrderId))
                    'LabelOrder30Days.Text = String.Format("{0:n0}", WMC.GetOrderSum30DaysEUR(OrderId))

                    'CheckBoxIsUserTrusted.Checked = WMC.IsUserTrusted(OrderId, LabelUserIsTrusted) : LabelUserIsTrusted.Text = CheckBoxIsUserTrusted.Checked
                    CheckBoxIsUserTrusted.Checked = IsUserTrusted : LabelUserIsTrusted.Text = CheckBoxIsUserTrusted.Checked

                    'WMC.GetOrigin(OrderId, LabelOrigin)

                    LabelCards.Text = CInt(WMC.GetDistinctUsedCards(UserId, OrderId, LabelCards)).ToString
                    LabelKYCDeclined.Text = WMC.GetOrdersByStatus(UserId, OrderId, "KYC Decline")


                    'LabelNames.Text = CInt(WMC.GetDifferentNameUser(OrderId, LabelNames)).ToString
                    'LabelEmails.Text = CInt(WMC.GetDifferentEmailUser(OrderId, LabelEmails)).ToString
                    'LabelKycApproved.Text = WMC.IsKycApproved(UserId, OrderId, LabelKycApproved)
                    LabelKycApproved.Text = IsKycApproved


                    'LabelPhoneCountry.Text = WMC.GetPhoneCountry(OrderId)

                    LabelPhoneCountry.Text = PhoneCountry
                    LabelCardCountry.Text = CardCountry
                    LabelIPCountry.Text = IpCountry


                    'HyperLinkTx.NavigateUrl = "https://app.wemovecoins.com/TxSecretRequest?useridentity=" + CreditCardUserIdentity



                    'Dim riskscore As Decimal = WMC.TrustLevelCalculation(OrderId, LabelTrustMessageNew)
                    'Dim riskscore As Decimal = tl.TrustLevelCalculation(OrderId, LabelTrustMessageNew.Text)

                    'LabelTrustMessage.Text = "RISKSCORE: " + riskscore.ToString
                    Try
                        Dim TrustMessage As String = eQuote.ExecuteScalar("SELECT TOP (1) Message FROM AuditTrail WHERE OrderId = " + OrderId.ToString + " AND Status = 11 ORDER BY Created DESC")
                        Dim tstart As Integer = TrustMessage.IndexOf("TrustMessage:")
                        Dim tend As Integer = TrustMessage.IndexOf("RiskScore:")
                        LabelTrustMessage.Text = TrustMessage.Substring(tstart, tend - tstart)

                    Catch ex As Exception

                    End Try
                Finally
                    dr.Close()
                End Try


                'PanelKeyIndicators.Visible = False
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

                'LabelCompleted.Text = WMC.GetOrdersByStatus(UserId, OrderId, "Completed")
                'LabelOrderSpan.Text = CInt(WMC.GetOrderSpan(OrderId)).ToString

                'LabelCardSpan.Text = CInt(WMC.GetOrderSpan(OrderId, CardNumber, LabelCardSpan)).ToString
                ''If CInt(WMC.GetOrderSpan(OrderId, CardNumber)) >= 45 And Not WMC.IsCardApproved(OrderId) Then ButtonApproveCard.Visible = True : ButtonApproveCard.CommandArgument = OrderId.ToString Else ButtonApproveCard.Visible = False
                'CheckBoxIsUserTrusted.Checked = WMC.IsUserTrusted(OrderId, LabelUserIsTrusted) : LabelUserIsTrusted.Text = CheckBoxIsUserTrusted.Checked
                'WMC.GetOrigin(OrderId, LabelOrigin)

                'LabelOrderSUM.Text = String.Format("{0:n0}", WMC.GetOrderSumEUR(OrderId))
                'LabelOrderTotal.Text = String.Format("{0:n0}", WMC.GetOrderSumTotalEUR(OrderId))
                'LabelOrder30Days.Text = String.Format("{0:n0}", WMC.GetOrderSum30DaysEUR(OrderId))

                'LabelUserIsTrusted.Text = WMC.IsUserTrusted(OrderId, LabelUserIsTrusted)
                'LabelCards.Text = CInt(WMC.GetDistinctUsedCards(OrderId, LabelCards)).ToString
                'LabelNames.Text = CInt(WMC.GetDifferentNameUser(OrderId, LabelNames)).ToString
                'LabelEmails.Text = CInt(WMC.GetDifferentEmailUser(OrderId, LabelEmails)).ToString
                'LabelKycApproved.Text = WMC.IsKycApproved(OrderId, LabelKycApproved)
                'LabelKYCDeclined.Text = WMC.GetOrdersByStatus(UserId, OrderId, "KYC Decline")
                'LabelPhoneCountry.Text = WMC.GetPhoneCountry(OrderId)
                'LabelCardCountry.Text = WMC.GetCardCountry(OrderId)
                'LabelIPCountry.Text = WMC.GetIpCountry(OrderId)

                'Dim riskscore As Decimal = WMC.TrustLevelCalculation(OrderId, LabelTrustMessageNew)
                'LabelTrustMessage.Text = "RISKSCORE: " + riskscore.ToString
            End Using

        End If
    End Sub

    Protected Sub ButtonAddNote_Click(sender As Object, e As EventArgs) Handles ButtonAddNote.Click
        Dim sqlSiteManagerWhere As String = ""
        If Session("UserRole") = "SiteMgr" Then
            If Session("SiteId") <> 1 Then
                sqlSiteManagerWhere = " [Order].SiteId = " + Session("SiteId").ToString + " AND "
            End If
        End If
        Dim nLineId As Integer = GridViewOrder.SelectedValue
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + TextBoxNote.Text + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + nLineId.ToString)
        GridViewOrder.DataSource = SqlDataSourceOrder
        Dim s As String = SqlDataSourceOrder.SelectCommand
        Dim w As String = ""
        Select Case DropDownListWhereType.SelectedValue
            Case "Number"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Number = N'" + TextBoxOrderID.Text + "' "
            Case "CC Number"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].CardNumber LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Email"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Email LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Name"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Name LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "YourPay ID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].ExtRef LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "TransactionHash"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].TransactionHash LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "CryptoAddress"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].CryptoAddress LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "TxSecret"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].TxSecret LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Phone"
                w = "WHERE" + sqlSiteManagerWhere + " [User].Phone LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "SiteID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].SiteId LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "PartnerID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].PartnerId LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "UserId"
                w = "WHERE" + sqlSiteManagerWhere + " [User].Id LIKE '" + TextBoxOrderID.Text + "' "
                'Case "Trusted Only"
                '    w = "WHERE" + sqlSiteManagerWhere + " [User].Trusted IS NOT NULL "
            Case "OrderId"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Id = " + TextBoxOrderID.Text
        End Select
        SqlDataSourceOrder.SelectCommand = s.Insert(s.IndexOf("ORDER BY"), w)
        GridViewOrder.DataBind()
        TextBoxNote.Text = ""
        DropDownListNoteOptions.SelectedIndex = 0
    End Sub

    Protected Sub DropDownListNoteOptions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListNoteOptions.SelectedIndexChanged
        If DropDownListNoteOptions.SelectedIndex > 0 Then
            TextBoxNote.Text = DropDownListNoteOptions.SelectedValue
        End If
    End Sub

    ' ********** PanelTrustCalculation ***************************************

    Protected Sub CheckBoxIsUserTrusted_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxIsUserTrusted.CheckedChanged
        WMC.TrustUser(GridViewOrder.SelectedValue, Session("UserId"), CheckBoxIsUserTrusted.Checked)
        GridViewOrder.DataBind()
        GridViewOrder.SelectedIndex = -1
        PanelNote.Visible = False
        PanelTrustCalculation.Visible = False
    End Sub

    'Protected Sub ButtonApproveCard_Click(sender As Object, e As EventArgs) Handles ButtonApproveCard.Click
    '    WMC.ApproveCard(CInt(ButtonApproveCard.CommandArgument), " (By CardSpan)")
    '    GridViewOrder.DataBind()
    '    GridViewOrder.SelectedIndex = -1
    '    PanelNote.Visible = False
    '    PanelTrustCalculation.Visible = False
    'End Sub
    ' ********** PanelTrustCalculation ***************************************





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
                CType(e.Row.FindControl("ButtonMoveCompliance"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonMoveCompliance"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonCancelOrder"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonCancelOrder"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonKYCDeclined"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonKYCDeclined"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkOrder"), HyperLink)
                lb.NavigateUrl = "FindOrders.aspx?id=" + lb.NavigateUrl
            End If
        End If
    End Sub

    'Protected Sub GridViewUser_DataBound(sender As Object, e As EventArgs) Handles GridViewUser.DataBound
    '    'https://ACd32f9f403312208a96707ec9356eb2ea:74dfe135a1259269c3a7817ffd9fd6e6@lookups.twilio.com/v1/PhoneNumbers/+447492195433?AddOns=whitepages_pro_phone_rep
    'End Sub

    'Protected Sub GridViewUser_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewUser.RowDataBound
    '    If e.Row.RowType = DataControlRowType.DataRow Then
    '        If e.Row.RowState = DataControlRowState.Edit Or e.Row.RowState = 5 Then
    '        Else
    '            Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkTwilio"), HyperLink)
    '            If lb.NavigateUrl.Length Then
    '                lb.NavigateUrl = "https://www.google.dk/#q=%2B" + lb.NavigateUrl.ToString + ""
    '            End If
    '        End If

    '    End If
    'End Sub


    Protected Sub GridViewTransaction_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewTransaction.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Try
                Dim lb As Label = CType(e.Row.FindControl("LabelInfo"), Label)
                Dim lb2 As Label = CType(e.Row.FindControl("LabelBINInfo"), Label)
                Dim bin As String = lb.Text.Replace(" ", "").Substring(0, 6)
                Dim wrGETURL As WebRequest
                wrGETURL = WebRequest.Create("https://lookup.binlist.net/" + bin)
                wrGETURL.Headers.Add("accept-version", "3")
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



    Protected Sub ButtonFindByOrderID_Click(sender As Object, e As EventArgs) Handles ButtonFindByOrderID.Click
        Dim sqlSiteManagerWhere As String = ""
        If Session("UserRole") = "SiteMgr" Then
            If Session("SiteId") <> 1 Then
                sqlSiteManagerWhere = " [Order].SiteId = " + Session("SiteId").ToString + " AND "
            End If
        End If
        GridViewOrder.DataSource = SqlDataSourceOrder
        Dim s As String = SqlDataSourceOrder.SelectCommand
        Dim w As String = ""
        Select Case DropDownListWhereType.SelectedValue
            Case "Number"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Number = N'" + TextBoxOrderID.Text + "' "
            Case "CC Number"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].CardNumber LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Email"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Email LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Name"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Name LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "YourPay ID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].ExtRef LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "CryptoAddress"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].CryptoAddress LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "TransactionHash"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].TransactionHash LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "CryptoAddress"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].CryptoAddress LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "TxSecret"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].TxSecret LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "Phone"
                w = "WHERE" + sqlSiteManagerWhere + " [User].Phone LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "SiteID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].SiteId LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "PartnerID"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].PartnerId LIKE N'%" + TextBoxOrderID.Text + "%' "
            Case "UserId"
                w = "WHERE" + sqlSiteManagerWhere + " [User].Id LIKE '" + TextBoxOrderID.Text + "' "
                'Case "Trusted Only"
                '    w = "WHERE" + sqlSiteManagerWhere + " [User].Trusted IS NOT NULL "
            Case "OrderId"
                w = "WHERE" + sqlSiteManagerWhere + " [Order].Id = " + TextBoxOrderID.Text
        End Select
        SqlDataSourceOrder.SelectCommand = s.Insert(s.IndexOf("ORDER BY"), w)
        GridViewOrder.DataBind()
        GridViewSubOrders.DataBind()
        GridViewTransaction.DataBind()
        GridViewAuditTrail.DataBind()
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

    Protected Sub LinkButtonCreditCardUsedElsewhere_Click(sender As Object, e As EventArgs) Handles LinkButtonCreditCardUsedElsewhere.Click
        Dim sql As String = "SELECT [User].Fname, [User].Phone, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN    Currency ON [Order].CurrencyId = Currency.Id INNER JOIN [User] ON [Order].UserId = [User].Id"
        Dim nLineId As Integer = GridViewOrder.SelectedDataKey(0)
        Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)

        PanelKeyIndicators.Visible = True
        GridViewKeyIndicator.DataSource = eQuote.GetDataReader(sql + " WHERE  ([Order].CardNumber = N'" + LinkButtonCreditCardUsedElsewhere.CommandArgument + "') AND (NOT (UserId = " + nUserId.ToString + ")) ORDER BY [Order].Id DESC")
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



    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        ' Read the file and convert it to Byte Array

        Dim filePath As String = FileUpload1.PostedFile.FileName

        Dim filename As String = Path.GetFileName(filePath)

        Dim ext As String = Path.GetExtension(filename).ToLower

        Dim contenttype As String = String.Empty

        'Set the contenttype based on File Extension
        Select Case ext

            Case ".doc"
                contenttype = "application/vnd.ms-word"
                Exit Select
            Case ".docx"
                contenttype = "application/vnd.ms-word"
                Exit Select
            Case ".xls"
                contenttype = "application/vnd.ms-excel"
                Exit Select
            Case ".xlsx"
                contenttype = "application/vnd.ms-excel"
                Exit Select
            Case ".jpeg"
                contenttype = "image/jpg"
                Exit Select
            Case ".jpg"
                contenttype = "image/jpg"
                Exit Select
            Case ".png"
                contenttype = "image/png"
                Exit Select
            Case ".gif"
                contenttype = "image/gif"
                Exit Select
            Case ".pdf"
                contenttype = "application/pdf"
                Exit Select
        End Select
        If contenttype <> String.Empty Then
            Dim fs As Stream = FileUpload1.PostedFile.InputStream
            Dim userId = GridViewUser.DataKeys(0).Value
            Dim kycTypeId = DropDownListKycType.SelectedValue
            WMC.Logic.KYCFileHandler.AddNewKYCFile(fs, userId, filename, kycTypeId)
            'Dim br As New BinaryReader(fs)
            'Dim bytes As Byte() = br.ReadBytes(fs.Length)
            ''' TODO: replace this logic with new
            'Dim strQuery As String = "INSERT INTO KycFile(UserId, [File], Type, UniqueFilename, OriginalFilename) VALUES (@UserId, @File, @Type, @UniqueFilename, @OriginalFilename)"
            'Dim cmd As New SqlCommand(strQuery)
            'Dim n As Integer = GridViewUser.DataKeys(0).Value
            'cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = n
            'cmd.Parameters.Add("@File", SqlDbType.Binary).Value = bytes
            'cmd.Parameters.Add("@Type", SqlDbType.Int).Value() = DropDownListKycType.SelectedValue
            'cmd.Parameters.Add("@UniqueFilename", SqlDbType.VarChar).Value = filename
            'cmd.Parameters.Add("@OriginalFilename", SqlDbType.VarChar).Value = filename
            'eQuote.ExecuteNonQuery(cmd)
            lblMessage.ForeColor = System.Drawing.Color.Green
            lblMessage.Text = "File Uploaded Successfully"
            GridViewKycFile.DataBind()
        Else
            lblMessage.ForeColor = System.Drawing.Color.Red
            lblMessage.Text = "File format not recognised." & " Upload Image/Word/PDF/Excel formats"
        End If
    End Sub

    Protected Sub GridViewKycFile_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewKycFile.RowCommand
        On Error Resume Next
        If e.CommandName = "Edit" Then GridViewKycFile.SelectedIndex = -1
        Dim nLineId As Integer = GridViewKycFile.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
        If e.CommandName = "Select" Then
            Dim btn As LinkButton = GridViewKycFile.Rows(Convert.ToInt32(e.CommandArgument)).FindControl("LinkButton1")
            btn.Focus()
            Dim strOname As String = eQuote.ExecuteScalar("SELECT [OriginalFilename] FROM KycFile WHERE Id =" + nLineId.ToString())
            Dim ext = strOname.Split(".").GetValue(1)
            If ext = "pdf" Then
                Dim embed As String = "<object data=""{0}{1}"" type=""application/pdf"" width=""500px"" height=""450px"">"
                embed += "</object>"
                ltEmbed.Visible = True
                ModalPopupExtender1.Show()
                GridView1.Visible = False
                ltEmbed.Text = String.Format(embed, ResolveUrl("KYCImageHandler.ashx?ImageId="), nLineId.ToString())
            Else
                GridView1.Visible = True
                ltEmbed.Visible = False
                ModalPopupExtender1.Show()
            End If
        End If
        If e.CommandName = "Reject" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = NULL, ApprovedBy = NULL, Rejected = { fn NOW() }, RejectedBy = " + Session("UserId").ToString + " where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "UndoReject" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = NULL, ApprovedBy = NULL, Rejected = NULL, RejectedBy = NULL where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "Approve" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Rejected = NULL, RejectedBy = NULL where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "UndoApprove" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = NULL, ApprovedBy = NULL, Rejected = NULL, RejectedBy = NULL where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "Obsolete" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Obsolete = { fn NOW() }, ObsoleteBy = " + Session("UserId").ToString + " where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "UndoObsolete" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Obsolete = NULL where Id= " + nLineId.ToString)
            GridViewKycFile.EditIndex = -1
        End If
        If e.CommandName = "Delete" Then
            eQuote.ExecuteNonQuery("DELETE FROM [OrderKycfile] WHERE [KycfileId] = N'" + nLineId.ToString + "'")
            eQuote.ExecuteNonQuery("DELETE FROM [KycFile] WHERE [Id] = N'" + nLineId.ToString + "'")
        End If
    End Sub
    Protected Sub btncancel_Click(sender As Object, e As EventArgs)
        btnCancel.Focus()
        ModalPopupExtender1.Hide()
        GridViewKycFile.DataBind()
    End Sub
    Protected Sub GridViewKycFile_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewKycFile.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Edit Or e.Row.RowState = 5 Then
                CType(e.Row.FindControl("ButtonReject"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonApprove"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonUndoReject"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonUndoApprove"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonObsolete"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonUndoObsolete"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonReject"), Button).Attributes.Add("onClick", "javascript: return confirm('Er du sikker?')")
                CType(e.Row.FindControl("ButtonApprove"), Button).Attributes.Add("onClick", "javascript: return confirm('Er du sikker?')")
            ElseIf e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                CType(e.Row.FindControl("LinkButtonDelete"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Er du sikker?')")
            End If
        End If
    End Sub

End Class

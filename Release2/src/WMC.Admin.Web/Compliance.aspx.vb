Option Strict Off
Imports BitGoSharp
Imports RestSharp
Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.Net
Imports System.IO

Partial Class Compliance
    Inherits System.Web.UI.Page
    Dim client As BitGoClient
    Public WeightCcOrigin As Integer = 1
    Public WeightPhoneOrigin As Integer = 50
    Public WeightOrderSpan As Integer = 50
    Public WeightCountryCoherence As Integer = 50
    Public WeightDataConsistency As Integer = 30
    Public WeightKycApprove As Integer = 20
    Public WeighAmountSize As Integer = 50

    Sub SetCountStatus()
        DropDownListStatus.Items(0).Text = DropDownListStatus.Items(0).Value + " (" + WMCData.StatusRecordCount("Compliance Officer Approval").ToString + ")"
        DropDownListStatus.Items(1).Text = DropDownListStatus.Items(1).Value + " (" + WMCData.StatusRecordCount("Customer Response Pending").ToString + ")"
        DropDownListStatus.Items(2).Text = DropDownListStatus.Items(2).Value + " (" + WMCData.StatusRecordCount("KYC Approval Pending").ToString + ")"
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (IsNumeric(Session("UserId")) And Session("UserRole") = "Admin") Then Response.Redirect("Default.aspx")
        Dim bitGoAccessCodeJson As String = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoAccessCode'")
        Dim bitGoAccessCode = SimpleJson.DeserializeObject(bitGoAccessCodeJson)
        client = New BitGoClient(bitGoAccessCode.Environment, bitGoAccessCode.AccessCode)

        SetCountStatus()

        If DropDownListStatus.SelectedValue = "Compliance Officer Approval" Then
            'GridViewOrder.Columns(11)                      'AwaitApproval
            GridViewOrder.Columns(12).Visible = False       'MoveToCompliance
            GridViewOrder.Columns(13).Visible = True        'CustomerPending
            GridViewOrder.Columns(14).Visible = True        'AskReviewKYC
            GridViewOrder.Columns(15).Visible = True        'AskTxSecret
            GridViewOrder.Columns(16).Visible = False        'TxApproved
            GridViewOrder.Columns(17).Visible = True        'KYCDeclined
            GridViewOrder.Columns(18).Visible = True        'CancelOrder
        ElseIf DropDownListStatus.SelectedValue = "Customer Response Pending" Then
            GridViewOrder.Columns(12).Visible = True
            GridViewOrder.Columns(13).Visible = False
            GridViewOrder.Columns(14).Visible = False
            GridViewOrder.Columns(15).Visible = False
            GridViewOrder.Columns(16).Visible = True
            GridViewOrder.Columns(17).Visible = False
            GridViewOrder.Columns(18).Visible = False
        ElseIf DropDownListStatus.SelectedValue = "KYC Approval Pending" Then
            GridViewOrder.Columns(12).Visible = True
            GridViewOrder.Columns(13).Visible = True
            GridViewOrder.Columns(14).Visible = False
            GridViewOrder.Columns(15).Visible = False
            GridViewOrder.Columns(16).Visible = False
            GridViewOrder.Columns(17).Visible = False
            GridViewOrder.Columns(18).Visible = False
        End If
    End Sub

    'Protected Sub ButtonTrustLevelCalc_Click(sender As Object, e As EventArgs) Handles ButtonTrustLevelCalc.Click
    '    Dim CreditCardScore As Integer
    '    Dim PhoneScore As Integer = 0
    '    Dim OrderSpanScore As Integer = 0
    '    Dim CountryCoherenceScore As Integer = 0
    '    Dim DataConsistencyScore As Integer = 0
    '    Dim KycApprovedScore As Integer = 0
    '    Dim AmountScore As Integer = 0
    '    Dim OrderId As Integer = GridViewOrder.SelectedValue
    '    Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Transaction].Info, [Order].UserId, [User].Phone, [Order].Amount, [Order].CurrencyId, AuditTrail.Message, AuditTrail_1.Message AS IP_Message, [Order].CryptoAddress, [User].Email, [Order].IP  FROM [Order] INNER JOIN [Transaction] ON [Order].Id = [Transaction].OrderId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN AuditTrail ON [Order].Id = AuditTrail.OrderId INNER JOIN AuditTrail AS AuditTrail_1 ON [Order].Id = AuditTrail_1.OrderId WHERE ([Order].Id = " + OrderId.ToString + ") AND ([Transaction].Type = 1) AND (AuditTrail.Status = 3) AND (AuditTrail_1.Status = 4)")
    '    If dr.HasRows Then
    '        dr.Read()
    '        If Not IsDBNull(dr!Info) Then CreditCardScore = WMC.GetCreditCardScore(dr!Info)
    '        PhoneScore = WMC.GetTelephoneScore(dr!Message)
    '        If Not IsDBNull(dr!Info) Then OrderSpanScore = WMC.GetOrderSpanScore(dr!UserId)
    '        If Not IsDBNull(dr!Info) Then CountryCoherenceScore = WMC.GetCountryCoherenceScore(dr!Message, dr!IP_Message, dr!Info)
    '        If Not IsDBNull(dr!Info) Then DataConsistencyScore = WMC.GetDataConsistencyScore(dr!UserId, dr!CryptoAddress, dr!Email, dr!IP, dr!Info)
    '        KycApprovedScore = WMC.GetKycApprovedScore(dr!UserId)
    '        AmountScore = WMC.GetAmountSizeScore(dr!Amount, dr!CurrencyId)
    '    End If
    '    dr.Close()
    '    Dim WeightSum As Integer = WeightPhoneOrigin + WeightOrderSpan + WeightCountryCoherence + WeightDataConsistency + WeightKycApprove + WeighAmountSize
    '    Dim TotalScore As Integer = CreditCardScore * (PhoneScore * WeightPhoneOrigin + OrderSpanScore * WeightOrderSpan + CountryCoherenceScore * WeightCountryCoherence + DataConsistencyScore * WeightDataConsistency + KycApprovedScore * WeightKycApprove + AmountScore * WeighAmountSize) / WeightSum

    '    If TotalScore < 18 Then
    '        PanelTotalTrustScore.BackColor = Drawing.Color.Red
    '    ElseIf TotalScore >= 18 And TotalScore < 30 Then
    '        PanelTotalTrustScore.BackColor = Drawing.Color.Yellow
    '    ElseIf TotalScore >= 30 Then
    '        PanelTotalTrustScore.BackColor = Drawing.Color.SpringGreen
    '    End If

    '    LabelCreditCardScore.Text = CreditCardScore.ToString
    '    LabelPhoneScore.Text = PhoneScore.ToString
    '    LabelOrderSpanScore.Text = OrderSpanScore.ToString
    '    LabelCountryCoherenceScore.Text = CountryCoherenceScore.ToString
    '    LabelDataConsistencyScore.Text = DataConsistencyScore.ToString
    '    LabelKycApprovedScore.Text = KycApprovedScore.ToString
    '    LabelAmountScore.Text = AmountScore.ToString
    '    LabelTotalScore.Text = TotalScore.ToString
    '    PanelTrustLevel.Visible = True
    'End Sub

    'Protected Sub btnUnlock_Click(sender As Object, e As EventArgs) Handles BtnUnlock.Click
    '    Dim response As IRestResponse = client.Unlock(txtOTP.Text, CInt(DropDownListUnlockPeriode.SelectedValue))
    '    Label1.Text = response.StatusCode.ToString
    'End Sub

    Protected Sub SqlDataSourceOrderChildNote_Updated(sender As Object, e As SqlDataSourceStatusEventArgs) Handles SqlDataSourceOrderChildNote.Updated
        GridViewOrder.SelectedIndex = -1
        GridViewOrder.DataBind()
        GridViewSubOrders.SelectedIndex = -1
        GridViewSubOrders.DataBind()
    End Sub

    Protected Sub GridViewOrder_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewOrder.RowCommand
        PanelKeyIndicators.Visible = False
        Dim statusOrder As String = DropDownListStatus.SelectedValue
        If e.CommandName = "MoveToCompliance" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Compliance Officer Approval'")
            If WMCData.IsOrderStatus(statusOrder, nLineId) Then
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "CustomerPending" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Customer Response Pending'")
            If WMCData.IsOrderStatus(statusOrder, nLineId) Then
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "AwaitApproval" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Payout awaits approval'")
            If WMCData.IsOrderStatus(statusOrder, nLineId) Then
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "ReviewKYC" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Approval Pending'")
            If WMCData.IsOrderStatus(statusOrder, nLineId) Then
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
                Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + nLineId.ToString)
                eQuote.ExecuteNonQuery("INSERT INTO KycFile (UserId, Type, UniqueFilename, OriginalFilename) VALUES (" + nUserId.ToString + ", 1, N'EnforceKYC', N'EnforceKYC')")

                Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, [Order].SiteId, [Order].Number, [Order].Amount, Currency.Code, [Order].Quoted, [Order].Name, [Order].Email, [Order].CardNumber, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [User] ON [Order].UserId = [User].Id WHERE [Order].Id =" + nLineId.ToString)
                If dr.HasRows Then
                    While dr.Read()
                        WMCData.SendEmail("RequestKycDocuments.htm", dr!SiteId, dr!Name, dr!Email, "YOUR ORDER #" + dr!Number + " IS NOW PAUSED", dr!Number)
                        s = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'SentEmail'")
                        Dim m As String = "RequestKycDocuments.htm" + vbCrLf + "Number:" + dr!Number
                        eQuote.ExecuteNonQuery("INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (" + dr!Id.ToString + ", " + s.ToString + ", N'" + m + "', { fn NOW() })")
                        GridViewAuditTrail.DataBind()
                    End While
                End If
                dr.Close()
                AddNote(nLineId, "KYC Documents Requested")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "TxSecret" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, [Order].SiteId, [Site].[Text], [Order].CreditCardUserIdentity, [Order].Number, [Order].Amount, Currency.Code, [Order].Quoted, [Order].Name, [Order].Email, 
                                        [Order].CardNumber, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [User] ON [Order].UserId = [User].Id 
                                        INNER JOIN [Site] ON [Order].SiteId = [Site].Id 
                                        WHERE [Order].Id =" + nLineId.ToString)
            If dr.HasRows Then
                While dr.Read()
                    WMC.Utilities.EmailHelper.SendEmail(dr!Email, "RequestTxSecret", New Dictionary(Of String, Object) From
                           {
                               {"UserIdentity", dr!CreditCardUserIdentity},
                               {"SiteName", dr!Text},
                               {"UserFirstName", dr!Name},
                               {"OrderNumber", dr!Number},
                               {"OrderCompleted", dr!Quoted},
                               {"OrderAmount", dr!Amount},
                               {"OrderCurrency", dr!Code},
                               {"CreditCard", dr!CardNumber}
                            }, dr!Text)
                End While
            End If
            dr.Close()
            'PanelSendEmail.Visible = True
            GridViewOrder.SelectedIndex = e.CommandArgument
        ElseIf e.CommandName = "TxApproved" Then

        ElseIf e.CommandName = "CancelOrder" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Cancel'")
            If WMCData.IsOrderStatus(statusOrder, nLineId) Then
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "KYCDeclined" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Decline'")
            If WMCData.IsOrderStatus(statusOrder, nLineId) Then
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "Select" Then
            PanelNote.Visible = True
            Dim sql As String = "SELECT  [Order].UserId, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN Currency ON [Order].CurrencyId = Currency.Id"
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
                Dim n As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT CardNumber) FROM [Order] WHERE UserId =" + nUserId.ToString)
                LabelNumberOfCC.Text = n.ToString
                If n < 4 Then
                    LabelNumberOfCC.BackColor = Drawing.Color.SpringGreen
                ElseIf n >= 4 And n < 7 Then
                    LabelNumberOfCC.BackColor = Drawing.Color.Yellow
                Else
                    LabelNumberOfCC.BackColor = Drawing.Color.Red
                End If
            Catch ex As Exception
                LabelNumberOfCC.Text = "?"
            End Try
            Try
                Dim r As String = WMCData.GetOrderSpan(nUserId)
                LabelOrderSpan.Text = r
                If r = "0" Then
                    LabelOrderSpan.BackColor = Drawing.Color.Red
                ElseIf r = "1-3" Or r = "4-7" Then
                    LabelOrderSpan.BackColor = Drawing.Color.Yellow
                Else
                    LabelOrderSpan.BackColor = Drawing.Color.SpringGreen
                End If
            Catch ex As Exception
                LabelOrderSpan.Text = "?"
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
            PanelTrustLevel.Visible = False
        End If
        SetCountStatus()
    End Sub

    Sub AddNote(id As Integer, note As String)
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + note + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + id.ToString)
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
                CType(e.Row.FindControl("ButtonAwaitApproval"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonAwaitApproval"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonMoveToCompliance"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonMoveToCompliance"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonCustomerPending"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonCustomerPending"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonReviewKYC"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonReviewKYC"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonTxSecret"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonTxApproved"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonTxApproved"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonKYCDeclined"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonKYCDeclined"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonCancelOrder"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonCancelOrder"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
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

    Protected Sub ButtonCloseTrustLevelPanel_Click(sender As Object, e As EventArgs) Handles ButtonCloseTrustLevelPanel.Click
        PanelTrustLevel.Visible = False
    End Sub

    Protected Sub ButtonAddNote_Click(sender As Object, e As EventArgs) Handles ButtonAddNote.Click
        Dim nLineId As Integer = GridViewOrder.SelectedValue
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + TextBoxNote.Text + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + nLineId.ToString)
        GridViewOrder.DataBind()
        TextBoxNote.Text = ""
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
            Dim br As New BinaryReader(fs)
            Dim bytes As Byte() = br.ReadBytes(fs.Length)
            Dim strQuery As String = "INSERT INTO KycFile(UserId, [File], Type, UniqueFilename, OriginalFilename) VALUES (@UserId, @File, @Type, @UniqueFilename, @OriginalFilename)"
            Dim cmd As New SqlCommand(strQuery)
            Dim n As Integer = GridViewUser.DataKeys(0).Value
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = n
            cmd.Parameters.Add("@File", SqlDbType.Binary).Value = bytes
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value() = DropDownListKycType.SelectedValue
            cmd.Parameters.Add("@UniqueFilename", SqlDbType.VarChar).Value = filename
            cmd.Parameters.Add("@OriginalFilename", SqlDbType.VarChar).Value = filename
            eQuote.ExecuteNonQuery(cmd)
            lblMessage.ForeColor = System.Drawing.Color.Green
            lblMessage.Text = "File Uploaded Successfully"
            GridViewKycFile.DataBind()
        Else
            lblMessage.ForeColor = System.Drawing.Color.Red
            lblMessage.Text = "File format not recognised." & " Upload Image/Word/PDF/Excel formats"
        End If
    End Sub
    Protected Sub GridViewKycFile_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewKycFile.RowCommand
        'On Error Resume Next
        'If e.CommandName = "Edit" Then GridViewKycFile.SelectedIndex = -1
        'Dim nLineId As Integer = GridViewKycFile.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
        'If e.CommandName = "Reject" Then
        '    eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = NULL, ApprovedBy = NULL, Rejected = { fn NOW() }, RejectedBy = " + Session("UserId").ToString + " where Id= " + nLineId.ToString)
        '    GridViewKycFile.EditIndex = -1
        'End If
        'If e.CommandName = "Approve" Then
        '    eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Rejected = NULL, RejectedBy = NULL where Id= " + nLineId.ToString)
        '    GridViewKycFile.EditIndex = -1
        'End If
        On Error Resume Next
        If e.CommandName = "Edit" Then GridViewKycFile.SelectedIndex = -1

        Dim nLineId As Integer = GridViewKycFile.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
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

    Protected Sub ButtonCloseEmail_Click(sender As Object, e As EventArgs) Handles ButtonCloseEmail.Click
        PanelSendEmail.Visible = False
    End Sub

    Protected Sub GridViewSendEmail_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewSendEmail.RowCommand
        If e.CommandName = "SendEmail" Then
            Dim nLineId As Integer = GridViewSendEmail.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim drOrder As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, [Order].Number, [Order].SiteId, [Order].Amount, Currency.Code, [Order].Quoted, [Order].Name, [Order].Email, [Order].CardNumber, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [User] ON [Order].UserId = [User].Id WHERE [Order].Id =" + GridViewOrder.SelectedValue.ToString)
            Dim dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, [Order].Number, [Order].Amount, Currency.Code, [Order].Quoted, [Order].Name, [Order].Email, [Order].CardNumber, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [User] ON [Order].UserId = [User].Id WHERE [Order].Id =" + nLineId.ToString)
            If dr.HasRows Then
                While dr.Read()
                    If drOrder.HasRows Then
                        While drOrder.Read()
                            WMCData.SendEmail("RequestTxSecret.htm", drOrder!SiteId, drOrder!Name, drOrder!Email, "CREDIT CARD VERIFICATION OF ORDER #" + dr!Number, dr!Number, String.Format("{0:d/M/yyyy}", dr!Quoted), String.Format("{0:n0}", dr!Amount) + " " + dr!Code, dr!CardNumber)
                            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'SentEmail'")
                            Dim m As String = "RequestTxSecret.htm" + vbCrLf + "Number:" + dr!Number + ", TxSecret:" + dr!TxSecret
                            eQuote.ExecuteNonQuery("INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (" + GridViewOrder.SelectedValue.ToString + ", " + s.ToString + ", N'" + m + "', { fn NOW() })")
                            GridViewAuditTrail.DataBind()
                            s = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Customer Response Pending'")
                            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + GridViewOrder.SelectedValue.ToString + ")")
                            AddNote(GridViewOrder.SelectedValue, "Tx Secret Requested")
                            GridViewOrder.SelectedIndex = -1
                            GridViewOrder.DataBind()
                        End While
                    End If
                    drOrder.Close()
                End While
            End If
            dr.Close()
        End If
        PanelSendEmail.Visible = False
    End Sub

    Protected Sub GridViewSEndEmail_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewSendEmail.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                CType(e.Row.FindControl("LinkButtonSendEmail"), LinkButton).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("LinkButtonSendEmail"), LinkButton).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
            End If
        End If
    End Sub

    Protected Sub GridViewSubOrders_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewSubOrders.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkIP"), HyperLink)
            If lb.NavigateUrl.Length Then
                lb.NavigateUrl = "http://www.infosniper.net/index.php?ip_address=" + lb.NavigateUrl.ToString + ""
            End If
        End If
    End Sub
End Class

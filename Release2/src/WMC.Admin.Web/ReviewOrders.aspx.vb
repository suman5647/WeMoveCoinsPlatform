Option Strict Off
Imports BitGoSharp
Imports RestSharp
Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.Net
Imports System.IO

Partial Class ReviewOrders
    Inherits System.Web.UI.Page
    Dim client As BitGoClient
    Public WeightCcOrigin As Integer = 1
    Public WeightPhoneOrigin As Integer = 50
    Public WeightOrderSpan As Integer = 50
    Public WeightCountryCoherence As Integer = 50
    Public WeightDataConsistency As Integer = 30
    Public WeightKycApprove As Integer = 20
    Public WeighAmountSize As Integer = 50



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
    '        If Not IsDBNull(dr!Info) Then CreditCardScore = WMCData.GetCreditCardScore(dr!Info)

    '        PhoneScore = WMCData.GetTelephoneScore(dr!Message)
    '        If Not IsDBNull(dr!Info) Then OrderSpanScore = WMCData.GetOrderSpanScore(dr!UserId)
    '        If Not IsDBNull(dr!Info) Then CountryCoherenceScore = WMCData.GetCountryCoherenceScore(dr!Message, dr!IP_Message, dr!Info)
    '        If Not IsDBNull(dr!Info) Then DataConsistencyScore = WMCData.GetDataConsistencyScore(dr!UserId, dr!CryptoAddress, dr!Email, dr!IP, dr!Info)
    '        KycApprovedScore = WMCData.GetKycApprovedScore(dr!UserId)
    '        AmountScore = WMCData.GetAmountSizeScore(dr!Amount, dr!CurrencyId)
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
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not (IsNumeric(Session("UserId")) And (Session("UserRole") = "Admin" Or Session("UserRole") = "SiteMgr")) Then Response.Redirect("Default.aspx")

        Dim bitGoAccessCodeJson As String = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoAccessCode'")
        Dim bitGoAccessCode = SimpleJson.DeserializeObject(bitGoAccessCodeJson)
        client = New BitGoClient(bitGoAccessCode.Environment, bitGoAccessCode.AccessCode)
        'LabelUserKYC.Text = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT KycFile.UserId)FROM KycFile INNER JOIN [User] ON KycFile.UserId = [User].Id WHERE  (KycFile.Obsolete IS NULL) AND (KycFile.Approved IS NULL)")
        'If IsNumeric(LabelUserKYC.Text) AndAlso CInt(LabelUserKYC.Text) > 0 Then LabelUserKYC.ForeColor = Drawing.Color.Red Else LabelUserKYC.ForeColor = Drawing.Color.Green

        If Session("UserRole") = "SiteMgr" Then
            GridViewOrder.Columns(10).Visible = False
            GridViewOrder.Columns(11).Visible = False
            GridViewOrder.Columns(12).Visible = False
            GridViewOrder.Columns(14).Visible = False
            GridViewUser.Columns(8).Visible = False
            GridViewUser.Columns(9).Visible = False
            PanelUploadKycDoc.Visible = False
        End If
        If Not IsPostBack Then
            If Session("UserRole") = "SiteMgr" Then
                DropDownListOrderStatus.DataSource = SqlDataSourceOrderStatusSiteMgr

            Else
                DropDownListOrderStatus.DataSource = SqlDataSourceOrderStatus
            End If
            DropDownListOrderStatus.DataBind()
        End If


    End Sub

    Protected Sub GridViewOrder_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewOrder.RowCommand
        If e.CommandName = "AwaitApproval" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Payout awaits approval'")
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "CancelOrder" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Cancel'")
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "KYCDeclined" Then
            Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Decline'")
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + nLineId.ToString + ")")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "Select" Then
            PanelNote.Visible = True
            'Dim nLineId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            PanelTrust2.Visible = True
            Dim sql As String = "SELECT  [Order].UserId, [Order].Number, [Order].Quoted, [Order].Rate, [Order].Amount, Currency.Code AS Currrency,[Order].Name, [Order].Note, [Order].CryptoAddress, [Order].Email, [Order].IP, [Order].CardNumber, OrderStatus.Text AS Status, [Order].SiteId FROM [Order] INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id INNER JOIN Currency ON [Order].CurrencyId = Currency.Id"
            Dim OrderId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
            Dim nUserId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM  [Order] WHERE Id=" + OrderId.ToString)
            Dim sPhoneNumber As String = eQuote.ExecuteScalar("SELECT Phone FROM  [User] WHERE Id=" + nUserId.ToString)
            Dim sCrypto As String = ""
            Dim sEmail As String = ""
            Dim sEmailOrder As String = ""
            Dim sIP As String = ""
            Dim sCardNumber As String = ""

            ''--------------Trust Level Calculation

            'LabelCompletedOrders.Text = WMC.GetOrdersByStatus(OrderId, "Completed")  'eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE Status = 17 WHERE UserId = " + nUserId.ToString).ToString
            'LabelKYCDeclined.Text = WMC.GetOrdersByStatus(OrderId, "KYC Decline")
            'If IsNumeric(LabelKYCDeclined.Text) Then If CInt(LabelKYCDeclined.Text) > 0 Then LabelKYCDeclined.BackColor = Drawing.Color.FromName("#FFA8C5")
            'LabelOrderSpan.Text = WMC.GetOrderSpan(OrderId)
            'LabelOrderSumEUR.Text = String.Format("{0:n0}", WMC.GetOrderSumEUR(OrderId))
            'LabelOrderSumTotalEUR.Text = String.Format("{0:n0}", WMC.GetOrderSumTotalEUR(OrderId))
            'LabeltOrderSum30Days.Text = String.Format("{0:n0}", WMC.GetOrderSum30DaysEUR(OrderId))

            'LabelCC_IsUS.Text = WMC.IsCardUS(OrderId, LabelCC_IsUS)
            'LabelCardRejected.Text = WMC.IsCardRejected(OrderId, LabelCardRejected).ToString

            'LabelNumberOfCC.Text = WMC.GetDistinctUsedCards(OrderId, LabelNumberOfCC)
            'LinkButtonCreditCardUsedElsewhere.Text = WMC.GetCardUsedElsewhere(OrderId, LinkButtonCreditCardUsedElsewhere)
            'LinkButtonEmailUsedElsewhere.Text = WMC.GetEmailUsedElsewhere(OrderId, LinkButtonEmailUsedElsewhere)
            'LinkButtonIpUsedElsewhere.Text = WMC.GetIpUsedElsewhere(OrderId, LinkButtonIpUsedElsewhere)
            'LabelPhoneCC.Text = WMC.PhoneCardOrigin_Match(OrderId)
            'LabelPhoneIP.Text = WMC.PhoneIP_Match(OrderId)
            ''LabelPhoneCulture.Text = WMC.GetPhoneCulture_Match(OrderId)
            ''LabelCcIP.Text = WMC.GetPhoneCulture_Match(OrderId)
            'LabelDistinctNames.Text = WMC.GetDifferentNameUser(OrderId, LabelDistinctNames)
            'LabelDistinctEmails.Text = WMC.GetDifferentEmailUser(OrderId, LabelDistinctEmails)

            '--------------Trust Level Calculation




            'Dim sPhoneNumber As String = eQuote.ExecuteScalar("SELECT Phone FROM  [User] WHERE Id=" + nUserId.ToString)
            'Dim sCrypto As String = ""
            'Dim sEmail As String = ""
            'Dim sEmailOrder As String = ""
            'Dim sIP As String = ""
            'Dim sCardNumber As String = ""
            'Try
            '    sCrypto = eQuote.ExecuteScalar("SELECT CryptoAddress FROM [Order] WHERE Id = " + nLineId.ToString)
            '    LinkButtonCryptoAddressSum.CommandArgument = sCrypto
            '    sEmail = eQuote.ExecuteScalar("SELECT Email FROM  [User] WHERE Id=" + nUserId.ToString)
            '    LinkButtonEmailSum.CommandArgument = sEmail
            '    sEmailOrder = eQuote.ExecuteScalar("SELECT Email FROM [Order] WHERE Id = " + nLineId.ToString)
            '    sIP = eQuote.ExecuteScalar("SELECT IP FROM [Order] WHERE Id = " + nLineId.ToString)
            '    LinkButtonIpSum.CommandArgument = sIP
            '    sCardNumber = eQuote.ExecuteScalar("SELECT CardNumber FROM [Order] WHERE Id = " + nLineId.ToString)
            '    LinkButtonCreditCard.CommandArgument = sCardNumber
            'Catch ex As Exception
            'End Try
            'Try
            '    LabelCompletedOrders.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE Status = 17 GROUP BY UserId HAVING UserId = " + nUserId.ToString).ToString
            '    LabelCompletedOrders.BackColor = Drawing.Color.SpringGreen
            'Catch ex As Exception
            '    LabelCompletedOrders.Text = "0"
            '    LabelCompletedOrders.BackColor = Drawing.Color.Yellow
            'End Try
            'Try
            '    LabelKYCDeclined.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE Status = 6 GROUP BY UserId HAVING UserId = " + nUserId.ToString).ToString
            '    LabelKYCDeclined.BackColor = Drawing.Color.Red
            'Catch ex As Exception
            '    LabelKYCDeclined.Text = "0"
            '    LabelKYCDeclined.BackColor = Drawing.Color.SpringGreen
            'End Try
            'Try
            '    LabelPendingOrders.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE Status = 1 GROUP BY UserId HAVING UserId = " + nUserId.ToString).ToString
            'Catch ex As Exception
            '    LabelPendingOrders.Text = "0"
            'End Try
            'Try
            '    Dim n As Integer = eQuote.ExecuteScalar("SELECT COUNT(DISTINCT CardNumber) FROM [Order] WHERE UserId =" + nUserId.ToString)
            '    LabelNumberOfCC.Text = n.ToString
            '    If n < 4 Then
            '        LabelNumberOfCC.BackColor = Drawing.Color.SpringGreen
            '    ElseIf n >= 4 And n < 7 Then
            '        LabelNumberOfCC.BackColor = Drawing.Color.Yellow
            '    Else
            '        LabelNumberOfCC.BackColor = Drawing.Color.Red
            '    End If
            'Catch ex As Exception
            '    LabelNumberOfCC.Text = "?"
            'End Try
            'Try
            '    LinkButtonCryptoAddressSum.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (CryptoAddress = N'" + sCrypto + "')").ToString
            '    LinkButtonCryptoAddressSum.BackColor = Drawing.Color.Yellow
            '    LinkButtonCryptoAddressSum.Enabled = True
            '    If CInt(LinkButtonCryptoAddressSum.Text) = 0 Then
            '        LinkButtonCryptoAddressSum.Enabled = False
            '        LinkButtonCryptoAddressSum.BackColor = Drawing.Color.White
            '    End If
            'Catch ex As Exception
            '    LinkButtonCryptoAddressSum.Text = "0"
            '    LinkButtonCryptoAddressSum.BackColor = Drawing.Color.White
            '    LinkButtonCryptoAddressSum.Enabled = False
            'End Try
            'Try
            '    LinkButtonEmailSum.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (Email = N'" + sEmailOrder + "')").ToString
            '    LinkButtonEmailSum.BackColor = Drawing.Color.Yellow
            '    LinkButtonEmailSum.Enabled = True
            '    If CInt(LinkButtonEmailSum.Text) = 0 Then
            '        LinkButtonEmailSum.Enabled = False
            '        LinkButtonEmailSum.BackColor = Drawing.Color.White
            '    End If
            'Catch ex As Exception
            '    LinkButtonEmailSum.Text = "0"
            '    LinkButtonEmailSum.BackColor = Drawing.Color.White
            '    LinkButtonEmailSum.Enabled = False
            'End Try

            'Try
            '    LinkButtonIpSum.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (IP = N'" + sIP + "')").ToString
            '    LinkButtonIpSum.BackColor = Drawing.Color.Yellow
            '    LinkButtonIpSum.Enabled = True
            '    If CInt(LinkButtonIpSum.Text) = 0 Then
            '        LinkButtonIpSum.Enabled = False
            '        LinkButtonIpSum.BackColor = Drawing.Color.White
            '    End If
            'Catch ex As Exception
            '    LinkButtonIpSum.Text = "0"
            '    LinkButtonIpSum.BackColor = Drawing.Color.White
            '    LinkButtonIpSum.Enabled = False
            'End Try

            'Try
            '    LinkButtonCreditCard.Text = eQuote.ExecuteScalar("SELECT COUNT(*) FROM [Order] WHERE  (NOT (UserId = " + nUserId.ToString + ")) AND (CardNumber = N'" + sCardNumber + "')").ToString
            '    LinkButtonCreditCard.BackColor = Drawing.Color.Yellow
            '    LinkButtonCreditCard.Enabled = True
            '    If CInt(LinkButtonCreditCard.Text) = 0 Then
            '        LinkButtonCreditCard.Enabled = False
            '        LinkButtonCreditCard.BackColor = Drawing.Color.White
            '    End If
            'Catch ex As Exception
            '    LinkButtonCreditCard.Text = "0"
            '    LinkButtonCreditCard.BackColor = Drawing.Color.White
            '    LinkButtonCreditCard.Enabled = False
            'End Try
            'PanelTrustLevel.Visible = False
        End If
    End Sub

    Protected Sub GridViewOrder_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewOrder.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Edit Or e.Row.RowState = 5 Then
            ElseIf e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                CType(e.Row.FindControl("ButtonAwaitApproval"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonAwaitApproval"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
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
            If e.Row.RowState = DataControlRowState.Edit Or e.Row.RowState = 5 Then
            Else
                Dim lb As HyperLink = CType(e.Row.FindControl("HyperLinkTwilio"), HyperLink)
                If lb.NavigateUrl.Length Then
                    lb.NavigateUrl = "https://www.google.dk/#q=%2B" + lb.NavigateUrl.ToString + ""
                End If
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

    Protected Sub DropDownListOrderStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListOrderStatus.SelectedIndexChanged
        GridViewOrder.SelectedIndex = -1
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
            GridViewKyc.DataBind()
        Else
            lblMessage.ForeColor = System.Drawing.Color.Red
            lblMessage.Text = "File format not recognised." & " Upload Image/Word/PDF/Excel formats"
        End If
    End Sub
    Protected Sub ButtonAddNote_Click(sender As Object, e As EventArgs) Handles ButtonAddNote.Click
        Dim nLineId As Integer = GridViewOrder.SelectedValue
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + TextBoxNote.Text + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + nLineId.ToString)
        GridViewOrder.DataBind()
        TextBoxNote.Text = ""
    End Sub

    Protected Sub SqlDataSourceOrder_Selecting(sender As Object, e As SqlDataSourceSelectingEventArgs) Handles SqlDataSourceOrder.Selecting
        Dim sqlSiteManagerWhere As String = " ORDER BY [Order].Quoted DESC"
        If Session("UserRole") = "SiteMgr" Then
            sqlSiteManagerWhere = " AND [Order].SiteId = " + Session("SiteId").ToString + " ORDER BY [Order].Quoted DESC"
        End If
        e.Command.CommandText = e.Command.CommandText + sqlSiteManagerWhere
    End Sub


End Class

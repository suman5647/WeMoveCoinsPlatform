Option Strict Off
Imports BitGoSharp
Imports RestSharp
Imports System
Imports System.Net
Imports System.IO
Imports System.Data
Imports System.Data.SqlClient
Imports Microsoft.VisualBasic
Imports WMC.Logic.Models

<WMC.PageAuth(True, "Admin")>
Partial Class ApproveBank2
    Inherits WMC.SecurePage

    Dim client As BitGoClient
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'If Not (IsNumeric(Session("UserId")) And Session("UserRole") = "Admin") Then Response.Redirect("Default.aspx")
        Dim bitGoAccessCodeJson As String = eQuote.ExecuteScalar("SELECT ConfigValue FROM AppSettings WHERE ConfigKey = 'BitGoAccessCode'")
        Dim bitGoAccessCode = WMC.Logic.SettingsManager.GetDefault.Get("BitGoAccessCode", True).GetJsonData(Of BitGoAccessSettings)
        'Dim bitGoAccessCode = SimpleJson.DeserializeObject(bitGoAccessCodeJson)
        client = New BitGoClient(bitGoAccessCode.Environment, bitGoAccessCode.AccessCode)

        If RadioButtonListSelectMode.SelectedValue = "Auto" Then
            If Not IsPostBack Then
                GridViewOrder.DataSource = SqlDataSourceOrderAuto
                GridViewOrder.DataBind()
            End If
            PanelMode.Visible = True
        Else
            If Not IsPostBack Then
                GridViewOrder.DataSource = SqlDataSourceOrderManual
                GridViewOrder.DataBind()
            End If
            PanelMode.Visible = False
        End If
    End Sub

    'Protected Sub btnUnlock_Click(sender As Object, e As EventArgs) Handles BtnUnlock.Click
    '    Dim response As IRestResponse = client.Unlock(txtOTP.Text, CInt(DropDownListUnlockPeriode.SelectedValue))
    '    Label1.Text = response.StatusCode.ToString
    '    eQuote.ExecuteScalar("UPDATE AppSettings SET ConfigValue = { fn NOW() } WHERE ConfigKey = 'BitGoUnlocked'")
    'End Sub

    Protected Sub SqlDataSourceOrderChildNote_Updated(sender As Object, e As SqlDataSourceStatusEventArgs) Handles SqlDataSourceOrderChildNote.Updated
        GridViewOrder.SelectedIndex = -1
        GridViewOrder.DataBind()
        GridViewSubOrders.SelectedIndex = -1
        GridViewSubOrders.DataBind()
    End Sub

    Protected Sub GridViewOrder_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewOrder.RowCommand
        Dim OrderId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
        Dim userId As Integer = eQuote.ExecuteScalar("SELECT [Order].UserId FROM [Order] WHERE [Order].Id = " + OrderId.ToString)
        If e.CommandName = "ReviewKYC" Then
            If WMC.IsOrderStatus("Quoted", OrderId) Then
                Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Quoted'")
                WMC.AddStatusChange(OrderId, "Quoted", s, Session("UserId"))
                eQuote.ExecuteNonQuery("INSERT INTO KycFile (UserId, Type,OriginalFilename) VALUES (" + userId.ToString + ", 1, N'EnforceKYC')")
                Using dr As SqlDataReader = eQuote.GetDataReader("SELECT [Order].Id, [Order].SiteId, [Site].[Text], [Order].CreditCardUserIdentity, [Order].Number, [Order].Amount, Currency.Code, [Order].Quoted, [Order].Name, [Order].Email, [Order].bccAddress, [Order].CardNumber, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN [User] ON [Order].UserId = [User].Id INNER JOIN [Site] ON [Order].SiteId = [Site].Id WHERE [Order].Id =" + OrderId.ToString)
                    Try
                        If dr.HasRows Then
                            While dr.Read()
                                WMC.SendEmail("KYCRequest", dr!SiteId, dr!Name, dr!Email, dr!Number, dr!Number)
                                s = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'SentEmail'")
                                Dim m As String = "KYC_Request.htm" + vbCrLf + "Number:" + dr!Number
                                eQuote.ExecuteNonQuery("INSERT INTO AuditTrail(OrderId, Status, Message, Created) VALUES (" + dr!Id.ToString + ", " + s.ToString + ", N'" + m + "', { fn NOW() })")
                                GridViewAuditTrail.DataBind()
                            End While
                        End If
                    Finally
                        dr.Close()
                    End Try
                End Using
            End If
        ElseIf e.CommandName = "Register" Then
            If WMC.IsOrderStatus("Quoted", OrderId) Then
                Dim tid As Object = eQuote.ExecuteScalar("SELECT [Transaction].Id FROM [Transaction] INNER JOIN TransactionMethod ON [Transaction].MethodId = TransactionMethod.Id WHERE  ([Transaction].OrderId = " + OrderId.ToString + ") AND  (TransactionMethod.Text = N'Bank')")
                Dim userRiskLevel As Integer = eQuote.ExecuteScalar("SELECT [User].UserRiskLevel FROM [User] WHERE [User].Id = " + userId.ToString)
                If userRiskLevel = 0 Then
                    Dim IsKYCApproved As Boolean = CheckKYC(OrderId)
                    If Not IsKYCApproved Then
                        Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Approval Pending'")
                        eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                        KYCErrorMessage()

                    Else
                        If IsNumeric(tid) Then
                            PayoutApproved(OrderId, tid)
                        End If
                    End If
                Else
                    If IsNumeric(tid) Then
                        Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Paid'")
                        WMC.AddStatusChange(OrderId, "Quoted", s, Session("UserId"))
                        eQuote.ExecuteNonQuery("UPDATE [Order] SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                        'Note:Incoming transaction to be updated when order is moved to Completed status'
                        'eQuote.ExecuteNonQuery("UPDATE [Transaction] SET Completed = { fn NOW() } WHERE (Id = " + tid.ToString + ")")
                        PanelTrustCalculation.Visible = False
                        PanelNote.Visible = False
                        TextBoxText.Text = ""
                        TextBoxAmount.Text = ""
                        TextBoxOrderNumber.Text = ""
                        GridViewOrder.SelectedIndex = -1
                        GridViewOrder.DataBind()
                    End If
                End If
            End If

            'ElseIf e.CommandName = "CancelOrder" Then
            '    Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Cancel'")
            '    eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
            '    GridViewOrder.SelectedIndex = -1
            '    GridViewOrder.DataBind()
            'ElseIf e.CommandName = "KYCDeclined" Then
            '    If WMC.IsOrderStatus("Quoted", OrderId) Then
            '        Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Decline'")
            '        eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
            '        GridViewOrder.SelectedIndex = -1
            '        GridViewOrder.DataBind()
            '    End If
        ElseIf e.CommandName = "Select" Then
            PanelNote.Visible = True
            Dim UserLogs = eQuote.GetDataSet("SELECT AuditTrail.OrderId, AuditTrail.Status, AuditTrail.Message, AuditTrail.Created, AuditTrailStatus.Text FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE AuditTrail.Message LIKE 'CUSTOMER:'+  CAST(" & userId & " AS NVARCHAR(10)) + '%' ORDER BY AuditTrail.Created")
            GridViewUserAuditTrail.DataSource = UserLogs.Tables(0)
            GridViewUserAuditTrail.DataBind()
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
            'LabelCardApproved.Text = "void" 'WMC.IsCardApproved(OrderId, LabelCardApproved)
            'LabelTxLimit.Text = CInt(TxLimitEUR * CountryTrust * SiteTrust).ToString
            'LabelCardTotal.Text = CInt(WMC.GetOrderSumTotalEUR(OrderId, CardNumber) + WMC.GetOrderSumEUR(OrderId)).ToString
            'LabelPhoneVirtual.Text = WMC.IsPhoneVitual(OrderId)
            'LabelPhoneCardMatch.Text = "void" 'WMC.PhoneCardOrigin_Match(OrderId)
            'LabelPhoneIpMatch.Text = WMC.PhoneIP_Match(OrderId)
            'LinkButtonCreditCardUsedElsewhere.Text = WMC.GetCardUsedElsewhere(OrderId, LinkButtonCreditCardUsedElsewhere)
            'LinkButtonEmailUsedElsewhere.Text = WMC.GetEmailUsedElsewhere(OrderId, LinkButtonEmailUsedElsewhere)
            'LabelCardUS.Text = "void" 'WMC.IsCardUS(OrderId, LabelCardUS)

            'LabelCompleted.Text = WMC.GetOrdersByStatus(OrderId, "Completed")
            'LabelOrderSpan.Text = CInt(WMC.GetOrderSpan(OrderId)).ToString
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
            'LabelCardCountry.Text = "void" 'WMC.GetCardCountry(OrderId)
            'LabelIPCountry.Text = WMC.GetIpCountry(OrderId)

            'LabelTrustMessage.Text = "void" 'WMC.TrustLevelCalculation(OrderId, False, LabelTrustMessage)
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
                CType(e.Row.FindControl("ButtonReviewKYC"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonReviewKYC"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonRegister"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonRegister"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonCancelOrder"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonCancelOrder"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonKYCDeclined"), Button).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonKYCDeclined"), Button).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                If RadioButtonListSelectMode.SelectedValue = "Auto" Then
                    If TextBoxText.Text.Length > 0 Then
                        eQuote.ExecuteNonQuery("UPDATE [Transaction] SET Info = '" + TextBoxText.Text + "' WHERE MethodId = 6 AND OrderId =" + GridViewOrder.DataKeys(e.Row.RowIndex).Value.ToString)
                    End If
                End If
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


    Protected Sub ButtonAddNote_Click(sender As Object, e As EventArgs) Handles ButtonAddNote.Click
        Dim nLineId As Integer = GridViewOrder.SelectedValue
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + TextBoxNote.Text + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + nLineId.ToString)
        GridViewOrder.DataBind()
        TextBoxNote.Text = ""
    End Sub

    Protected Sub ButtonUpdateFee_Click(sender As Object, e As EventArgs) Handles ButtonUpdateFee.Click
        Dim nLineId As Integer = GridViewOrder.SelectedValue
        eQuote.ExecuteNonQuery("UPDATE [Order] SET CommissionProcent = " + TextBoxFee.Text.Replace(",", ".") + " WHERE  Id = " + nLineId.ToString)
        GridViewOrder.DataBind()
        TextBoxFee.Text = ""
    End Sub

    Protected Sub ButtonSearch_Click(sender As Object, e As EventArgs) Handles ButtonSearch.Click
        GridViewOrder.DataSource = SqlDataSourceOrderAuto
        GridViewOrder.DataBind()
    End Sub

    Protected Sub RadioButtonListSelectMode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles RadioButtonListSelectMode.SelectedIndexChanged
        TextBoxText.Text = ""
        TextBoxAmount.Text = ""
        TextBoxOrderNumber.Text = ""
        PanelNote.Visible = False
        PanelTrustCalculation.Visible = False
        GridViewUserAuditTrail.DataSource = ""
        GridViewUserAuditTrail.DataBind()
        If RadioButtonListSelectMode.SelectedValue = "Auto" Then
            GridViewOrder.DataSource = SqlDataSourceOrderAuto
            GridViewOrder.DataBind()
        Else
            GridViewOrder.DataSource = SqlDataSourceOrderManual
            GridViewOrder.DataBind()
        End If
    End Sub


    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        Dim filePath As String = FileUpload1.PostedFile.FileName
        Dim filename As String = Path.GetFileName(filePath)
        Dim ext As String = Path.GetExtension(filename).ToLower
        Dim contenttype As String = String.Empty
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
            '' TODO: replace this logic with new
            Dim fs As Stream = FileUpload1.PostedFile.InputStream
            Dim userId = GridViewUser.DataKeys(0).Value
            Dim kycTypeId = DropDownListKycType.SelectedValue
            WMC.Logic.KYCFileHandler.AddNewKYCFile(fs, userId, filename, kycTypeId)
            'Dim br As New BinaryReader(fs)
            'Dim bytes As Byte() = br.ReadBytes(fs.Length)
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

    Protected Sub GridViewKycFile_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewKycFile.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If e.Row.RowState = DataControlRowState.Edit Or e.Row.RowState = 5 Then
                CType(e.Row.FindControl("ButtonReject"), Button).CommandArgument = e.Row.RowIndex
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

    Protected Sub ButtonDeleteSelected_Click(sender As Object, e As EventArgs) Handles ButtonDeleteSelected.Click
        Dim OrderId As String
        For Each item As GridViewRow In GridViewOrder.Rows
            If TryCast(item.Cells(0).FindControl("cbSelect"), CheckBox).Checked Then
                OrderId = item.Cells(3).Text
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = 23 WHERE Number = N'" + OrderId.ToString + "'")
            End If
        Next
        GridViewOrder.DataBind()
    End Sub

    Protected Sub GridViewOrder_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GridViewOrder.SelectedIndexChanged

    End Sub

    Sub KYCErrorMessage()
        Dim message As String = "Please Approve the KYC before proceding"
        Dim sb As New System.Text.StringBuilder()
        sb.Append("<script type = 'text/javascript'>")
        sb.Append("window.onload=function(){")
        sb.Append("alert('")
        sb.Append(message)
        sb.Append("')};")
        sb.Append("</script>")
        ClientScript.RegisterClientScriptBlock(Me.GetType(), "alert", sb.ToString())
    End Sub

    Sub PayoutApproved(OrderId, tid)
        Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Payout approved'")
        WMC.AddStatusChange(OrderId, "Quoted", s, Session("UserId"))
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
        'Note:Incoming transaction to be updated when order is moved to Completed status'
        'eQuote.ExecuteNonQuery("UPDATE [Transaction] SET Completed = { fn NOW() } WHERE (Id = " + tid.ToString + ")")'
        PanelTrustCalculation.Visible = False
        PanelNote.Visible = False
        TextBoxText.Text = ""
        TextBoxAmount.Text = ""
        TextBoxOrderNumber.Text = ""
        GridViewOrder.SelectedIndex = -1
        GridViewOrder.DataBind()
    End Sub

    Function CheckKYC(OrderId As Integer) As Boolean
        Dim paymentType As Integer = eQuote.ExecuteScalar("SELECT PaymentType FROM [Order] WHERE Id = " + OrderId.ToString)
        Dim userId As Integer = eQuote.ExecuteScalar("SELECT UserId FROM [Order] WHERE Id = " + OrderId.ToString)
        Dim DataUnitOfWork = New WMC.Data.DataUnitOfWork(New WMC.Data.RepositoryProvider(New WMC.Data.RepositoryFactories()))

        Dim type1Kyc As Boolean = DataUnitOfWork.KycFiles.CheckKycFile(userId, WMC.Data.Enums.KYCFileTypes.PhotoID)
        Dim type4Kyc As Boolean = DataUnitOfWork.KycFiles.CheckKycFile(userId, WMC.Data.Enums.KYCFileTypes.SelfieID)

        'For PaymentType Bank then one of the PhotoID is enough to be considered as approved KYC
        If paymentType = WMC.Data.Enums.OrderPaymentType.Bank Then
            If type1Kyc Then
                Return True
            Else
                Return False
            End If
            'For PaymentType CreditCard then one of the PhotoID and SelfieID is enough to be considered as approved KYC
        ElseIf paymentType = WMC.Data.Enums.OrderPaymentType.CreditCard Then
            If type1Kyc And type4Kyc Then
                Return True
            Else
                Return False
            End If
        End If
        Return False
    End Function
End Class

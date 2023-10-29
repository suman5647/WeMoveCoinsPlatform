Option Strict Off
Imports BitGoSharp
Imports RestSharp
Imports System.Data
Imports System.Data.SqlClient
Imports System
Imports System.Net
Imports System.IO
Imports WMC.Logic.Models

<WMC.PageAuth(True, "Admin")>
Partial Class ApprovePayout
    Inherits WMC.SecurePage
    Dim client As BitGoClient
    Public WeightCcOrigin As Integer = 1
    Public WeightPhoneOrigin As Integer = 50
    Public WeightOrderSpan As Integer = 50
    Public WeightCountryCoherence As Integer = 50
    Public WeightDataConsistency As Integer = 30
    Public WeightKycApprove As Integer = 20
    Public WeighAmountSize As Integer = 50

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        LabelResetText.Visible = False
        ButtonReset.Visible = False
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
            CheckBoxIsUserTrusted.Visible = False
        End If
        If Not IsPostBack Then
            If Session("UserRole") = "SiteMgr" Then
                If Session("SiteId") <> 1 Then
                    DropDownListOrderStatus.DataSource = SqlDataSourceOrderStatusSiteMgr
                    DropDownListOrderStatus.DataBind()
                Else
                    DropDownListOrderStatus.DataSource = SqlDataSourceOrderStatus
                    DropDownListOrderStatus.DataBind()
                End If
            Else
                DropDownListOrderStatus.DataSource = SqlDataSourceOrderStatus
                DropDownListOrderStatus.DataBind()
                If DropDownListOrderStatus.SelectedValue = 12 Then
                    GridViewOrder.Columns(11).Visible = True      'Sending Aborted'
                Else
                    GridViewOrder.Columns(11).Visible = False      'Sending Aborted'
                End If
                If DropDownListOrderStatus.SelectedValue = 12 Or DropDownListOrderStatus.SelectedValue = 15 Or DropDownListOrderStatus.SelectedValue = 20 Then
                    Try
                        LabelResetText.Text = eQuote.ExecuteScalar("SELECT Description FROM OrderStatus WHERE Id =" + DropDownListOrderStatus.SelectedValue)
                        LabelResetText.Visible = True
                        ButtonReset.Visible = True
                    Catch ex As Exception

                    End Try
                Else
                    LabelResetText.Visible = False
                    ButtonReset.Visible = False
                End If
            End If
        End If
    End Sub

    Protected Sub ButtonReset_Click(sender As Object, e As EventArgs) Handles ButtonReset.Click
        If WMC.StatusRecordCount("Sending Aborted") > 0 And DropDownListOrderStatus.SelectedValue = 12 Then  'Updated 8/7-17
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = 16 WHERE Status = 12")
        End If
        If WMC.StatusRecordCount("Capture Errored") > 0 And DropDownListOrderStatus.SelectedValue = 20 Then
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = 18 WHERE Status = 20")
        End If
        If WMC.StatusRecordCount("Releasing payment Aborted") > 0 And DropDownListOrderStatus.SelectedValue = 15 Then
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = 13 WHERE Status = 15")
        End If
        Response.Redirect("Default.aspx")
    End Sub

    Protected Sub GridViewOrder_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewOrder.RowCommand
        PanelKeyIndicators.Visible = False
        Dim OrderId As Integer = GridViewOrder.DataKeys(Convert.ToInt32(e.CommandArgument)).Value
        Dim statusOrder As String = DropDownListOrderStatus.SelectedItem.Text
        If e.CommandName = "MoveCompliance" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Compliance Officer Approval'")
            eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
            GridViewOrder.SelectedIndex = -1
            GridViewOrder.DataBind()
        ElseIf e.CommandName = "CancelOrder" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'Cancel'")
            If WMC.IsOrderStatus(statusOrder, OrderId) Then
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "KYCDeclined" Then
            Dim s As Integer = eQuote.ExecuteScalar("SELECT Id FROM OrderStatus WHERE Text = N'KYC Decline'")
            If WMC.IsOrderStatus(statusOrder, OrderId) Then
                eQuote.ExecuteNonQuery("UPDATE [Order] SET Status = " + s.ToString + " WHERE (Id = " + OrderId.ToString + ")")
                GridViewOrder.SelectedIndex = -1
                GridViewOrder.DataBind()
            End If
        ElseIf e.CommandName = "Select" Then
            PanelNote.Visible = True
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

            'LabelCompleted.Text = WMC.GetOrdersByStatus(OrderId, "Completed", LabelCompleted)
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
            'LabelKYCDeclined.Text = WMC.GetOrdersByStatus(OrderId, "KYC Decline")
            'LabelPhoneCountry.Text = WMC.GetPhoneCountry(OrderId)
            'LabelCardCountry.Text = WMC.GetCardCountry(OrderId)
            'LabelIPCountry.Text = WMC.GetIpCountry(OrderId)

            'Dim riskscore As Decimal = WMC.TrustLevelCalculation(OrderId, LabelTrustMessageNew)
            'LabelTrustMessage.Text = "RISKSCORE: " + riskscore.ToString
        End If
    End Sub

    Protected Sub ButtonAddNote_Click(sender As Object, e As EventArgs) Handles ButtonAddNote.Click
        Dim nLineId As Integer = GridViewOrder.SelectedValue
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + TextBoxNote.Text + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [Order] SET Note = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(Note, '') }) } WHERE  Id = " + nLineId.ToString)
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
            If e.Row.RowState = DataControlRowState.Edit Or e.Row.RowState = 5 Then
            ElseIf e.Row.RowState = DataControlRowState.Selected Or e.Row.RowState = DataControlRowState.Normal Or e.Row.RowState = DataControlRowState.Alternate Then
                CType(e.Row.FindControl("ButtonMoveCompliance"), IButtonControl).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonMoveCompliance"), WebControl).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonCancelOrder"), IButtonControl).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonCancelOrder"), WebControl).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
                CType(e.Row.FindControl("ButtonKYCDeclined"), IButtonControl).CommandArgument = e.Row.RowIndex
                CType(e.Row.FindControl("ButtonKYCDeclined"), WebControl).Attributes.Add("onClick", "javascript: return confirm('Are you sure?')")
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

    Protected Sub DropDownListOrderStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListOrderStatus.SelectedIndexChanged
        GridViewOrder.SelectedIndex = -1
        PanelTrustCalculation.Visible = False
        If Not Session("UserRole") = "SiteMgr" Then
            If DropDownListOrderStatus.SelectedValue = 12 Then
                GridViewOrder.Columns(11).Visible = True      'Sending Aborted'
            Else
                GridViewOrder.Columns(11).Visible = False      'Sending Aborted'
            End If
            If WMC.StatusRecordCount("Sending Aborted") > 0 Or WMC.StatusRecordCount("Capture Errored") > 0 Or WMC.StatusRecordCount("Releasing payment Aborted") > 0 Then
                Try
                    LabelResetText.Text = eQuote.ExecuteScalar("SELECT Description FROM OrderStatus WHERE Id =" + DropDownListOrderStatus.SelectedValue)
                    LabelResetText.Visible = True
                    ButtonReset.Visible = True
                Catch ex As Exception

                End Try
            Else
                LabelResetText.Visible = False
                ButtonReset.Visible = False
            End If
        End If
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
            GridViewKyc.DataBind()
        Else
            lblMessage.ForeColor = System.Drawing.Color.Red
            lblMessage.Text = "File format not recognised." & " Upload Image/Word/PDF/Excel formats"
        End If
    End Sub


    Protected Sub SqlDataSourceOrder_Selecting(sender As Object, e As SqlDataSourceSelectingEventArgs) Handles SqlDataSourceOrder.Selecting
        Dim sqlSiteManagerWhere As String = " ORDER BY [Order].Quoted DESC"
        If Session("UserRole") = "SiteMgr" Then
            If Session("SiteId") <> 1 Then
                sqlSiteManagerWhere = " AND [Order].SiteId = " + Session("SiteId").ToString + " ORDER BY [Order].Quoted DESC"
            End If
        End If
        e.Command.CommandText = e.Command.CommandText + sqlSiteManagerWhere
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

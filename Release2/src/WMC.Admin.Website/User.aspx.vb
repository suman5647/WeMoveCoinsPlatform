

Imports System.Data.SqlClient
Imports System.IO
Imports WMC.Logic

<WMC.PageAuth(True, "Admin")>
Partial Class User
    Inherits WMC.SecurePage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            If IsNumeric(Request.QueryString("Id")) Then
                GridViewAuditTrail.DataSource = SqlDataSourceAuditTrail
                Dim s As String = SqlDataSourceAuditTrail.SelectCommand
                Dim w As String = ""
                If IsNumeric(Request.QueryString("Id")) Then
                    w = "WHERE [User].Id = N'" + Request.QueryString("Id") + "' "
                End If

                Dim drAudit = eQuote.GetDataSet("SELECT AuditTrail.OrderId, AuditTrail.Status, AuditTrail.Message, AuditTrail.Created, AuditTrailStatus.Text FROM AuditTrail INNER JOIN AuditTrailStatus ON AuditTrail.Status = AuditTrailStatus.Id WHERE AuditTrail.Message LIKE 'CUSTOMER:'+  CAST(" + Request.QueryString("Id") + " AS NVARCHAR(10)) + '%' ORDER BY AuditTrail.Created")

                Dim drSubOrder = eQuote.GetDataSet("SELECT [Order].Id, [Order].UserId, [Order].Number, [Order].Amount, Currency.Code, [Order].Quoted, [Order].CryptoAddress, [Order].Name, [Order].Email, [Order].IP, OrderStatus.Text AS Status, [Order].Note, [Order].Rate, [Order].CommissionProcent, [Order].CardNumber, [Order].SiteId, SUBSTRING([Order].CountryCode, 1, 5) AS BrowserCulture, [Order].TxSecret FROM Currency INNER JOIN [Order] ON Currency.Id = [Order].CurrencyId INNER JOIN OrderStatus ON [Order].Status = OrderStatus.Id WHERE ([Order].UserId = " + Request.QueryString("Id") + ") ORDER BY [Order].Quoted DESC")

                BindMyData()
                Button1.Visible = True
                GridViewAuditTrail.DataSourceID = Nothing
                GridViewAuditTrail.DataSource = drAudit

                GridViewSubOrders.DataSourceID = Nothing
                GridViewSubOrders.DataSource = drSubOrder
            End If
        End If
    End Sub
    Protected Sub GridViewUser_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridViewUser.RowCommand
        If e.CommandName = "Update" Then
            Dim row As GridViewRow = CType(CType(e.CommandSource, Control).NamingContainer, GridViewRow)
            Dim UserId As Integer = GridViewUser.DataKeys(row.RowIndex).Value
            Dim txLimitsText As TextBox = GridViewUser.Rows(Convert.ToInt32(row.RowIndex)).FindControl("TextBox2")
            Dim ccLimitsText As TextBox = GridViewUser.Rows(Convert.ToInt32(row.RowIndex)).FindControl("TextBox3")
            Dim status As String = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'Admin'")
            Dim TransactionLimitsDetails = eQuote.ExecuteScalar("SELECT TransactionLimitsDetails FROM [User] Where Id=" & UserId.ToString)
            Dim CreditCardLimitsDetails = eQuote.ExecuteScalar("SELECT CreditCardLimitsDetails FROM [User] Where Id=" & UserId.ToString)
            Dim txLimitChangedValue As String = txLimitsText.Text
            Dim ccLimitChangedValue As String = ccLimitsText.Text
            Dim isTxLimitChange As Boolean = True
            Dim isCCLimitChange As Boolean = True

            If String.Compare(txLimitChangedValue, TransactionLimitsDetails.ToString) = 0 Then
                isTxLimitChange = False
            End If

            If String.Compare(ccLimitChangedValue, CreditCardLimitsDetails.ToString) = 0 Then
                isCCLimitChange = False
            End If

            'If both txLimits and ccLimits are changed'
            If isTxLimitChange And isCCLimitChange Then
                Dim message As String = "CUSTOMER:" & UserId.ToString & ":Updated UserLimit for UserId:(" & UserId.ToString & ")  TransactionLimitsDetails change: " & TransactionLimitsDetails & " -> " & txLimitsText.Text & " CreditCardLimitsDetails change" & CreditCardLimitsDetails & " -> " & ccLimitsText.Text & " (Updated By:" & Session("UserId").ToString & ")"
                AuditLog.log(message, status, 2)
                If IsNumeric(Request.QueryString("Id")) Then
                    UpdateUserCCLimits(UserId, ccLimitsText)
                    UpdateUserTxLimits(UserId, txLimitsText)
                    Me.BindMyData()
                Else
                    GridViewUser.DataBind()
                End If


                'If txLimits only changed'
            ElseIf isTxLimitChange Then
                Dim message As String = "CUSTOMER:" & UserId.ToString & ":Updated UserLimit for UserId:(" & UserId.ToString & ") TransactionLimits change: " & TransactionLimitsDetails & " -> " & txLimitsText.Text & " (Updated By:" & Session("UserId").ToString & ")"
                AuditLog.log(message, status, 2)
                If IsNumeric(Request.QueryString("Id")) Then
                    UpdateUserTxLimits(UserId, txLimitsText)
                    Me.BindMyData()
                Else
                    GridViewUser.DataBind()
                End If

                'If only CClimits changed'
            ElseIf isCCLimitChange Then
                Dim message As String = "CUSTOMER:" & UserId.ToString & ":Updated UserLimit for UserId:(" & UserId.ToString & ") CreditCardLimits change: " & CreditCardLimitsDetails & " -> " & ccLimitsText.Text & " (Updated By:" & Session("UserId").ToString & ")"
                AuditLog.log(message, status, 2)
                If IsNumeric(Request.QueryString("Id")) Then
                    UpdateUserCCLimits(UserId, ccLimitsText)
                    Me.BindMyData()
                Else
                    GridViewUser.DataBind()
                End If
            End If
        End If

        If e.CommandName = "Select" Then
            GridViewUser.DataBind()
            GridViewAuditTrail.DataBind()


            'Dim UserId As Integer = GridViewUser.DataKeys(Convert.ToInt32(e.CommandArgument)).Value

            'PanelTrustCalculation.Visible = True
            'Dim OrderId As Integer = eQuote.ExecuteScalar("SELECT TOP 1 Id FROM  [Order] WHERE UserId=" + UserId.ToString)
            ''Dim sPhoneNumber As String = eQuote.ExecuteScalar("SELECT Phone FROM  [User] WHERE Id=" + nUserId.ToString)
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

            'LabelCompleted.Text = WMC.GetOrdersByStatus(OrderId, "Completed", LabelCompleted)
            'LabelOrderSpan.Text = WMC.GetChainalysisWithdrawalAddress(UserId, OrderId).ToString
            'LabelOrderSpan.Text = CInt(WMC.GetOrderSpan(OrderId)).ToString
            'LabelOrderSUM.Text = String.Format("{0:n0}", WMC.GetOrderSumEUR(OrderId))
            'LabelOrderTotal.Text = String.Format("{0:n0}", WMC.GetOrderSumTotalEUR(OrderId))
            'LabelOrder30Days.Text = String.Format("{0:n0}", WMC.GetOrderSum30DaysEUR(OrderId))

            'CheckBoxIsUserTrusted.Checked = WMC.IsUserTrusted(OrderId, LabelUserIsTrusted) : LabelUserIsTrusted.Text = CheckBoxIsUserTrusted.Checked
            'WMC.GetOrigin(OrderId, LabelOrigin)

            'LabelCards.Text = CInt(WMC.GetDistinctUsedCards(OrderId, LabelCards)).ToString
            'LabelNames.Text = CInt(WMC.GetDifferentNameUser(OrderId, LabelNames)).ToString
            'LabelEmails.Text = CInt(WMC.GetDifferentEmailUser(OrderId, LabelEmails)).ToString
            'LabelKycApproved.Text = WMC.IsKycApproved(OrderId, LabelKycApproved)
            'LabelKYCDeclined.Text = WMC.GetOrdersByStatus(OrderId, "KYC Decline")

        End If
    End Sub

    Protected Sub CheckBoxIsUserTrusted_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxIsUserTrusted.CheckedChanged
        Dim UserId As Integer = GridViewUser.SelectedValue
        Dim OrderId As Integer = eQuote.ExecuteScalar("SELECT TOP 1 Id FROM  [Order] WHERE UserId=" + UserId.ToString)
        WMC.TrustUser(OrderId, Session("UserId"), CheckBoxIsUserTrusted.Checked)
        GridViewUser.DataBind()
        GridViewUser.SelectedIndex = -1
        PanelTrustCalculation.Visible = False
    End Sub

    Protected Sub GridViewUser_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewUser.RowDataBound
        If (e.Row.RowType = DataControlRowType.DataRow) Then
            Dim UserId = e.Row.Cells(2).Text
            Using dr As SqlDataReader = eQuote.GetDataReader("SELECT UserRiskLevel FROM [User] WHERE [User].Id =" + UserId.ToString)
                Try
                    'Find the DropDownList in the Row.
                    Dim ddlRiskLevel As DropDownList = CType(e.Row.FindControl("DropDownListRiskLevel"), DropDownList)
                    While dr.Read()
                        Dim itemTobeSelected = ddlRiskLevel.Items.FindByValue(dr!UserRiskLevel)
                        If Not itemTobeSelected Is Nothing Then
                            Dim selindex = ddlRiskLevel.Items.IndexOf(itemTobeSelected)
                            ddlRiskLevel.SelectedIndex = selindex
                        End If
                    End While
                Finally
                    dr.Close()
                End Try
            End Using
        End If
    End Sub

    Protected Sub SelectedIndexChanged(sender As Object, e As EventArgs)
        'Reference the DropDownList.s
        Dim dropDownList As DropDownList = CType(sender, DropDownList)
        Dim row As GridViewRow = CType(dropDownList.NamingContainer, GridViewRow)
        Dim gv As GridView = CType(row.NamingContainer, GridView)
        Dim UserId = Integer.Parse(gv.DataKeys(row.RowIndex).Item("Id").ToString())
        Using dr As SqlDataReader = eQuote.GetDataReader("SELECT UserRiskLevel FROM [User] WHERE [User].Id =" + UserId.ToString)
            Try
                dr.Read()
                Dim status As String = eQuote.ExecuteScalar("SELECT Id FROM AuditTrailStatus WHERE Text = N'Admin'")
                'Get the value from the DropDownList.
                Dim id = dropDownList.SelectedItem.Value
                eQuote.ExecuteNonQuery("Update [User] SET UserRiskLevel=" + id + "WHERE [User].Id =" + UserId.ToString)
                Dim message As String = "CUSTOMER:" & UserId.ToString & ":Updated UserRiskLevel for UserId:(" & UserId.ToString & ") UserRiskLevel change to: " & id & " from " & dr!UserRiskLevel & " (Updated By:" & Session("UserId").ToString & ")"
                AuditLog.log(message, status, 2)
                GridViewUser.DataBind()
                GridViewAuditTrail.DataBind()
            Finally
                dr.Close()
            End Try
        End Using
    End Sub
    Protected Sub GridViewUser_RowEditing(ByVal sender As Object, ByVal e As GridViewEditEventArgs) Handles GridViewUser.RowEditing
        GridViewUser.EditIndex = e.NewEditIndex
        If IsNumeric(Request.QueryString("Id")) Then
            BindMyData()
        End If
    End Sub
    Protected Sub GridViewUser_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs) Handles GridViewUser.RowUpdating
        GridViewUser.EditIndex = -1
        If IsNumeric(Request.QueryString("Id")) Then
            BindMyData()
        End If
    End Sub
    Protected Sub GridViewUser_RowCancel(ByVal sender As Object, ByVal e As GridViewCancelEditEventArgs) Handles GridViewUser.RowCancelingEdit
        GridViewUser.EditIndex = -1
        If IsNumeric(Request.QueryString("Id")) Then
            BindMyData()
        End If
    End Sub
    Protected Sub button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        'Redirect to same page again
        Response.Redirect(Request.RawUrl.Replace(Request.Url.Query, ""))

    End Sub

    Private Sub BindMyData()
        Dim id = Request.QueryString("Id")
        Dim Command = SqlDataSourceUser.SelectCommand
        SqlDataSourceUser.SelectCommand = "SELECT [User].Id, Fname, Phone, Email, KycNote, Created, LanguageId, Trusted, TrustedBy, CountryId, Tier, UserRiskLevel, TransactionLimitsDetails, CreditCardLimitsDetails ,Country.Code AS CountryCode FROM Country INNER JOIN [User] ON [Country].Id = [User].CountryId WHERE [User].Id =" + id

        SqlDataSourceUser.DataBind()
        GridViewUser.AllowPaging = False
        GridViewUser.DataBind()
    End Sub

    Private Sub UpdateUserTxLimits(userId As Integer, txLimitsText As TextBox)
        Dim constr As String = ConfigurationManager.ConnectionStrings("LocalConnectionString").ConnectionString
        Using con As New SqlConnection(constr)
            Using cmd As New SqlCommand("UPDATE [User] SET TransactionLimitsDetails = @TransactionLimitsDetails WHERE (Id = @Id)")
                cmd.Connection = con
                cmd.Parameters.AddWithValue("@TransactionLimitsDetails", txLimitsText.Text)
                cmd.Parameters.AddWithValue("@Id", userId.ToString)
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End Using
    End Sub

    Private Sub UpdateUserCCLimits(userId As Integer, ccLimitsText As TextBox)
        Dim constr As String = ConfigurationManager.ConnectionStrings("LocalConnectionString").ConnectionString
        Using con As New SqlConnection(constr)
            Using cmd As New SqlCommand("UPDATE [User] SET CreditCardLimitsDetails = @CreditCardLimitsDetails WHERE (Id = @Id)")
                cmd.Connection = con
                cmd.Parameters.AddWithValue("@CreditCardLimitsDetails", ccLimitsText.Text)
                cmd.Parameters.AddWithValue("@Id", userId.ToString)
                con.Open()
                cmd.ExecuteNonQuery()
                con.Close()
            End Using
        End Using
    End Sub

    Protected Sub GridViewKycFile_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridViewKycFile.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            CType(e.Row.FindControl("ButtonApproveNow"), Button).CommandArgument = e.Row.RowIndex
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
                If CType(e.Row.FindControl("LabelKycType"), Label).Text = "CardApproval" Then
                    e.Row.BackColor = Drawing.Color.GreenYellow
                    CType(e.Row.FindControl("ButtonApproveNow"), Button).Enabled = False
                End If
            End If
        End If
    End Sub

    Protected Sub ButtonAddNote_Click(sender As Object, e As EventArgs) Handles ButtonAddNote.Click
        Dim sqlSiteManagerWhere As String = ""
        If Session("UserRole") = "SiteMgr" Then
            If Session("SiteId") <> 1 Then
                sqlSiteManagerWhere = " [Order].SiteId = " + Session("SiteId").ToString + " AND "
            End If
        End If
        Dim nLineId As Integer = GridViewUser.SelectedValue
        Dim GetMemoLogScript As String = String.Format(" {0:d}", Date.Now) + " at " + String.Format(" {0:t}", Date.Now) + " by " + eQuote.ExecuteScalar("SELECT Lname FROM [User] WHERE Id = " + Session("UserId").ToString) + ":<br />" + TextBoxNote.Text + "<br />"
        eQuote.ExecuteNonQuery("UPDATE [User] SET KycNote = { fn CONCAT('" + Replace(GetMemoLogScript, Environment.NewLine, "<br/>") + "', { fn IFNULL(KycNote, '') }) } WHERE  Id = " + nLineId.ToString)
        Dim s As String = SqlDataSourceUser.SelectCommand
        Dim w As String = ""
        SqlDataSourceUser.SelectCommand = s.Insert(s.IndexOf("ORDER BY"), w)
        TextBoxNote.Text = ""
        DropDownListNoteOptions.SelectedIndex = 0
    End Sub

    Protected Sub DropDownListNoteOptions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownListNoteOptions.SelectedIndexChanged
        If DropDownListNoteOptions.SelectedIndex > 0 Then
            TextBoxNote.Text = DropDownListNoteOptions.SelectedValue
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
        If e.CommandName = "ApproveNow" Then
            eQuote.ExecuteNonQuery("UPDATE KycFile SET Approved = { fn NOW() }, ApprovedBy = " + Session("UserId").ToString + ", Rejected = NULL, RejectedBy = NULL where Id= " + nLineId.ToString)
            GridViewKycFile.DataBind()
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
        If e.CommandName = "Select" Then
            Dim btn As LinkButton = GridViewKycFile.Rows(Convert.ToInt32(e.CommandArgument)).FindControl("LinkButton1")
            btn.Focus()
            Dim strOname = eQuote.ExecuteScalar("SELECT OriginalFilename FROM KycFile WHERE Id = " + nLineId.ToString).ToString()
            Dim ext = strOname.Split(".").GetValue(1)
            If ext = "pdf" Then
                Dim embed As String = "<object data=""{0}{1}"" type=""application/pdf"" width=""550px"" height=""500px"">"
                embed += "</object>"
                ltEmbed.Visible = True
                ModalPopupExtender1.Show()
                imagePreview.Visible = False
                ltEmbed.Text = String.Format(embed, ResolveUrl("KYCImageHandler.ashx?ImageId="), nLineId.ToString)
            Else
                ltEmbed.Visible = False
                editImageBtn.Visible = True
                imagePreview.Visible = True
                ModalPopupExtender1.Show()
                imagePreview.Src = String.Format("KYCImageHandler.ashx?ImageId={0}", nLineId.ToString)
                imageId.Value = e.CommandArgument
            End If
        End If
        If e.CommandName = "ShowDoc" Then
            ' + nLineId.ToString)
            Dim strImg = eQuote.ExecuteScalar("SELECT UniqueFilename FROM KycFile WHERE Id = " + nLineId.ToString)
            If Not IsDBNull(strImg) Then
                If eQuote.FileExists(strImg) Then
                    ImageDoc.Visible = True
                    ImageDoc.ImageUrl = strImg
                End If
            End If
        End If
        If e.CommandName = "Delete" Then
            Dim KycFileId = e.CommandArgument
            eQuote.ExecuteNonQuery("DELETE FROM [OrderKycfile] WHERE [KycfileId] = N'" + KycFileId.ToString + "'")
            eQuote.ExecuteNonQuery("DELETE FROM [KycFile] WHERE [Id] = N'" + KycFileId.ToString + "'")
        End If
    End Sub
    Protected Sub btncancel_Click(sender As Object, e As EventArgs)
        btnCancel.Focus()
        ModalPopupExtender1.Hide()
        GridViewKycFile.DataBind()
    End Sub

End Class

